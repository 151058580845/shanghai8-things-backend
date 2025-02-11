using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message;
using Hgzn.Mes.Domain.ValueObjects.Message.Base;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Hgzn.Mes.Iot.EquipManager;

public class RfidReaderConnector : IEquipConnector
{
    private GClient _client = new ();
    private string? SerialNum { get; set; }
    private readonly IMqttExplorer _mqtt;

    public RfidReaderConnector(
        IMqttExplorer mqtt)
    {
        _mqtt = mqtt;
    }

    public async Task StartAsync()
    {
        StartReadingTag(_client);
        await UpdateStateAsync(ConnStateType.Run);

    }

    /// <summary>
    /// 全部停止采集
    /// </summary>
    public async Task StopAsync()
    {
        StopReadingTag(_client);
        await UpdateStateAsync(ConnStateType.Stop);
    }

    public Task SendDataAsync(byte[] buffer)
    {
        throw new NotImplementedException();
    }


    public async Task<bool> ConnectAsync(ConnInfo connInfo)
    {
            if (connInfo?.ConnString is null) throw new ArgumentNullException("connIfo");
            if(connInfo.EquipType is not null)
            {
                switch (connInfo.ConnType)
                {
                    case ConnType.Socket:
                        var conn = JsonSerializer.Deserialize<SocketConnInfo>(connInfo.ConnString, Options.CustomJsonSerializerOptions) 
                        ?? throw new ArgumentNullException("conn");
                        if (_client.OpenTcp(conn.Address + ":" + conn.Port, 5000, out var status) &&
                            status == eConnectionAttemptEventStatusType.OK)
                        {
                            _client.OnEncapedTagEpcLog = TagEpcLogEncapedHandler;
                            // 获得读写器信息
                            var readerInfo = new MsgAppGetReaderInfo();
                            _client.SendSynMsg(readerInfo);
                            if (readerInfo.RtCode == 0)
                            {
                                LoggerAdapter.LogTrace("ger reader info success");
                                SerialNum = readerInfo.Imei;
                            await UpdateStateAsync(ConnStateType.On);
                            }
                            return true;
                        }
                        break;
                    default :
                        return false;
                }
            }

            return false;
    }

    public async Task CloseConnectionAsync()
    {
        _client?.Close();
        await UpdateStateAsync(ConnStateType.Stop);
    }

    private void StartReadingTag(GClient client)
    {
        // 停止指令，空闲态
        StopReadingTag(client);

        // 4个天线读卡, 读取EPC数据区以及TID数据区
        var msgBaseInventoryEpc = new MsgBaseInventoryEpc
        {
            AntennaEnable = (ushort)(eAntennaNo._1 | eAntennaNo._2 |eAntennaNo._3 | eAntennaNo._4),
            InventoryMode = (byte)eInventoryMode.Inventory,
            ReadTid = new ParamEpcReadTid
            {
                Mode = (byte)eParamTidMode.Auto,
                Len = 6
            }
        };
        client.SendSynMsg(msgBaseInventoryEpc);
        if (0 != msgBaseInventoryEpc.RtCode)
        {
            LoggerAdapter.LogWarning("inventory epc error.");
        }
    }

    private void StopReadingTag(GClient client)
    {
        // 停止指令，空闲态
        var msgBaseStop = new MsgBaseStop();
        client.SendSynMsg(msgBaseStop);
        if (0 != msgBaseStop.RtCode)
        {
            LoggerAdapter.LogWarning("epc stop error.");
        }
    }

    private async void TagEpcLogEncapedHandler(EncapedLogBaseEpcInfo msg)
    {
        var data = new RfidMsg
        {
            Tid = msg.logBaseEpcInfo.Tid,
            Userdata = msg.logBaseEpcInfo.Userdata
        };
        var plain = JsonSerializer.Serialize(data);
        await _mqtt.PublishAsync(TopicBuilder
            .CreateBuilder()
            .WithPrefix(TopicType.Iot)
            .WithDirection(MqttDirection.Up)
            .WithTag(MqttTag.Data)
            .Build(), Encoding.UTF8.GetBytes(plain));
    }

    private async Task UpdateStateAsync(ConnStateType stateType)
    {
        await _mqtt.PublishAsync(TopicBuilder
        .CreateBuilder()
        .WithPrefix(TopicType.App)
        .WithDirection(MqttDirection.Up)
        .WithTag(MqttTag.State)
        .Build(), BitConverter.GetBytes((int)stateType));
    }
}
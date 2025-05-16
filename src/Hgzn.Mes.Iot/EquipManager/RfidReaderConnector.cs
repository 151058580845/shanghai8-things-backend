using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using SqlSugar;
using Hgzn.Mes.Domain.Shared.Utilities;

namespace Hgzn.Mes.Iot.EquipManager;

public class RfidReaderConnector : EquipConnectorBase
{
    private GClient _client = new();
    private string? _serialNum { get; set; }

    private int _pushInterval { get; set; } 

    private DateTime _timeLast { get; set; }

    public RfidReaderConnector(
        IConnectionMultiplexer connectionMultiplexer,
        IMqttExplorer mqtt,
        ISqlSugarClient sqlSugarClient,
        string uri, EquipConnType connType, int pushInterval) :
        base(connectionMultiplexer, mqtt, sqlSugarClient, uri, connType)
    {
        _pushInterval = pushInterval;
    }

    public override async Task StartAsync(Guid uri)
    {
        StartReadingTag(_client);
        await UpdateStateAsync(ConnStateType.Run);

    }

    /// <summary>
    /// 全部停止采集
    /// </summary>
    public override async Task StopAsync(Guid uri)
    {
        StopReadingTag(_client);
        await UpdateStateAsync(ConnStateType.Stop);
    }

    public override Task SendDataAsync(byte[] buffer)
    {
        throw new NotImplementedException();
    }

    public override async Task<bool> ConnectAsync(ConnInfo connInfo)
    {
        if (connInfo?.ConnString is null) throw new ArgumentNullException("connIfo");
        switch (connInfo.ConnType)
        {
            case ConnType.Socket:
                var conn = JsonSerializer.Deserialize<SocketConnInfo>(connInfo.ConnString, Options.CustomJsonSerializerOptions)
                ?? throw new ArgumentNullException("conn");
                if (_client.OpenTcp(conn.Address + ":" + conn.Port, 5000, out var status) &&
                    status == eConnectionAttemptEventStatusType.OK)
                {
                    _client.OnEncapedTagEpcLog = TagEpcLogEncapedHandler;
                    _client.OnTcpDisconnected = TcpDisconnectedHandler;
                    // 获得读写器信息
                    var readerInfo = new MsgAppGetReaderInfo();
                    _client.SendSynMsg(readerInfo);
                    if (readerInfo.RtCode == 0)
                    {
                        LoggerAdapter.LogTrace("ger reader info success");
                        _serialNum = readerInfo.Imei;
                        await UpdateStateAsync(ConnStateType.On);
                    }
                    return true;
                }
                break;
            default:
                return false;
        }

        return false;
    }

    public override async Task CloseConnectionAsync()
    {
        StopReadingTag(_client);
        await CloseConnOnlyAsync();
    }

    private async Task CloseConnOnlyAsync()
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
            //AntennaEnable = (ushort)(eAntennaNo._1),
            AntennaEnable = (uint)(eAntennaNo._1 | eAntennaNo._2 | eAntennaNo._3 | eAntennaNo._4),
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
        LoggerAdapter.LogTrace("epc on");

        if ((DateTime.UtcNow - _timeLast).TotalSeconds - _pushInterval < 0)
        {
            return;
        }
        _timeLast = DateTime.UtcNow;
        var data = new RfidMsg
        {
            Tid = msg.logBaseEpcInfo.Tid,
            Userdata = msg.logBaseEpcInfo.Userdata
        };

        var plain = JsonSerializer.Serialize(data, Options.CustomJsonSerializerOptions);
        await _mqttExplorer.PublishAsync(IotTopicBuilder
            .CreateIotBuilder()
            .WithPrefix(TopicType.Iot)
            .WithDirection(MqttDirection.Up)
            .WithTag(MqttTag.Data)
            .WithUri(_uri!)
            .WithDeviceType(_connType.ToString()!)
            .Build(), Encoding.UTF8.GetBytes(plain));
    }    
    
    private async void TcpDisconnectedHandler(string readerName)
    {
        LoggerAdapter.LogWarning($"serilnum: {_serialNum}, reader: {readerName} disconnected!");
        await CloseConnOnlyAsync();
    }
}
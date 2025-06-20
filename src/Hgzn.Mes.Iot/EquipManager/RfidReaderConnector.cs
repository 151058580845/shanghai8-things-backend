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
using System.Collections.Concurrent;

namespace Hgzn.Mes.Iot.EquipManager;

public class RfidReaderConnector : EquipConnectorBase
{
    protected GClient _client = new();
    protected string? _serialNum { get; set; }

    protected int _pushInterval { get; set; }

    protected DateTime _timeLast { get; set; }
    protected ConcurrentQueue<string> _cacheTids {  get; set; } = new ConcurrentQueue<string>();

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
                    LoggerAdapter.LogInformation(
                            $"connection[{_equipConnect!.Name}](connId:{_equipConnect.Id})<{_equipConnect.EquipId}> connect success!");
                    // 获得读写器信息
                    var readerInfo = new MsgAppGetReaderInfo();
                    _client.SendSynMsg(readerInfo);
                    if (readerInfo.RtCode == 0)
                    {
                        LoggerAdapter.LogInformation(
                            $"connection[{_equipConnect!.Name}](connId:{_equipConnect.Id})<{_equipConnect.EquipId}> ger reader info success!");
                        _serialNum = readerInfo.Imei;
                        await UpdateStateAsync(ConnStateType.On);
                    }
                    else
                    {
                        LoggerAdapter.LogWarning(
                            $"connection[{_equipConnect!.Name}](connId:{_equipConnect.Id})<{_equipConnect.EquipId}> ger reader info error.");
                    }
                    ConnState = true;
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
        ConnState = false;
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
            LoggerAdapter.LogWarning(
                $"connection[{_equipConnect!.Name}](connId:{_equipConnect.Id})<{_equipConnect.EquipId}> inventory epc error.");
        }
        else LoggerAdapter.LogInformation(
            $"connection[{_equipConnect!.Name}](connId:{_equipConnect.Id})<{_equipConnect.EquipId}> inventory epc success!");
    }

    private void StopReadingTag(GClient client)
    {
        // 停止指令，空闲态
        var msgBaseStop = new MsgBaseStop();
        client.SendSynMsg(msgBaseStop);
        if (0 != msgBaseStop.RtCode)
        {
            LoggerAdapter.LogWarning(
                $"connection[{_equipConnect!.Name}](connId:{_equipConnect.Id})<{_equipConnect.EquipId}> epc stop error.");
        }
        else LoggerAdapter.LogInformation(
            $"connection[{_equipConnect!.Name}](connId:{_equipConnect.Id})<{_equipConnect.EquipId}> stop epc success!");
    }

    private async void TagEpcLogEncapedHandler(EncapedLogBaseEpcInfo msg)
    {
        LoggerAdapter.LogTrace($"connection[{_equipConnect!.Name}](connId:{_equipConnect.Id})<{_equipConnect.EquipId}> epc flag on!");
        var tidExist = _cacheTids.Contains(msg.logBaseEpcInfo.Tid);
        var timeout = (DateTime.UtcNow - _timeLast).TotalSeconds - _pushInterval > 0;
        if (!tidExist)
        {
            _cacheTids.Enqueue(msg.logBaseEpcInfo.Tid);
        }

        if (!timeout && tidExist)
        {
            return;
        }
        if (timeout)
        {
            _cacheTids.Clear();
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
        LoggerAdapter.LogTrace($"connection[{_equipConnect!.Name}](connId:{_equipConnect.Id})<{_equipConnect.EquipId}>tid:{msg.logBaseEpcInfo.Tid} flag updated!");
    }

    protected async void TcpDisconnectedHandler(string readerName)
    {
        LoggerAdapter.LogWarning($"connection[{_equipConnect!.Name}](connId:{_equipConnect.Id})<{_equipConnect.EquipId}> serilnum: {_serialNum}, reader: {readerName} disconnected!");
        _client.OnTcpDisconnected -= TcpDisconnectedHandler;
        await CloseConnOnlyAsync();
    }
}
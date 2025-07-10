using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hgzn.Mes.Iot.EquipManager
{
    public class CardIssuerConnector : RfidReaderConnector
    {
        public CardIssuerConnector(
        IConnectionMultiplexer connectionMultiplexer,
        IMqttExplorer mqtt,
        ISqlSugarClient sqlSugarClient,
        string uri, EquipConnType connType, int pushInterval) :
        base(connectionMultiplexer, mqtt, sqlSugarClient, uri, connType, pushInterval)
        {
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

        private async void TagEpcLogEncapedHandler(EncapedLogBaseEpcInfo msg)
        {
            LoggerAdapter.LogTrace("epc on");

            if ((DateTime.Now.ToLocalTime() - _timeLast).TotalSeconds - _pushInterval < 0)
            {
                return;
            }
            _timeLast = DateTime.Now.ToLocalTime();
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
    }
}

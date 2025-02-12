using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Events;
using Hgzn.Mes.Domain.ProtocolManagers.TcpServer;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceive;
using MediatR;
using NetCoreServer;
using SqlSugar;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Buffer = System.Buffer;

namespace Hgzn.Mes.Iot.EquipManager
{
    public class TcpServerConnector : TcpSession
    {
        // private readonly Timer _timer;
        private readonly EquipConnect? _equipConnect;
        private readonly ISqlSugarClient _client;
        private readonly IMqttExplorer _mqttExplorer;
        private string? Ip;
        private string? Mac;
        // 多少个转一个（从前端配置进行）
        private int _forwardLength = 10;

        public TcpServerConnector(EquipTcpServer server,
             EquipConnect equipConnect, ISqlSugarClient client, IMqttExplorer mqttExplorer) : base(server)
        {
            _equipConnect = equipConnect;
            _client = client;
            this._mqttExplorer = mqttExplorer;
            if (equipConnect.ForwardRate != null)
                _forwardLength = equipConnect.ForwardRate.Value;
        }

        // public DataProcessor DataProcessor { get; set; }
        private const int BodyStartIndex = 6;
        //缓存数据
        private readonly ConcurrentQueue<byte[]> _dataQueue = new();
        //接收数据就开启
        private volatile bool _hasData = false;
        private volatile int _forwardNum = 1;

        private async Task ProcessDataAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_dataQueue.TryDequeue(out var buffer))
                {
                    var size = buffer.Length;
                    //标准头
                    var header = buffer[0];
                    if (header != 0x5A)
                    {
                        Console.WriteLine("报头符错误。");
                        return;
                    }

                    //报文长度
                    var messageLength = BitConverter.ToUInt32(buffer, 1);
                    if (messageLength != size)
                    {
                        Console.WriteLine($"报文长度错误：接收到的长度为 {size}，报文中声明的长度为 {messageLength}");
                        return;
                    }

                    //解析报文流水号（1字节）
                    var number = buffer[5];
                    //报文数据
                    var length = messageLength - 6;
                    var newBuffer = new byte[length];
                    Buffer.BlockCopy(buffer, BodyStartIndex, newBuffer, 0, newBuffer.Length);

                    //获取所有转发地址
                    if (_equipConnect.ForwardEntities != null)
                    {
                        List<Guid> list = _equipConnect.ForwardEntities.Select(x => x.Id).ToList();
                        if (_forwardNum % _forwardLength == 0)
                        {
                            foreach (var guid in list)
                            {
                                var topic = IotTopicBuilder.CreateIotBuilder()
                                    .WithPrefix(TopicType.Iot)
                                    .WithDirection(MqttDirection.Down)
                                    .WithTag(MqttTag.Data)
                                    .WithDeviceType("tcp-server").Build();
                                // .WithUri(connect.Id.ToString()).Build();
                                if (await _mqttExplorer.IsConnectedAsync())
                                {
                                    await _mqttExplorer.PublishAsync(topic, buffer);
                                }
                            }
                            _forwardNum = 1;
                        }
                    }

                    //本地解析数据
                    TestDataReceive testDataReceive = new TestDataReceive(_client);
                    await testDataReceive.Handle(buffer);
                }
                _hasData = false;
            }
        }

        protected override void OnConnected()
        {
            IPEndPoint? ipEndPoint = this.Socket.RemoteEndPoint as IPEndPoint;
            if (ipEndPoint == null) return;
            Ip = ipEndPoint.ToString();
            Mac = ipEndPoint.Address.ToString();
            base.OnConnected();
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            _dataQueue.Enqueue(buffer);
            if (!_hasData)
            {
                _hasData = true;
                var cancellationTokenSource = new CancellationTokenSource();
                var token = cancellationTokenSource.Token;
                _ = Task.Run(async () => await ProcessDataAsync(token), token);
            }
        }

        public Task<bool> ConnectAsync(ConnInfo connInfo)
        {
            //IPEndPoint? ipEndPoint = this.Socket.RemoteEndPoint as IPEndPoint;
            //if (ipEndPoint == null) return Task.FromResult(false);
            //Ip = ipEndPoint.ToString();
            //Mac = ipEndPoint.Address.ToString();

            //Ip = connInfo.ConnString; // TODO 这里还需要解析出IP

            Ip = "127.0.0.1";
            this.StartAsync();
            base.OnConnected();
            return Task.FromResult(true);
        }

        public Task CloseConnectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task StartAsync()
        {
            base.Server.Start();
            return Task.FromResult(true);
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }

        public Task SendDataAsync(byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }
}

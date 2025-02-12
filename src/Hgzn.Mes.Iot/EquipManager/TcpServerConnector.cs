using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Events;
using Hgzn.Mes.Domain.ProtocolManagers.TcpServer;
using MediatR;
using NetCoreServer;
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
        private readonly string _heartBeatMessage;

        private readonly string _heartBeatAck;

        // private readonly Timer _timer;
        private readonly IMediator _mediator;
        private readonly EquipConnect _equipConnect;
        private string Ip;
        private string Mac;
        // 多少个转一个（从前端配置进行）
        private int _forwardLength = 10;

        public TcpServerConnector(EquipTcpServer server, string heartBeatMessage, string heartBeatAck, int heartTime,
            IMediator mediator, EquipConnect equipConnect) : base(server)
        {
            _heartBeatMessage = heartBeatMessage;
            _heartBeatAck = heartBeatAck;
            _mediator = mediator;
            _equipConnect = equipConnect;
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
                            // TODO 用MQTT转发
                            ////因为基本上下一条的地址只会有一个两个，就不并行了，也不合并发布数据
                            //if (list.Count > 0)
                            //{
                            //    foreach (var guid in list)
                            //    {
                            //        await _mediator.Publish(new EquipForwardNotification()
                            //        {
                            //            OriginId = _equipConnect.Id,
                            //            TargetId = guid,
                            //            Buffer = buffer
                            //        });
                            //    }
                            //}
                            _forwardNum = 1;
                        }
                    }

                    //本地解析数据
                    await _mediator.Publish(new TestDataReceiveNotification()
                    {
                        ConnectionId = _equipConnect.Id,
                        EquipId = _equipConnect.EquipId,
                        Data = buffer,
                        Ip = Ip
                    });
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
    }
}

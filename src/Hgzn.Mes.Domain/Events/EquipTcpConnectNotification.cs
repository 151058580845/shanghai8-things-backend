using Hgzn.Mes.Domain.Shared.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Events
{
    public class EquipTcpConnectNotification : INotification
    {
        /// <summary>
        /// 连接主键
        /// </summary>
        public Guid SessionId { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public IPAddress? IpAddress { get; set; }
        /// <summary>
        /// MAC地址
        /// </summary>
        public string MacAddress { get; set; }
        public EquipConnectEnum ConnectEnum { get; set; }
    }
}

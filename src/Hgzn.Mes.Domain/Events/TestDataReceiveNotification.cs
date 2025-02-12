using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Events
{
    public class TestDataReceiveNotification : INotification
    {
        public string Ip { get; set; }
        public Guid ConnectionId { get; set; }
        public Guid EquipId { get; set; }
        public byte[] Data { get; set; }
    }
}

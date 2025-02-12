using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Events
{
    public class EquipForwardNotification : INotification
    {
        /// <summary>
        /// 源链接
        /// </summary>
        public Guid OriginId { get; set; }
        /// <summary>
        /// 目标链接
        /// </summary>
        public Guid TargetId { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Buffer { get; set; }
    }
}

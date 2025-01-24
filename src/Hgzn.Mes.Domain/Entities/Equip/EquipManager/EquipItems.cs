using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipManager
{
    public class EquipItems : UniversalEntity, IAudited
    {
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public Guid? LastModifierId { get; set; }
        public bool IsDeleted { get; set; }
        public bool State { get; set; }

        [Description("编号")]
        public string? ItemCode { get; set; }

        [Description("名称")]
        public string? ItemName { get; set; }

        /// <summary>
        /// 数据字典值
        /// </summary>
        [Description("类型")]
        public string? EquipMaintenanceType { get; set; }

        [Description("标准")]
        public string? Standard { get; set; }

        [Description("内容")]
        public string? Content { get; set; }

        [Description("备注")]
        public string? Remark { get; set; }

        [Description("资源Id")]
        public Guid? ResourceId { get; set; }
    }
}

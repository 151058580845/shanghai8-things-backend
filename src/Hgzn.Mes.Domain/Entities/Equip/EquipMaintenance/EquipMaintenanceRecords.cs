using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipMaintenance
{
    public class EquipMaintenanceRecords : UniversalEntity
    {
        [Description("维护任务Id")]
        public Guid TaskId { get; set; }

        [Description("维护开始日期")]
        public DateTime MaintenanceStartDate { get; set; }

        [Description("维护完成日期")]
        public DateTime MaintenanceFinishDate { get; set; }

        [Description("维护类型")]
        public EquipMaintenanceStatus? MaintenanceType { get; set; }

        [Description("维护结果")]
        public EquipMaintenanceResult? EquipMaintenanceResult { get; set; }

        [Description("维护人员")]
        public Guid? UserId { get; set; }

        [Description("维护描述")]
        public String? Description { get; set; }

        [Description("创建")]
        public DateTime CreationTime { get; set; }

        [Description("最后修改时间")]
        public DateTime? LastModificationTime { get; set; }

        [Description("软删除")]
        public bool IsDeleted { get; set; }

        [Description("资源Id")]
        public string? ResourceId { get; set; }
        // [Navigate(NavigateType.OneToOne,nameof(UserId))]
        // public UserAggregateRoot User { get; set; }
    }
}

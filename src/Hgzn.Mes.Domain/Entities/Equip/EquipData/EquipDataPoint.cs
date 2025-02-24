using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData
{
    /// <summary>
    /// 设备采集点位配置
    /// </summary>
    [Description("采集点位配置表")]
    public class EquipDataPoint : UniversalEntity, IAudited
    {
        public Guid? CreatorId { get; set; }

        [Description("创建时间")] public DateTime CreationTime { get; set; }

        [Description("编号")] public string Code { get; set; } = null!;

        [Description("连接Id")] public Guid ConnectionId { get; set; }
        
        public EquipConnect? Connection { get; set; }

        [Description("备注")] public string? Remark { get; set; }

        [Description("采集频率")] public Guid? CollectionConfigId { get; set; }

        public CollectionConfig? CollectionConfig { get; set; }

        [Description("采集地址设置")] public string? CollectionAddressStr { get; set; }

        [Description("采集数据主键")] public Guid EquipReceiveDataId { get; set; }
        
        [Description("采集数据")] public EquipReceiveData? EquipReceiveData { get; set; }

        public bool State { get; set; } = true;

        public string CollectStatus { get; set; }

        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}
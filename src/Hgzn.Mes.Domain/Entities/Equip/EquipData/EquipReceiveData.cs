using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.System.Equip.EquipData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData
{
    public class EquipReceiveData : UniversalEntity
    {
        public EquipReceiveData()
        {
        }

        public EquipReceiveData(Guid id)
        {
            Id = id;
        }

        public DateTime CreationTime { get; set; }

        [Description("设备主键")]
        public Guid? EquipId { get; set; }

        [Description("设备资产编号")]
        public string? EquipAssetNumber { get; set; }

        [Description("采集数据")]
        public ReceiveData? ReceiveData { get; set; }

        public bool IsDeleted { get; set; } = false;

        [Description("最大值")]
        public double? MaxValue { get; set; }

        [Description("最小值")]
        public double? MinValue { get; set; }

        [Description("备注")]
        public string? Remark { get; set; }
    }
}

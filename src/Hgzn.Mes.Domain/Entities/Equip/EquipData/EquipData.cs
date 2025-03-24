using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData
{
    public class EquipData : UniversalEntity
    {
        public DateTime CreationTime { get; set; }

        [Description("类型主键")]
        public Guid? TypeId { get; set; }

        [Description("采集数据")]
        public ReceiveData? ReceiveData { get; set; }

        [Description("最大值")]
        public double? MaxValue { get; set; }

        [Description("最小值")]
        public double? MinValue { get; set; }

        [Description("备注")]
        public string Remark { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}

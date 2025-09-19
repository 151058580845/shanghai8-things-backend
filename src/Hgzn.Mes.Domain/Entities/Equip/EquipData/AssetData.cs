using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData
{
    [Description("成本数据")]
    public class AssetData : UniversalEntity, ICreationAudited
    {
        [Description("系统名称")]
        public string SystemName { get; set; } = null!;

        [Description("厂房使用费")]
        public decimal? FactoryUsageFee { get; set; }

        [Description("设备使用费")]
        public decimal? EquipmentUsageFee { get; set; }

        [Description("人力成本")]
        public decimal? LaborCost { get; set; }

        [Description("电费")]
        public decimal? ElectricityCost { get; set; }

        [Description("燃料动力费")]
        public decimal? FuelPowerCost { get; set; }

        [Description("设备保养费用")]
        public decimal? EquipmentMaintenanceCost { get; set; }

        [Description("系统空置成本")]
        public decimal? SystemIdleCost { get; set; }

        [Description("系统试验成本")]
        public decimal? SystemExperimentCost { get; set; }

        [Description("区域(面积)")]
        public decimal? Area { get; set; }

        [Description("员工职位数量")]
        public int? StaffPositions { get; set; }

        [Description("员工年工时")]
        public int? StaffAnnualHours { get; set; }

        [Description("系统能耗")]
        public decimal? SystemEnergyConsumption { get; set; }

        [Description("项目列表")]
        [SugarColumn(IsIgnore = true)]
        public List<AssetDataProjectItem>? Projects { get; set; }
        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }
        public int CreatorLevel { get; set; }
    }
}

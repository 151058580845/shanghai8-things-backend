using Hgzn.Mes.Application.Main.Dtos.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Dtos.Equip
{
    public class AssetDataReadDto : ReadDto
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

        [Description("人力成本(元/小时)")]
        public decimal? LaborCostPerHour { get; set; }

        [Description("系统能耗")]
        public decimal? SystemEnergyConsumption { get; set; }

        [Description("燃料动力费(万元/小时)")]
        public decimal? FuelPowerCostPerHour { get; set; }

        [Description("项目列表")]
        public List<ProjectItemDto>? Projects { get; set; }
    }

    public class AssetDataQueryDto : PaginatedQueryDto
    {
        [Description("系统名称")]
        public string? SystemName { get; set; }
    }

    public class AssetDataCreateDto : CreateDto
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

        [Description("人力成本(元/小时)")]
        public decimal? LaborCostPerHour { get; set; }

        [Description("系统能耗")]
        public decimal? SystemEnergyConsumption { get; set; }

        [Description("燃料动力费(万元/小时)")]
        public decimal? FuelPowerCostPerHour { get; set; }

        [Description("项目列表")]
        public List<ProjectItemDto>? Projects { get; set; }
    }

    public class AssetDataUpdateDto : UpdateDto
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

        [Description("人力成本(元/小时)")]
        public decimal? LaborCostPerHour { get; set; }

        [Description("系统能耗")]
        public decimal? SystemEnergyConsumption { get; set; }

        [Description("燃料动力费(万元/小时)")]
        public decimal? FuelPowerCostPerHour { get; set; }

        [Description("项目列表")]
        public List<ProjectItemDto>? Projects { get; set; }
    }

    public sealed class ProjectItemDto
    {
        public string ProjectType { get; set; } = null!;
        public decimal? Amount { get; set; }
        public DateOnly? StartDate { get; set; }
        public string? Remark { get; set; }
        public string? Responsible { get; set; }
    }

    /// <summary>
    /// 成本计算明细DTO
    /// </summary>
    public class AssetDataCalculationDetailsDto
    {
        [Description("系统名称")]
        public string SystemName { get; set; } = null!;

        [Description("厂房使用费明细")]
        public CostDetailDto FactoryUsageFee { get; set; } = null!;

        [Description("设备使用费明细")]
        public CostDetailDto EquipmentUsageFee { get; set; } = null!;

        [Description("人力成本明细")]
        public CostDetailDto LaborCost { get; set; } = null!;

        [Description("电费明细")]
        public CostDetailDto ElectricityCost { get; set; } = null!;

        [Description("燃料动力费明细")]
        public CostDetailDto FuelPowerCost { get; set; } = null!;

        [Description("设备保养费用明细")]
        public CostDetailDto EquipmentMaintenanceCost { get; set; } = null!;

        [Description("系统空置成本明细")]
        public CostDetailDto SystemIdleCost { get; set; } = null!;

        [Description("系统试验成本明细")]
        public CostDetailDto SystemExperimentCost { get; set; } = null!;
    }

    /// <summary>
    /// 成本明细DTO
    /// </summary>
    public class CostDetailDto
    {
        [Description("成本名称")]
        public string CostName { get; set; } = null!;

        [Description("计算公式")]
        public string CalculationFormula { get; set; } = null!;

        [Description("数据来源")]
        public string DataSource { get; set; } = null!;

        [Description("计算过程")]
        public string CalculationProcess { get; set; } = null!;

        [Description("最终结果")]
        public decimal? FinalResult { get; set; }

        [Description("相关参数")]
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
}

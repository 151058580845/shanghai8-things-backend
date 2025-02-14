using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData;

[Description("实验数据")]
public class TestDataProduct:UniversalEntity
{
    [Description("实验计划主键")]
    public Guid TestDataId { get; set; }
    [Description("名称")]
    public string Name { get; set; }
    [Description("编号")]
    public string Code { get; set; }
    [Description("技术状")]
    public string TechnicalStatus { get; set; }
}
using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using SqlSugar;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData;

//唯一索引 (true表示唯一索引 或者叫 唯一约束)
// [SugarIndex("TestEquipData_TestEquip",nameof(TestEquipData.TestEquip),OrderByType.Desc,true)]
[Description("试验系统设备自动增加")]
public class TestEquipData : UniversalEntity
{
    [Description("仿真试验系统识别编码,设备类型识别编码,本机识别编码")]
    public byte[] TestEquip { get; set; } = null!;
    
    [Description("仿真试验系统识别编码")]
    public byte SimuTestSysld { get; set; }

    [Description("设备类型识别编码")]
    public byte DevTypeld { get; set; }

    [Description("本机识别编码")]
    public string Compld { get; set; }
    
}
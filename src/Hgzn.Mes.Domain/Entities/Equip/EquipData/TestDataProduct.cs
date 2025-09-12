using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData;

[Description("实验数据")]
public class TestDataProduct : UniversalEntity
{
    
    [Description("实验计划主键")]
    public string? TestDataId { get; set; }
    [Description("名称")]
    public string? Name { get; set; }
    [Description("编号")]
    public string? Code { get; set; }
    [Description("技术状")]
    public string? TechnicalStatus { get; set; }

    public static TestDataProduct[] Seeds { get; } = new TestDataProduct[]
    {
        new TestDataProduct()
        {
            Id = Guid.Parse("018060da-3cb6-4f80-b179-1d6319d9663a"),
            TestDataId = "1234",
            Name = "种子名称1",
            Code = "种子编号1",
            TechnicalStatus = "种子技术状1"
        },
        new TestDataProduct()
        {
            Id = Guid.Parse("931946e0-1d9e-4e6f-a1a4-a10453ad6a3c"),
            TestDataId = "1234",
            Name = "种子名称2",
            Code = "种子编号2",
            TechnicalStatus = "种子技术状2"
        },
        new TestDataProduct()
        {
            Id = Guid.Parse("d32cc889-bda8-4393-8ef4-887b2be5685d"),
            TestDataId = "2345",
            Name = "种子名称1",
            Code = "种子编号1",
            TechnicalStatus = "种子技术状1"
        },
        new TestDataProduct()
        {
            Id = Guid.Parse("1e8a1edf-558d-4678-b880-9ef65c139b1e"),
            TestDataId = "4567",
            Name = "种子名称2",
            Code = "种子编号2",
            TechnicalStatus = "种子技术状2"
        },
        new TestDataProduct()
        {
            Id = Guid.Parse("bb9cb0b0-769d-4f0b-832a-576b9549f6cb"),
            TestDataId = "6789",
            Name = "种子名称1",
            Code = "种子编号1",
            TechnicalStatus = "种子技术状1"
        },
        new TestDataProduct()
        {
            Id = Guid.Parse("e5d3d4c0-f7f5-4c1a-b843-2486c8772dff"),
            TestDataId = "6789",
            Name = "种子名称2",
            Code = "种子编号2",
            TechnicalStatus = "种子技术状2"
        }
    };
}
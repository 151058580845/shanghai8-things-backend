using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData;

[Description("试验数据UST")]
public class TestDataUST : UniversalEntity
{
    [Description("试验计划主键")]
    public string? TestDataId { get; set; }
    
    [Description("编号")]
    public string? Code { get; set; }
    
    [Description("型号规格")]
    public string? ModelSpecification { get; set; }
    
    [Description("名称")]
    public string? Name { get; set; }
    
    [Description("有效期")]
    public string? ValidityPeriod { get; set; }

    #region audit

    public Guid? CreatorId { get; set; }
    public DateTime? CreationTime { get; set; }

    #endregion audit

#if DEBUG
    public static TestDataUST[] Seeds { get; } = new TestDataUST[]
    {
        new TestDataUST()
        {
            Id = Guid.Parse("018060da-3cb6-4f80-b179-1d6319d9663a"),
            TestDataId = "1234",
            Code = "873112010314",
            ModelSpecification = "GEN40-38",
            Name = "直流稳压电源",
            ValidityPeriod = "2027-05-29 00:00:00.0"
        },
        new TestDataUST()
        {
            Id = Guid.Parse("931946e0-1d9e-4e6f-a1a4-a10453ad6a3c"),
            TestDataId = "2345",
            Code = "873112010315",
            ModelSpecification = "GEN40-39",
            Name = "交流稳压电源",
            ValidityPeriod = "2027-06-15 00:00:00.0"
        },
        new TestDataUST()
        {
            Id = Guid.Parse("d32cc889-bda8-4393-8ef4-887b2be5685d"),
            TestDataId = "4567",
            Code = "873112010316",
            ModelSpecification = "GEN40-40",
            Name = "数字示波器",
            ValidityPeriod = "2027-07-20 00:00:00.0"
        }
    };
#endif
}

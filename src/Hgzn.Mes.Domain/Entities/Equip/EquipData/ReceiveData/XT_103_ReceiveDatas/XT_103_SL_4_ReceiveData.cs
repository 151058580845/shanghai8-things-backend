using System.ComponentModel;
using Hgzn.Mes.Domain.Attributes;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_103_ReceiveDatas;

/// <summary>
/// _固定电源
/// </summary>
public class XT_103_SL_4_ReceiveData : UniversalEntity, IAudited, IPowerSupplyData
{
    [TableNotShow]
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }

    [Description("仿真试验系统识别编码")]
    [TableNotShow]
    public byte SimuTestSysld { get; set; }

    [Description("设备类型识别编码")]
    [TableNotShow]
    public byte DevTypeld { get; set; }

    [Description("本机识别编码")]
    public string? Compld { get; set; }

    #region 工作模式信息 6个

    [Description("本地还是远程")]
    [TableNotShow]
    public byte LocalOrRemote { get; set; }

    [Description("电源数量")]
    [TableNotShow]
    public byte PowerSupplyCount { get; set; }

    [Description("电源类型1")]
    [TableNotShow]
    public byte PowerSupplyType1 { get; set; }

    [Description("电源类型2")]
    [TableNotShow]
    public byte PowerSupplyType2 { get; set; }

    [Description("是否上电")]
    [TableNotShow]
    public byte IsPoweredOn { get; set; }

    [Description("预留")]
    [TableNotShow]
    public byte Reserved { get; set; }

    #endregion

    #region 健康状态信息 2个

    [Description("状态类型")]
    [TableNotShow]
    public byte StatusType { get; set; }

    [Description("工作状态")]
    [TableNotShow]
    public byte OperationStatus { get; set; }

    #endregion

    #region 物理量

    [Description("物理量参数数量")]
    [TableNotShow]
    public uint PhysicalParameterCount { get; set; }

    [Description("电源1电压设置值")]
    public float Power1VoltageSet { get; set; }

    [Description("电源1电流设置值")]
    public float Power1CurrentSet { get; set; }

    [Description("电源2电压设置值")]
    public float Power2VoltageSet { get; set; }

    [Description("电源2电流设置值")]
    public float Power2CurrentSet { get; set; }

    [Description("电源3电压设置值")]
    public float Power3VoltageSet { get; set; }

    [Description("电源3电流设置值")]
    public float Power3CurrentSet { get; set; }

    [Description("电源4电压设置值")]
    public float Power4VoltageSet { get; set; }

    [Description("电源4电流设置值")]
    public float Power4CurrentSet { get; set; }

    [Description("电源5电压设置值")]
    public float Power5VoltageSet { get; set; }

    [Description("电源5电流设置值")]
    public float Power5CurrentSet { get; set; }

    [Description("电源6电压设置值")]
    public float Power6VoltageSet { get; set; }

    [Description("电源6电流设置值")]
    public float Power6CurrentSet { get; set; }

    [Description("电源7电压设置值")]
    public float Power7VoltageSet { get; set; }

    [Description("电源7电流设置值")]
    public float Power7CurrentSet { get; set; }

    [Description("电源8电压设置值")]
    public float Power8VoltageSet { get; set; }

    [Description("电源8电流设置值")]
    public float Power8CurrentSet { get; set; }

    [Description("电源1电压采集值")]
    public float Power1VoltageRead { get; set; }

    [Description("电源1电流采集值")]
    public float Power1CurrentRead { get; set; }

    [Description("电源2电压采集值")]
    public float Power2VoltageRead { get; set; }

    [Description("电源2电流采集值")]
    public float Power2CurrentRead { get; set; }

    [Description("电源3电压采集值")]
    public float Power3VoltageRead { get; set; }

    [Description("电源3电流采集值")]
    public float Power3CurrentRead { get; set; }

    [Description("电源4电压采集值")]
    public float Power4VoltageRead { get; set; }

    [Description("电源4电流采集值")]
    public float Power4CurrentRead { get; set; }

    [Description("电源5电压采集值")]
    public float Power5VoltageRead { get; set; }

    [Description("电源5电流采集值")]
    public float Power5CurrentRead { get; set; }

    [Description("电源6电压采集值")]
    public float Power6VoltageRead { get; set; }

    [Description("电源6电流采集值")]
    public float Power6CurrentRead { get; set; }

    [Description("电源7电压采集值")]
    public float Power7VoltageRead { get; set; }

    [Description("电源7电流采集值")]
    public float Power7CurrentRead { get; set; }

    [Description("电源8电压采集值")]
    public float Power8VoltageRead { get; set; }

    [Description("电源8电流采集值")]
    public float Power8CurrentRead { get; set; }

    #endregion

    [Description("运行时间")]
    [TableNotShow]
    public uint? RunTime { get; set; }

    [TableNotShow]
    public Guid? LastModifierId { get; set; }
    [TableNotShow]
    public DateTime? LastModificationTime { get; set; }
    [TableNotShow]
    public int CreatorLevel { get; set; } = 0;

#if DEBUG
    public static XT_103_SL_4_ReceiveData[] Seeds
    {
        get
        {
            List<XT_103_SL_4_ReceiveData> list = [];
            List<string> uuids = [
                "0198BC84-A661-769E-9679-952EAD97BB40",
                "0198BC84-A661-769E-9679-9ABF23C47134",
                "0198BC84-A661-769E-9679-9EAD4D7E265C",
                "0198BC84-A661-769E-9679-A06DD58807C6",
                "0198BC84-A661-769E-9679-A732C81F110B",
                "0198BC84-A661-769E-9679-A99639D96C9D",
                "0198BC84-A661-769E-9679-AE2B83E91A90",
                "0198BC84-A661-769E-9679-B37994EF3D36",
                "0198BC84-A661-769E-9679-B40545D6C6FB",
                "0198BC84-A661-769E-9679-BA38CB43EE0B",
                "0198BC84-A661-769E-9679-BED486BEC613",
                "0198BC84-A661-769E-9679-C22F0787B5BA",
                "0198BC84-A661-769E-9679-C7D77C818F70",
                "0198BC84-A661-769E-9679-CB5F738D7604",
                "0198BC84-A661-769E-9679-CDC52BA40958",
                "0198BC84-A661-769E-9679-D35D2BB4AEE3",
                "0198BC84-A661-769E-9679-D503938D17A1",
                "0198BC84-A661-769E-9679-D8A00A4D18E1",
                "0198BC84-A661-769E-9679-DCEDBD4BC883",
                "0198BC84-A661-769E-9679-E38B209A6FD7",
                "0198BC84-A661-769E-9679-E7E2E26B8F5F",
                "0198BC84-A661-769E-9679-E819B7717402",
                "0198BC84-A661-769E-9679-EF8ADE832C86",
                "0198BC84-A661-769E-9679-F087D29F1347",
                "0198BC84-A661-769E-9679-F77B2E82DEF5",
                "0198BC84-A661-769E-9679-FA6D65D75A73",
                "0198BC84-A661-769E-9679-FD3F2E204E65",
                "0198BC84-A661-769E-967A-012B7038833E",
                "0198BC84-A661-769E-967A-05204A5D5174",
                "0198BC84-A661-769E-967A-0A041B666A39",
                "0198BC84-A661-769E-967A-0DB2D7090FC9",
                "0198BC84-A661-769E-967A-136FA73FC459",
                "0198BC84-A661-769E-967A-16A775E9FA65",
                "0198BC84-A661-769E-967A-1BD4139E2969",
                "0198BC84-A661-769E-967A-1D67E837FE78",
                "0198BC84-A661-769E-967A-21F6EFAACCEA",
                "0198BC84-A661-769E-967A-24579A189B2D",
                "0198BC84-A661-769E-967A-2B0BDC43BDF6",
                "0198BC84-A661-769E-967A-2F5ABA36FC9A",
                "0198BC84-A661-769E-967A-3176615B583A",
                "0198BC84-A661-769E-967A-36DF6D74D2A5",
                "0198BC84-A661-769E-967A-3A3A067E433A",
                "0198BC84-A661-769E-967A-3CBE096C5C7E",
                "0198BC84-A661-769E-967A-4240191888E7",
                "0198BC84-A661-769E-967A-44A082142B2E",
                "0198BC84-A661-769E-967A-496D0EC3FE70",
                "0198BC84-A661-769E-967A-4F8FB2C66F9E",
                "0198BC84-A661-769E-967A-523A41CFF300",
                "0198BC84-A661-769E-967A-566B468C13B8",
                "0198BC84-A661-769E-967A-58B416A67ADF",
                "0198BC84-A661-769E-967A-5E1D374B807A",
                "0198BC84-A661-769E-967A-62B339A2D24A",
                "0198BC84-A661-769E-967A-65DB0E8F0BBE",
                "0198BC84-A661-769E-967A-6B760023CBD0",
                "0198BC84-A661-769E-967A-6DF2B437B9F3",
                "0198BC84-A661-769E-967A-7112382B8E96",
                "0198BC84-A661-769E-967A-753018EB8A29",
                "0198BC84-A661-769E-967A-788C0006AE95",
                "0198BC84-A661-769E-967A-7E9D961AAE4A",
                "0198BC84-A661-769E-967A-8099E0D76254",
                "0198BC84-A661-769E-967A-869743734C98",
                "0198BC84-A661-769E-967A-8AE03E2C1DB1",
                "0198BC84-A662-7738-9374-CA8DF0952681",
                "0198BC84-A662-7738-9374-CFB97E2A58AC",
                "0198BC84-A662-7738-9374-D30BF525F91D",
                "0198BC84-A662-7738-9374-D7BEE2F168AC",
                "0198BC84-A662-7738-9374-DBD1919D5999",
                "0198BC84-A662-7738-9374-DC3DF892E342",
                "0198BC84-A662-7738-9374-E302B7704244",
                "0198BC84-A662-7738-9374-E5AE518CFEC0",
                "0198BC84-A662-7738-9374-E9ED18B83EFB",
                "0198BC84-A662-7738-9374-EE2A29BFE00A",
                "0198BC84-A662-7738-9374-F08821A4EB4B",
                "0198BC84-A662-7738-9374-F4106D3D9DBC",
                "0198BC84-A662-7738-9374-F975E74B8EAE",
                "0198BC84-A662-7738-9374-FDE7E6170318",
                "0198BC84-A662-7738-9375-0044F4ACA4CE",
                "0198BC84-A662-7738-9375-079FDBC87E7E",
                "0198BC84-A662-7738-9375-090562CD9579",
                "0198BC84-A662-7738-9375-0D52E13E2DD2",
                "0198BC84-A662-7738-9375-138E3912571E",
                "0198BC84-A662-7738-9375-15255C38C3CD",
                "0198BC84-A662-7738-9375-187AC5543D2D",
                "0198BC84-A662-7738-9375-1CFB524BA99A",
                "0198BC84-A662-7738-9375-200CA65C15D0",
                "0198BC84-A662-7738-9375-27A0882FF5C3",
                "0198BC84-A662-7738-9375-2A079F64CC7D",
                "0198BC84-A662-7738-9375-2EBB80040F9F",
                "0198BC84-A662-7738-9375-32A0E93CCF94",
                "0198BC84-A662-7738-9375-367968A787A0",
                "0198BC84-A662-7738-9375-39279A2F0AF5",
                "0198BC84-A662-7738-9375-3C3594900EEC",
                "0198BC84-A662-7738-9375-420B54343A96",
                "0198BC84-A662-7738-9375-4606E4B91D86",
                "0198BC84-A662-7738-9375-49020DE71B8B",
                "0198BC84-A662-7738-9375-4CF6200B5A39",
                "0198BC84-A662-7738-9375-50029ADDB69A",
                "0198BC84-A662-7738-9375-54B0F839760D",
                "0198BC84-A662-7738-9375-5A0AA34A6351",
                "0198BC84-A662-7738-9375-5CF6AB09E64C"
            ];
            List<string> equipUuids = [
                "0198BBB3A649726DBD9EE8928FA743B6",
                "0198BBB3A649726DBD9EEE4F2CB78199",
                "0198BBB3A649726DBD9EF12B4123AD64",
                "0198BBB3A649726DBD9EF4DCA2883266",
                "0198BBB3A649726DBD9EFBF02A0B7FB9",
                "0198BBB3A649726DBD9EFC4A17A9CB79",
                "0198BBB3A649726DBD9F001C62338EF0",
                "0198BBB3A649726DBD9F04E7E98AD250",
                "0198BBB3A649726DBD9F0A90F1533D80",
                "0198BBB3A649726DBD9F0F460F70191E"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                list.Add(new XT_103_SL_4_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 8,
                    DevTypeld = 4,
                    Compld = equipUuids[rand.Next(equipUuids.Count - 1)],
                    LocalOrRemote = 1,
                    PowerSupplyCount = 8,
                    PowerSupplyType1 = 1,
                    PowerSupplyType2 = 1,
                    IsPoweredOn = 1,
                    Reserved = 1,
                    StatusType = 1,
                    OperationStatus = 1,
                    PhysicalParameterCount = 32,
                    Power1VoltageSet = (float)(rand.NextDouble() * 28 + 0.9),
                    Power1CurrentSet = (float)(rand.NextDouble() + 2),
                    Power2VoltageSet = (float)(rand.NextDouble() * 10 + 65),
                    Power2CurrentSet = (float)(rand.NextDouble() + 9),
                    Power3VoltageSet = (float)(rand.NextDouble() * 28 + 0.9),
                    Power3CurrentSet = (float)(rand.NextDouble() + 2),
                    Power4VoltageSet = (float)(rand.NextDouble() * 10 + 65),
                    Power4CurrentSet = (float)(rand.NextDouble() + 9),
                    Power5VoltageSet = (float)(rand.NextDouble() * 28 + 0.9),
                    Power5CurrentSet = (float)(rand.NextDouble() + 2),
                    Power6VoltageSet = (float)(rand.NextDouble() * 10 + 65),
                    Power6CurrentSet = (float)(rand.NextDouble() + 9),
                    Power7VoltageSet = (float)(rand.NextDouble() * 28 + 0.9),
                    Power7CurrentSet = (float)(rand.NextDouble() + 2),
                    Power8VoltageSet = (float)(rand.NextDouble() * 10 + 65),
                    Power8CurrentSet = (float)(rand.NextDouble() + 9),
                    Power1VoltageRead = (float)(rand.NextDouble() * 28 + 0.9),
                    Power1CurrentRead = (float)(rand.NextDouble() + 2),
                    Power2VoltageRead = (float)(rand.NextDouble() * 10 + 65),
                    Power2CurrentRead = (float)(rand.NextDouble() + 9),
                    Power3VoltageRead = (float)(rand.NextDouble() * 28 + 0.9),
                    Power3CurrentRead = (float)(rand.NextDouble() + 2),
                    Power4VoltageRead = (float)(rand.NextDouble() * 10 + 65),
                    Power4CurrentRead = (float)(rand.NextDouble() + 9),
                    Power5VoltageRead = (float)(rand.NextDouble() * 28 + 0.9),
                    Power5CurrentRead = (float)(rand.NextDouble() + 2),
                    Power6VoltageRead = (float)(rand.NextDouble() * 10 + 65),
                    Power6CurrentRead = (float)(rand.NextDouble() + 9),
                    Power7VoltageRead = (float)(rand.NextDouble() * 28 + 0.9),
                    Power7CurrentRead = (float)(rand.NextDouble() + 2),
                    Power8VoltageRead = (float)(rand.NextDouble() * 10 + 65),
                    Power8CurrentRead = (float)(rand.NextDouble() + 9),
                    RunTime = (uint?)new DateTimeOffset(now.AddSeconds(a)).ToUnixTimeSeconds(),
                    CreationTime = now.AddSeconds(a)
                });
                a++;
            }
            return [.. list];
        }
    }
#endif
}

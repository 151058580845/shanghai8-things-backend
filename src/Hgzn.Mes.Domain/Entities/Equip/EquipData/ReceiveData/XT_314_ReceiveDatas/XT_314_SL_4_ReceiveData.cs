using System.ComponentModel;
using Hgzn.Mes.Domain.Attributes;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_314_ReceiveDatas;

/// <summary>
/// 314_固定电源
/// </summary>
public class XT_314_SL_4_ReceiveData : UniversalEntity, IAudited, IPowerSupplyData
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

    #region 工作模式信息

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

    #region 健康状态信息

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
    public static XT_314_SL_4_ReceiveData[] Seeds
    {
        get
        {
            List<XT_314_SL_4_ReceiveData> list = [];
            List<string> uuids = [
                "0198BCB1-5B11-72EC-A11F-C68608E2DC98",
                "0198BCB1-5B11-72EC-A11F-CB69EA518C59",
                "0198BCB1-5B11-72EC-A11F-CDA0A3F2F217",
                "0198BCB1-5B11-72EC-A11F-D34C7CC366B7",
                "0198BCB1-5B11-72EC-A11F-D447BE5785AD",
                "0198BCB1-5B11-72EC-A11F-D89016197736",
                "0198BCB1-5B11-72EC-A11F-DD1BDB8FFB21",
                "0198BCB1-5B11-72EC-A11F-E0D207307813",
                "0198BCB1-5B11-72EC-A11F-E4A8BC6216B4",
                "0198BCB1-5B11-72EC-A11F-E9F4074B96F5",
                "0198BCB1-5B11-72EC-A11F-EDCC6CE0A053",
                "0198BCB1-5B11-72EC-A11F-F05475EF1B3E",
                "0198BCB1-5B11-72EC-A11F-F474B73FE6DD",
                "0198BCB1-5B11-72EC-A11F-FBCB2EDC527F",
                "0198BCB1-5B11-72EC-A11F-FD0B96E8E763",
                "0198BCB1-5B11-72EC-A120-037A8A531815",
                "0198BCB1-5B11-72EC-A120-074FF5D8D8A9",
                "0198BCB1-5B11-72EC-A120-0A7430EA0458",
                "0198BCB1-5B11-72EC-A120-0D31AA10D539",
                "0198BCB1-5B11-72EC-A120-12A91239CA7C",
                "0198BCB1-5B11-72EC-A120-14B08F4AAEC3",
                "0198BCB1-5B11-72EC-A120-194000EE636A",
                "0198BCB1-5B11-72EC-A120-1D9195459255",
                "0198BCB1-5B11-72EC-A120-221BD1102544",
                "0198BCB1-5B11-72EC-A120-257C2DDCC24F",
                "0198BCB1-5B11-72EC-A120-2BAC64F8175A",
                "0198BCB1-5B11-72EC-A120-2FA24D8AB9F4",
                "0198BCB1-5B11-72EC-A120-3291DF194E00",
                "0198BCB1-5B11-72EC-A120-35482B97CCD3",
                "0198BCB1-5B11-72EC-A120-3B6B83BE6E7E",
                "0198BCB1-5B11-72EC-A120-3CDBB70215F8",
                "0198BCB1-5B11-72EC-A120-40DD104A16B7",
                "0198BCB1-5B11-72EC-A120-4579D369FCD6",
                "0198BCB1-5B11-72EC-A120-48BB994078D6",
                "0198BCB1-5B11-72EC-A120-4C92218751F4",
                "0198BCB1-5B11-72EC-A120-5392CD348099",
                "0198BCB1-5B11-72EC-A120-560D4DF02114",
                "0198BCB1-5B11-72EC-A120-58A606D1F445",
                "0198BCB1-5B11-72EC-A120-5F5BED6FB16B",
                "0198BCB1-5B11-72EC-A120-6330490EFFA6",
                "0198BCB1-5B11-72EC-A120-66B46B34A5C3",
                "0198BCB1-5B11-72EC-A120-69FEB6EF963D",
                "0198BCB1-5B11-72EC-A120-6F9EA95E4429",
                "0198BCB1-5B11-72EC-A120-71B0A6F2FD1C",
                "0198BCB1-5B11-72EC-A120-74A81FEF24A0",
                "0198BCB1-5B11-72EC-A120-794FCE805008",
                "0198BCB1-5B11-72EC-A120-7FDABC8EADAB",
                "0198BCB1-5B11-72EC-A120-82B6DA0A8B30",
                "0198BCB1-5B11-72EC-A120-85A6D14FBE26",
                "0198BCB1-5B11-72EC-A120-8A7F76F43703",
                "0198BCB1-5B11-72EC-A120-8E1657A94765",
                "0198BCB1-5B11-72EC-A120-909EC56CBE2E",
                "0198BCB1-5B11-72EC-A120-94BCD901FBCD",
                "0198BCB1-5B11-72EC-A120-9983171FB50C",
                "0198BCB1-5B11-72EC-A120-9C0295891CC5",
                "0198BCB1-5B11-72EC-A120-A3E389CC2898",
                "0198BCB1-5B11-72EC-A120-A5372A13F73C",
                "0198BCB1-5B11-72EC-A120-A8679166067E",
                "0198BCB1-5B11-72EC-A120-AEF0B85929CF",
                "0198BCB1-5B11-72EC-A120-B262D7B11630",
                "0198BCB1-5B11-72EC-A120-B5D358958DFF",
                "0198BCB1-5B11-72EC-A120-BAE76D67ACBB",
                "0198BCB1-5B11-72EC-A120-BFE173E9E943",
                "0198BCB1-5B11-72EC-A120-C3BB2EF2C2A3",
                "0198BCB1-5B11-72EC-A120-C78B0D48ECF0",
                "0198BCB1-5B11-72EC-A120-C97799A03957",
                "0198BCB1-5B11-72EC-A120-CD72122CB40C",
                "0198BCB1-5B11-72EC-A120-D2D073FD00B2",
                "0198BCB1-5B11-72EC-A120-D604164A9D65",
                "0198BCB1-5B11-72EC-A120-DBEFEA0EB15A",
                "0198BCB1-5B11-72EC-A120-DD0B38B70F71",
                "0198BCB1-5B11-72EC-A120-E2EE6E32E1D1",
                "0198BCB1-5B11-72EC-A120-E515585B38BC",
                "0198BCB1-5B11-72EC-A120-E8669DF57BC4",
                "0198BCB1-5B11-72EC-A120-EE1A24ED4D0F",
                "0198BCB1-5B11-72EC-A120-F1D259039818",
                "0198BCB1-5B11-72EC-A120-F7DAE553ACEC",
                "0198BCB1-5B11-72EC-A120-FAE9FBE81B0F",
                "0198BCB1-5B12-704A-9AFC-BB1FBE20B10C",
                "0198BCB1-5B12-704A-9AFC-BFB1D8408907",
                "0198BCB1-5B12-704A-9AFC-C10DE5C255D0",
                "0198BCB1-5B12-704A-9AFC-C716C76F22B6",
                "0198BCB1-5B12-704A-9AFC-CB9FE264DC24",
                "0198BCB1-5B12-704A-9AFC-CD1454F2383C",
                "0198BCB1-5B12-704A-9AFC-D057393DEBA3",
                "0198BCB1-5B12-704A-9AFC-D790690087B2",
                "0198BCB1-5B12-704A-9AFC-D9CE7AB8D8FA",
                "0198BCB1-5B12-704A-9AFC-DF003C73E21C",
                "0198BCB1-5B12-704A-9AFC-E004FA6F57B1",
                "0198BCB1-5B12-704A-9AFC-E4B6F0C301B1",
                "0198BCB1-5B12-704A-9AFC-E84F1742AECB",
                "0198BCB1-5B12-704A-9AFC-EFCA277EA541",
                "0198BCB1-5B12-704A-9AFC-F24E2DD665DB",
                "0198BCB1-5B12-704A-9AFC-F46770B88264",
                "0198BCB1-5B12-704A-9AFC-FA988E06DF92",
                "0198BCB1-5B12-704A-9AFC-FD4D5FF3BD0A",
                "0198BCB1-5B12-704A-9AFD-03C25467F798",
                "0198BCB1-5B12-704A-9AFD-06F29DC156D0",
                "0198BCB1-5B12-704A-9AFD-0BEDA30B7F0F",
                "0198BCB1-5B12-704A-9AFD-0E41324BB4BC"
            ];
            List<string> equipUuids = [
                "0198BBB27F43750AAA633CF1174268A1",
                "0198BBB27F43750AAA6340FCBBF5BF33",
                "0198BBB27F43750AAA6345770BCBF93C",
                "0198BBB27F43750AAA634906946E6EBC",
                "0198BBB27F43750AAA634EF5A2FC50DB",
                "0198BBB27F43750AAA6352613FB16651",
                "0198BBB27F43750AAA635535C0A5BEE0",
                "0198BBB27F43750AAA63584C9C88C667",
                "0198BBB27F43750AAA635E98280D5010",
                "0198BBB27F43750AAA636327E0C8B831"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                list.Add(new XT_314_SL_4_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 3,
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

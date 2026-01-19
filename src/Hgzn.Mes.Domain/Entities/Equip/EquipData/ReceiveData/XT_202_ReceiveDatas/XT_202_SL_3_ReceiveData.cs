using System.ComponentModel;
using Hgzn.Mes.Domain.Attributes;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_202_ReceiveDatas;

/// <summary>
/// _红外转台
/// </summary>
public class XT_202_SL_3_ReceiveData : UniversalEntity, IAudited
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

    #region 工作模式信息 3个

    [Description("本地还是远程")]
    [TableNotShow]
    public byte LocalOrRemote { get; set; }

    [Description("工作模式")]
    [TableNotShow]
    public byte WorkPattern { get; set; }

    [Description("预留")]
    [TableNotShow]
    public byte Reserved { get; set; }

    #endregion

    #region 健康状态信息 6个

    [Description("三轴转台滚动轴故障号")]
    [TableNotShow]
    public short RollAxisFaultCode { get; set; }

    [Description("三轴转台偏航轴故障号")]
    [TableNotShow]
    public short YawAxisFaultCode { get; set; }

    [Description("三轴转台俯仰轴故障号")]
    [TableNotShow]
    public short PitchAxisFaultCode { get; set; }

    [Description("两轴转台高低轴故障号")]
    [TableNotShow]
    public short ElevationAxisFaultCode { get; set; }

    [Description("两轴转台方位轴故障号")]
    [TableNotShow]
    public short AzimuthAxisFaultCode { get; set; }

    #endregion

    #region 物理量

    // 物理量参数数量
    [Description("物理量参数数量")]
    [TableNotShow]
    public uint PhysicalQuantityCount { get; set; }

    // 三轴转台控制参数
    [Description("三轴转台滚动轴给定")]
    public float ThreeAxisRollGiven { get; set; }
    [Description("三轴转台偏航轴给定")]
    public float ThreeAxisYawGiven { get; set; }
    [Description("三轴转台俯仰轴给定")]
    public float ThreeAxisPitchGiven { get; set; }

    // 两轴转台控制参数
    [Description("两轴转台方位轴给定")]
    public float TwoAxisAzimuthGiven { get; set; }
    [Description("两轴转台俯仰轴给定")]
    public float TwoAxisPitchGiven { get; set; }

    // 三轴转台反馈参数
    [Description("三轴转台滚动轴反馈")]
    public float ThreeAxisRollFeedback { get; set; }
    [Description("三轴转台偏航轴反馈")]
    public float ThreeAxisYawFeedback { get; set; }
    [Description("三轴转台俯仰轴反馈")]
    public float ThreeAxisPitchFeedback { get; set; }

    // 两轴转台反馈参数
    [Description("两轴转台方位轴反馈")]
    public float TwoAxisAzimuthFeedback { get; set; }
    [Description("两轴转台俯仰轴反馈")]
    public float TwoAxisPitchFeedback { get; set; }

    // 工作时长
    [Description("工作时长")]
    public float WorkingDuration { get; set; }

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
    public static XT_202_SL_3_ReceiveData[] Seeds
    {
        get
        {
            List<XT_202_SL_3_ReceiveData> list = [];
            List<string> uuids = [
                "0198BCE4-F796-7001-BD42-D1C40CA26DF0",
                "0198BCE4-F796-7001-BD42-D46AE50CEA9F",
                "0198BCE4-F796-7001-BD42-DBC66700BF7A",
                "0198BCE4-F796-7001-BD42-DDEA250574CF",
                "0198BCE4-F796-7001-BD42-E1AABA0FA86E",
                "0198BCE4-F796-7001-BD42-E78988C25CF9",
                "0198BCE4-F796-7001-BD42-E9CD4461D070",
                "0198BCE4-F796-7001-BD42-ECCBDAF8DAAD",
                "0198BCE4-F796-7001-BD42-F079E1FE8D2D",
                "0198BCE4-F796-7001-BD42-F4AD60F02E88",
                "0198BCE4-F796-7001-BD42-FACE7AD55DDD",
                "0198BCE4-F796-7001-BD42-FFEE0DE3B78F",
                "0198BCE4-F796-7001-BD43-01C782F3A83B",
                "0198BCE4-F796-7001-BD43-05B0A65D2F68",
                "0198BCE4-F796-7001-BD43-0A6E49FC0093",
                "0198BCE4-F796-7001-BD43-0DE8D5C0853B",
                "0198BCE4-F796-7001-BD43-1062B83F01E3",
                "0198BCE4-F796-7001-BD43-14C075D53D61",
                "0198BCE4-F796-7001-BD43-18D77C8D926F",
                "0198BCE4-F796-7001-BD43-1D10DBF6DBC8",
                "0198BCE4-F796-7001-BD43-203F690BD915",
                "0198BCE4-F796-7001-BD43-275C9F0BAD2E",
                "0198BCE4-F796-7001-BD43-29D601FBDD94",
                "0198BCE4-F796-7001-BD43-2C3DB310A6AE",
                "0198BCE4-F796-7001-BD43-338768080F7A",
                "0198BCE4-F796-7001-BD43-361A0CCAB376",
                "0198BCE4-F796-7001-BD43-39D35A5D019F",
                "0198BCE4-F796-7001-BD43-3FBD7C1F8A80",
                "0198BCE4-F796-7001-BD43-41FDF13D77C4",
                "0198BCE4-F796-7001-BD43-44FD54CB1089",
                "0198BCE4-F796-7001-BD43-4AC8FDBD5F7C",
                "0198BCE4-F796-7001-BD43-4DF812699BE5",
                "0198BCE4-F796-7001-BD43-5268C84B168A",
                "0198BCE4-F796-7001-BD43-567222DE7123",
                "0198BCE4-F796-7001-BD43-5B8E4FEAC725",
                "0198BCE4-F796-7001-BD43-5E2F7546A6E4",
                "0198BCE4-F796-7001-BD43-6240869DD873",
                "0198BCE4-F796-7001-BD43-65CCE0BBD702",
                "0198BCE4-F796-7001-BD43-693620CF3D79",
                "0198BCE4-F796-7001-BD43-6D5674E89CC1",
                "0198BCE4-F796-7001-BD43-72B65725DDE4",
                "0198BCE4-F796-7001-BD43-77A18A889F6E",
                "0198BCE4-F796-7001-BD43-786C8D5D798B",
                "0198BCE4-F796-7001-BD43-7EC9C9654B97",
                "0198BCE4-F796-7001-BD43-8080CFCEDF2E",
                "0198BCE4-F796-7001-BD43-866C148785B3",
                "0198BCE4-F796-7001-BD43-89EE6F176A95",
                "0198BCE4-F796-7001-BD43-8E1D21F64AA0",
                "0198BCE4-F796-7001-BD43-935CC6C1FD5D",
                "0198BCE4-F796-7001-BD43-97627DB65C4F",
                "0198BCE4-F796-7001-BD43-9AC26E0E518B",
                "0198BCE4-F796-7001-BD43-9E54BE81E304",
                "0198BCE4-F796-7001-BD43-A273B19EA7D4",
                "0198BCE4-F796-7001-BD43-A6B62463F5DF",
                "0198BCE4-F796-7001-BD43-A9C1F3B63B99",
                "0198BCE4-F796-7001-BD43-AE2F32A31832",
                "0198BCE4-F796-7001-BD43-B31093379509",
                "0198BCE4-F796-7001-BD43-B661F6809E46",
                "0198BCE4-F796-7001-BD43-B9B30414708E",
                "0198BCE4-F796-7001-BD43-BEC9D1CDA0C7",
                "0198BCE4-F796-7001-BD43-C16C27B1C049",
                "0198BCE4-F796-7001-BD43-C4EE68713629",
                "0198BCE4-F796-7001-BD43-CBFE896A4652",
                "0198BCE4-F796-7001-BD43-CD92071944B4",
                "0198BCE4-F796-7001-BD43-D0CC8D519B94",
                "0198BCE4-F796-7001-BD43-D7F53624284C",
                "0198BCE4-F796-7001-BD43-DA9812042D17",
                "0198BCE4-F796-7001-BD43-DF846CDAEEED",
                "0198BCE4-F796-7001-BD43-E3C051F616BD",
                "0198BCE4-F796-7001-BD43-E4079CE9C32D",
                "0198BCE4-F796-7001-BD43-E8675EF85D50",
                "0198BCE4-F796-7001-BD43-EE6645AB5C8B",
                "0198BCE4-F796-7001-BD43-F1DB8CEDCA79",
                "0198BCE4-F796-7001-BD43-F73BC5C29A4E",
                "0198BCE4-F796-7001-BD43-FBE029DA5FFA",
                "0198BCE4-F796-7001-BD43-FFAABDC04990",
                "0198BCE4-F796-7001-BD44-03AB741251C5",
                "0198BCE4-F796-7001-BD44-06CFAC52E182",
                "0198BCE4-F796-7001-BD44-09461E83BCF3",
                "0198BCE4-F796-7001-BD44-0D0F32B882D3",
                "0198BCE4-F796-7001-BD44-12BED5E0F9AB",
                "0198BCE4-F796-7001-BD44-151D52569D97",
                "0198BCE4-F796-7001-BD44-19A4D6CC4B4F",
                "0198BCE4-F796-7001-BD44-1EAF75065F08",
                "0198BCE4-F796-7001-BD44-22943BAC5353",
                "0198BCE4-F796-7001-BD44-26962BD0B24B",
                "0198BCE4-F796-7001-BD44-2A1DA0D453B0",
                "0198BCE4-F796-7001-BD44-2F6209D94EF0",
                "0198BCE4-F796-7001-BD44-30A24198FB2F",
                "0198BCE4-F796-7001-BD44-35A31E1AD684",
                "0198BCE4-F796-7001-BD44-39893BF52AF0",
                "0198BCE4-F796-7001-BD44-3EA175283E57",
                "0198BCE4-F796-7001-BD44-403213E753E9",
                "0198BCE4-F796-7001-BD44-476599C198A6",
                "0198BCE4-F796-7001-BD44-4ACC66270A00",
                "0198BCE4-F796-7001-BD44-4E7D59A08B19",
                "0198BCE4-F796-7001-BD44-522CDAA3C77D",
                "0198BCE4-F796-7001-BD44-56148A62327D",
                "0198BCE4-F796-7001-BD44-5BEBEEE0C695",
                "0198BCE4-F796-7001-BD44-5F23D85C5477"
            ];
            List<string> equipUuids = [
                "0198BBB31BD8703DB5F2828621041CDB",
                "0198BBB31BD8703DB5F285386CA0D878",
                "0198BBB31BD8703DB5F288AF65B2F23D",
                "0198BBB31BD8703DB5F28C8B76FC1E95",
                "0198BBB31BD8703DB5F291CD663CF052",
                "0198BBB31BD8703DB5F29655E948778A",
                "0198BBB31BD9716DA5EB00A448257464",
                "0198BBB31BD9716DA5EB070A691D2E13",
                "0198BBB31BD9716DA5EB0BE00D3731AB",
                "0198BBB31BD9716DA5EB0EEAA93E06E1"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                list.Add(new XT_202_SL_3_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 7,
                    DevTypeld = 3,
                    Compld = equipUuids[rand.Next(equipUuids.Count - 1)],
                    LocalOrRemote = 1,
                    WorkPattern = 1,
                    Reserved = 1,
                    RollAxisFaultCode = 1,
                    YawAxisFaultCode = 1,
                    PitchAxisFaultCode = 1,
                    ElevationAxisFaultCode = 1,
                    AzimuthAxisFaultCode = 1,
                    PhysicalQuantityCount = 10,
                    ThreeAxisRollGiven = (float)((rand.NextDouble() - 0.5) * 360),
                    ThreeAxisYawGiven = (float)((rand.NextDouble() - 0.5) * 360),
                    ThreeAxisPitchGiven = (float)((rand.NextDouble() - 0.5) * 180),
                    ThreeAxisRollFeedback = (float)((rand.NextDouble() - 0.5) * 360),
                    ThreeAxisYawFeedback = (float)((rand.NextDouble() - 0.5) * 360),
                    ThreeAxisPitchFeedback = (float)((rand.NextDouble() - 0.5) * 180),
                    TwoAxisAzimuthGiven = (float)((rand.NextDouble() - 0.5) * 360),
                    TwoAxisPitchGiven = (float)((rand.NextDouble() - 0.5) * 180),
                    TwoAxisAzimuthFeedback = (float)((rand.NextDouble() - 0.5) * 360),
                    TwoAxisPitchFeedback = (float)((rand.NextDouble() - 0.5) * 180),
                    WorkingDuration = (float)(rand.NextDouble() * 1000),
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

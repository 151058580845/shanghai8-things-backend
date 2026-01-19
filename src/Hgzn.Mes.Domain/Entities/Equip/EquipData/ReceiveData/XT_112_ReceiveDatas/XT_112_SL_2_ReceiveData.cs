using System.ComponentModel;
using Hgzn.Mes.Domain.Attributes;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_112_ReceiveDatas;

/// <summary>
/// _雷达转台
/// </summary>
public class XT_112_SL_2_ReceiveData : UniversalEntity, IAudited
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

    [Description("运行状态")]
    [TableNotShow]
    public byte OperationStatus { get; set; }

    [Description("是否接入弹道状态")]
    [TableNotShow]
    public byte IsTrajectoryConnected { get; set; }

    [Description("预留")]
    [TableNotShow]
    public byte Reserved { get; set; }

    #endregion

    #region 健康状态信息 4个

    [Description("状态类型")]
    [TableNotShow]
    public byte StatusType { get; set; }

    [Description("滚转轴工作状态")]
    [TableNotShow]
    public byte RollingAxisOperationStatus { get; set; }
    [Description("俯仰轴工作状态")]
    [TableNotShow]
    public byte PitchAxisOperationStatus { get; set; }
    [Description("偏航轴工作状态")]
    [TableNotShow]
    public byte YawAxisOperationStatus { get; set; }

    #endregion

    #region 物理量

    [Description("物理量参数数量")]
    [TableNotShow]
    public uint PhysicalQuantityCount { get; set; }

    [Description("内框位置")]
    public float InnerFramePosition { get; set; }

    [Description("中框位置")]
    public float MiddleFramePosition { get; set; }

    [Description("外框位置")]
    public float OuterFramePosition { get; set; }

    [Description("内框速度")]
    public float InnerFrameVelocity { get; set; }

    [Description("中框速度")]
    public float MiddleFrameVelocity { get; set; }

    [Description("外框速度")]
    public float OuterFrameVelocity { get; set; }

    [Description("内框加速度")]
    public float InnerFrameAcceleration { get; set; }

    [Description("中框加速度")]
    public float MiddleFrameAcceleration { get; set; }

    [Description("外框加速度")]
    public float OuterFrameAcceleration { get; set; }

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
    public static XT_112_SL_2_ReceiveData[] Seeds
    {
        get
        {
            List<string> uuids = [
                "0198BC69-1F4E-717E-88D1-7ADC228DF09B",
                "0198BC69-1F4E-717E-88D1-7D225270C4CA",
                "0198BC69-1F4E-717E-88D1-8089CEE3CED0",
                "0198BC69-1F4E-717E-88D1-84D1C19375C8",
                "0198BC69-1F4E-717E-88D1-89BC175C70CA",
                "0198BC69-1F4E-717E-88D1-8C2B97EEDEB4",
                "0198BC69-1F4E-717E-88D1-9199E984B5CB",
                "0198BC69-1F4E-717E-88D1-979ADFBDD9DC",
                "0198BC69-1F4E-717E-88D1-98C764066EDB",
                "0198BC69-1F4E-717E-88D1-9E5942C6FC26",
                "0198BC69-1F4E-717E-88D1-A34C17122125",
                "0198BC69-1F4E-717E-88D1-A6DC0F12F48F",
                "0198BC69-1F4E-717E-88D1-A8190B853481",
                "0198BC69-1F4E-717E-88D1-ADC059D960EB",
                "0198BC69-1F4E-717E-88D1-B0907A25BE23",
                "0198BC69-1F4E-717E-88D1-B5FA80A98134",
                "0198BC69-1F4E-717E-88D1-B8B45C9DD20B",
                "0198BC69-1F4E-717E-88D1-BD9958478C9E",
                "0198BC69-1F4E-717E-88D1-C1CB81F03153",
                "0198BC69-1F4E-717E-88D1-C6C975CF6A95",
                "0198BC69-1F4E-717E-88D1-C877E9C59A03",
                "0198BC69-1F4E-717E-88D1-CF5DA8ADC11D",
                "0198BC69-1F4E-717E-88D1-D2229CFC4B33",
                "0198BC69-1F4E-717E-88D1-D5D7ED3F709D",
                "0198BC69-1F4E-717E-88D1-DB44AC8779AF",
                "0198BC69-1F4E-717E-88D1-DC3EA9F9F76B",
                "0198BC69-1F4E-717E-88D1-E1825B9ACBC9",
                "0198BC69-1F4E-717E-88D1-E46248C1B8F2",
                "0198BC69-1F4E-717E-88D1-E8CF6C142F5A",
                "0198BC69-1F4E-717E-88D1-EF7261566D63",
                "0198BC69-1F4E-717E-88D1-F19FC9E4617D",
                "0198BC69-1F4E-717E-88D1-F4E5E0C8EB9C",
                "0198BC69-1F4E-717E-88D1-FA062E36B6F0",
                "0198BC69-1F4E-717E-88D1-FEFBBC37B3F5",
                "0198BC69-1F4E-717E-88D2-0280CFD9EAA4",
                "0198BC69-1F4E-717E-88D2-056EC8B66A80",
                "0198BC69-1F4E-717E-88D2-0A82D429458D",
                "0198BC69-1F4E-717E-88D2-0EA66DF8F5B8",
                "0198BC69-1F4E-717E-88D2-132EE80BEB01",
                "0198BC69-1F4E-717E-88D2-15620ED915B0",
                "0198BC69-1F4E-717E-88D2-18A870E6C303",
                "0198BC69-1F4E-717E-88D2-1C886FC7FD3B",
                "0198BC69-1F4E-717E-88D2-21703AF42D65",
                "0198BC69-1F4E-717E-88D2-255B2A5CD647",
                "0198BC69-1F4E-717E-88D2-2A29074084E1",
                "0198BC69-1F4E-717E-88D2-2F6D7E07AE68",
                "0198BC69-1F4E-717E-88D2-3278E82C9194",
                "0198BC69-1F4E-717E-88D2-34FF300B7EA1",
                "0198BC69-1F4E-717E-88D2-3B197FF07A95",
                "0198BC69-1F4E-717E-88D2-3CAF0B8DC690",
                "0198BC69-1F4E-717E-88D2-418C403B66A3",
                "0198BC69-1F4E-717E-88D2-46F3805BF0A7",
                "0198BC69-1F4E-717E-88D2-4AE86E902E36",
                "0198BC69-1F4E-717E-88D2-4D4F688B4133",
                "0198BC69-1F4E-717E-88D2-511B2A986341",
                "0198BC69-1F4E-717E-88D2-56F3FD751EC9",
                "0198BC69-1F4E-717E-88D2-58165E064D54",
                "0198BC69-1F4E-717E-88D2-5DD6D6E8BFCA",
                "0198BC69-1F4E-717E-88D2-63BD779F5779",
                "0198BC69-1F4E-717E-88D2-67574B3A59BA",
                "0198BC69-1F4E-717E-88D2-6882AAFD391F",
                "0198BC69-1F4E-717E-88D2-6F373C05A005",
                "0198BC69-1F4E-717E-88D2-72545E8328EC",
                "0198BC69-1F4E-717E-88D2-77447E965C4E",
                "0198BC69-1F4E-717E-88D2-7993B65B6602",
                "0198BC69-1F4E-717E-88D2-7C29EE680315",
                "0198BC69-1F4E-717E-88D2-81DD3686C05B",
                "0198BC69-1F4E-717E-88D2-86414655EC84",
                "0198BC69-1F4E-717E-88D2-8B971DA69B46",
                "0198BC69-1F4E-717E-88D2-8D1AED32E286",
                "0198BC69-1F4E-717E-88D2-92AE9FF70881",
                "0198BC69-1F4E-717E-88D2-95467136EF92",
                "0198BC69-1F4E-717E-88D2-9889614DB4D3",
                "0198BC69-1F4E-717E-88D2-9E6F687DB9E6",
                "0198BC69-1F4E-717E-88D2-A3F39DF76C88",
                "0198BC69-1F4E-717E-88D2-A4EA902D10DE",
                "0198BC69-1F4E-717E-88D2-A99CF1D9C4F5",
                "0198BC69-1F4E-717E-88D2-AFF2E3E2AD85",
                "0198BC69-1F4E-717E-88D2-B3AC62AA2AAB",
                "0198BC69-1F4E-717E-88D2-B5EC7F868246",
                "0198BC69-1F4E-717E-88D2-B8014E43BEBE",
                "0198BC69-1F4E-717E-88D2-BE5DBD65229F",
                "0198BC69-1F4E-717E-88D2-C2566FCD7AED",
                "0198BC69-1F4E-717E-88D2-C7FF9ED355A5",
                "0198BC69-1F4E-717E-88D2-CAEFBF88EFA7",
                "0198BC69-1F4E-717E-88D2-CDD3232F0BDD",
                "0198BC69-1F4E-717E-88D2-D36C47061AD0",
                "0198BC69-1F4E-717E-88D2-D5F1F37B9A7D",
                "0198BC69-1F4E-717E-88D2-DA39522937B3",
                "0198BC69-1F4E-717E-88D2-DC4D8A7200D6",
                "0198BC69-1F4E-717E-88D2-E16DDE4970EE",
                "0198BC69-1F4E-717E-88D2-E76A489CEF36",
                "0198BC69-1F4E-717E-88D2-E9BB0D972F94",
                "0198BC69-1F4E-717E-88D2-ECEEBC5BFC7A",
                "0198BC69-1F4E-717E-88D2-F1CDA6FD8A89",
                "0198BC69-1F4E-717E-88D2-F60CB2B8E12C",
                "0198BC69-1F4E-717E-88D2-F813C984AAD9",
                "0198BC69-1F4E-717E-88D2-FD1CC72DF871",
                "0198BC69-1F4E-717E-88D3-003DC3DFD6B7",
                "0198BC69-1F4E-717E-88D3-04961D3F28FC"
            ];
            List<XT_112_SL_2_ReceiveData> list = [];
            List<string> equipUuids = [
                "0198BBB3A649726DBD9F1135B95E8335",
                "0198BBB3A649726DBD9F145DC874CE0D",
                "0198BBB3A649726DBD9F18E67BCEA3C9",
                "0198BBB3A649726DBD9F1C0EAA1630A7",
                "0198BBB3A649726DBD9F220C5EB1B9A5",
                "0198BBB3A649726DBD9F27B84B6B9874",
                "0198BBB3A649726DBD9F2A5E984296AE",
                "0198BBB3A649726DBD9F2CC6B10EB679",
                "0198BBB3A649726DBD9F31786CCEA17E",
                "0198BBB3A649726DBD9F35996AAD2AC9"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                var innerFrameVelocity = rand.NextDouble() * 40 + 20;
                var innerFrameAcceleration = rand.NextDouble() * 19.9 + 0.1;
                list.Add(new XT_112_SL_2_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 9,
                    DevTypeld = 2,
                    Compld = equipUuids[rand.Next(equipUuids.Count - 1)],
                    OperationStatus = 1,
                    IsTrajectoryConnected = 1,
                    Reserved = 1,
                    StatusType = 1,
                    RollingAxisOperationStatus = 1,
                    PitchAxisOperationStatus = 1,
                    YawAxisOperationStatus = 1,
                    PhysicalQuantityCount = 9,
                    InnerFramePosition = rand.Next(-180, 180),
                    MiddleFramePosition = rand.Next(-120, 120),
                    OuterFramePosition = rand.Next(-180, 180),
                    InnerFrameVelocity = (float)innerFrameVelocity,
                    MiddleFrameVelocity = (float)(rand.NextDouble() * ((innerFrameVelocity < 30 ? innerFrameVelocity : 30) - 0.1) + 0.1),
                    OuterFrameVelocity = (float)(rand.NextDouble() * 19.9 + 0.1),
                    InnerFrameAcceleration = (float)innerFrameAcceleration,
                    MiddleFrameAcceleration = (float)(rand.NextDouble() * ((innerFrameAcceleration < 10 ? innerFrameVelocity : 10) - 0.05) + 0.05),
                    OuterFrameAcceleration = (float)(rand.NextDouble() * 4.95 + 0.05),
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

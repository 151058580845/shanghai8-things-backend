using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_109_ReceiveDatas;

/// <summary>
/// 109_红外转台
/// </summary>
public class XT_109_SL_3_ReceiveData : UniversalEntity, IAudited
{
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
    [Description("仿真试验系统识别编码")]
    public byte SimuTestSysld { get; set; }

    [Description("设备类型识别编码")]
    public byte DevTypeld { get; set; }

    [Description("本机识别编码")]
    public string? Compld { get; set; }

    #region 工作信息模式 3个

    [Description("本地还是远程")]
    public byte LocalOrRemote { get; set; }

    [Description("工作模式")]
    public byte WorkPattern { get; set; }

    [Description("预留")]
    public byte Reserved { get; set; }

    #endregion

    #region 健康状态信息 6个

    [Description("状态类型")]
    public byte StatusType { get; set; }

    [Description("滚转轴工作状态")]
    public byte RollingAxisOperationStatus { get; set; }
    [Description("俯仰轴工作状态")]
    public byte PitchAxisOperationStatus { get; set; }
    [Description("偏航轴工作状态")]
    public byte YawAxisOperationStatus { get; set; }
    [Description("高低轴工作状态")]
    public byte ElevationAxisOperationStatus { get; set; }
    [Description("方位轴工作状态")]
    public byte AzimuthAxisOperationStatus { get; set; }

    #endregion

    #region 物理量

    // 物理量参数数量
    [Description("物理量参数数量")]
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
    [Description("两轴转台高低轴给定")]
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
    public float TwoAxisAzimuthFeedback1 { get; set; }
    [Description("两轴转台高低轴反馈")]
    public float TwoAxisPitchFeedback { get; set; }

    // 液压系统参数
    [Description("油压")]
    public float OilPressure { get; set; }
    [Description("油温")]
    public float OilTemperature { get; set; }

    #endregion

    [Description("运行时间")]
    public uint? RunTime { get; set; }

    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public int CreatorLevel { get; set; } = 0;

#if DEBUG
    public static XT_109_SL_3_ReceiveData[] Seeds
    {
        get
        {
            List<XT_109_SL_3_ReceiveData> list = [];
            List<string> uuids = [
                "0198BCDC-03DE-70F8-8065-3E37C7D9B08D",
                "0198BCDC-03DE-70F8-8065-40A44BD1F91C",
                "0198BCDC-03DE-70F8-8065-44A448780A4C",
                "0198BCDC-03DE-70F8-8065-4ACCA407B3CD",
                "0198BCDC-03DE-70F8-8065-4CF49876FA1D",
                "0198BCDC-03DE-70F8-8065-503EE351D28A",
                "0198BCDC-03DE-70F8-8065-565C7D54FE26",
                "0198BCDC-03DE-70F8-8065-5AC300E11D2B",
                "0198BCDC-03DE-70F8-8065-5CD7BC495B46",
                "0198BCDC-03DE-70F8-8065-6298DBB3747E",
                "0198BCDC-03DE-70F8-8065-64B844EE47CE",
                "0198BCDC-03DE-70F8-8065-69CDA6BAD7B9",
                "0198BCDC-03DE-70F8-8065-6E69504A42D9",
                "0198BCDC-03DE-70F8-8065-72EE310D8973",
                "0198BCDC-03DE-70F8-8065-77D790B1B391",
                "0198BCDC-03DE-70F8-8065-79030CCA2735",
                "0198BCDC-03DE-70F8-8065-7D2E29B348F6",
                "0198BCDC-03DE-70F8-8065-80AEF8E039A9",
                "0198BCDC-03DE-70F8-8065-87F48B561554",
                "0198BCDC-03DE-70F8-8065-89A6B2816130",
                "0198BCDC-03DE-70F8-8065-8F482EFE3B44",
                "0198BCDC-03DE-70F8-8065-90DAECCFF690",
                "0198BCDC-03DE-70F8-8065-9697EE11EB52",
                "0198BCDC-03DE-70F8-8065-9876C5413D6A",
                "0198BCDC-03DE-70F8-8065-9FF066BA960B",
                "0198BCDC-03DE-70F8-8065-A36E64CBC4E1",
                "0198BCDC-03DE-70F8-8065-A4CCAE98F19A",
                "0198BCDC-03DE-70F8-8065-AA707F041B8A",
                "0198BCDC-03DE-70F8-8065-ADB56D175AFB",
                "0198BCDC-03DE-70F8-8065-B231113A6D6A",
                "0198BCDC-03DE-70F8-8065-B7BD3DAD4B50",
                "0198BCDC-03DE-70F8-8065-BB4E942A9960",
                "0198BCDC-03DE-70F8-8065-BC0F1799750B",
                "0198BCDC-03DE-70F8-8065-C22DEE30E97B",
                "0198BCDC-03DE-70F8-8065-C592FCCA0C6F",
                "0198BCDC-03DE-70F8-8065-C9C461F80A2A",
                "0198BCDC-03DE-70F8-8065-CE271F1D3B20",
                "0198BCDC-03DE-70F8-8065-D314A325CB02",
                "0198BCDC-03DE-70F8-8065-D531293C5489",
                "0198BCDC-03DE-70F8-8065-DB60963B6F2E",
                "0198BCDC-03DE-70F8-8065-DE19E083AE19",
                "0198BCDC-03DE-70F8-8065-E298B275194D",
                "0198BCDC-03DE-70F8-8065-E657A7BC4C9B",
                "0198BCDC-03DE-70F8-8065-E982A28F90E5",
                "0198BCDC-03DE-70F8-8065-EE610BD47566",
                "0198BCDC-03DE-70F8-8065-F1C7BA0F6A66",
                "0198BCDC-03DE-70F8-8065-F77F79546E24",
                "0198BCDC-03DE-70F8-8065-FA13C68A1049",
                "0198BCDC-03DE-70F8-8065-FE2A7C0E038B",
                "0198BCDC-03DE-70F8-8066-01E23A792ECA",
                "0198BCDC-03DE-70F8-8066-04F7E3A17C75",
                "0198BCDC-03DE-70F8-8066-08F920B060B7",
                "0198BCDC-03DE-70F8-8066-0FB4396A39EC",
                "0198BCDC-03DE-70F8-8066-13A22D15632F",
                "0198BCDC-03DE-70F8-8066-160C57A91021",
                "0198BCDC-03DE-70F8-8066-1AB4D69A757C",
                "0198BCDC-03DE-70F8-8066-1C767027D25D",
                "0198BCDC-03DE-70F8-8066-22229452EBD1",
                "0198BCDC-03DE-70F8-8066-2502A63BB38E",
                "0198BCDC-03DE-70F8-8066-2907ECD160B9",
                "0198BCDC-03DE-70F8-8066-2ED63DB5382A",
                "0198BCDC-03DE-70F8-8066-32EEF645E10B",
                "0198BCDC-03DE-70F8-8066-35F98C71F131",
                "0198BCDC-03DE-70F8-8066-395937EF4BBE",
                "0198BCDC-03DE-70F8-8066-3FC04B0D7BB7",
                "0198BCDC-03DE-70F8-8066-41F3798245F7",
                "0198BCDC-03DE-70F8-8066-477C705FF4B4",
                "0198BCDC-03DE-70F8-8066-4BEC6EA76874",
                "0198BCDC-03DE-70F8-8066-4D0C519521BC",
                "0198BCDC-03DE-70F8-8066-5291A15DD851",
                "0198BCDC-03DE-70F8-8066-54C05C496410",
                "0198BCDC-03DE-70F8-8066-5A16CAA51D4B",
                "0198BCDC-03DE-70F8-8066-5F84A0A65B17",
                "0198BCDC-03DE-70F8-8066-635FED2F9AB2",
                "0198BCDC-03DE-70F8-8066-66A8FD461E81",
                "0198BCDC-03DE-70F8-8066-6B6BA645429F",
                "0198BCDC-03DE-70F8-8066-6F3F235A911C",
                "0198BCDC-03DE-70F8-8066-73AEB8CAC3B8",
                "0198BCDC-03DE-70F8-8066-7776E0BD37EC",
                "0198BCDC-03DE-70F8-8066-780F1DC9C2FA",
                "0198BCDC-03DE-70F8-8066-7CA4CECC1024",
                "0198BCDC-03DE-70F8-8066-826DD96D2D87",
                "0198BCDC-03DE-70F8-8066-840AD1D1A484",
                "0198BCDC-03DE-70F8-8066-8A9F96705B47",
                "0198BCDC-03DE-70F8-8066-8C925E7411A1",
                "0198BCDC-03DE-70F8-8066-93966C6EF791",
                "0198BCDC-03DE-70F8-8066-948A4D3E71A7",
                "0198BCDC-03DE-70F8-8066-98549AAA2701",
                "0198BCDC-03DE-70F8-8066-9FAC061BFBEA",
                "0198BCDC-03DE-70F8-8066-A040EFAEB838",
                "0198BCDC-03DE-70F8-8066-A5B5AEF5FBB0",
                "0198BCDC-03DE-70F8-8066-ABE68D6A26F6",
                "0198BCDC-03DE-70F8-8066-AD1E75E861F3",
                "0198BCDC-03DE-70F8-8066-B10384ADA125",
                "0198BCDC-03DE-70F8-8066-B59296A5175A",
                "0198BCDC-03DE-70F8-8066-BA83F0A5BE32",
                "0198BCDC-03DE-70F8-8066-BD044339ED0C",
                "0198BCDC-03DE-70F8-8066-C23F1010BCD5",
                "0198BCDC-03DE-70F8-8066-C5D37B3FA5A4",
                "0198BCDC-03DE-70F8-8066-CA510D0108FB"
            ];
            List<string> equipUuids = [
                "0198BBB27F43750AAA6366EB51DBD90E",
                "0198BBB27F43750AAA6368681DED616B",
                "0198BBB27F43750AAA636E27CA6D1B2F",
                "0198BBB27F43750AAA637162B3FA1A3A",
                "0198BBB27F43750AAA63756026BBB238",
                "0198BBB27F43750AAA63783107083E6D",
                "0198BBB27F43750AAA637D9D168FE765",
                "0198BBB27F43750AAA6381B8D2033FD8",
                "0198BBB27F43750AAA63847A9A146AF9",
                "0198BBB27F43750AAA638A9490329F2F"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                list.Add(new XT_109_SL_3_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 4,
                    DevTypeld = 3,
                    Compld = equipUuids[rand.Next(equipUuids.Count - 1)],
                    LocalOrRemote = 1,
                    WorkPattern = 1,
                    Reserved = 1,
                    StatusType = 1,
                    RollingAxisOperationStatus = 1,
                    PitchAxisOperationStatus = 1,
                    YawAxisOperationStatus = 1,
                    ElevationAxisOperationStatus = 1,
                    AzimuthAxisOperationStatus = 1,
                    PhysicalQuantityCount = 12,
                    ThreeAxisRollGiven = (float)((rand.NextDouble() - 0.5) * 360),
                    ThreeAxisYawGiven = (float)((rand.NextDouble() - 0.5) * 360),
                    ThreeAxisPitchGiven = (float)((rand.NextDouble() - 0.5) * 180),
                    ThreeAxisRollFeedback = (float)((rand.NextDouble() - 0.5) * 360),
                    ThreeAxisYawFeedback = (float)((rand.NextDouble() - 0.5) * 360),
                    ThreeAxisPitchFeedback = (float)((rand.NextDouble() - 0.5) * 180),
                    TwoAxisAzimuthGiven = (float)((rand.NextDouble() - 0.5) * 360),
                    TwoAxisPitchGiven = (float)((rand.NextDouble() - 0.5) * 180),
                    TwoAxisAzimuthFeedback1 = (float)((rand.NextDouble() - 0.5) * 360),
                    TwoAxisPitchFeedback = (float)((rand.NextDouble() - 0.5) * 180),
                    OilPressure = (float)(rand.NextDouble() * 4 + 3),
                    OilTemperature = (float)(rand.NextDouble() * 20 + 30),
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

using System.ComponentModel;
using Hgzn.Mes.Domain.Attributes;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_121_ReceiveDatas;

/// <summary>
/// 121_红外转台
/// </summary>
public class XT_121_SL_3_ReceiveData : UniversalEntity, IAudited
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

    #region 工作信息模式

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

    #region 健康状态信息

    [Description("状态类型")]
    [TableNotShow]
    public byte StatusType { get; set; }

    [Description("滚转轴工作状态")]
    [TableNotShow]
    public byte RollingAxisOperationStatus { get; set; }
    [Description("偏航轴工作状态")]
    [TableNotShow]
    public byte YawAxisOperationStatus { get; set; }
    [Description("俯仰轴工作状态")]
    [TableNotShow]
    public byte PitchAxisOperationStatus { get; set; }
    [Description("高低轴工作状态")]
    [TableNotShow]
    public byte ElevationAxisOperationStatus { get; set; }
    [Description("方位轴工作状态")]
    [TableNotShow]
    public byte AzimuthAxisOperationStatus { get; set; }

    #endregion

    #region 物理量

    // 物理量参数数量
    [Description("物理量参数数量")]
    [TableNotShow]
    public uint PhysicalQuantityCount { get; set; }

    // 三轴转台给定值
    [Description("三轴转台滚动轴给定")]
    public float ThreeAxisRollGiven { get; set; }
    [Description("三轴转台偏航轴给定")]
    public float ThreeAxisYawGiven { get; set; }
    [Description("三轴转台俯仰轴给定")]
    public float ThreeAxisPitchGiven { get; set; }

    // 两轴转台给定值
    [Description("两轴转台高低轴给定")]
    public float TwoAxisElevationGiven { get; set; }
    [Description("两轴转台方位轴给定")]
    public float TwoAxisPitchGiven { get; set; }

    // 三轴转台反馈值
    [Description("三轴转台滚动轴反馈")]
    public float ThreeAxisRollFeedback { get; set; }
    [Description("三轴转台偏航轴反馈")]
    public float ThreeAxisYawFeedback { get; set; }
    [Description("三轴转台俯仰轴反馈")]
    public float ThreeAxisPitchFeedback { get; set; }

    // 两轴转台反馈值
    [Description("两轴转台高低轴反馈")]
    public float TwoAxisElevationFeedback { get; set; }
    [Description("两轴转台方位轴反馈")]
    public float TwoAxisPitchFeedback { get; set; }

    // 周期
    [Description("周期")]
    public float Period { get; set; }

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
    public static XT_121_SL_3_ReceiveData[] Seeds
    {
        get
        {
            List<XT_121_SL_3_ReceiveData> list = [];
            List<string> uuids = [
                "0198BCE0-D322-7564-BE60-68ADC9C7E837",
                "0198BCE0-D322-7564-BE60-6E103D4C84CC",
                "0198BCE0-D322-7564-BE60-72EECAB4581A",
                "0198BCE0-D322-7564-BE60-742CCD9A4AFA",
                "0198BCE0-D322-7564-BE60-7872819354BE",
                "0198BCE0-D322-7564-BE60-7D076A3ADD45",
                "0198BCE0-D322-7564-BE60-817C656BB063",
                "0198BCE0-D322-7564-BE60-8507A26F4992",
                "0198BCE0-D322-7564-BE60-880355790F38",
                "0198BCE0-D322-7564-BE60-8D21618D7B6F",
                "0198BCE0-D322-7564-BE60-92FB5BF53714",
                "0198BCE0-D322-7564-BE60-95A8515612E0",
                "0198BCE0-D322-7564-BE60-9B584AC17D20",
                "0198BCE0-D322-7564-BE60-9E162306C4E4",
                "0198BCE0-D322-7564-BE60-A2AB0AFCE4F4",
                "0198BCE0-D322-7564-BE60-A615990A1CF0",
                "0198BCE0-D322-7564-BE60-ABE41BE547F5",
                "0198BCE0-D322-7564-BE60-AC494779B5EF",
                "0198BCE0-D322-7564-BE60-B0D878F1D2AC",
                "0198BCE0-D322-7564-BE60-B5AD28472F68",
                "0198BCE0-D322-7564-BE60-B80AEB905211",
                "0198BCE0-D322-7564-BE60-BFA1E7E8E11B",
                "0198BCE0-D322-7564-BE60-C0E536CCFE26",
                "0198BCE0-D322-7564-BE60-C43C41648ED7",
                "0198BCE0-D322-7564-BE60-CA79E6F28429",
                "0198BCE0-D322-7564-BE60-CC5D3A9539BE",
                "0198BCE0-D322-7564-BE60-D194F0967C2A",
                "0198BCE0-D322-7564-BE60-D60F8B88302E",
                "0198BCE0-D322-7564-BE60-D9A1BF9C842B",
                "0198BCE0-D322-7564-BE60-DD822A1E988E",
                "0198BCE0-D322-7564-BE60-E2EF4E2E3B7C",
                "0198BCE0-D322-7564-BE60-E6BBAC9ECC32",
                "0198BCE0-D322-7564-BE60-EB40E3BDB4A1",
                "0198BCE0-D322-7564-BE60-EDD2A2AFC69C",
                "0198BCE0-D322-7564-BE60-F159AB6D4344",
                "0198BCE0-D322-7564-BE60-F6EA11DC2181",
                "0198BCE0-D322-7564-BE60-F95B68CB40FD",
                "0198BCE0-D322-7564-BE60-FC76D1B95600",
                "0198BCE0-D322-7564-BE61-02EF0742B4B3",
                "0198BCE0-D322-7564-BE61-06A9036C9E5D",
                "0198BCE0-D322-7564-BE61-083BE49F5C1F",
                "0198BCE0-D322-7564-BE61-0EB4E66898B2",
                "0198BCE0-D322-7564-BE61-1359E65A5D53",
                "0198BCE0-D322-7564-BE61-143840C08620",
                "0198BCE0-D322-7564-BE61-19E967D8B417",
                "0198BCE0-D322-7564-BE61-1CA2D681A025",
                "0198BCE0-D322-7564-BE61-22DDCF2B7F2B",
                "0198BCE0-D322-7564-BE61-24E7DE0DDBA3",
                "0198BCE0-D322-7564-BE61-2A6F2DEC947F",
                "0198BCE0-D322-7564-BE61-2F4391BDEADD",
                "0198BCE0-D322-7564-BE61-32A4B9F0DAC4",
                "0198BCE0-D322-7564-BE61-374DCE795641",
                "0198BCE0-D322-7564-BE61-3B31F791D039",
                "0198BCE0-D322-7564-BE61-3E0DC5989D6E",
                "0198BCE0-D322-7564-BE61-4229D8F1A79E",
                "0198BCE0-D322-7564-BE61-456647067A60",
                "0198BCE0-D322-7564-BE61-4BFB505D9262",
                "0198BCE0-D322-7564-BE61-4C8F6FDBC48B",
                "0198BCE0-D322-7564-BE61-53F4DAFA0422",
                "0198BCE0-D322-7564-BE61-578D03483820",
                "0198BCE0-D322-7564-BE61-5BDCB71BEBDC",
                "0198BCE0-D322-7564-BE61-5EC07CAF7007",
                "0198BCE0-D322-7564-BE61-62A005DC4CD7",
                "0198BCE0-D322-7564-BE61-66DAF9083C42",
                "0198BCE0-D322-7564-BE61-684B2824D743",
                "0198BCE0-D322-7564-BE61-6D640DE0C5C8",
                "0198BCE0-D322-7564-BE61-7050145E28D3",
                "0198BCE0-D322-7564-BE61-768835E167BA",
                "0198BCE0-D322-7564-BE61-7BC2AED22B15",
                "0198BCE0-D322-7564-BE61-7DBC969960FD",
                "0198BCE0-D322-7564-BE61-836C0ECD65F3",
                "0198BCE0-D322-7564-BE61-879EF4F7F81D",
                "0198BCE0-D322-7564-BE61-8BD7F9E9E0CA",
                "0198BCE0-D322-7564-BE61-8EF79D068F87",
                "0198BCE0-D322-7564-BE61-90CB3F47932D",
                "0198BCE0-D322-7564-BE61-964AF626A2CF",
                "0198BCE0-D322-7564-BE61-9963F81ABC22",
                "0198BCE0-D322-7564-BE61-9D376D3BD805",
                "0198BCE0-D322-7564-BE61-A20AC116116F",
                "0198BCE0-D322-7564-BE61-A560DE84190B",
                "0198BCE0-D322-7564-BE61-ABC350328F40",
                "0198BCE0-D322-7564-BE61-AE3937B9B083",
                "0198BCE0-D322-7564-BE61-B30FB36846DC",
                "0198BCE0-D322-7564-BE61-B6DE6D2238E6",
                "0198BCE0-D322-7564-BE61-BBD887B99387",
                "0198BCE0-D322-7564-BE61-BDDFFD9B36B2",
                "0198BCE0-D322-7564-BE61-C3D30130F4D8",
                "0198BCE0-D322-7564-BE61-C5B5BC168D88",
                "0198BCE0-D322-7564-BE61-CA3BDECE1A40",
                "0198BCE0-D322-7564-BE61-CD5BCA2A3747",
                "0198BCE0-D322-7564-BE61-D06DB2F593CF",
                "0198BCE0-D322-7564-BE61-D64AB5DE1F8A",
                "0198BCE0-D322-7564-BE61-D986FD30649F",
                "0198BCE0-D322-7564-BE61-DC3086CD0CE5",
                "0198BCE0-D322-7564-BE61-E02C83A54902",
                "0198BCE0-D322-7564-BE61-E6FB497549A3",
                "0198BCE0-D322-7564-BE61-EA7C4287DFA4",
                "0198BCE0-D322-7564-BE61-EC7BABF802AD",
                "0198BCE0-D322-7564-BE61-F12EDD3AF4DC",
                "0198BCE0-D322-7564-BE61-F570EC0BB252"
            ];
            List<string> equipUuids = [
                "0198BBB31BD8703DB5F208485E16C26F",
                "0198BBB31BD8703DB5F20F284C5C5BED",
                "0198BBB31BD8703DB5F211EDFCA68D4C",
                "0198BBB31BD8703DB5F216624839DEC8",
                "0198BBB31BD8703DB5F21B5BAC1F28E2",
                "0198BBB31BD8703DB5F21E7F3FC15D8C",
                "0198BBB31BD8703DB5F220FA9B7DFD14",
                "0198BBB31BD8703DB5F2271CBA89C90A",
                "0198BBB31BD8703DB5F22A7CD7D297D4",
                "0198BBB31BD8703DB5F22F0F28645B77"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                list.Add(new XT_121_SL_3_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 6,
                    DevTypeld = 3,
                    Compld = equipUuids[rand.Next(equipUuids.Count - 1)],
                    LocalOrRemote = 1,
                    WorkPattern = 1,
                    Reserved = 1,
                    StatusType = 1,
                    RollingAxisOperationStatus = 1,
                    YawAxisOperationStatus = 1,
                    PitchAxisOperationStatus = 1,
                    ElevationAxisOperationStatus = 1,
                    AzimuthAxisOperationStatus = 1,
                    PhysicalQuantityCount = 10,
                    ThreeAxisRollGiven = (float)((rand.NextDouble() - 0.5) * 360),
                    ThreeAxisYawGiven = (float)((rand.NextDouble() - 0.5) * 360),
                    ThreeAxisPitchGiven = (float)((rand.NextDouble() - 0.5) * 180),
                    ThreeAxisRollFeedback = (float)((rand.NextDouble() - 0.5) * 360),
                    ThreeAxisYawFeedback = (float)((rand.NextDouble() - 0.5) * 360),
                    ThreeAxisPitchFeedback = (float)((rand.NextDouble() - 0.5) * 180),
                    TwoAxisElevationGiven = (float)((rand.NextDouble() - 0.5) * 360),
                    TwoAxisPitchGiven = (float)((rand.NextDouble() - 0.5) * 180),
                    TwoAxisElevationFeedback = (float)((rand.NextDouble() - 0.5) * 360),
                    TwoAxisPitchFeedback = (float)((rand.NextDouble() - 0.5) * 180),
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

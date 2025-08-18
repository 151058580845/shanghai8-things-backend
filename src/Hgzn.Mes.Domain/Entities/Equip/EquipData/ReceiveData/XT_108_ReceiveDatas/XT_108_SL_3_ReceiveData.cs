using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_108_ReceiveDatas;

/// <summary>
/// _红外转台
/// </summary>
public class XT_108_SL_3_ReceiveData : UniversalEntity, IAudited
{
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }

    [Description("仿真试验系统识别编码")]
    public byte SimuTestSysld { get; set; }

    [Description("设备类型识别编码")]
    public byte DevTypeld { get; set; }

    [Description("本机识别编码")]
    public string? Compld { get; set; }

    #region 工作模式信息 3个

    [Description("本地还是远程")]
    public byte LocalOrRemote { get; set; }

    [Description("工作模式")]
    public byte WorkPattern { get; set; }

    [Description("预留")]
    public byte Reserved { get; set; }

    #endregion

    #region 健康状态信息 2个

    [Description("状态类型")]
    public byte StatusType { get; set; }

    [Description("工作状态")]
    public byte OperationStatus { get; set; }

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

    #endregion

    [Description("运行时间")]
    public uint? RunTime { get; set; }

    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public int CreatorLevel { get; set; } = 0;

#if DEBUG
    public static XT_108_SL_3_ReceiveData[] Seeds
    {
        get
        {
            List<XT_108_SL_3_ReceiveData> list = [];
            List<string> uuids = [
                "0198BCCB-E76C-7669-83DF-8D839284DE59",
                "0198BCCB-E76C-7669-83DF-927534A117E5",
                "0198BCCB-E76C-7669-83DF-9428D0470810",
                "0198BCCB-E76C-7669-83DF-9857955DDF6A",
                "0198BCCB-E76C-7669-83DF-9DC325E1D229",
                "0198BCCB-E76C-7669-83DF-A20428FB0AAE",
                "0198BCCB-E76C-7669-83DF-A72F22F0B081",
                "0198BCCB-E76C-7669-83DF-AB84A061F808",
                "0198BCCB-E76C-7669-83DF-AFE70995FD18",
                "0198BCCB-E76C-7669-83DF-B06A4C089EDC",
                "0198BCCB-E76C-7669-83DF-B61B702F5C57",
                "0198BCCB-E76C-7669-83DF-B878AF12456B",
                "0198BCCB-E76C-7669-83DF-BF8920056C79",
                "0198BCCB-E76C-7669-83DF-C0BAF7C846ED",
                "0198BCCB-E76C-7669-83DF-C46C7BC92548",
                "0198BCCB-E76C-7669-83DF-CB679CE48E83",
                "0198BCCB-E76C-7669-83DF-CF2C5C02EB5A",
                "0198BCCB-E76C-7669-83DF-D0A85C641290",
                "0198BCCB-E76C-7669-83DF-D60727CFB913",
                "0198BCCB-E76C-7669-83DF-D8C71B19F95A",
                "0198BCCB-E76C-7669-83DF-DD68E5D8768F",
                "0198BCCB-E76C-7669-83DF-E2306DCCFB70",
                "0198BCCB-E76C-7669-83DF-E56B01916BC4",
                "0198BCCB-E76C-7669-83DF-EB956037E648",
                "0198BCCB-E76C-7669-83DF-EFBA7852B7BE",
                "0198BCCB-E76C-7669-83DF-F113C85B334E",
                "0198BCCB-E76C-7669-83DF-F6423F9F1E32",
                "0198BCCB-E76C-7669-83DF-F9F56498E4D8",
                "0198BCCB-E76C-7669-83DF-FC6676BB78B2",
                "0198BCCB-E76C-7669-83E0-028ABD11EA8F",
                "0198BCCB-E76C-7669-83E0-06E3F8F2B25A",
                "0198BCCB-E76D-73CE-B554-1B77FD1ED65D",
                "0198BCCB-E76D-73CE-B554-1C3AF37FDC22",
                "0198BCCB-E76D-73CE-B554-23C70FDEE765",
                "0198BCCB-E76D-73CE-B554-25E1B667A49F",
                "0198BCCB-E76D-73CE-B554-28FC96074DD0",
                "0198BCCB-E76D-73CE-B554-2F8148369468",
                "0198BCCB-E76D-73CE-B554-331D9A3FF052",
                "0198BCCB-E76D-73CE-B554-3417E2CEEFEF",
                "0198BCCB-E76D-73CE-B554-390023F1E214",
                "0198BCCB-E76D-73CE-B554-3E4B5DDB2ABB",
                "0198BCCB-E76D-73CE-B554-411EE8B39E60",
                "0198BCCB-E76D-73CE-B554-440435E221F4",
                "0198BCCB-E76D-73CE-B554-4939DAA2572A",
                "0198BCCB-E76D-73CE-B554-4C2F535E5BFC",
                "0198BCCB-E76D-73CE-B554-513FF82D9434",
                "0198BCCB-E76D-73CE-B554-565331B1319C",
                "0198BCCB-E76D-73CE-B554-5BFDBB58A3B4",
                "0198BCCB-E76D-73CE-B554-5C809620D2D2",
                "0198BCCB-E76D-73CE-B554-626C12487704",
                "0198BCCB-E76D-73CE-B554-64477C5C47E8",
                "0198BCCB-E76D-73CE-B554-6B6AD395731F",
                "0198BCCB-E76D-73CE-B554-6FC1C5500BE6",
                "0198BCCB-E76D-73CE-B554-731B2F2CE7CB",
                "0198BCCB-E76D-73CE-B554-743AE04CB3C6",
                "0198BCCB-E76D-73CE-B554-7AF0FB8450FF",
                "0198BCCB-E76D-73CE-B554-7DBFACA7734A",
                "0198BCCB-E76D-73CE-B554-832F65600A0E",
                "0198BCCB-E76D-73CE-B554-876D43F9A53D",
                "0198BCCB-E76D-73CE-B554-88DA84BCF410",
                "0198BCCB-E76D-73CE-B554-8E4BB1899C89",
                "0198BCCB-E76D-73CE-B554-906143BEDAB5",
                "0198BCCB-E76D-73CE-B554-965474FDEDE3",
                "0198BCCB-E76D-73CE-B554-9BF60FB8945D",
                "0198BCCB-E76D-73CE-B554-9CDC622A1EBD",
                "0198BCCB-E76D-73CE-B554-A1D5BBE04D9F",
                "0198BCCB-E76D-73CE-B554-A5805DA47412",
                "0198BCCB-E76D-73CE-B554-A96B8E4ACD1C",
                "0198BCCB-E76D-73CE-B554-AC9C6E2E265B",
                "0198BCCB-E76D-73CE-B554-B1C37C6E953E",
                "0198BCCB-E76D-73CE-B554-B616AC7A4A99",
                "0198BCCB-E76D-73CE-B554-BB32759F03A1",
                "0198BCCB-E76D-73CE-B554-BE3CAB552826",
                "0198BCCB-E76D-73CE-B554-C11C71DDA252",
                "0198BCCB-E76D-73CE-B554-C6BC935A8782",
                "0198BCCB-E76D-73CE-B554-CBCDE9E2E46A",
                "0198BCCB-E76D-73CE-B554-CD6497465735",
                "0198BCCB-E76D-73CE-B554-D24A5782745C",
                "0198BCCB-E76D-73CE-B554-D7FED9830292",
                "0198BCCB-E76D-73CE-B554-D9506ECA820B",
                "0198BCCB-E76D-73CE-B554-DE9E67F117FB",
                "0198BCCB-E76D-73CE-B554-E17D152CA253",
                "0198BCCB-E76D-73CE-B554-E5FC0743A3BA",
                "0198BCCB-E76D-73CE-B554-EA5E7EF11061",
                "0198BCCB-E76D-73CE-B554-ED58618007F0",
                "0198BCCB-E76D-73CE-B554-F12C5E552E9B",
                "0198BCCB-E76D-73CE-B554-F68C54C44A7E",
                "0198BCCB-E76D-73CE-B554-F91EFFDC247B",
                "0198BCCB-E76D-73CE-B554-FD7D17573ECD",
                "0198BCCB-E76D-73CE-B555-005DA1C6B624",
                "0198BCCB-E76D-73CE-B555-0721BAB0F64B",
                "0198BCCB-E76D-73CE-B555-0B474F084431",
                "0198BCCB-E76D-73CE-B555-0F743A155FF9",
                "0198BCCB-E76D-73CE-B555-119F729280DF",
                "0198BCCB-E76D-73CE-B555-1481ABFCEF4A",
                "0198BCCB-E76D-73CE-B555-1A08DE8679D1",
                "0198BCCB-E76D-73CE-B555-1EEAAEBDC482",
                "0198BCCB-E76D-73CE-B555-23BB04F05D69",
                "0198BCCB-E76D-73CE-B555-27C004637223",
                "0198BCCB-E76D-73CE-B555-2B2764992D87"
            ];
            List<string> equipUuids = [
                "0198BBB31BD8703DB5F1927F5C308F3C",
                "0198BBB31BD8703DB5F197F6D4771AA8",
                "0198BBB31BD8703DB5F19AFFFC0126AC",
                "0198BBB31BD8703DB5F19E13F3D3EE86",
                "0198BBB31BD8703DB5F1A147B34710A9",
                "0198BBB31BD8703DB5F1A5A5985C568E",
                "0198BBB31BD8703DB5F1AB8D9971EC11",
                "0198BBB31BD8703DB5F1AFB3CECC104A",
                "0198BBB31BD8703DB5F1B3B8C76E21AB",
                "0198BBB31BD8703DB5F1B6278D59F351"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                list.Add(new XT_108_SL_3_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 5,
                    DevTypeld = 3,
                    Compld = equipUuids[rand.Next(equipUuids.Count - 1)],
                    LocalOrRemote = 1,
                    WorkPattern = 1,
                    Reserved = 1,
                    StatusType = 1,
                    OperationStatus = 1,
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

using System.ComponentModel;
using Hgzn.Mes.Domain.Attributes;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_310_ReceiveDatas;

/// <summary>
/// 310_雷达转台
/// </summary>
public class XT_310_SL_2_ReceiveData : UniversalEntity, IAudited
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

    #region 健康状态信息

    [Description("状态类型")]
    [TableNotShow]
    public byte StatusType { get; set; }

    [Description("自检状态")]
    [TableNotShow]
    public byte SelfTestStatus { get; set; }

    [Description("运行状态")]
    [TableNotShow]
    public byte HealthOperationStatus { get; set; }

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
    public static XT_310_SL_2_ReceiveData[] Seeds
    {
        get
        {
            List<string> uuids = [
                "0198BC7C-A4F1-768F-8325-2C8B223DE480",
                "0198BC7C-A4F1-768F-8325-316F1C814FAD",
                "0198BC7C-A4F1-768F-8325-371D8BF47075",
                "0198BC7C-A4F1-768F-8325-3ADE8D0B37ED",
                "0198BC7C-A4F1-768F-8325-3DEC1E70575D",
                "0198BC7C-A4F1-768F-8325-42F280506A5C",
                "0198BC7C-A4F1-768F-8325-479FE23B4454",
                "0198BC7C-A4F1-768F-8325-4980E7E7CB33",
                "0198BC7C-A4F1-768F-8325-4EEE79999CF0",
                "0198BC7C-A4F1-768F-8325-50B720A3B422",
                "0198BC7C-A4F1-768F-8325-577F56D0D631",
                "0198BC7C-A4F1-768F-8325-5813A2C9564D",
                "0198BC7C-A4F1-768F-8325-5C9F424DB07E",
                "0198BC7C-A4F1-768F-8325-60AEFA9A9360",
                "0198BC7C-A4F1-768F-8325-673C815E8850",
                "0198BC7C-A4F1-768F-8325-6A521F6FA0D0",
                "0198BC7C-A4F1-768F-8325-6E07EEDD5018",
                "0198BC7C-A4F1-768F-8325-71DCDA5D68C4",
                "0198BC7C-A4F1-768F-8325-766A7C80C15D",
                "0198BC7C-A4F1-768F-8325-788D8B6E9FE0",
                "0198BC7C-A4F1-768F-8325-7C55226EF5B2",
                "0198BC7C-A4F1-768F-8325-835FACA411D1",
                "0198BC7C-A4F1-768F-8325-87AB75862F29",
                "0198BC7C-A4F1-768F-8325-8B4542AF1731",
                "0198BC7C-A4F1-768F-8325-8FB58C8F4A0C",
                "0198BC7C-A4F1-768F-8325-907380F2871D",
                "0198BC7C-A4F1-768F-8325-9526D59C482F",
                "0198BC7C-A4F1-768F-8325-9A056285A648",
                "0198BC7C-A4F1-768F-8325-9D2B36CE41D0",
                "0198BC7C-A4F1-768F-8325-A3021648B608",
                "0198BC7C-A4F1-768F-8325-A61140BE2547",
                "0198BC7C-A4F1-768F-8325-AAC75EAE5C7A",
                "0198BC7C-A4F1-768F-8325-AF9D3E6BDD2E",
                "0198BC7C-A4F1-768F-8325-B048BAA31125",
                "0198BC7C-A4F1-768F-8325-B721CD1E8764",
                "0198BC7C-A4F1-768F-8325-B8086D408F22",
                "0198BC7C-A4F1-768F-8325-BE6A137011C3",
                "0198BC7C-A4F1-768F-8325-C362854AEE96",
                "0198BC7C-A4F1-768F-8325-C6362BB1A0A9",
                "0198BC7C-A4F1-768F-8325-C9ACB5607376",
                "0198BC7C-A4F1-768F-8325-CE297F6E058B",
                "0198BC7C-A4F1-768F-8325-D179B849B728",
                "0198BC7C-A4F1-768F-8325-D5A9A883CC23",
                "0198BC7C-A4F1-768F-8325-D87F9AC76988",
                "0198BC7C-A4F1-768F-8325-DD61983EA110",
                "0198BC7C-A4F1-768F-8325-E3F9CE772B1C",
                "0198BC7C-A4F1-768F-8325-E4B3F9B736A3",
                "0198BC7C-A4F1-768F-8325-EA9CCA17F5AA",
                "0198BC7C-A4F1-768F-8325-EF43155466D2",
                "0198BC7C-A4F1-768F-8325-F2E04B159EDA",
                "0198BC7C-A4F1-768F-8325-F54E09B3BB80",
                "0198BC7C-A4F1-768F-8325-FA6F161477DE",
                "0198BC7C-A4F1-768F-8325-FFBD4E01A497",
                "0198BC7C-A4F1-768F-8326-012280E3DEB6",
                "0198BC7C-A4F1-768F-8326-04FFE3F554A6",
                "0198BC7C-A4F1-768F-8326-080B0AA06F93",
                "0198BC7C-A4F1-768F-8326-0C37078156B7",
                "0198BC7C-A4F1-768F-8326-1216155BA80F",
                "0198BC7C-A4F1-768F-8326-1675467D7012",
                "0198BC7C-A4F1-768F-8326-1990140A5DD8",
                "0198BC7C-A4F1-768F-8326-1FAF69B8846D",
                "0198BC7C-A4F1-768F-8326-20D41CC76862",
                "0198BC7C-A4F1-768F-8326-268803A22E45",
                "0198BC7C-A4F1-768F-8326-2A5D235FB905",
                "0198BC7C-A4F1-768F-8326-2E2BB970606A",
                "0198BC7C-A4F1-768F-8326-3109CC5F41D7",
                "0198BC7C-A4F1-768F-8326-37440DF38274",
                "0198BC7C-A4F1-768F-8326-38F323351540",
                "0198BC7C-A4F1-768F-8326-3D0A1D7E3281",
                "0198BC7C-A4F1-768F-8326-43C6B46C9006",
                "0198BC7C-A4F1-768F-8326-46FE6EF7E13E",
                "0198BC7C-A4F1-768F-8326-4945AADF3697",
                "0198BC7C-A4F1-768F-8326-4C811FC63ECD",
                "0198BC7C-A4F1-768F-8326-523236BEA2AE",
                "0198BC7C-A4F1-768F-8326-558973F60D00",
                "0198BC7C-A4F1-768F-8326-599C06FA3127",
                "0198BC7C-A4F1-768F-8326-5C0304929313",
                "0198BC7C-A4F1-768F-8326-616030C59D47",
                "0198BC7C-A4F1-768F-8326-64177C490563",
                "0198BC7C-A4F1-768F-8326-69DC5BAA9949",
                "0198BC7C-A4F1-768F-8326-6C8C477E3341",
                "0198BC7C-A4F1-768F-8326-7122D6081099",
                "0198BC7C-A4F1-768F-8326-74DFDED2C3FD",
                "0198BC7C-A4F1-768F-8326-788775E00CA9",
                "0198BC7C-A4F1-768F-8326-7DEBBFAF22D4",
                "0198BC7C-A4F1-768F-8326-8025D6DE86E0",
                "0198BC7C-A4F1-768F-8326-850AA5D92BA3",
                "0198BC7C-A4F1-768F-8326-8B1A45306067",
                "0198BC7C-A4F1-768F-8326-8EAAA23B99D3",
                "0198BC7C-A4F1-768F-8326-91AC9C55BDFD",
                "0198BC7C-A4F1-768F-8326-961EC8DE6BB2",
                "0198BC7C-A4F1-768F-8326-98D35BC454C2",
                "0198BC7C-A4F1-768F-8326-9FE339178B9C",
                "0198BC7C-A4F1-768F-8326-A39F9D919128",
                "0198BC7C-A4F1-768F-8326-A7DDDAB2B77C",
                "0198BC7C-A4F1-768F-8326-A9FF118EF7F0",
                "0198BC7C-A4F1-768F-8326-AD3DA0297E05",
                "0198BC7C-A4F1-768F-8326-B133A10B9252",
                "0198BC7C-A4F1-768F-8326-B51AF1BFFB96",
                "0198BC7C-A4F1-768F-8326-BA46796C63AD"
            ];
            List<XT_310_SL_2_ReceiveData> list = [];
            List<string> equipUuids = [
                "0198BBB27F42727EB6F45022B4A43D01",
                "0198BBB27F42727EB6F45496B92E75E4",
                "0198BBB27F42727EB6F45B8AE3AC62D9",
                "0198BBB27F42727EB6F45C663EAE3336",
                "0198BBB27F42727EB6F460153153C45C",
                "0198BBB27F42727EB6F465C642145755",
                "0198BBB27F42727EB6F468E883E1A6B4",
                "0198BBB27F42727EB6F46D5550D5CFEE",
                "0198BBB27F42727EB6F4735AE47D3773",
                "0198BBB27F42727EB6F476D401A8E56E"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                var innerFrameVelocity = rand.NextDouble() * 40 + 20;
                var innerFrameAcceleration = rand.NextDouble() * 19.9 + 0.1;
                list.Add(new XT_310_SL_2_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 1,
                    DevTypeld = 2,
                    Compld = equipUuids[rand.Next(equipUuids.Count - 1)],
                    OperationStatus = 1,
                    IsTrajectoryConnected = 1,
                    Reserved = 1,
                    StatusType = 1,
                    SelfTestStatus = 1,
                    HealthOperationStatus = 1,
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

using System.ComponentModel;
using Hgzn.Mes.Domain.Attributes;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_103_ReceiveDatas;

/// <summary>
/// _雷达转台
/// </summary>
public class XT_103_SL_2_ReceiveData : UniversalEntity, IAudited
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

    #region 健康状态信息 3个

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
    public static XT_103_SL_2_ReceiveData[] Seeds
    {
        get
        {
            List<string> uuids = [
                "0198BC25-670B-7609-95E3-15DE6CE6C9E9",
                "0198BC25-670B-7609-95E3-1957130813E0",
                "0198BC25-670B-7609-95E3-1D1D3664056F",
                "0198BC25-670B-7609-95E3-21EAF942775E",
                "0198BC25-670B-7609-95E3-25D31D5AFFF7",
                "0198BC25-670B-7609-95E3-28764DD9163B",
                "0198BC25-670B-7609-95E3-2EE3D6B4E4B5",
                "0198BC25-670B-7609-95E3-30795A6BD5B1",
                "0198BC25-670B-7609-95E3-3555E90F3B0A",
                "0198BC25-670B-7609-95E3-38E2D0BDDA66",
                "0198BC25-670B-7609-95E3-3EDDE620DFA3",
                "0198BC25-670B-7609-95E3-40C19FBD3816",
                "0198BC25-670B-7609-95E3-46A06CD40B2E",
                "0198BC25-670B-7609-95E3-4A136BD48FE3",
                "0198BC25-670B-7609-95E3-4E82A70E26B0",
                "0198BC25-670B-7609-95E3-52D09EE8457D",
                "0198BC25-670B-7609-95E3-5402092EAA40",
                "0198BC25-670B-7609-95E3-5A89A73A3D9C",
                "0198BC25-670B-7609-95E3-5E358C3E8341",
                "0198BC25-670B-7609-95E3-603FB0A73DC8",
                "0198BC25-670B-7609-95E3-645951D3404D",
                "0198BC25-670B-7609-95E3-6B7B98BDAA2A",
                "0198BC25-670B-7609-95E3-6CF04A4AC8D9",
                "0198BC25-670B-7609-95E3-72DE5F878DF1",
                "0198BC25-670B-7609-95E3-7481850B21CC",
                "0198BC25-670B-7609-95E3-797C7B379EF7",
                "0198BC25-670B-7609-95E3-7F2693F2402D",
                "0198BC25-670B-7609-95E3-82B2DF6684C8",
                "0198BC25-670B-7609-95E3-84F0F3702325",
                "0198BC25-670B-7609-95E3-8A0957F3001B",
                "0198BC25-670B-7609-95E3-8EFDFE607F77",
                "0198BC25-670B-7609-95E3-921AD972CFBC",
                "0198BC25-670B-7609-95E3-97E5F7474DC5",
                "0198BC25-670B-7609-95E3-996307C7209C",
                "0198BC25-670B-7609-95E3-9C2E1D591747",
                "0198BC25-670B-7609-95E3-A0118B7F63B5",
                "0198BC25-670B-7609-95E3-A4A7F7B0731A",
                "0198BC25-670B-7609-95E3-AB4200DEE628",
                "0198BC25-670B-7609-95E3-AF07655AFBC5",
                "0198BC25-670B-7609-95E3-B01CD6952BFF",
                "0198BC25-670B-7609-95E3-B787A455992A",
                "0198BC25-670B-7609-95E3-BA864399DDDA",
                "0198BC25-670B-7609-95E3-BE2FFEEDEF34",
                "0198BC25-670B-7609-95E3-C125FC286CC5",
                "0198BC25-670B-7609-95E3-C454B5932759",
                "0198BC25-670B-7609-95E3-CB073BE021D4",
                "0198BC25-670B-7609-95E3-CF251586F66B",
                "0198BC25-670B-7609-95E3-D0E58D0FC54C",
                "0198BC25-670B-7609-95E3-D545835892DB",
                "0198BC25-670B-7609-95E3-DA8E534466CD",
                "0198BC25-670B-7609-95E3-DC0970A67A8C",
                "0198BC25-670B-7609-95E3-E293017EEA7C",
                "0198BC25-670B-7609-95E3-E53A37E0B819",
                "0198BC25-670B-7609-95E3-E8CCCAC4996E",
                "0198BC25-670B-7609-95E3-EC7A0F0AFB8E",
                "0198BC25-670B-7609-95E3-F39E99AE42A6",
                "0198BC25-670B-7609-95E3-F45584D2E06B",
                "0198BC25-670B-7609-95E3-F998200CA628",
                "0198BC25-670B-7609-95E3-FDD1FDA19AE3",
                "0198BC25-670B-7609-95E4-02BC4B083C8A",
                "0198BC25-670B-7609-95E4-06108722F09A",
                "0198BC25-670B-7609-95E4-0A452CACB9E1",
                "0198BC25-670B-7609-95E4-0CE6074F8D5D",
                "0198BC25-670B-7609-95E4-137199D24259",
                "0198BC25-670B-7609-95E4-17CCC850A8A1",
                "0198BC25-670B-7609-95E4-1A77DC7354EE",
                "0198BC25-670B-7609-95E4-1DDF40DED784",
                "0198BC25-670B-7609-95E4-23F1F29D6D51",
                "0198BC25-670B-7609-95E4-26178F4B8F5B",
                "0198BC25-670B-7609-95E4-28E70E37F5A2",
                "0198BC25-670B-7609-95E4-2DFBB8094A2D",
                "0198BC25-670B-7609-95E4-309376252CD2",
                "0198BC25-670B-7609-95E4-352BC995AA8D",
                "0198BC25-670B-7609-95E4-381646EC565C",
                "0198BC25-670B-7609-95E4-3F072A60B934",
                "0198BC25-670B-7609-95E4-42F68CB1482B",
                "0198BC25-670B-7609-95E4-44BAB143FFCF",
                "0198BC25-670B-7609-95E4-49D4C610703A",
                "0198BC25-670B-7609-95E4-4F105108E18E",
                "0198BC25-670B-7609-95E4-50911F68AF6B",
                "0198BC25-670B-7609-95E4-55CC5A85F4E9",
                "0198BC25-670B-7609-95E4-58C8FB545AD2",
                "0198BC25-670B-7609-95E4-5FD213B7C981",
                "0198BC25-670B-7609-95E4-63FE3F6999E2",
                "0198BC25-670B-7609-95E4-641A6538F1EF",
                "0198BC25-670B-7609-95E4-68A1A8A4700A",
                "0198BC25-670B-7609-95E4-6C0F8EA5C510",
                "0198BC25-670B-7609-95E4-737C494AEBDA",
                "0198BC25-670B-7609-95E4-75E22A635543",
                "0198BC25-670B-7609-95E4-78745B51E563",
                "0198BC25-670B-7609-95E4-7D5B4D0BA86D",
                "0198BC25-670B-7609-95E4-830DFED5758C",
                "0198BC25-670B-7609-95E4-85B72B507B9E",
                "0198BC25-670B-7609-95E4-88E1D58D1561",
                "0198BC25-670B-7609-95E4-8F6F5615F5F1",
                "0198BC25-670B-7609-95E4-91EBDC2C16B2",
                "0198BC25-670B-7609-95E4-96037609A797",
                "0198BC25-670B-7609-95E4-9A6C6FC5B9F7",
                "0198BC25-670B-7609-95E4-9F2D82122E73",
                "0198BC25-670B-7609-95E4-A1B06A38690F"
            ];
            List<XT_103_SL_2_ReceiveData> list = [];
            List<string> equipUuids = [
                "0198BBB3A649726DBD9EC0AD28E9E41E",
                "0198BBB3A649726DBD9EC7ABC009B742",
                "0198BBB3A649726DBD9ECA9DFCC610D9",
                "0198BBB3A649726DBD9ECE060E697007",
                "0198BBB3A649726DBD9ED1946AD220CC",
                "0198BBB3A649726DBD9ED497D886BB4D",
                "0198BBB3A649726DBD9EDBC4AE90A46B",
                "0198BBB3A649726DBD9EDD9B6A5909CA",
                "0198BBB3A649726DBD9EE24E65AF7F75",
                "0198BBB3A649726DBD9EE4DD99018535"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                var innerFrameVelocity = rand.NextDouble() * 40 + 20;
                var innerFrameAcceleration = rand.NextDouble() * 19.9 + 0.1;
                list.Add(new XT_103_SL_2_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 8,
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

using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_314_ReceiveDatas;

/// <summary>
/// 314_红外转台
/// </summary>
public class XT_314_SL_3_ReceiveData : UniversalEntity, IAudited
{
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }

    [Description("仿真试验系统识别编码")]
    public byte SimuTestSysld { get; set; }

    [Description("设备类型识别编码")]
    public byte DevTypeld { get; set; }

    [Description("本机识别编码")]
    public string? Compld { get; set; }

    #region 工作模式信息

    [Description("本地还是远程")]
    public byte LocalOrRemote { get; set; }

    [Description("工作模式")]
    public byte WorkPattern { get; set; }

    [Description("预留")]
    public byte Reserved { get; set; }

    #endregion

    #region 健康状态信息

    [Description("状态类型")]
    public byte StatusType { get; set; }

    [Description("消旋轴工作状态")]
    public byte RacemizationAxisOperationStatus { get; set; }
    [Description("短臂轴工作状态")]
    public byte ShortArmAxisOperationStatus { get; set; }
    [Description("长臂轴工作状态")]
    public byte LongArmAxisOperationStatus { get; set; }

    #endregion

    #region 物理量

    [Description("物理量参数数量")]
    public uint PhysicalParameterCount { get; set; }

    [Description("旋转轴给定")]
    public float RotationAxisCommand { get; set; }

    [Description("小臂轴给定")]
    public float ForearmAxisCommand { get; set; }

    [Description("大臂轴给定")]
    public float ArmAxisCommand { get; set; }

    [Description("消旋轴反馈")]
    public float DerotationAxisFeedback { get; set; }

    [Description("小臂轴反馈")]
    public float ForearmAxisFeedback { get; set; }

    [Description("大臂轴反馈")]
    public float ArmAxisFeedback { get; set; }
    [Description("周期")]
    public float Period { get; set; }

    #endregion

    [Description("运行时间")]
    public uint? RunTime { get; set; }

    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public int CreatorLevel { get; set; } = 0;

#if DEBUG
    public static XT_314_SL_3_ReceiveData[] Seeds
    {
        get
        {
            List<XT_314_SL_3_ReceiveData> list = [];
            List<string> uuids = [
                "0198BCEF-849E-73AF-AF17-1D0B03CE29B1",
                "0198BCEF-849E-73AF-AF17-22797CBE9B61",
                "0198BCEF-849E-73AF-AF17-250B291D2C01",
                "0198BCEF-849E-73AF-AF17-28AF731C3010",
                "0198BCEF-849E-73AF-AF17-2C8096658E8E",
                "0198BCEF-849E-73AF-AF17-33D455A6490A",
                "0198BCEF-849E-73AF-AF17-3448B219D2C8",
                "0198BCEF-849E-73AF-AF17-3A6DD43CA62B",
                "0198BCEF-849E-73AF-AF17-3D770DA116C7",
                "0198BCEF-849E-73AF-AF17-432C0C089490",
                "0198BCEF-849E-73AF-AF17-45DEA99F53FE",
                "0198BCEF-849E-73AF-AF17-4ADBAA199DEA",
                "0198BCEF-849E-73AF-AF17-4DBF180E345F",
                "0198BCEF-849E-73AF-AF17-5029FF508E8F",
                "0198BCEF-849E-73AF-AF17-56C6ED27E8B8",
                "0198BCEF-849E-73AF-AF17-5A82318FC424",
                "0198BCEF-849E-73AF-AF17-5F4E9224D819",
                "0198BCEF-849E-73AF-AF17-6393BA334AAD",
                "0198BCEF-849E-73AF-AF17-66F3924D2126",
                "0198BCEF-849E-73AF-AF17-6BBD185C424A",
                "0198BCEF-849E-73AF-AF17-6D8989D8A8DC",
                "0198BCEF-849E-73AF-AF17-716345F0843C",
                "0198BCEF-849F-77AE-A73F-D20B2099FF08",
                "0198BCEF-849F-77AE-A73F-D73ADF5DF2FA",
                "0198BCEF-849F-77AE-A73F-D97A6829C592",
                "0198BCEF-849F-77AE-A73F-DF657822F3B6",
                "0198BCEF-849F-77AE-A73F-E2388670B0B9",
                "0198BCEF-849F-77AE-A73F-E4773CB83E93",
                "0198BCEF-849F-77AE-A73F-E9946D824A08",
                "0198BCEF-849F-77AE-A73F-EE377E9EA219",
                "0198BCEF-849F-77AE-A73F-F35CBE02F129",
                "0198BCEF-849F-77AE-A73F-F41FA5DA0B8B",
                "0198BCEF-849F-77AE-A73F-F86B24FBCEA0",
                "0198BCEF-849F-77AE-A73F-FD0EF4FA7479",
                "0198BCEF-849F-77AE-A740-0197CB47B949",
                "0198BCEF-849F-77AE-A740-07DE7AB29AD9",
                "0198BCEF-849F-77AE-A740-08CC356F89C8",
                "0198BCEF-849F-77AE-A740-0E1C89ED688E",
                "0198BCEF-849F-77AE-A740-11488453C4BD",
                "0198BCEF-849F-77AE-A740-15EF1A899345",
                "0198BCEF-849F-77AE-A740-1AD597C220EC",
                "0198BCEF-849F-77AE-A740-1EA24DEEAFA6",
                "0198BCEF-849F-77AE-A740-230D39ECE4CB",
                "0198BCEF-849F-77AE-A740-26FB0AC9EB19",
                "0198BCEF-849F-77AE-A740-280A79B473E5",
                "0198BCEF-849F-77AE-A740-2E4713AB4A6F",
                "0198BCEF-849F-77AE-A740-32BB045607A9",
                "0198BCEF-849F-77AE-A740-341F2715F398",
                "0198BCEF-849F-77AE-A740-381067F67304",
                "0198BCEF-849F-77AE-A740-3CAEF22419B4",
                "0198BCEF-849F-77AE-A740-425E6692B717",
                "0198BCEF-849F-77AE-A740-443CB8BC7056",
                "0198BCEF-849F-77AE-A740-4ABEEDDC4E71",
                "0198BCEF-849F-77AE-A740-4E512ACB3CEF",
                "0198BCEF-849F-77AE-A740-53621D5433CD",
                "0198BCEF-849F-77AE-A740-54BCBDA70BF3",
                "0198BCEF-849F-77AE-A740-586156BB4983",
                "0198BCEF-849F-77AE-A740-5F5CCC105CB9",
                "0198BCEF-849F-77AE-A740-60E96672CF74",
                "0198BCEF-849F-77AE-A740-66533A070851",
                "0198BCEF-849F-77AE-A740-6ACD2F35BB60",
                "0198BCEF-849F-77AE-A740-6F96D679E952",
                "0198BCEF-849F-77AE-A740-71358CF0E848",
                "0198BCEF-849F-77AE-A740-76DF5A720E5F",
                "0198BCEF-849F-77AE-A740-79B13B67CF31",
                "0198BCEF-849F-77AE-A740-7F61F3AD2E30",
                "0198BCEF-849F-77AE-A740-813AE1B1EB8D",
                "0198BCEF-849F-77AE-A740-86E58183C96D",
                "0198BCEF-849F-77AE-A740-8A8701560543",
                "0198BCEF-849F-77AE-A740-8E868F27A9F0",
                "0198BCEF-849F-77AE-A740-90F0B90B963A",
                "0198BCEF-849F-77AE-A740-94A5B8FFF8FB",
                "0198BCEF-849F-77AE-A740-9B1BDB48601B",
                "0198BCEF-849F-77AE-A740-9F78763E9A14",
                "0198BCEF-849F-77AE-A740-A02CEC8EDB43",
                "0198BCEF-849F-77AE-A740-A651E5FCA164",
                "0198BCEF-849F-77AE-A740-ABF301174ABC",
                "0198BCEF-849F-77AE-A740-AC0FA6EB8BA3",
                "0198BCEF-849F-77AE-A740-B3E220E5AC2E",
                "0198BCEF-849F-77AE-A740-B61FDFAE5D7D",
                "0198BCEF-849F-77AE-A740-B8D5B630F3C9",
                "0198BCEF-849F-77AE-A740-BC80C6256D15",
                "0198BCEF-849F-77AE-A740-C15A61C9051D",
                "0198BCEF-849F-77AE-A740-C4D894126AC0",
                "0198BCEF-849F-77AE-A740-C8C25654ED56",
                "0198BCEF-849F-77AE-A740-CD4C6FC08675",
                "0198BCEF-849F-77AE-A740-D2F70660D981",
                "0198BCEF-849F-77AE-A740-D5EB0A84AE53",
                "0198BCEF-849F-77AE-A740-DA781AD926F6",
                "0198BCEF-849F-77AE-A740-DFA27B50B912",
                "0198BCEF-849F-77AE-A740-E281E894AD3F",
                "0198BCEF-849F-77AE-A740-E5DABB4F109C",
                "0198BCEF-849F-77AE-A740-E9A5B67B3917",
                "0198BCEF-849F-77AE-A740-EF62B7C4AF30",
                "0198BCEF-849F-77AE-A740-F29F70B550BC",
                "0198BCEF-849F-77AE-A740-F6AB6456F3DA",
                "0198BCEF-849F-77AE-A740-FA0BDC978834",
                "0198BCEF-849F-77AE-A740-FC100A727BB0",
                "0198BCEF-849F-77AE-A741-002067D56B48",
                "0198BCEF-849F-77AE-A741-056FC729DB41"
            ];
            List<string> equipUuids = [
                "0198BBB27F42727EB6F56BAEE80704D0",
                "0198BBB27F42727EB6F56EC65C7170A8",
                "0198BBB27F42727EB6F57106638AE14A",
                "0198BBB27F42727EB6F574F53363C1E5",
                "0198BBB27F42727EB6F57A8CE459EA21",
                "0198BBB27F42727EB6F57FD701A727D7",
                "0198BBB27F42727EB6F58167FCEABD52",
                "0198BBB27F42727EB6F58600DC3A1EAA",
                "0198BBB27F42727EB6F589FE4A56747C",
                "0198BBB27F43750AAA633ACE96ECC733"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                list.Add(new XT_314_SL_3_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 3,
                    DevTypeld = 3,
                    Compld = equipUuids[rand.Next(equipUuids.Count - 1)],
                    LocalOrRemote = 1,
                    WorkPattern = 1,
                    Reserved = 1,
                    StatusType = 1,
                    RacemizationAxisOperationStatus = 1,
                    ShortArmAxisOperationStatus = 1,
                    LongArmAxisOperationStatus = 1,
                    PhysicalParameterCount = 7,
                    RotationAxisCommand = (float)((rand.NextDouble() - 0.5) * 360),
                    ForearmAxisCommand = (float)((rand.NextDouble() - 0.5) * 60),
                    ArmAxisCommand = (float)((rand.NextDouble() - 0.5) * 180),
                    DerotationAxisFeedback = (float)((rand.NextDouble() - 0.5) * 360),
                    ForearmAxisFeedback = (float)((rand.NextDouble() - 0.5) * 60),
                    ArmAxisFeedback = (float)((rand.NextDouble() - 0.5) * 180),
                    Period = (float)(rand.NextDouble() * 9 + 1),
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

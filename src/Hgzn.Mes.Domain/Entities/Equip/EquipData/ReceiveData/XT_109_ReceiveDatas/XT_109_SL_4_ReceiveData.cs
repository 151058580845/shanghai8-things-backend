using System.ComponentModel;
using Hgzn.Mes.Domain.Attributes;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_109_ReceiveDatas;

/// <summary>
/// 109_固定电源
/// </summary>
public class XT_109_SL_4_ReceiveData : UniversalEntity, IAudited, IPowerSupplyData
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
    public static XT_109_SL_4_ReceiveData[] Seeds
    {
        get
        {
            List<XT_109_SL_4_ReceiveData> list = [];
            List<string> uuids = [
                "0198BC9B-EC80-712D-93EC-67839C11D94B",
                "0198BC9B-EC80-712D-93EC-6A9E8F724B74",
                "0198BC9B-EC80-712D-93EC-6FACF1D85BFD",
                "0198BC9B-EC80-712D-93EC-7388B5DBF5C4",
                "0198BC9B-EC80-712D-93EC-766BC232573A",
                "0198BC9B-EC80-712D-93EC-79EA1A0BF967",
                "0198BC9B-EC80-712D-93EC-7C9BB3551884",
                "0198BC9B-EC80-712D-93EC-82FB85799991",
                "0198BC9B-EC80-712D-93EC-878865D82631",
                "0198BC9B-EC80-712D-93EC-8920243E11BF",
                "0198BC9B-EC80-712D-93EC-8F60A42CCFB9",
                "0198BC9B-EC80-712D-93EC-922F461397BF",
                "0198BC9B-EC80-712D-93EC-9754056EEEDB",
                "0198BC9B-EC80-712D-93EC-9B59238DDA13",
                "0198BC9B-EC80-712D-93EC-9CC88DE31AC9",
                "0198BC9B-EC80-712D-93EC-A29076788769",
                "0198BC9B-EC80-712D-93EC-A7C8BDCCAF02",
                "0198BC9B-EC80-712D-93EC-AAB76C0C9E7B",
                "0198BC9B-EC80-712D-93EC-AC824CF49970",
                "0198BC9B-EC80-712D-93EC-B1D2453A84DD",
                "0198BC9B-EC80-712D-93EC-B528E5315EAE",
                "0198BC9B-EC80-712D-93EC-BA7C8FE6E3D5",
                "0198BC9B-EC80-712D-93EC-BFAA2544493D",
                "0198BC9B-EC80-712D-93EC-C056B7817AB4",
                "0198BC9B-EC80-712D-93EC-C641C93C03B5",
                "0198BC9B-EC80-712D-93EC-C899E96D29F9",
                "0198BC9B-EC80-712D-93EC-CF86B84912C4",
                "0198BC9B-EC80-712D-93EC-D23B34426D8B",
                "0198BC9B-EC80-712D-93EC-D71FC618D5B2",
                "0198BC9B-EC80-712D-93EC-D80D1926AC21",
                "0198BC9B-EC80-712D-93EC-DF6E6AE24F85",
                "0198BC9B-EC80-712D-93EC-E1EF7CC1DBE5",
                "0198BC9B-EC80-712D-93EC-E6058347AD82",
                "0198BC9B-EC80-712D-93EC-E9E71223E1CE",
                "0198BC9B-EC80-712D-93EC-EFD0EF36BD5B",
                "0198BC9B-EC80-712D-93EC-F307ABFD52DE",
                "0198BC9B-EC80-712D-93EC-F6B074823ED0",
                "0198BC9B-EC80-712D-93EC-F84C1E686CD3",
                "0198BC9B-EC80-712D-93EC-FCE2DC046B1E",
                "0198BC9B-EC80-712D-93ED-03771B4C2B53",
                "0198BC9B-EC80-712D-93ED-048F397F04F3",
                "0198BC9B-EC80-712D-93ED-0B1E84146C54",
                "0198BC9B-EC80-712D-93ED-0E67043C0F6C",
                "0198BC9B-EC80-712D-93ED-1294104C8F0B",
                "0198BC9B-EC80-712D-93ED-17B522822354",
                "0198BC9B-EC80-712D-93ED-1B89D9E089ED",
                "0198BC9B-EC80-712D-93ED-1C2426A4E43B",
                "0198BC9B-EC80-712D-93ED-2245EA6FE1DA",
                "0198BC9B-EC80-712D-93ED-26C56FC5A1F3",
                "0198BC9B-EC80-712D-93ED-28E784C52F55",
                "0198BC9B-EC80-712D-93ED-2E9BFDD0E8F3",
                "0198BC9B-EC80-712D-93ED-31B5712931C0",
                "0198BC9B-EC80-712D-93ED-37BD8212A9A3",
                "0198BC9B-EC80-712D-93ED-39EA87FB0735",
                "0198BC9B-EC80-712D-93ED-3E0D27A3F038",
                "0198BC9B-EC80-712D-93ED-418B32551A44",
                "0198BC9B-EC80-712D-93ED-4409C30F9312",
                "0198BC9B-EC80-712D-93ED-4BD23CD357DE",
                "0198BC9B-EC80-712D-93ED-4E34D0230AB6",
                "0198BC9B-EC80-712D-93ED-5145BAE7E9FE",
                "0198BC9B-EC80-712D-93ED-567B3EEDE116",
                "0198BC9B-EC80-712D-93ED-59B9D79322FD",
                "0198BC9B-EC80-712D-93ED-5DA7320277CD",
                "0198BC9B-EC80-712D-93ED-6091F3B7B016",
                "0198BC9B-EC80-712D-93ED-667243FD4677",
                "0198BC9B-EC80-712D-93ED-687EFB4241FF",
                "0198BC9B-EC80-712D-93ED-6FF316E6E223",
                "0198BC9B-EC80-712D-93ED-7029D7385DE7",
                "0198BC9B-EC80-712D-93ED-76F6CE860F5F",
                "0198BC9B-EC80-712D-93ED-7AD99CD08685",
                "0198BC9B-EC80-712D-93ED-7CBAC2752B2E",
                "0198BC9B-EC80-712D-93ED-815850446854",
                "0198BC9B-EC80-712D-93ED-84EE53A70BBC",
                "0198BC9B-EC80-712D-93ED-89381C1864B5",
                "0198BC9B-EC80-712D-93ED-8C6005ED59CE",
                "0198BC9B-EC80-712D-93ED-9301C2A92303",
                "0198BC9B-EC80-712D-93ED-96E4CF7E9EA7",
                "0198BC9B-EC80-712D-93ED-9837DC232B3D",
                "0198BC9B-EC80-712D-93ED-9D86E553D98E",
                "0198BC9B-EC80-712D-93ED-A1688EA86563",
                "0198BC9B-EC80-712D-93ED-A67310DA74E3",
                "0198BC9B-EC80-712D-93ED-A94E61C1E6DA",
                "0198BC9B-EC80-712D-93ED-AF68D87272EF",
                "0198BC9B-EC80-712D-93ED-B1AB1BDCAD47",
                "0198BC9B-EC80-712D-93ED-B471A3E2AE4F",
                "0198BC9B-EC80-712D-93ED-B96F5F1F0344",
                "0198BC9B-EC80-712D-93ED-BF0B89318ACB",
                "0198BC9B-EC80-712D-93ED-C30832C903EF",
                "0198BC9B-EC80-712D-93ED-C4B166DD8426",
                "0198BC9B-EC80-712D-93ED-C9DC62AEF2D7",
                "0198BC9B-EC80-712D-93ED-CC52593B2A15",
                "0198BC9B-EC80-712D-93ED-D20EC5EA2DCA",
                "0198BC9B-EC80-712D-93ED-D466EBDD0997",
                "0198BC9B-EC80-712D-93ED-D80A9BDB20A3",
                "0198BC9B-EC80-712D-93ED-DFE19293C719",
                "0198BC9B-EC80-712D-93ED-E28C5E5429C7",
                "0198BC9B-EC80-712D-93ED-E5FF76606E26",
                "0198BC9B-EC80-712D-93ED-E82E2A51217D",
                "0198BC9B-EC80-712D-93ED-ECED0D7D4C6F",
                "0198BC9B-EC80-712D-93ED-F0DA7B6F7136"
            ];
            List<string> equipUuids = [
                "0198BBB31BD8703DB5F140CDA96D2681",
                "0198BBB31BD8703DB5F145AC3BC2EFEB",
                "0198BBB31BD8703DB5F14A56651930F7",
                "0198BBB31BD8703DB5F14C9FA080359F",
                "0198BBB31BD8703DB5F1525B56FFA57B",
                "0198BBB31BD8703DB5F154F27879A72F",
                "0198BBB31BD8703DB5F15AE4E5AB2A3E",
                "0198BBB31BD8703DB5F15D3EDF854943",
                "0198BBB31BD8703DB5F160EE1442291B",
                "0198BBB31BD8703DB5F1674210F49C0C"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                list.Add(new XT_109_SL_4_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 4,
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

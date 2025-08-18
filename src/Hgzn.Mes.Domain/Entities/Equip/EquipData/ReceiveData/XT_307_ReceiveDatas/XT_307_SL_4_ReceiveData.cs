using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_307_ReceiveDatas;

/// <summary>
/// 307_固定电源
/// </summary>
public class XT_307_SL_4_ReceiveData : UniversalEntity, IAudited, IPowerSupplyData
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

    [Description("电源数量")]
    public byte PowerSupplyCount { get; set; }

    [Description("电源类型1")]
    public byte PowerSupplyType1 { get; set; }

    [Description("电源类型2")]
    public byte PowerSupplyType2 { get; set; }

    [Description("是否上电")]
    public byte IsPoweredOn { get; set; }

    [Description("预留")]
    public byte Reserved { get; set; }

    #endregion

    #region 健康状态信息

    [Description("状态类型")]
    public byte StatusType { get; set; }

    [Description("工作状态")]
    public byte OperationStatus { get; set; }

    #endregion

    #region 物理量

    [Description("物理量参数数量")]
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
    public uint? RunTime { get; set; }

    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public int CreatorLevel { get; set; } = 0;

#if DEBUG
    public static XT_307_SL_4_ReceiveData[] Seeds
    {
        get
        {
            List<XT_307_SL_4_ReceiveData> list = [];
            List<string> uuids = [
                "0198BCA9-681A-76DB-AE61-8C17DCFA1D2E",
                "0198BCA9-681A-76DB-AE61-92458C94CACC",
                "0198BCA9-681A-76DB-AE61-94B5A3E250C1",
                "0198BCA9-681A-76DB-AE61-99BBBE7DA359",
                "0198BCA9-681A-76DB-AE61-9F0D51FA02E6",
                "0198BCA9-681A-76DB-AE61-A105C19D4393",
                "0198BCA9-681A-76DB-AE61-A7D1A9748012",
                "0198BCA9-681A-76DB-AE61-A82FFB5B4C8E",
                "0198BCA9-681A-76DB-AE61-AF01C2F84D0D",
                "0198BCA9-681A-76DB-AE61-B0646743B22F",
                "0198BCA9-681A-76DB-AE61-B5C2C6A99492",
                "0198BCA9-681A-76DB-AE61-BB8EC5D1ACB7",
                "0198BCA9-681A-76DB-AE61-BF4BC244D316",
                "0198BCA9-681A-76DB-AE61-C2BC799F2EC7",
                "0198BCA9-681A-76DB-AE61-C50027E8F216",
                "0198BCA9-681A-76DB-AE61-CB130993E79A",
                "0198BCA9-681A-76DB-AE61-CCFA88495D80",
                "0198BCA9-681A-76DB-AE61-D0DF80C2B4ED",
                "0198BCA9-681A-76DB-AE61-D65A5D3A17C4",
                "0198BCA9-681A-76DB-AE61-D86D736799B2",
                "0198BCA9-681A-76DB-AE61-DE2338264843",
                "0198BCA9-681A-76DB-AE61-E389E4B23E7B",
                "0198BCA9-681A-76DB-AE61-E6DE2B236B24",
                "0198BCA9-681A-76DB-AE61-E9A45606601D",
                "0198BCA9-681A-76DB-AE61-EEADD2B393ED",
                "0198BCA9-681A-76DB-AE61-F1F584FE53FF",
                "0198BCA9-681A-76DB-AE61-F50AD5A12F2E",
                "0198BCA9-681A-76DB-AE61-F99E37C70700",
                "0198BCA9-681A-76DB-AE61-FF9898F03526",
                "0198BCA9-681A-76DB-AE62-025E780BE73A",
                "0198BCA9-681A-76DB-AE62-06AC9A84DDA4",
                "0198BCA9-681A-76DB-AE62-0AFB1C2B6896",
                "0198BCA9-681A-76DB-AE62-0F82F31BF345",
                "0198BCA9-681A-76DB-AE62-136793515070",
                "0198BCA9-681A-76DB-AE62-1771FD463322",
                "0198BCA9-681A-76DB-AE62-1940CD130B7E",
                "0198BCA9-681A-76DB-AE62-1FED152A63E0",
                "0198BCA9-681A-76DB-AE62-22646EE6B343",
                "0198BCA9-681A-76DB-AE62-27059E711CF8",
                "0198BCA9-681A-76DB-AE62-2B1953253BA0",
                "0198BCA9-681A-76DB-AE62-2EAEC75BF995",
                "0198BCA9-681A-76DB-AE62-31579D825C1B",
                "0198BCA9-681A-76DB-AE62-37E793F133FE",
                "0198BCA9-681A-76DB-AE62-38EC0A2757F5",
                "0198BCA9-681A-76DB-AE62-3FD8BEF21BCF",
                "0198BCA9-681A-76DB-AE62-41F961861207",
                "0198BCA9-681A-76DB-AE62-46F3C88553CF",
                "0198BCA9-681A-76DB-AE62-4822E242D755",
                "0198BCA9-681A-76DB-AE62-4F537716828D",
                "0198BCA9-681A-76DB-AE62-514332300089",
                "0198BCA9-681A-76DB-AE62-545A8D6A0BB9",
                "0198BCA9-681A-76DB-AE62-5B01F97B0FA4",
                "0198BCA9-681A-76DB-AE62-5FD4825C19B5",
                "0198BCA9-681A-76DB-AE62-63DEC11DBA54",
                "0198BCA9-681A-76DB-AE62-6589F23C8B77",
                "0198BCA9-681A-76DB-AE62-6A68369B8160",
                "0198BCA9-681A-76DB-AE62-6D4A6D0EE048",
                "0198BCA9-681A-76DB-AE62-70726BEE55E2",
                "0198BCA9-681A-76DB-AE62-75F8D09CB108",
                "0198BCA9-681A-76DB-AE62-79725407853D",
                "0198BCA9-681A-76DB-AE62-7F60F4D81D4C",
                "0198BCA9-681A-76DB-AE62-80DAE23D4AE7",
                "0198BCA9-681A-76DB-AE62-84DC436E15B4",
                "0198BCA9-681B-76CC-A73B-E1AE5A33173F",
                "0198BCA9-681B-76CC-A73B-E48CF1D2956F",
                "0198BCA9-681B-76CC-A73B-EA52D4DED66F",
                "0198BCA9-681B-76CC-A73B-ED84E3ABB046",
                "0198BCA9-681B-76CC-A73B-F284B3594DE6",
                "0198BCA9-681B-76CC-A73B-F4599D2410FD",
                "0198BCA9-681B-76CC-A73B-FA3212C849F7",
                "0198BCA9-681B-76CC-A73B-FED2DA32C0EC",
                "0198BCA9-681B-76CC-A73C-02BE9C7E37CE",
                "0198BCA9-681B-76CC-A73C-047FE01CA0EC",
                "0198BCA9-681B-76CC-A73C-08B938A987DA",
                "0198BCA9-681B-76CC-A73C-0C18A8A9BB27",
                "0198BCA9-681B-76CC-A73C-10EA366CE8C6",
                "0198BCA9-681B-76CC-A73C-171F4C41D071",
                "0198BCA9-681B-76CC-A73C-18F195DC2E94",
                "0198BCA9-681B-76CC-A73C-1E27E2B15B01",
                "0198BCA9-681B-76CC-A73C-22E648E1FF3B",
                "0198BCA9-681B-76CC-A73C-26FDA97545D0",
                "0198BCA9-681B-76CC-A73C-2A4B71AC9FB2",
                "0198BCA9-681B-76CC-A73C-2E3F155B5AE4",
                "0198BCA9-681B-76CC-A73C-32D03AA8D0CD",
                "0198BCA9-681B-76CC-A73C-3698AAE13BEA",
                "0198BCA9-681B-76CC-A73C-3BF7467449F6",
                "0198BCA9-681B-76CC-A73C-3CAF14AA5F27",
                "0198BCA9-681B-76CC-A73C-419640896C55",
                "0198BCA9-681B-76CC-A73C-44B4CB514FBE",
                "0198BCA9-681B-76CC-A73C-4AF5AD70F78A",
                "0198BCA9-681B-76CC-A73C-4E61FF17BCCD",
                "0198BCA9-681B-76CC-A73C-5111398817F2",
                "0198BCA9-681B-76CC-A73C-55956E901CAD",
                "0198BCA9-681B-76CC-A73C-5A881B9A9DAD",
                "0198BCA9-681B-76CC-A73C-5E0EB57FB7EC",
                "0198BCA9-681B-76CC-A73C-63EEC8F72E94",
                "0198BCA9-681B-76CC-A73C-677F50A6B2F0",
                "0198BCA9-681B-76CC-A73C-6A23E9F8304B",
                "0198BCA9-681B-76CC-A73C-6CF4F572F5A4",
                "0198BCA9-681B-76CC-A73C-7272BA6F4CC6"
            ];
            List<string> equipUuids = [
                "0198BBB27F42727EB6F4F0047E330F68",
                "0198BBB27F42727EB6F4F7DE4B5A4DB8",
                "0198BBB27F42727EB6F4FAE2D191CAB8",
                "0198BBB27F42727EB6F4FF6D958A5153",
                "0198BBB27F42727EB6F500E959E747AB",
                "0198BBB27F42727EB6F507FBBAD32384",
                "0198BBB27F42727EB6F50B6D25D90999",
                "0198BBB27F42727EB6F50D8555C0EAAB",
                "0198BBB27F42727EB6F513823C5D61D3",
                "0198BBB27F42727EB6F51702BBC8C010"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                list.Add(new XT_307_SL_4_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 2,
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

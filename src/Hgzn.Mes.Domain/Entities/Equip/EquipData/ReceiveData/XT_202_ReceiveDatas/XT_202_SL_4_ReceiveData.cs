using System.ComponentModel;
using System.Runtime.Intrinsics.Arm;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_202_ReceiveDatas;

/// <summary>
/// _固定电源
/// </summary>
public class XT_202_SL_4_ReceiveData : UniversalEntity, IAudited, IPowerSupplyData
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
    public static XT_202_SL_4_ReceiveData[] Seeds
    {
        get
        {
            List<XT_202_SL_4_ReceiveData> list = [];
            List<string> uuids = [
                "0198BCA6-F46F-722E-B360-8E48F640456B",
                "0198BCA6-F46F-722E-B360-9359BC83EF28",
                "0198BCA6-F46F-722E-B360-97D8344BBC91",
                "0198BCA6-F46F-722E-B360-9869A2420BBA",
                "0198BCA6-F46F-722E-B360-9D9594AA377E",
                "0198BCA6-F46F-722E-B360-A3D436C42A1E",
                "0198BCA6-F46F-722E-B360-A5CEF3E95D22",
                "0198BCA6-F46F-722E-B360-A9AB13D26C6F",
                "0198BCA6-F46F-722E-B360-AE472A1B1265",
                "0198BCA6-F46F-722E-B360-B0E1BD53CD63",
                "0198BCA6-F46F-722E-B360-B6803C4B773A",
                "0198BCA6-F46F-722E-B360-BB9ADD229A28",
                "0198BCA6-F46F-722E-B360-BC166D3DB628",
                "0198BCA6-F46F-722E-B360-C25244FEE202",
                "0198BCA6-F46F-722E-B360-C59E0B6D03C3",
                "0198BCA6-F46F-722E-B360-CB778B28B2FC",
                "0198BCA6-F46F-722E-B360-CFD394162F17",
                "0198BCA6-F46F-722E-B360-D366E17D19B6",
                "0198BCA6-F46F-722E-B360-D6101F3ACC48",
                "0198BCA6-F46F-722E-B360-DA67862E7205",
                "0198BCA6-F46F-722E-B360-DEAB910FD7F4",
                "0198BCA6-F46F-722E-B360-E37068F9E979",
                "0198BCA6-F46F-722E-B360-E462C18737DC",
                "0198BCA6-F46F-722E-B360-E8295DAE3248",
                "0198BCA6-F46F-722E-B360-EC01667E0124",
                "0198BCA6-F46F-722E-B360-F1A54BE65F18",
                "0198BCA6-F46F-722E-B360-F62C5FDB4CEB",
                "0198BCA6-F46F-722E-B360-F96D21BE9A32",
                "0198BCA6-F46F-722E-B360-FD350E2F2B31",
                "0198BCA6-F46F-722E-B361-03175995C858",
                "0198BCA6-F46F-722E-B361-06E3AE99D434",
                "0198BCA6-F46F-722E-B361-0BB934C1B062",
                "0198BCA6-F46F-722E-B361-0EBB12F99BE4",
                "0198BCA6-F46F-722E-B361-11F80F83BD6C",
                "0198BCA6-F46F-722E-B361-156F6ACCB1FE",
                "0198BCA6-F46F-722E-B361-198682ADB47A",
                "0198BCA6-F46F-722E-B361-1E95A2D7954F",
                "0198BCA6-F46F-722E-B361-21D0AD8E9DF4",
                "0198BCA6-F46F-722E-B361-249795271538",
                "0198BCA6-F46F-722E-B361-299824F85A57",
                "0198BCA6-F46F-722E-B361-2D3B1AD0E815",
                "0198BCA6-F46F-722E-B361-30ECCF8DE786",
                "0198BCA6-F46F-722E-B361-379761415377",
                "0198BCA6-F46F-722E-B361-3952BB441D09",
                "0198BCA6-F46F-722E-B361-3CBA53CA374E",
                "0198BCA6-F46F-722E-B361-40E5266310D8",
                "0198BCA6-F46F-722E-B361-461796D93A29",
                "0198BCA6-F46F-722E-B361-48C4CDAFBB6F",
                "0198BCA6-F46F-722E-B361-4E38D5F4AAA0",
                "0198BCA6-F46F-722E-B361-53DF198AB52C",
                "0198BCA6-F46F-722E-B361-5777EE56481F",
                "0198BCA6-F46F-722E-B361-58D22E0CEC3D",
                "0198BCA6-F46F-722E-B361-5CB61BABAD3B",
                "0198BCA6-F46F-722E-B361-62C172147C7D",
                "0198BCA6-F46F-722E-B361-654AC972E1F7",
                "0198BCA6-F46F-722E-B361-68A162B4C833",
                "0198BCA6-F46F-722E-B361-6C720EB6F8F9",
                "0198BCA6-F46F-722E-B361-71AD2B34083D",
                "0198BCA6-F46F-722E-B361-76C59302301A",
                "0198BCA6-F46F-722E-B361-7B826495904B",
                "0198BCA6-F46F-722E-B361-7C4D920BD790",
                "0198BCA6-F46F-722E-B361-8058EF873C9D",
                "0198BCA6-F46F-722E-B361-85BFA9B7FA2E",
                "0198BCA6-F46F-722E-B361-88F2A53EA8E6",
                "0198BCA6-F46F-722E-B361-8E6EC22696C9",
                "0198BCA6-F46F-722E-B361-912DA7946337",
                "0198BCA6-F46F-722E-B361-94ADB8D7B209",
                "0198BCA6-F46F-722E-B361-9BAFC46E1293",
                "0198BCA6-F46F-722E-B361-9CBB1C22C2EF",
                "0198BCA6-F46F-722E-B361-A09E796B2B54",
                "0198BCA6-F46F-722E-B361-A5122076FA74",
                "0198BCA6-F46F-722E-B361-A8C2701BE0CF",
                "0198BCA6-F46F-722E-B361-AC54B664FEC6",
                "0198BCA6-F46F-722E-B361-B094759220EB",
                "0198BCA6-F46F-722E-B361-B42AEBCAF77C",
                "0198BCA6-F46F-722E-B361-B8F34D7A993D",
                "0198BCA6-F46F-722E-B361-BE54D28D9CA3",
                "0198BCA6-F46F-722E-B361-C25871695B89",
                "0198BCA6-F46F-722E-B361-C4F707D64661",
                "0198BCA6-F46F-722E-B361-CAF22FBA1642",
                "0198BCA6-F46F-722E-B361-CF9220B3B980",
                "0198BCA6-F46F-722E-B361-D0D5B544BC1E",
                "0198BCA6-F46F-722E-B361-D5CE92FE525A",
                "0198BCA6-F46F-722E-B361-D8E592D33F83",
                "0198BCA6-F46F-722E-B361-DDF6B8F4659D",
                "0198BCA6-F46F-722E-B361-E040828CD391",
                "0198BCA6-F46F-722E-B361-E4C44EB962C4",
                "0198BCA6-F46F-722E-B361-EA87FC9259AB",
                "0198BCA6-F46F-722E-B361-EFD64BF0770B",
                "0198BCA6-F46F-722E-B361-F007AFA20357",
                "0198BCA6-F46F-722E-B361-F661FD7E3A55",
                "0198BCA6-F46F-722E-B361-FA79EDB4E33D",
                "0198BCA6-F46F-722E-B361-FCBDDB75F7EE",
                "0198BCA6-F46F-722E-B362-01C1DEFD76E2",
                "0198BCA6-F46F-722E-B362-05069E672158",
                "0198BCA6-F46F-722E-B362-0B9D01EA3F03",
                "0198BCA6-F46F-722E-B362-0F255D0A896C",
                "0198BCA6-F46F-722E-B362-10E94CAC99EF",
                "0198BCA6-F46F-722E-B362-14FBD31B08DB",
                "0198BCA6-F46F-722E-B362-1AE9379A254E"
            ];
            List<string> equipUuids = [
                "0198BBB31BD9716DA5EB13DAA096CC71",
                "0198BBB31BD9716DA5EB14AAE025B57C",
                "0198BBB31BD9716DA5EB193D88695540",
                "0198BBB31BD9716DA5EB1D8342BFB54D",
                "0198BBB31BD9716DA5EB20668727126C",
                "0198BBB31BD9716DA5EB243AEF0C303A",
                "0198BBB31BD9716DA5EB29B3DAB51D56",
                "0198BBB31BD9716DA5EB2D6C8ABF20E7",
                "0198BBB31BD9716DA5EB31717F99D843",
                "0198BBB31BD9716DA5EB37B4655A79FE"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                list.Add(new XT_202_SL_4_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 7,
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

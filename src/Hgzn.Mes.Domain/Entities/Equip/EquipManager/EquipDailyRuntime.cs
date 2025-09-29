using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipManager;

/// <summary>
/// 设备每日运行时长记录
/// </summary>
[Description("设备每日运行时长")]
public class EquipDailyRuntime : UniversalEntity, ICreationAudited
{
    /// <summary>
    /// 设备ID
    /// </summary>
    [Description("设备ID")]
    public Guid EquipId { get; set; }

    /// <summary>
    /// 系统编号
    /// </summary>
    [Description("仿真试验系统识别编码")]
    public byte SystemNumber { get; set; }

    /// <summary>
    /// 设备类型编号
    /// </summary>
    /// /// /// [Description("设备类型识别编码")]
    public byte DeviceTypeNumber { get; set; }

    /// <summary>
    /// 记录日期
    /// </summary>
    [Description("记录日期")]
    public DateTime RecordDate { get; set; }

    /// <summary>
    /// 当日运行时长（秒）
    /// </summary>
    [Description("当日运行时长（秒）")]
    public uint RunningSeconds { get; set; }

    /// <summary>
    /// 当日运行时长（小时，保留2位小数）
    /// </summary>
    [Description("当日运行时长（小时）")]
    public decimal RunningHours => Math.Round((decimal)RunningSeconds / 3600, 2);

    /// <summary>
    /// 数据来源（Redis/Manual/Import等）
    /// </summary>
    [Description("数据来源")]
    public string DataSource { get; set; } = "Redis";

    /// <summary>
    /// 备注
    /// </summary>
    [Description("备注")]
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 创建者ID
    /// </summary>
    public Guid? CreatorId { get; set; }

    /// <summary>
    /// 创建者级别
    /// </summary>
    public int CreatorLevel { get; set; } = 0;

#if DEBUG
    public static EquipDailyRuntime[] Seeds
    {
        get
        {
            var list = new List<EquipDailyRuntime>();
            var random = new Random(12345); // 固定种子确保数据一致性
            var baseDate = DateTime.Now.Date.AddDays(-30); // 从30天前开始
            
            // 预定义的设备ID（从TestEquipData的种子数据中提取）
            var equipIds = new List<Guid>
            {
                Guid.Parse("0198BBB2-7F42-727E-B6F4-5022B4A43D01"), // 系统1-设备类型2
                Guid.Parse("0198BBB2-7F42-727E-B6F4-5496B92E75E4"), // 系统1-设备类型4
                Guid.Parse("0198BBB2-7F42-727E-B6F4-5B8AE3AC62D9"), // 系统2-设备类型1
                Guid.Parse("0198BBB2-7F42-727E-B6F4-5C663EAE3336"), // 系统2-设备类型2
                Guid.Parse("0198BBB2-7F42-727E-B6F4-60153153C45C"), // 系统2-设备类型4
                Guid.Parse("0198BBB2-7F42-727E-B6F4-65C642145755"), // 系统3-设备类型1
                Guid.Parse("0198BBB2-7F42-727E-B6F4-68E883E1A6B4"), // 系统3-设备类型2
                Guid.Parse("0198BBB2-7F42-727E-B6F4-6D5550D5CFEE"), // 系统3-设备类型3
                Guid.Parse("0198BBB2-7F42-727E-B6F4-735AE47D3773"), // 系统3-设备类型4
                Guid.Parse("0198BBB2-7F42-727E-B6F4-76D401A8E56E"), // 系统4-设备类型3
                Guid.Parse("0198BBB2-7F42-727E-B6F4-788374221197"), // 系统4-设备类型4
                Guid.Parse("0198BBB2-7F42-727E-B6F4-7DC357D544E7"), // 系统4-设备类型7
                Guid.Parse("0198BBB2-7F42-727E-B6F4-81D65BA3E6D2"), // 系统5-设备类型3
                Guid.Parse("0198BBB2-7F42-727E-B6F4-8762D4BFFD02"), // 系统5-设备类型4
                Guid.Parse("0198BBB2-7F42-727E-B6F4-8B2FCEE54C5C"), // 系统5-设备类型7
                Guid.Parse("0198BBB2-7F42-727E-B6F4-8D9FDFA083E0"), // 系统6-设备类型3
                Guid.Parse("0198BBB2-7F42-727E-B6F4-90FB289681AA"), // 系统6-设备类型4
                Guid.Parse("0198BBB2-7F42-727E-B6F4-97C6D956B37F"), // 系统6-设备类型7
                Guid.Parse("0198BBB2-7F42-727E-B6F4-99EE535AAFDF"), // 系统7-设备类型3
                Guid.Parse("0198BBB2-7F42-727E-B6F4-9C09FE87EC24"), // 系统7-设备类型4
                Guid.Parse("0198BBB2-7F42-727E-B6F4-A1AAA67A828B"), // 系统7-设备类型7
                Guid.Parse("0198BBB2-7F42-727E-B6F4-A68D9702EFE6"), // 系统8-设备类型2
                Guid.Parse("0198BBB2-7F42-727E-B6F4-AB3FBC007942"), // 系统8-设备类型4
                Guid.Parse("0198BBB2-7F42-727E-B6F4-ACE6DF594ABC"), // 系统9-设备类型2
                Guid.Parse("0198BBB2-7F42-727E-B6F4-B382B5DAB7E5"), // 系统9-设备类型4
                Guid.Parse("0198BBB2-7F42-727E-B6F4-B5169125885C"), // 系统10-设备类型2
                Guid.Parse("0198BBB2-7F42-727E-B6F4-BB0D2FB237AE"), // 系统10-设备类型4
            };

            // 设备和系统/设备类型的映射关系
            var equipSystemMapping = new Dictionary<Guid, (byte systemNumber, byte deviceTypeNumber)>
            {
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-5022B4A43D01"), (1, 2) }, // 微波/毫米波系统-雷达转台
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-5496B92E75E4"), (1, 4) }, // 微波/毫米波系统-固定电源
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-5B8AE3AC62D9"), (2, 1) }, // 微波寻的系统-阵列馈电
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-5C663EAE3336"), (2, 2) }, // 微波寻的系统-雷达转台
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-60153153C45C"), (2, 4) }, // 微波寻的系统-固定电源
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-65C642145755"), (3, 1) }, // 射频/光学系统-阵列馈电
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-68E883E1A6B4"), (3, 2) }, // 射频/光学系统-雷达转台
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-6D5550D5CFEE"), (3, 3) }, // 射频/光学系统-红外转台
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-735AE47D3773"), (3, 4) }, // 射频/光学系统-固定电源
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-76D401A8E56E"), (4, 3) }, // 紧缩场系统-红外转台
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-788374221197"), (4, 4) }, // 紧缩场系统-固定电源
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-7DC357D544E7"), (4, 7) }, // 紧缩场系统-红外源
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-81D65BA3E6D2"), (5, 3) }, // 光学复合系统-红外转台
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-8762D4BFFD02"), (5, 4) }, // 光学复合系统-固定电源
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-8B2FCEE54C5C"), (5, 7) }, // 光学复合系统-红外源
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-8D9FDFA083E0"), (6, 3) }, // 三通道控制系统-红外转台
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-90FB289681AA"), (6, 4) }, // 三通道控制系统-固定电源
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-97C6D956B37F"), (6, 7) }, // 三通道控制系统-红外源
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-99EE535AAFDF"), (7, 3) }, // 低温环境系统-红外转台
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-9C09FE87EC24"), (7, 4) }, // 低温环境系统-固定电源
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-A1AAA67A828B"), (7, 7) }, // 低温环境系统-红外源
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-A68D9702EFE6"), (8, 2) }, // 机械式系统-雷达转台
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-AB3FBC007942"), (8, 4) }, // 机械式系统-固定电源
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-ACE6DF594ABC"), (9, 2) }, // 独立回路系统-雷达转台
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-B382B5DAB7E5"), (9, 4) }, // 独立回路系统-固定电源
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-B5169125885C"), (10, 2) }, // 独立回路/可见光系统-雷达转台
                { Guid.Parse("0198BBB2-7F42-727E-B6F4-BB0D2FB237AE"), (10, 4) }, // 独立回路/可见光系统-固定电源
            };

            // 生成30天的数据
            for (int day = 0; day < 30; day++)
            {
                var recordDate = baseDate.AddDays(day);
                
                foreach (var equipId in equipIds)
                {
                    if (equipSystemMapping.TryGetValue(equipId, out var mapping))
                    {
                        var (systemNumber, deviceTypeNumber) = mapping;
                        
                        // 模拟不同设备的运行时长（秒）
                        // 电源设备通常运行时间较长，转台设备运行时间适中，其他设备相对较短
                        var baseRunningSeconds = (uint)(deviceTypeNumber switch
                        {
                            4 => random.Next(8, 12) * 3600, // 固定电源：8-12小时
                            2 => random.Next(4, 8) * 3600,  // 雷达转台：4-8小时
                            3 => random.Next(3, 6) * 3600,  // 红外转台：3-6小时
                            7 => random.Next(2, 4) * 3600,  // 红外源：2-4小时
                            1 => random.Next(1, 3) * 3600,  // 阵列馈电：1-3小时
                            _ => random.Next(2, 6) * 3600   // 其他设备：2-6小时
                        });

                        // 周末运行时间较少
                        if (recordDate.DayOfWeek == DayOfWeek.Saturday || recordDate.DayOfWeek == DayOfWeek.Sunday)
                        {
                            baseRunningSeconds = (uint)((int)baseRunningSeconds * random.Next(20, 60) / 100); // 周末减少到20%-60%
                        }

                        // 添加一些随机性
                        var runningSeconds = (uint)((int)baseRunningSeconds * random.Next(80, 120) / 100);

                        var runtime = new EquipDailyRuntime
                        {
                            Id = Guid.NewGuid(),
                            EquipId = equipId,
                            SystemNumber = systemNumber,
                            DeviceTypeNumber = deviceTypeNumber,
                            RecordDate = recordDate,
                            RunningSeconds = runningSeconds,
                            DataSource = "Redis",
                            Remark = $"系统{systemNumber}-{TestEquipData.GetEquipTypeName(deviceTypeNumber)}设备运行记录",
                            CreationTime = recordDate.AddHours(random.Next(18, 23)), // 当天晚上创建记录
                            CreatorId = Guid.Parse("00000000-0000-0000-0000-000000000001"), // 系统用户ID
                            CreatorLevel = 1
                        };

                        list.Add(runtime);
                    }
                }
            }

            return list.ToArray();
        }
    }
#endif
}

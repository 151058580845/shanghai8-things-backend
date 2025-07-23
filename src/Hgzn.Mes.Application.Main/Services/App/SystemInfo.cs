using Hgzn.Mes.Application.Main.Dtos.App;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_307_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_310_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;
using NetCoreServer;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.App
{
    public class SystemInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public byte SystemNum { get; set; }
        /// <summary>
        /// 系统名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 房间ID
        /// </summary>
        public Guid RoomId { get; set; }
        /// <summary>
        /// 房间号
        /// </summary>
        public string RoomNumber { get; set; } // 添加房间号信息
        /// <summary>
        /// 转台设备ID,现在转台设备默认的都是,1e14e36a-ec3a-4358-aca4-8e655a252f54 虚拟设备用于测试
        /// </summary>
        public Guid TurntableEquipId { get; set; } // 转台设备ID,因为每个系统的利用率用转台设备统计,所以我需要知道每个系统的转台设备对应的是哪一台
        /// <summary>
        /// 关键设备名字-设备ID
        /// </summary>
        public List<KeyDevice> keyDevices { get; set; } // 在主页展示的关键设备,每个系统的转台和雷达源,红外源是关键设备,所以我需要知道这些指定设备对应的是哪个ID

        /// <summary>
        /// 异常数量
        /// </summary>
        public int AbnormalCount { get; set; }
    }

    public class KeyDevice
    {
        /// <summary>
        /// 类型编号
        /// </summary>
        public byte EquipTypeNum { get; set; }
        /// <summary>
        /// 设备名
        /// </summary>
        public string EquipName { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid EquipId { get; set; }
    }

    /// <summary>
    /// 异常
    /// </summary>
    public class Abnormal
    {
        /// <summary>
        /// 异常设备的系统信息
        /// </summary>
        public SystemInfo SystemInfo { get; set; }
        /// <summary>
        /// 异常设备的类型
        /// </summary>
        public byte EquipTypeNum { get; set; }
        /// <summary>
        /// 异常设备的ID
        /// </summary>
        public Guid EquipId { get; set; }
        /// <summary>
        /// 异常描述
        /// </summary>
        public List<string> AbnormalDescription { get; set; }
    }

    public class SystemInfoManager
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        protected readonly ISqlSugarClient _sqlSugarClient;
        private const string EquipHealthStatusRedisKey = "equipHealthStatus";
        private readonly RedisHelper _redisHelper;

        public RedisTreeNode EquipHealthStatusRedisTree;
        public List<SystemInfo> SystemInfos = new List<SystemInfo>();
        // key是设备ID,value是设备异常信息
        public List<Abnormal> Abnormals = new List<Abnormal>();

        public SystemInfoManager(IConnectionMultiplexer connectionMultiplexer, RedisHelper redisHelper, ISqlSugarClient client)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _redisHelper = redisHelper;
            _sqlSugarClient = client;
            // 初始化数据
            SystemInfos = new List<SystemInfo>
            {
                new SystemInfo
                {
                    SystemNum = 1,
                    Name = "微波/毫米波复合半实物仿真系统",
                    RoomId = Guid.Parse("4c246470-da66-4408-b287-09fd82ffa3d4"),
                    RoomNumber = "310",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<KeyDevice>()
                    {
                        new KeyDevice()
                        {
                            EquipTypeNum = 3,
                            EquipName = "虚拟设备",
                            EquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54")
                        }
                    },
                },
                new SystemInfo
                {
                    SystemNum = 2,
                    Name = "微波寻的半实物仿真系统",
                    RoomId = Guid.Parse("83168845-ef46-4aed-9187-de2024488230"),
                    RoomNumber = "307",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<KeyDevice>(),
                },
                new SystemInfo
                {
                    SystemNum = 3,
                    Name = "射频/光学制导半实物仿真系统",
                    RoomId = Guid.Parse("7412dda2-5413-43ab-9976-255df60c3e14"),
                    RoomNumber = "314",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<KeyDevice>(),
                },
                new SystemInfo
                {
                    SystemNum = 4,
                    Name = "紧缩场射频光学半实物仿真系统",
                    RoomId = Guid.Parse("09b374e8-4e7f-4146-9fe0-375edc7b9d7a"),
                    RoomNumber = "109",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<KeyDevice>(),
                },
                new SystemInfo
                {
                    SystemNum = 5,
                    Name = "光学复合半实物仿真系统",
                    RoomId = Guid.Parse("0ac9885d-da23-4c0f-a66c-2ba467b8086c"),
                    RoomNumber = "108",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<KeyDevice>(),
                },
                new SystemInfo
                {
                    SystemNum = 6,
                    Name = "三通道控制红外制导半实物仿真系统",
                    RoomId = Guid.Parse("24be4856-d95f-4ba0-b2aa-7049fedc3e39"),
                    RoomNumber = "121",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<KeyDevice>(),
                },
                new SystemInfo
                {
                    SystemNum = 7,
                    Name = "低温环境红外制导控制半实物仿真系统",
                    RoomId = Guid.Parse("916edd0e-df70-4137-806c-41817587e438"),
                    RoomNumber = "202-2",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<KeyDevice>(),
                },
                new SystemInfo
                {
                    SystemNum = 8,
                    Name = "机械式制导控制半实物仿真系统",
                    RoomId = Guid.Parse("7d6b322d-bf48-4963-bfe6-579560e84530"),
                    RoomNumber = "103",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<KeyDevice>(),
                },
                new SystemInfo
                {
                    SystemNum = 9,
                    Name = "独立回路半实物仿真系统",
                    RoomId = Guid.Parse("a6ce46f1-d51f-45c8-a22e-2db3126da6cf"),
                    RoomNumber = "119",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<KeyDevice>(),
                },
                new SystemInfo
                {
                    SystemNum = 10,
                    Name = "独立回路/可见光制导半实物仿真系统",
                    RoomId = Guid.Parse("ddd64e08-5f2a-4578-84cb-2f90caa898e9"),
                    RoomNumber = "112",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<KeyDevice>(),
                },
                new SystemInfo
                {
                    SystemNum = 0,
                    Name = "移动设备",
                    RoomId = Guid.Empty,
                    RoomNumber = "0",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<KeyDevice>(),
                }
            };
        }

        public async Task SnapshootHomeData()
        {
            // redis树
            try
            {
                EquipHealthStatusRedisTree = await _redisHelper.GetTreeAsync(EquipHealthStatusRedisKey);
            }
            catch (Exception e) { }
            Abnormals = new List<Abnormal>();
            foreach (SystemInfo si in SystemInfos)
            {
                int sum = 0;
                RedisTreeNode sysSets = await _redisHelper.FindTreeNodeByPathAsync(EquipHealthStatusRedisTree, $"{EquipHealthStatusRedisKey}:{si.SystemNum}");
                if (sysSets == null) continue;
                if (!sysSets.IsLeaf && sysSets.Children.Any())
                {
                    foreach (RedisTreeNode typeNode in sysSets.Children)
                    {
                        List<RedisTreeNode> sets = await _redisHelper.FindTreeNodesByTypeAsync(typeNode, RedisKeyType.Set);
                        if (!byte.TryParse(typeNode.Name, out byte equipTypeNum)) continue;
                        foreach (RedisTreeNode set in sets)
                        {
                            if (!Guid.TryParse(set.Name, out Guid equipId)) continue;
                            IEnumerable<string> abnormal = await _redisHelper.GetTreeNodeChildrenValuesAsync(set);
                            if (abnormal == null) continue;
                            sum += abnormal.Count();
                            Abnormals.Add(new Abnormal()
                            {
                                SystemInfo = si,
                                EquipTypeNum = equipTypeNum,
                                EquipId = equipId,
                                AbnormalDescription = abnormal.ToList(),
                            });
                        }
                    }
                }
                si.AbnormalCount = sum;
            }
        }

        public async Task<bool> GetEquipOnline(List<EquipConnect> connectEquips, Guid equipId)
        {
            EquipConnect ce = connectEquips.Where(x => x.EquipId == equipId).FirstOrDefault()!;
            if (ce == null)
                return false;
            IDatabase database = _connectionMultiplexer.GetDatabase();
            var key = string.Format(CacheKeyFormatter.EquipState, equipId, ce.Id);
            return await database.StringGetAsync(key) == 3;
        }

        /// <summary>
        /// 获取运行时间
        /// </summary>
        /// <param name="systemNum"></param>
        /// <param name="equipTypeNum"></param>
        /// <param name="equipId"></param>
        /// <returns></returns>
        public async Task<uint> GetRunTime(byte systemNum, byte equipTypeNum, Guid equipId)
        {
            string runTimeKey = string.Format(CacheKeyFormatter.EquipRunTime, systemNum, equipTypeNum, equipId);
            uint runTime = await ReceiveHelper.GetLast30DaysRunningTimeAsync(_connectionMultiplexer, runTimeKey);
            return runTime;
        }

        /// <summary>
        /// 获取展示数据的Table
        /// </summary>
        /// <param name="systemInfo"></param>
        /// <returns></returns>
        public async Task<List<TableDto>> GetTableDtos(SystemInfo systemInfo)
        {
            List<TableDto> ret = new List<TableDto>();
            switch (systemInfo.SystemNum)
            {
                case 1:
                    XT_310_SL_2_ReceiveData data = await _sqlSugarClient.Queryable<XT_310_SL_2_ReceiveData>().OrderByDescending(x => x.CreationTime).FirstAsync();
                    TableDto td = new TableDto()
                    {
                        Title = "雷达源",
                        Header = new List<List<string>>() // 这三列必须是 name , control , power
                        {
                            new List<string> { "name", "物理量定义" },
                            new List<string> { "control", "物理量" }
                        },
                        Data = new List<Dictionary<string, string>>()
                        {
                            new Dictionary<string, string>()
                            {
                                { "name", "内框位置" },
                                { "control" , data.InnerFramePosition.ToString() }
                            },
                            new Dictionary<string, string>()
                            {
                                { "name", "中框位置" },
                                { "control" , data.MiddleFramePosition.ToString() }
                            },
                            new Dictionary<string, string>()
                            {
                                { "name", "外框位置" },
                                { "control" , data.OuterFramePosition.ToString() }
                            },
                            new Dictionary<string, string>()
                            {
                                { "name", "内框速度" },
                                { "control" , data.InnerFrameVelocity.ToString() }
                            },
                            new Dictionary<string, string>()
                            {
                                { "name", "中框速度" },
                                { "control" , data.MiddleFrameVelocity.ToString() }
                            },
                            new Dictionary<string, string>()
                            {
                                { "name", "外框速度" },
                                { "control" , data.OuterFrameVelocity.ToString() }
                            },
                            new Dictionary<string, string>()
                            {
                                { "name", "内框加速度" },
                                { "control" , data.InnerFrameAcceleration.ToString() }
                            },
                            new Dictionary<string, string>()
                            {
                                { "name", "中框加速度" },
                                { "control" , data.MiddleFrameAcceleration.ToString() }
                            },
                            new Dictionary<string, string>()
                            {
                                { "name", "外框加速度" },
                                { "control" , data.OuterFrameAcceleration.ToString() }
                            }
                        }
                    };
                    ret.Add(td);
                    return ret;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获取图标数据
        /// </summary>
        /// <param name="systemInfo"></param>
        /// <returns></returns>
        public async Task<List<ChartDataDto>> GetChartDataPointDto(SystemInfo systemInfo)
        {
            // 图标里展示的电源
            // 其他系统都是固定电源,也就是取第4类型,只有移动设备用的是移动电源,取5类型
            // 固定电源在工作信息中有电源数量,移动电源没有,估计就一个
            // 只输出采集电压值和电流值
            List<ChartDataDto> ret = new List<ChartDataDto>();
            switch (systemInfo.SystemNum)
            {
                case 1:
                    // 获取今天的日期（时间部分为 00:00:00）
                    DateTime today = DateTime.Today;
                    // 查询今天的所有数据，并按 CreationTime 降序排列
                    List<XT_310_SL_4_ReceiveData> todayData = await _sqlSugarClient
                        .Queryable<XT_310_SL_4_ReceiveData>()
                        .Where(x => x.CreationTime >= today && x.CreationTime < today.AddDays(1)) // 今天 00:00:00 ~ 23:59:59
                        .OrderBy(x => x.CreationTime)
                        .ToListAsync();
                    XT_310_SL_4_ReceiveData data = todayData.LastOrDefault()!;
                    if (data == null) return ret;
                    for (int i = 1; i < data.PowerSupplyCount + 1; i++)
                    {
                        // 获取采集电压
                        List<ChartDataPointDto> cdps = new List<ChartDataPointDto>();
                        foreach (XT_310_SL_4_ReceiveData item in todayData)
                        {
                            ChartDataPointDto cdp = new ChartDataPointDto();
                            cdp.Time = item.CreationTime.ToString("HH:mm");
                            GetVolValue(i, item, cdp);
                            cdps.Add(cdp);
                        }
                        ret.Add(new ChartDataDto()
                        {
                            Name = $"电源{i}电压",
                            Data = cdps
                        });

                        // 获取采集电流
                        List<ChartDataPointDto> cdps2 = new List<ChartDataPointDto>();
                        foreach (XT_310_SL_4_ReceiveData item in todayData)
                        {
                            ChartDataPointDto cdp = new ChartDataPointDto();
                            cdp.Time = item.CreationTime.ToString("HH:mm");
                            GetCurValue(i, item, cdp);
                            cdps2.Add(cdp);
                        }
                        ret.Add(new ChartDataDto()
                        {
                            Name = "电源" + i + "电流",
                            Data = cdps2
                        });
                    }
                    return ret;
                default:
                    return await Task.FromResult(ret);
            }
        }

        private static void GetVolValue(int i, XT_310_SL_4_ReceiveData item, ChartDataPointDto cdp)
        {
            switch (i)
            {
                case 1:
                    cdp.Value = item.Power1VoltageRead;
                    break;
                case 2:
                    cdp.Value = item.Power2VoltageRead;
                    break;
                case 3:
                    cdp.Value = item.Power3VoltageRead;
                    break;
                case 4:
                    cdp.Value = item.Power4VoltageRead;
                    break;
                case 5:
                    cdp.Value = item.Power5VoltageRead;
                    break;
                case 6:
                    cdp.Value = item.Power6VoltageRead;
                    break;
                case 7:
                    cdp.Value = item.Power7VoltageRead;
                    break;
                case 8:
                    cdp.Value = item.Power8VoltageRead;
                    break;
                default:
                    break;
            }
        }

        private static void GetCurValue(int i, XT_310_SL_4_ReceiveData item, ChartDataPointDto cdp)
        {
            switch (i)
            {
                case 1:
                    cdp.Value = item.Power1CurrentRead;
                    break;
                case 2:
                    cdp.Value = item.Power2CurrentRead;
                    break;
                case 3:
                    cdp.Value = item.Power3CurrentRead;
                    break;
                case 4:
                    cdp.Value = item.Power4CurrentRead;
                    break;
                case 5:
                    cdp.Value = item.Power5CurrentRead;
                    break;
                case 6:
                    cdp.Value = item.Power6CurrentRead;
                    break;
                case 7:
                    cdp.Value = item.Power7CurrentRead;
                    break;
                case 8:
                    cdp.Value = item.Power8CurrentRead;
                    break;
                default:
                    break;
            }
        }
    }
}

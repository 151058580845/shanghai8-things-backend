using Hgzn.Mes.Application.Main.Dtos.App;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT__ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_121_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_307_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_310_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.Equip.EquipMeasurementManager;
using Hgzn.Mes.Domain.Entities.System.Location;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Utilities;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
using MathNet.Numerics.Distributions;
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
        /// 关键设备名字-设备ID,在主页展示的关键设备,每个系统的转台和雷达源,红外源是关键设备,所以我需要知道这些指定设备对应的是哪个ID
        /// </summary>
        public List<SDevice> keyDevices { get; set; } = new List<SDevice>();
        /// <summary>
        /// 活跃的设备
        /// </summary>
        public List<LDevice> LiveDevices { get; set; } = new List<LDevice>();

        /// <summary>
        /// 房间里所有的设备
        /// </summary>
        public List<EquipLedger> AllEquip { get; set; } = new List<EquipLedger>();

        /// <summary>
        /// 异常数量
        /// </summary>
        public int AbnormalCount { get; set; }
    }

    public class LDevice
    {
        /// <summary>
        /// 类型编号
        /// </summary>
        public byte EquipTypeNum { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid EquipId { get; set; }
    }

    public class SDevice
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
        public SystemInfo? SystemInfo { get; set; }
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
        public List<string>? AbnormalDescription { get; set; }

        /// <summary>
        /// 距离到期的天数
        /// </summary>
        public string UntilDays { get; set; } = string.Empty;
    }

    public class SystemInfoManager
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        protected readonly ISqlSugarClient _sqlSugarClient;
        private const string EquipHealthStatusRedisKey = "equipHealthStatus";
        private const string EquipLiveRedisKey = "equipLive";
        private readonly RedisHelper _redisHelper;

        public RedisTreeNode EquipHealthStatusRedisTree;
        public RedisTreeNode EquipLiveRedisTree;
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
                    keyDevices = new List<SDevice>()
                    {
                        new SDevice()
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
                    keyDevices = new List<SDevice>(),
                },
                new SystemInfo
                {
                    SystemNum = 3,
                    Name = "射频/光学制导半实物仿真系统",
                    RoomId = Guid.Parse("7412dda2-5413-43ab-9976-255df60c3e14"),
                    RoomNumber = "314",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<SDevice>(),
                },
                new SystemInfo
                {
                    SystemNum = 4,
                    Name = "紧缩场射频光学半实物仿真系统",
                    RoomId = Guid.Parse("09b374e8-4e7f-4146-9fe0-375edc7b9d7a"),
                    RoomNumber = "109",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<SDevice>(),
                },
                new SystemInfo
                {
                    SystemNum = 5,
                    Name = "光学复合半实物仿真系统",
                    RoomId = Guid.Parse("0ac9885d-da23-4c0f-a66c-2ba467b8086c"),
                    RoomNumber = "108",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<SDevice>(),
                },
                new SystemInfo
                {
                    SystemNum = 6,
                    Name = "三通道控制红外制导半实物仿真系统",
                    RoomId = Guid.Parse("24be4856-d95f-4ba0-b2aa-7049fedc3e39"),
                    RoomNumber = "121",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<SDevice>(),
                },
                new SystemInfo
                {
                    SystemNum = 7,
                    Name = "低温环境红外制导控制半实物仿真系统",
                    RoomId = Guid.Parse("916edd0e-df70-4137-806c-41817587e438"),
                    RoomNumber = "202-2",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<SDevice>(),
                },
                new SystemInfo
                {
                    SystemNum = 8,
                    Name = "机械式制导控制半实物仿真系统",
                    RoomId = Guid.Parse("7d6b322d-bf48-4963-bfe6-579560e84530"),
                    RoomNumber = "103",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<SDevice>(),
                },
                new SystemInfo
                {
                    SystemNum = 9,
                    Name = "独立回路半实物仿真系统",
                    RoomId = Guid.Parse("a6ce46f1-d51f-45c8-a22e-2db3126da6cf"),
                    RoomNumber = "119",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<SDevice>(),
                },
                new SystemInfo
                {
                    SystemNum = 10,
                    Name = "独立回路/可见光制导半实物仿真系统",
                    RoomId = Guid.Parse("ddd64e08-5f2a-4578-84cb-2f90caa898e9"),
                    RoomNumber = "112",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<SDevice>(),
                },
                new SystemInfo
                {
                    SystemNum = 0,
                    Name = "移动设备",
                    RoomId = Guid.Empty,
                    RoomNumber = "0",
                    TurntableEquipId = Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"),
                    keyDevices = new List<SDevice>(),
                }
            };
        }

        public async Task SnapshootHomeData()
        {
            DateTime now = DateTime.Now;
            // 一次性查询即将到期和已过期的设备
            List<EquipMeasurement> allMeasurements = await _sqlSugarClient.Queryable<EquipMeasurement>().ToListAsync();
            List<EquipMeasurement> expiringSoon = allMeasurements.Where(x => (x.ExpiryDate - now)?.TotalDays <= 30 && (x.ExpiryDate - now)?.TotalDays > 0).ToList();
            List<EquipMeasurement> pastDue = allMeasurements.Where(x => (x.ExpiryDate - now)?.TotalDays <= 0).ToList();
            // 提取资产编号列表
            List<string?> expiringSoonAssetNumbers = expiringSoon.Select(x => x.LocalAssetNumber).ToList();
            List<string?> pastDueAssetNumbers = pastDue.Select(x => x.LocalAssetNumber).ToList();
            // 一次性查询所有相关设备及其房间信息
            List<EquipLedger> allEquips = await _sqlSugarClient.Queryable<EquipLedger>().Includes(x => x.Room).Where(x => expiringSoonAssetNumbers.Contains(x.AssetNumber) || pastDueAssetNumbers.Contains(x.AssetNumber)).ToListAsync();
            List<EquipLedger> expiringSoonEquips = allEquips.Where(x => expiringSoonAssetNumbers.Contains(x.AssetNumber)).ToList();
            List<EquipLedger> pastDueEquips = allEquips.Where(x => pastDueAssetNumbers.Contains(x.AssetNumber)).ToList();

            // redis
            try
            {
                EquipHealthStatusRedisTree = await _redisHelper.GetTreeAsync(EquipHealthStatusRedisKey);
                EquipLiveRedisTree = await _redisHelper.GetTreeAsync(EquipLiveRedisKey);
            }
            catch (Exception) { }
            Abnormals = new List<Abnormal>();
            foreach (SystemInfo si in SystemInfos)
            {
                int sum = 0;
                // 记录异常
                RedisTreeNode sysSets = await _redisHelper.FindTreeNodeByPathAsync(EquipHealthStatusRedisTree, $"{EquipHealthStatusRedisKey}:{si.SystemNum}");
                if (sysSets != null)
                {
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
                        foreach (EquipLedger item in expiringSoonEquips)
                        {
                            if (item.RoomId == si.RoomId)
                            {
                                sum += 1;
                                Abnormals.Add(new Abnormal()
                                {
                                    SystemInfo = si,
                                    EquipTypeNum = 0,
                                    EquipId = item.Id,
                                    AbnormalDescription = new List<string>() { $"{item.EquipName}计量即将到期" },
                                    UntilDays = ((int)(expiringSoon.First(x => x.LocalAssetNumber == item.AssetNumber).ExpiryDate - now)?.TotalDays!).ToString()
                                });
                            }
                        }
                        foreach (EquipLedger item in pastDueEquips)
                        {
                            if (item.RoomId == si.RoomId)
                            {
                                sum += 1;
                                Abnormals.Add(new Abnormal()
                                {
                                    SystemInfo = si,
                                    EquipTypeNum = 0,
                                    EquipId = item.Id,
                                    AbnormalDescription = new List<string>() { $"{item.EquipName}计量过期" },
                                    UntilDays = ((int)(pastDue.First(x => x.LocalAssetNumber == item.AssetNumber).ExpiryDate - now)?.TotalDays!).ToString()
                                });
                            }
                        }
                    }
                }
                // 记录心跳
                RedisTreeNode sysLive = await _redisHelper.FindTreeNodeByPathAsync(EquipLiveRedisTree, $"{EquipLiveRedisKey}:{si.SystemNum}");
                if (sysLive != null)
                {
                    if (sysLive.Children.Any())
                    {
                        foreach (RedisTreeNode type in sysLive.Children)
                        {
                            string sEquipId = await _redisHelper.GetTreeNodeValueAsync(type);
                            Guid equipId = Guid.Empty;
                            if (Guid.TryParse(sEquipId, out Guid gEquipId))
                                equipId = gEquipId;
                            byte equipTypeNum = 0;
                            if (byte.TryParse(type.Name, out byte bEquipTypeNum))
                                equipTypeNum = bEquipTypeNum;
                            si.LiveDevices.Add(new LDevice()
                            {
                                EquipId = equipId,
                                EquipTypeNum = equipTypeNum,
                            });
                        }
                    }

                }
                // 记录所有设备
                List<EquipLedger> allEquip = await _sqlSugarClient.Queryable<EquipLedger>().Where(x => x.RoomId == si.RoomId).ToListAsync();
                si.AllEquip = allEquip;
                si.AbnormalCount = sum;
            }
        }

        public async Task<bool> GetEquipOnline(List<EquipConnect> connectEquips, Guid equipId)
        {
            EquipConnect ce = connectEquips.Where(x => x.EquipId == equipId).FirstOrDefault()!;
            if (ce == null)
                return false;
            IDatabase database = _connectionMultiplexer.GetDatabase();
            string key = string.Format(CacheKeyFormatter.EquipState, equipId, ce.Id);
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

        #region ====== 获取展示数据的Table ======

        public TableDto CreateTable(string title, Func<List<List<string>>?> createHeader, params (string key, string value)[] rows)
        {
            if (rows == null || !rows.Any())
            {
                return new TableDto
                {
                    Title = title,
                    Header = createHeader(),
                    Data = new List<Dictionary<string, string>>()
                };
            }
            return new TableDto
            {
                Title = title,
                Header = createHeader(),
                Data = rows.Select(r => CreateDataRow(r.key, r.value)).ToList()
            };
        }

        /// <summary>
        /// 根据系统信息和类型获取表格数据DTO列表
        /// </summary>
        /// <param name="systemInfo">系统信息</param>
        /// <param name="type">表格类型</param>
        /// <returns>表格DTO列表</returns>
        public async Task<List<Tuple<TableDto, TableDto>>> GetTableDtos(SystemInfo systemInfo)
        {
            // 根据系统编号路由到对应的处理方法
            List<Tuple<TableDto, TableDto>> ret = systemInfo.SystemNum switch
            {
                0 => await HandleSystem0(systemInfo),
                1 => await HandleSystem1(systemInfo),
                6 => await HandleSystem6(systemInfo),
                _ => null!
            };
            return ret;
        }

        private async Task<List<Tuple<TableDto, TableDto>>> HandleSystem0(SystemInfo systemInfo)
        {
            List<Tuple<TableDto, TableDto>> tables = new List<Tuple<TableDto, TableDto>>();
            XT__SL_1_ReceiveData data = (await _sqlSugarClient.Queryable<XT__SL_1_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            if (data == null) return tables;
            TableDto table = new TableDto
            {
                Title = "XX类型",
                Header = CreateStandardHeader(),
                Data = new List<Dictionary<string, string>>
                {
                    CreateDataRow("XX状态", "正常")
                }
            };
            IndexBasedTableGenerator detailGenerator = new IndexBasedTableGenerator();
            TableDto detailTable = detailGenerator.GenerateTableFromInstance(data, "xxx物理量");
            Tuple<TableDto, TableDto> table1 = new Tuple<TableDto, TableDto>(table, detailTable);
            tables.Add(table1);
            return tables;
        }

        #region 310_微波/毫米波复合半实物仿真系统(310的4是电源,应该用移动的雷达源)

        private async Task<List<Tuple<TableDto, TableDto>>> HandleSystem1(SystemInfo systemInfo)
        {
            List<Tuple<TableDto, TableDto>> tables = [await CreateTable_310_4(systemInfo), await CreateTable_310_2(systemInfo)];
            return tables;
        }

        private async Task<Tuple<TableDto, TableDto>> CreateTable_310_4(SystemInfo systemInfo)
        {
            string typeName = "雷达源";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader, ("内框位置", "离线"), ("运行状态", "离线"));
            TableDto defaultDetailTableDto = CreateTable("雷达源物理量", CreateStandardHeader);
            XT_310_SL_4_ReceiveData data = (await _sqlSugarClient.Queryable<XT_310_SL_4_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            // 检查是否有数据,以及310的4类型设备是否有心跳,要是没有数据或没有心跳,则返回默认数据
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 4)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader, ("内框位置", "正常"), ("运行状态", "正常"));
            IndexBasedTableGenerator detailGenerator = new IndexBasedTableGenerator();
            TableDto detailTable = detailGenerator.GenerateTableFromInstance(data, "雷达源物理量", 12, 20);
            Tuple<TableDto, TableDto> type2 = new Tuple<TableDto, TableDto>(table, detailTable);
            return type2;
        }

        private async Task<Tuple<TableDto, TableDto>> CreateTable_310_2(SystemInfo systemInfo)
        {
            string typeName = "雷达转台";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader, ("自检状态", "离线"), ("运行状态", "离线"));
            TableDto defaultDetailTableDto = CreateTable("雷达转台物理量", CreateStandardHeader);
            XT_310_SL_2_ReceiveData data = (await _sqlSugarClient.Queryable<XT_310_SL_2_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            // 检查是否有数据,以及310的4类型设备是否有心跳,要是没有数据或没有心跳,则返回默认数据
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 2)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);

            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("自检状态", data.SelfTestStatus == 0 ? "正常" : "异常"),
                ("运行状态", data.HealthOperationStatus == 0 ? "正常" : "异常"));
            IndexBasedTableGenerator detailGenerator = new IndexBasedTableGenerator();
            TableDto detailTable = detailGenerator.GenerateTableFromInstance(data, "雷达源物理量", 12, 20);
            Tuple<TableDto, TableDto> type2 = new Tuple<TableDto, TableDto>(table, detailTable);
            return type2;
        }

        #endregion

        #region 121_三通道控制红外制导半实物仿真系统

        private async Task<List<Tuple<TableDto, TableDto>>> HandleSystem6(SystemInfo systemInfo)
        {
            List<Tuple<TableDto, TableDto>> tables = [await CreateTable_121_3(systemInfo), await CreateTable_121_7(systemInfo)];
            return tables;
        }

        private async Task<Tuple<TableDto, TableDto>> CreateTable_121_3(SystemInfo systemInfo)
        {
            XT_121_SL_3_ReceiveData data = (await _sqlSugarClient.Queryable<XT_121_SL_3_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            if (data == null) return null!;

            TableDto table = new TableDto
            {
                Title = "红外转台",
                Header = CreateStandardHeader(),
                Data = new List<Dictionary<string, string>>
                {
                    CreateDataRow("滚动轴工作状态", data.RollingAxisOperationStatus == 0 ? "正常" : "异常"),
                    CreateDataRow("偏航轴工作状态", data.YawAxisOperationStatus == 0 ? "正常" : "异常"),
                    CreateDataRow("俯仰轴工作状态", data.YawAxisOperationStatus == 0 ? "正常" : "异常"),
                    CreateDataRow("高低轴工作状态", data.ElevationAxisOperationStatus == 0 ? "正常" : "异常"),
                    CreateDataRow("方位轴工作状态", data.AzimuthAxisOperationStatus == 0 ? "正常" : "异常")
                }
            };
            IndexBasedTableGenerator detailGenerator = new IndexBasedTableGenerator();
            TableDto detailTable = detailGenerator.GenerateTableFromInstance(data, "红外转台物理量", 19, 25);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        private async Task<Tuple<TableDto, TableDto>> CreateTable_121_7(SystemInfo systemInfo)
        {
            XT_121_SL_7_ReceiveData data = (await _sqlSugarClient.Queryable<XT_121_SL_7_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            if (data == null) return null!;

            TableDto table = new TableDto
            {
                Title = "红外源",
                Header = CreateStandardHeader(),
                Data = new List<Dictionary<string, string>>
                {
                    CreateDataRow("露点温度状态", data.DewPointTemperatureStatus == 1 ? "正常" : "异常"),
                    CreateDataRow("真空度状态", data.VacuumStatus == 1 ? "正常" : "异常"),
                    CreateDataRow("冷水机流量状态", data.ChillerFlowStatus == 1 ? "正常" : "异常"),
                    CreateDataRow("环境箱温度状态", data.EnvironmentalChamberTemperatureStatus == 1 ? "正常" : "异常"),
                    CreateDataRow("衬底温度状态", data.SubstrateTemperatureStatus == 1 ? "正常" : "异常"),
                    CreateDataRow("功率电源状态", data.PowerSupplyStatus == 1 ? "正常" : "异常"),
                    CreateDataRow("控制电源状态", data.ControlPowerStatus == 1 ? "正常" : "异常")
                }
            };
            IndexBasedTableGenerator detailGenerator = new IndexBasedTableGenerator();
            TableDto detailTable = detailGenerator.GenerateTableFromInstance(data, "红外源物理量", 19, 25);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        #endregion

        /// <summary>
        /// 创建标准表格表头
        /// </summary>
        /// <returns>表头数据</returns>
        private List<List<string>> CreateStandardHeader()
        {
            return new List<List<string>>
            {
                new List<string> { "name", "健康信息" },
                new List<string> { "control", "状态" }
            };
        }

        /// <summary>
        /// 创建表格数据行
        /// </summary>
        /// <param name="name">数据项名称</param>
        /// <param name="controlValue">控制值</param>
        /// <returns>数据行字典</returns>
        private Dictionary<string, string> CreateDataRow(string name, string controlValue)
        {
            return new Dictionary<string, string>
            {
                { "name", name },
                { "control", controlValue }
            };
        }

        #endregion

        #region 获取图表数据

        /// <summary>
        /// 获取图表数据
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
                            ChartDataPointDto cdp2 = new ChartDataPointDto();
                            cdp.Time = item.CreationTime.ToString("HH:mm");
                            GetVolValue(i, item, cdp);
                            GetCurValue(i, item, cdp);
                            cdps.Add(cdp);
                        }
                        ret.Add(new ChartDataDto()
                        {
                            Name = $"电源{i}",
                            Data = cdps
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
                    cdp.Value2 = item.Power1CurrentRead;
                    break;
                case 2:
                    cdp.Value2 = item.Power2CurrentRead;
                    break;
                case 3:
                    cdp.Value2 = item.Power3CurrentRead;
                    break;
                case 4:
                    cdp.Value2 = item.Power4CurrentRead;
                    break;
                case 5:
                    cdp.Value2 = item.Power5CurrentRead;
                    break;
                case 6:
                    cdp.Value2 = item.Power6CurrentRead;
                    break;
                case 7:
                    cdp.Value2 = item.Power7CurrentRead;
                    break;
                case 8:
                    cdp.Value2 = item.Power8CurrentRead;
                    break;
                default:
                    break;
            }
        }

        #endregion

        private bool IsExpiringSoon(EquipMeasurement em)
        {
            return (em.ExpiryDate - DateTime.Now.ToLocalTime())?.TotalDays <= 30 && (em.ExpiryDate - DateTime.Now.ToLocalTime())?.TotalDays > 0;
        }

        private bool IsPastDue(EquipMeasurement em)
        {
            return (em.ExpiryDate - DateTime.Now.ToLocalTime())?.TotalDays <= 0;
        }
    }
}

using Hgzn.Mes.Application.Main.Dtos.App;
using Hgzn.Mes.Application.Main.Services.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Application.Main.Services.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT__ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_0_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_103_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_108_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_109_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_112_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_119_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_121_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_202_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_307_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_310_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_314_ReceiveDatas;
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
using NPOI.SS.Formula.Functions;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public byte? EquipTypeNum { get; set; }
        /// <summary>
        /// 异常设备的ID
        /// </summary>
        public Guid? EquipId { get; set; }
        /// <summary>
        /// 异常设备名称
        /// </summary>
        public string? EquipName { get; set; }
        /// <summary>
        /// 异常描述
        /// </summary>
        public List<string>? AbnormalDescription { get; set; }

        /// <summary>
        /// 距离到期的天数
        /// </summary>
        public string? UntilDays { get; set; } = string.Empty;
    }

    public class SystemInfoManager
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        protected readonly ISqlSugarClient _sqlSugarClient;
        private const string EquipHealthStatusRedisKey = "equipHealthStatus";
        private const string EquipLiveRedisKey = "equipLive";
        private const string EquipRunTime = "equipRunTime";
        private readonly RedisHelper _redisHelper;
        private IEquipLedgerService _equipLedgerService;
        private readonly IBaseConfigService _baseConfigService;
        private IndexBasedTableGenerator _detailGenerator;

        public RedisTreeNode EquipHealthStatusRedisTree;
        public RedisTreeNode EquipLiveRedisTree;
        public RedisTreeNode EquipRunTimeRedisTree;
        public List<SystemInfo> SystemInfos = new List<SystemInfo>();
        // key是设备ID,value是设备异常信息
        public List<Abnormal> Abnormals = new List<Abnormal>();

        public SystemInfoManager(IConnectionMultiplexer connectionMultiplexer, RedisHelper redisHelper, ISqlSugarClient client, IEquipLedgerService equipLedgerService, IBaseConfigService baseConfigService)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _redisHelper = redisHelper;
            _sqlSugarClient = client;
            _equipLedgerService = equipLedgerService;
            _baseConfigService = baseConfigService;
            _detailGenerator = new IndexBasedTableGenerator();
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

        /// <summary>
        /// 初始化主页数据
        /// </summary>
        /// <returns></returns>
        public async Task SnapshootHomeData()
        {
            DateTime now = DateTime.Now;
            // 一次性查询所有相关设备及其房间信息
            List<EquipLedger> allEquips = await _sqlSugarClient.Queryable<EquipLedger>().Includes(x => x.Room).Where(x => x.IsMeasurementDevice == null).ToListAsync();
            List<EquipLedger> expiringSoonEquips = allEquips.Where(x => x.ValidityDate != null && (x.ValidityDate - now)?.TotalDays <= 30 && (x.ValidityDate - now)?.TotalDays > 0).ToList();
            List<EquipLedger> pastDueEquips = allEquips.Where(x => x.ValidityDate != null && (x.ValidityDate - now)?.TotalDays <= 0).ToList();

            // redis
            try
            {
                EquipHealthStatusRedisTree = await _redisHelper.GetTreeAsync(EquipHealthStatusRedisKey);
                EquipLiveRedisTree = await _redisHelper.GetTreeAsync(EquipLiveRedisKey);
                EquipRunTimeRedisTree = await _redisHelper.GetTreeAsync(EquipRunTime);
            }
            catch (Exception) { }
            Abnormals = new List<Abnormal>();
            foreach (SystemInfo si in SystemInfos)
            {
                int sum = 0;
                // 记录异常
                RedisTreeNode sysSets = await _redisHelper.FindTreeNodeByPathAsync(EquipHealthStatusRedisTree, $"{EquipHealthStatusRedisKey}:{si.SystemNum}");
                if (sysSets != null && !sysSets.IsLeaf && sysSets.Children.Any())
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
                            string equipName = await _equipLedgerService.GetEquipName(equipId);
                            Abnormals.Add(new Abnormal()
                            {
                                SystemInfo = si,
                                EquipTypeNum = equipTypeNum,
                                EquipId = equipId,
                                EquipName = equipName,
                                AbnormalDescription = abnormal.ToList(),
                            });
                        }
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
                            EquipName = item.EquipName,
                            UntilDays = (item.ValidityDate.Value.Day - now.Day).ToString()
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
                            EquipName = item.EquipName,
                            AbnormalDescription = new List<string>() { $"{item.EquipName}计量过期" },
                            UntilDays = (item.ValidityDate.Value.Day - now.Day).ToString()
                        });
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
                // 记录关键设备
                List<SDevice> keyEquip = allEquip.Where(x => x.EquipLevel == EquipLevelEnum.Important).Select(x => new SDevice()
                {
                    EquipName = x.EquipName,
                    EquipId = x.Id,
                    EquipTypeNum = 0,
                }).ToList();
                si.keyDevices = keyEquip;

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

        /// <summary>
        /// 获取运行时间
        /// </summary>
        /// <param name="systemNum"></param>
        /// <param name="equipTypeNum"></param>
        /// <param name="equipId"></param>
        /// <returns></returns>
        public async Task<uint> GetRunTime(byte systemNum, Guid equipId)
        {
            RedisTreeNode sysRuntimes = await _redisHelper.FindTreeNodeByPathAsync(EquipRunTimeRedisTree, $"{EquipRunTime}:{systemNum}");
            if (sysRuntimes == null) return 0;
            RedisTreeNode equipNode = await _redisHelper.FindTreeNodeFirstByNameAsync(sysRuntimes, equipId.ToString());
            if (equipNode == null) return 0;
            uint runTime = await ReceiveHelper.GetLast30DaysRunningTimeAsync(_connectionMultiplexer, equipNode.FullPath);
            return runTime;
        }

        public async Task<List<CameraDto>> GetCameraData()
        {
            List<CameraDto> cameraData = new List<CameraDto>();
            string? url = await _baseConfigService.GetValueByKeyAsync("camera_ip");
            if (url == null) return cameraData;

            string? userName = await _baseConfigService.GetValueByKeyAsync("camera_user");
            string? password = await _baseConfigService.GetValueByKeyAsync("camera_password");

            List<string> ips = url.Split(',').ToList();
            foreach (string ip in ips)
            {
                if (string.IsNullOrEmpty(ip)) continue;
                List<string> info = ip.Split(':').ToList();
                cameraData.Add(new CameraDto()
                {
                    Ip = info[0],
                    Port = info[1],
                    UserName = userName,
                    Password = password
                });
            }
            return cameraData;
        }

        #region ====== 获取展示数据的Table ======



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
                1 => await HandleSystem1(systemInfo),
                2 => await HandleSystem2(systemInfo),
                3 => await HandleSystem3(systemInfo),
                4 => await HandleSystem4(systemInfo),
                5 => await HandleSystem5(systemInfo),
                6 => await HandleSystem6(systemInfo),
                7 => await HandleSystem7(systemInfo),
                8 => await HandleSystem8(systemInfo),
                9 => await HandleSystem9(systemInfo),
                10 => await HandleSystem10(systemInfo),
                _ => null!
            };
            return ret;
        }

        #region 310_微波/毫米波复合半实物仿真系统 雷达转台,雷达源,固定电源

        private async Task<List<Tuple<TableDto, TableDto>>> HandleSystem1(SystemInfo systemInfo)
        {
            List<Tuple<TableDto, TableDto>> tables = [await CreateTable_310_6(systemInfo), await CreateTable_310_2(systemInfo)];
            return tables;
        }

        // 雷达源
        private async Task<Tuple<TableDto, TableDto>> CreateTable_310_6(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_0_SL_6_ReceiveData), "MainCtrlDrfm1NetStatus");
            int indexEnd = GetPropertyIndex(typeof(XT_0_SL_6_ReceiveData), "IfOutputPower");

            string typeName = "雷达源";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader, ("状态类型", "离线"), ("自检状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_0_SL_6_ReceiveData>("雷达源物理量", indexStart, indexEnd);
            XT_0_SL_6_ReceiveData data = (await _sqlSugarClient.Queryable<XT_0_SL_6_ReceiveData>()
                .Where(x => x.SimuTestSysld == 1) // 雷达源是个移动设备,它可能给任何系统使用,所以要过滤一下当前系统
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            // 检查是否有数据,以及类型设备是否有心跳,要是没有数据或没有心跳,则返回默认数据
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 6)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StatusType == 1)),
                ("自检状态", GetStatus(() => data.SelfTestStatus == 0)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "雷达源物理量", indexStart, indexEnd);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        // 雷达转台
        private async Task<Tuple<TableDto, TableDto>> CreateTable_310_2(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_310_SL_2_ReceiveData), "InnerFramePosition");
            int indexEnd = GetPropertyIndex(typeof(XT_310_SL_2_ReceiveData), "OuterFrameAcceleration");
            string typeName = "转台";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", "离线"),
                ("自检状态", "离线"),
                ("运行状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_310_SL_2_ReceiveData>("转台物理量", indexStart, indexEnd);
            XT_310_SL_2_ReceiveData data = (await _sqlSugarClient.Queryable<XT_310_SL_2_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            // 检查是否有数据,以及设备是否有心跳,要是没有数据或没有心跳,则返回默认数据
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 2)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StatusType == 1)),
                ("自检状态", GetStatus(() => data.SelfTestStatus == 0)),
                ("运行状态", GetStatus(() => data.HealthOperationStatus == 0)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "转台物理量", indexStart, indexEnd);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        #endregion

        #region 307_微波寻的半实物仿真系统 阵列馈电,雷达转台,雷达源,固定电源

        private async Task<List<Tuple<TableDto, TableDto>>> HandleSystem2(SystemInfo systemInfo)
        {
            List<Tuple<TableDto, TableDto>> tables = [await CreateTable_307_1(systemInfo), await CreateTable_307_6(systemInfo), await CreateTable_307_2(systemInfo)];
            return tables;
        }

        // 阵列馈电
        private async Task<Tuple<TableDto, TableDto>> CreateTable_307_1(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_307_SL_1_ReceiveData), "Channel1ParserDevice12V");
            int indexEnd = GetPropertyIndex(typeof(XT_307_SL_1_ReceiveData), "Zone6HorizontalCoarseControlFanControl12V");
            string typeName = "阵列馈电";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", "离线"),
                ("自检状态", "离线"),
                ("电源电压状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_307_SL_1_ReceiveData>("阵列馈电物理量", indexStart, indexEnd);
            XT_307_SL_1_ReceiveData data = (await _sqlSugarClient.Queryable<XT_307_SL_1_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 1)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StateType == 1)),
                ("自检状态", GetStatus(() => data.SelfTest == 0)),
                ("电源电压状态", GetStatus(() => data.SupplyVoltageState == 0)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "阵列馈电物理量", indexStart, indexEnd);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        // 雷达转台
        private async Task<Tuple<TableDto, TableDto>> CreateTable_307_2(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_307_SL_2_ReceiveData), "InnerFramePosition");
            int indexEnd = GetPropertyIndex(typeof(XT_307_SL_2_ReceiveData), "OuterFrameAcceleration");
            string typeName = "转台";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", "离线"),
                ("自检状态", "离线"),
                ("运行状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_307_SL_2_ReceiveData>("转台物理量", indexStart, indexEnd);
            XT_307_SL_2_ReceiveData data = (await _sqlSugarClient.Queryable<XT_307_SL_2_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            // 检查是否有数据,以及设备是否有心跳,要是没有数据或没有心跳,则返回默认数据
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 2)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StatusType == 1)),
                ("自检状态", GetStatus(() => data.SelfTestStatus == 0)),
                ("运行状态", GetStatus(() => data.HealthOperationStatus == 0)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "转台物理量", indexStart, indexEnd);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        // 雷达源
        private async Task<Tuple<TableDto, TableDto>> CreateTable_307_6(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_0_SL_6_ReceiveData), "MainCtrlDrfm1NetStatus");
            int indexEnd = GetPropertyIndex(typeof(XT_0_SL_6_ReceiveData), "IfOutputPower");
            string typeName = "雷达源";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader, ("状态类型", "离线"), ("自检状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_0_SL_6_ReceiveData>("雷达源物理量", indexStart, indexEnd);
            XT_0_SL_6_ReceiveData data = (await _sqlSugarClient.Queryable<XT_0_SL_6_ReceiveData>()
                .Where(x => x.SimuTestSysld == 2) // 雷达源是个移动设备,它可能给任何系统使用,所以要过滤一下当前系统
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            // 检查是否有数据,以及类型设备是否有心跳,要是没有数据或没有心跳,则返回默认数据
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 6)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StatusType == 1)),
                ("自检状态", GetStatus(() => data.SelfTestStatus == 0)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "雷达源物理量", indexStart, indexEnd);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        #endregion

        #region 314_射频/光学制导半实物仿真系统,实际机器位置在213 阵列馈电,雷达转台+红外转台(合成一个),雷达源,固定电源

        private async Task<List<Tuple<TableDto, TableDto>>> HandleSystem3(SystemInfo systemInfo)
        {
            List<Tuple<TableDto, TableDto>> tables = [await CreateTable_314_1(systemInfo), await CreateTable_314_3(systemInfo), await CreateTable_314_6(systemInfo)];
            return tables;
        }

        // 阵列馈电
        private async Task<Tuple<TableDto, TableDto>> CreateTable_314_1(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_314_SL_1_ReceiveData), "Channel1ParserDevice12V");
            int indexEnd = GetPropertyIndex(typeof(XT_314_SL_1_ReceiveData), "Zone6HorizontalCoarseControlFanControl12V");
            string typeName = "阵列馈电";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", "离线"),
                ("自检状态", "离线"),
                ("电源电压状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_314_SL_1_ReceiveData>("阵列馈电物理量", indexStart, indexEnd);
            XT_314_SL_1_ReceiveData data = (await _sqlSugarClient.Queryable<XT_314_SL_1_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 1)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StateType == 1)),
                ("自检状态", GetStatus(() => data.SelfTest == 0)),
                ("电源电压状态", GetStatus(() => data.SupplyVoltageState == 0)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "阵列馈电物理量", indexStart, indexEnd);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        // 转台
        private async Task<Tuple<TableDto, TableDto>> CreateTable_314_3(SystemInfo systemInfo)
        {
            int indexStart1 = GetPropertyIndex(typeof(XT_314_SL_2_ReceiveData), "InnerFramePosition");
            int indexEnd1 = GetPropertyIndex(typeof(XT_314_SL_2_ReceiveData), "OuterFrameAcceleration");
            int indexStart2 = GetPropertyIndex(typeof(XT_314_SL_3_ReceiveData), "RotationAxisCommand");
            int indexEnd2 = GetPropertyIndex(typeof(XT_314_SL_3_ReceiveData), "Period");

            string typeName = "转台";
            TableDto defaultTable1 = CreateTable(typeName, CreateStandardHeader,
                ("雷达转台状态类型", "离线"),
                ("自检状态", "离线"),
                ("运行状态", "离线"));
            TableDto defaultTable2 = CreateTable(typeName, CreateStandardHeader,
                ("红外转台状态类型", "离线"),
                ("消旋工作状态", "离线"),
                ("短臂工作状态", "离线"),
                ("长臂工作状态", "离线"));
            TableDto defaultTableRet = Combine("转台", defaultTable1, defaultTable2);
            TableDto defaultDetailTable1 = _detailGenerator.GenerateTableFromInstance<XT_314_SL_2_ReceiveData>("雷达转台物理量", indexStart1, indexEnd1);
            TableDto defaultDetailTable2 = _detailGenerator.GenerateTableFromInstance<XT_314_SL_2_ReceiveData>("红外转台物理量", indexStart2, indexEnd2);
            TableDto defaultDetailTableRet = Combine("物理量", defaultDetailTable1, defaultDetailTable2);
            XT_314_SL_2_ReceiveData data1 = (await _sqlSugarClient.Queryable<XT_314_SL_2_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            XT_314_SL_3_ReceiveData data2 = (await _sqlSugarClient.Queryable<XT_314_SL_3_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            if ((data2 == null && data1 == null) || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 3)) return new Tuple<TableDto, TableDto>(defaultTableRet, defaultDetailTableRet);
            TableDto table1 = defaultTable1;
            TableDto table2 = defaultTable2;
            TableDto detailTable1 = defaultDetailTable1;
            TableDto detailTable2 = defaultDetailTable2;
            if (data1 != null)
            {
                table1 = CreateTable(typeName, CreateStandardHeader,
                    ("状态类型", GetStatus(() => data1.StatusType == 1)),
                    ("自检状态", GetStatus(() => data1.SelfTestStatus == 0)),
                    ("运行状态", GetStatus(() => data1.HealthOperationStatus == 0)));
                detailTable1 = _detailGenerator.GenerateTableFromInstance(data1, "雷达转台物理量", indexStart1, indexEnd1);
            }
            if (data2 != null)
            {
                table2 = CreateTable(typeName, CreateStandardHeader,
                    ("红外转台状态类型", GetStatus(() => data2.StatusType == 1)),
                    ("消旋工作状态", GetStatus(() => data2.RacemizationAxisOperationStatus == 0)),
                    ("短臂工作状态", GetStatus(() => data2.ShortArmAxisOperationStatus == 0)),
                    ("长臂工作状态", GetStatus(() => data2.LongArmAxisOperationStatus == 0)));
                detailTable2 = _detailGenerator.GenerateTableFromInstance(data2, "红外转台物理量", indexStart2, indexEnd2);
            }
            TableDto tableRet = Combine("转台", table1!, table2!);
            TableDto detailTableRet = Combine("转台物理量", detailTable1!, detailTable2!);
            return new Tuple<TableDto, TableDto>(tableRet, detailTableRet);
        }

        // 雷达源
        private async Task<Tuple<TableDto, TableDto>> CreateTable_314_6(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_0_SL_6_ReceiveData), "MainCtrlDrfm1NetStatus");
            int indexEnd = GetPropertyIndex(typeof(XT_0_SL_6_ReceiveData), "IfOutputPower");
            string typeName = "雷达源";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader, ("状态类型", "离线"), ("自检状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_0_SL_6_ReceiveData>("雷达源物理量", indexStart, indexEnd);
            XT_0_SL_6_ReceiveData data = (await _sqlSugarClient.Queryable<XT_0_SL_6_ReceiveData>()
                .Where(x => x.SimuTestSysld == 3) // 雷达源是个移动设备,它可能给任何系统使用,所以要过滤一下当前系统
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            // 检查是否有数据,以及类型设备是否有心跳,要是没有数据或没有心跳,则返回默认数据
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 6)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StatusType == 1)),
                ("自检状态", GetStatus(() => data.SelfTestStatus == 0)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "雷达源物理量", indexStart, indexEnd);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        #endregion

        #region 109_紧缩场射频光学半实物仿真系统 红外转台,红外源,固定电源

        private async Task<List<Tuple<TableDto, TableDto>>> HandleSystem4(SystemInfo systemInfo)
        {
            List<Tuple<TableDto, TableDto>> tables = [await CreateTable_109_7(systemInfo), await CreateTable_109_3(systemInfo)];
            return tables;
        }

        // 红外源(通用)
        private async Task<Tuple<TableDto, TableDto>> CreateTable_109_7(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_109_SL_7_ReceiveData), "Vacuum");
            int indexEnd = GetPropertyIndex(typeof(XT_109_SL_7_ReceiveData), "CoolantFlow");
            string typeName = "红外源";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", "离线"),
                ("露点温度状态", "离线"),
                ("真空度状态", "离线"),
                ("冷水机流量状态", "离线"),
                ("环境箱温度状态", "离线"),
                ("衬底温度状态", "离线"),
                ("功率电源状态", "离线"),
                ("控制电源状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_109_SL_7_ReceiveData>("红外源", indexStart, indexEnd);
            XT_109_SL_7_ReceiveData data = (await _sqlSugarClient.Queryable<XT_109_SL_7_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 7)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StatusType == 1)),
                ("露点温度状态", GetStatus(() => data.DewPointTemperatureStatus == 1)),
                ("真空度状态", GetStatus(() => data.VacuumStatus == 1)),
                ("冷水机流量状态", GetStatus(() => data.ChillerFlowStatus == 1)),
                ("环境箱温度状态", GetStatus(() => data.EnvironmentalChamberTemperatureStatus == 1)),
                ("衬底温度状态", GetStatus(() => data.SubstrateTemperatureStatus == 1)),
                ("功率电源状态", GetStatus(() => data.PowerSupplyStatus == 1)),
                ("控制电源状态", GetStatus(() => data.ControlPowerStatus == 1)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "红外源物理量", indexStart, indexEnd);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        // 红外转台
        private async Task<Tuple<TableDto, TableDto>> CreateTable_109_3(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_109_SL_3_ReceiveData), "ThreeAxisRollGiven");
            int indexEnd = GetPropertyIndex(typeof(XT_109_SL_3_ReceiveData), "OilTemperature");
            string typeName = "转台";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", "离线"),
                ("滚转轴工作状态", "离线"),
                ("俯仰轴工作状态", "离线"),
                ("偏航轴工作状态", "离线"),
                ("高低轴工作状态", "离线"),
                ("方位轴工作状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_109_SL_3_ReceiveData>("转台物理量", indexStart, indexEnd);
            XT_109_SL_3_ReceiveData data = (await _sqlSugarClient.Queryable<XT_109_SL_3_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 3)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StatusType == 1)),
                ("滚转轴工作状态", GetStatus(() => data.RollingAxisOperationStatus == 0)),
                ("俯仰轴工作状态", GetStatus(() => data.PitchAxisOperationStatus == 0)),
                ("偏航轴工作状态", GetStatus(() => data.YawAxisOperationStatus == 0)),
                ("高低轴工作状态", GetStatus(() => data.ElevationAxisOperationStatus == 0)),
                ("方位轴工作状态", GetStatus(() => data.AzimuthAxisOperationStatus == 0)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "转台物理量", indexStart, indexEnd);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        #endregion

        #region 108_光学复合半实物仿真系统 红外转台,红外源,固定电源

        private async Task<List<Tuple<TableDto, TableDto>>> HandleSystem5(SystemInfo systemInfo)
        {
            List<Tuple<TableDto, TableDto>> tables = [await CreateTable_108_7(systemInfo), await CreateTable_108_3(systemInfo)];
            return tables;
        }

        // 红外源(通用)
        private async Task<Tuple<TableDto, TableDto>> CreateTable_108_7(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_108_SL_7_ReceiveData), "Vacuum");
            int indexEnd = GetPropertyIndex(typeof(XT_108_SL_7_ReceiveData), "CoolantFlow");
            string typeName = "红外源";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", "离线"),
                ("露点温度状态", "离线"),
                ("真空度状态", "离线"),
                ("冷水机流量状态", "离线"),
                ("环境箱温度状态", "离线"),
                ("衬底温度状态", "离线"),
                ("功率电源状态", "离线"),
                ("控制电源状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_108_SL_7_ReceiveData>("红外源", indexStart, indexEnd);
            XT_108_SL_7_ReceiveData data = (await _sqlSugarClient.Queryable<XT_108_SL_7_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 7)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StatusType == 1)),
                ("露点温度状态", GetStatus(() => data.DewPointTemperatureStatus == 1)),
                ("真空度状态", GetStatus(() => data.VacuumStatus == 1)),
                ("冷水机流量状态", GetStatus(() => data.ChillerFlowStatus == 1)),
                ("环境箱温度状态", GetStatus(() => data.EnvironmentalChamberTemperatureStatus == 1)),
                ("衬底温度状态", GetStatus(() => data.SubstrateTemperatureStatus == 1)),
                ("功率电源状态", GetStatus(() => data.PowerSupplyStatus == 1)),
                ("控制电源状态", GetStatus(() => data.ControlPowerStatus == 1)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "红外源物理量", indexStart, indexEnd);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        // 红外转台
        private async Task<Tuple<TableDto, TableDto>> CreateTable_108_3(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_108_SL_3_ReceiveData), "ThreeAxisRollGiven");
            int indexEnd = GetPropertyIndex(typeof(XT_108_SL_3_ReceiveData), "TwoAxisPitchFeedback");
            string typeName = "转台";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", "离线"),
                ("工作状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_108_SL_3_ReceiveData>("转台物理量", indexStart, indexEnd);
            XT_108_SL_3_ReceiveData data = (await _sqlSugarClient.Queryable<XT_108_SL_3_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 3)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StatusType == 1)),
                ("工作状态", GetStatus(() => data.OperationStatus == 0)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "转台物理量", indexStart, indexEnd);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        #endregion

        #region 121_三通道控制红外制导半实物仿真系统 红外转台,红外源,固定电源

        private async Task<List<Tuple<TableDto, TableDto>>> HandleSystem6(SystemInfo systemInfo)
        {
            List<Tuple<TableDto, TableDto>> tables = [await CreateTable_121_7(systemInfo), await CreateTable_121_3(systemInfo)];
            return tables;
        }

        // 红外源(通用)
        private async Task<Tuple<TableDto, TableDto>> CreateTable_121_7(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_121_SL_7_ReceiveData), "Vacuum");
            int indexEnd = GetPropertyIndex(typeof(XT_121_SL_7_ReceiveData), "CoolantFlow");
            string typeName = "红外源";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", "离线"),
                ("露点温度状态", "离线"),
                ("真空度状态", "离线"),
                ("冷水机流量状态", "离线"),
                ("环境箱温度状态", "离线"),
                ("衬底温度状态", "离线"),
                ("功率电源状态", "离线"),
                ("控制电源状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_121_SL_7_ReceiveData>("红外源", indexStart, indexEnd);
            XT_121_SL_7_ReceiveData data = (await _sqlSugarClient.Queryable<XT_121_SL_7_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 7)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StatusType == 1)),
                ("露点温度状态", GetStatus(() => data.DewPointTemperatureStatus == 1)),
                ("真空度状态", GetStatus(() => data.VacuumStatus == 1)),
                ("冷水机流量状态", GetStatus(() => data.ChillerFlowStatus == 1)),
                ("环境箱温度状态", GetStatus(() => data.EnvironmentalChamberTemperatureStatus == 1)),
                ("衬底温度状态", GetStatus(() => data.SubstrateTemperatureStatus == 1)),
                ("功率电源状态", GetStatus(() => data.PowerSupplyStatus == 1)),
                ("控制电源状态", GetStatus(() => data.ControlPowerStatus == 1)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "红外源物理量", indexStart, indexEnd);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        // 红外转台
        private async Task<Tuple<TableDto, TableDto>> CreateTable_121_3(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_121_SL_3_ReceiveData), "ThreeAxisRollGiven");
            int indexEnd = GetPropertyIndex(typeof(XT_121_SL_3_ReceiveData), "Period");
            string typeName = "红外转台";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", "离线"),
                ("滚动轴工作状态", "离线"),
                ("偏航轴工作状态", "离线"),
                ("俯仰轴工作状态", "离线"),
                ("高低轴工作状态", "离线"),
                ("方位轴工作状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_121_SL_3_ReceiveData>("红外转台物理量", indexStart, indexEnd);
            XT_121_SL_3_ReceiveData data = (await _sqlSugarClient.Queryable<XT_121_SL_3_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 3)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StatusType == 1)),
                ("滚动轴工作状态", GetStatus(() => data.RollingAxisOperationStatus == 0)),
                ("偏航轴工作状态", GetStatus(() => data.YawAxisOperationStatus == 0)),
                ("俯仰轴工作状态", GetStatus(() => data.PitchAxisOperationStatus == 0)),
                ("高低轴工作状态", GetStatus(() => data.ElevationAxisOperationStatus == 0)),
                ("方位轴工作状态", GetStatus(() => data.AzimuthAxisOperationStatus == 0)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "红外转台物理量", indexStart, indexEnd);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        #endregion

        #region 202_低温环境红外制导控制半实物仿真系统 红外转台,红外源,固定电源

        private async Task<List<Tuple<TableDto, TableDto>>> HandleSystem7(SystemInfo systemInfo)
        {
            List<Tuple<TableDto, TableDto>> tables = [await CreateTable_202_7(systemInfo), await CreateTable_202_3(systemInfo)];
            return tables;
        }

        // 红外源(通用)
        private async Task<Tuple<TableDto, TableDto>> CreateTable_202_7(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_202_SL_7_ReceiveData), "Vacuum");
            int indexEnd = GetPropertyIndex(typeof(XT_202_SL_7_ReceiveData), "CoolantFlow");
            string typeName = "红外源";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", "离线"),
                ("露点温度状态", "离线"),
                ("真空度状态", "离线"),
                ("冷水机流量状态", "离线"),
                ("环境箱温度状态", "离线"),
                ("衬底温度状态", "离线"),
                ("功率电源状态", "离线"),
                ("控制电源状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_202_SL_7_ReceiveData>("红外源", indexStart, indexEnd);
            XT_202_SL_7_ReceiveData data = (await _sqlSugarClient.Queryable<XT_202_SL_7_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 7)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StatusType == 1)),
                ("露点温度状态", GetStatus(() => data.DewPointTemperatureStatus == 1)),
                ("真空度状态", GetStatus(() => data.VacuumStatus == 1)),
                ("冷水机流量状态", GetStatus(() => data.ChillerFlowStatus == 1)),
                ("环境箱温度状态", GetStatus(() => data.EnvironmentalChamberTemperatureStatus == 1)),
                ("衬底温度状态", GetStatus(() => data.SubstrateTemperatureStatus == 1)),
                ("功率电源状态", GetStatus(() => data.PowerSupplyStatus == 1)),
                ("控制电源状态", GetStatus(() => data.ControlPowerStatus == 1)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "红外源物理量", indexStart, indexEnd);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        // 红外转台
        private async Task<Tuple<TableDto, TableDto>> CreateTable_202_3(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_202_SL_3_ReceiveData), "ThreeAxisRollGiven");
            int indexEnd = GetPropertyIndex(typeof(XT_202_SL_3_ReceiveData), "WorkingDuration");
            string typeName = "转台";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", "离线"),
                ("三轴转台滚动轴状态", "离线"),
                ("三轴转台偏航轴状态", "离线"),
                ("三轴转台俯仰轴状态", "离线"),
                ("两轴转台高低轴状态", "离线"),
                ("两轴转台方位轴状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_202_SL_3_ReceiveData>("转台物理量", indexStart, indexEnd);
            XT_202_SL_3_ReceiveData data = (await _sqlSugarClient.Queryable<XT_202_SL_3_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 3)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StatusType == 1)),
                ("三轴转台滚动轴状态", GetStatus(() => data.RollAxisFaultCode == 0)),
                ("三轴转台偏航轴状态", GetStatus(() => data.YawAxisFaultCode == 0)),
                ("三轴转台俯仰轴状态", GetStatus(() => data.PitchAxisFaultCode == 0)),
                ("两轴转台高低轴状态", GetStatus(() => data.ElevationAxisFaultCode == 0)),
                ("两轴转台方位轴状态", GetStatus(() => data.AzimuthAxisFaultCode == 0)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "转台物理量", indexStart, indexEnd);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        #endregion

        #region 103_机械式制导控制半实物仿真系统 雷达转台,雷达源,固定电源

        private async Task<List<Tuple<TableDto, TableDto>>> HandleSystem8(SystemInfo systemInfo)
        {
            List<Tuple<TableDto, TableDto>> tables = [await CreateTable_103_6(systemInfo), await CreateTable_103_2(systemInfo)];
            return tables;
        }

        // 雷达转台 (通用)
        private async Task<Tuple<TableDto, TableDto>> CreateTable_103_2(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_103_SL_2_ReceiveData), "InnerFramePosition");
            int indexEnd = GetPropertyIndex(typeof(XT_103_SL_2_ReceiveData), "OuterFrameAcceleration");
            string typeName = "转台";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", "离线"),
                ("自检状态", "离线"),
                ("运行状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_103_SL_2_ReceiveData>("转台物理量", indexStart, indexEnd);
            XT_103_SL_2_ReceiveData data = (await _sqlSugarClient.Queryable<XT_103_SL_2_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            // 检查是否有数据,以及设备是否有心跳,要是没有数据或没有心跳,则返回默认数据
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 2)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StatusType == 1)),
                ("自检状态", GetStatus(() => data.SelfTestStatus == 0)),
                ("运行状态", GetStatus(() => data.HealthOperationStatus == 0)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "转台物理量", indexStart, indexEnd);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        // 雷达源
        private async Task<Tuple<TableDto, TableDto>> CreateTable_103_6(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_0_SL_6_ReceiveData), "MainCtrlDrfm1NetStatus");
            int indexEnd = GetPropertyIndex(typeof(XT_0_SL_6_ReceiveData), "IfOutputPower");
            string typeName = "雷达源";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader, ("状态类型", "离线"), ("自检状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_0_SL_6_ReceiveData>("雷达源物理量", indexStart, indexEnd);
            XT_0_SL_6_ReceiveData data = (await _sqlSugarClient.Queryable<XT_0_SL_6_ReceiveData>()
                .Where(x => x.SimuTestSysld == 8) // 雷达源是个移动设备,它可能给任何系统使用,所以要过滤一下当前系统
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            // 检查是否有数据,以及类型设备是否有心跳,要是没有数据或没有心跳,则返回默认数据
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 6)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StatusType == 1)),
                ("自检状态", GetStatus(() => data.SelfTestStatus == 0)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "雷达源物理量", indexStart, indexEnd);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        #endregion

        #region 112_独立回路半实物仿真系统 雷达转台,固定电源

        private async Task<List<Tuple<TableDto, TableDto>>> HandleSystem9(SystemInfo systemInfo)
        {
            List<Tuple<TableDto, TableDto>> tables = [await CreateTable_112_2(systemInfo)];
            return tables;
        }

        // 雷达转台
        private async Task<Tuple<TableDto, TableDto>> CreateTable_112_2(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_112_SL_2_ReceiveData), "InnerFramePosition");
            int indexEnd = GetPropertyIndex(typeof(XT_112_SL_2_ReceiveData), "OuterFrameAcceleration");
            string typeName = "转台";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", "离线"),
                ("滚动轴工作状态", "离线"),
                ("俯仰轴工作状态", "离线"),
                ("偏航轴工作状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_112_SL_2_ReceiveData>("转台物理量", indexStart, indexEnd);
            XT_112_SL_2_ReceiveData data = (await _sqlSugarClient.Queryable<XT_112_SL_2_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            // 检查是否有数据,以及设备是否有心跳,要是没有数据或没有心跳,则返回默认数据
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 2)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StatusType == 1)),
                ("滚动轴工作状态", GetStatus(() => data.RollingAxisOperationStatus == 0)),
                ("俯仰轴工作状态", GetStatus(() => data.PitchAxisOperationStatus == 0)),
                ("偏航轴工作状态", GetStatus(() => data.YawAxisOperationStatus == 0)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "转台物理量", indexStart, indexEnd);
            return new Tuple<TableDto, TableDto>(table, detailTable);
        }

        #endregion

        #region 119_独立回路/可见光制导半实物仿真系统 雷达转台,固定电源

        private async Task<List<Tuple<TableDto, TableDto>>> HandleSystem10(SystemInfo systemInfo)
        {
            List<Tuple<TableDto, TableDto>> tables = [await CreateTable_119_2(systemInfo)];
            return tables;
        }

        // 雷达转台
        private async Task<Tuple<TableDto, TableDto>> CreateTable_119_2(SystemInfo systemInfo)
        {
            int indexStart = GetPropertyIndex(typeof(XT_119_SL_2_ReceiveData), "InnerFramePosition");
            int indexEnd = GetPropertyIndex(typeof(XT_119_SL_2_ReceiveData), "OuterFrameAcceleration");
            string typeName = "转台";
            TableDto defaultTable = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", "离线"),
                ("自检状态", "离线"),
                ("运行状态", "离线"));
            TableDto defaultDetailTableDto = _detailGenerator.GenerateTableFromInstance<XT_119_SL_2_ReceiveData>("转台物理量", indexStart, indexEnd);
            XT_119_SL_2_ReceiveData data = (await _sqlSugarClient.Queryable<XT_119_SL_2_ReceiveData>()
                .OrderByDescending(x => x.CreationTime)
                .Take(1)
                .ToListAsync()).FirstOrDefault()!;
            // 检查是否有数据,以及设备是否有心跳,要是没有数据或没有心跳,则返回默认数据
            if (data == null || !systemInfo.LiveDevices.Any(x => x.EquipTypeNum == 2)) return new Tuple<TableDto, TableDto>(defaultTable, defaultDetailTableDto);
            TableDto table = CreateTable(typeName, CreateStandardHeader,
                ("状态类型", GetStatus(() => data.StatusType == 1)),
                ("自检状态", GetStatus(() => data.SelfTestStatus == 0)),
                ("运行状态", GetStatus(() => data.HealthOperationStatus == 0)));
            TableDto detailTable = _detailGenerator.GenerateTableFromInstance(data, "转台物理量", indexStart, indexEnd);
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

        /// <summary>
        /// 获取属性在类中的索引,我会先用这个函数获取一遍,然后将值硬编码到代码中,因为类结构通常不变,不需要每次都用反射获取
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private int GetPropertyIndex(Type type, string propertyName)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Name == propertyName)
                {
                    return i;
                }
            }
            return -1; // 未找到
        }

        private string GetStatus(Func<bool> where)
        {
            return where() ? "正常" : "异常";
        }

        private TableDto Combine(string title, TableDto table1, TableDto table2)
        {
            TableDto ret = new TableDto();
            ret.Title = title;
            ret.Header = CreateStandardHeader();
            ret.Data = new List<Dictionary<string, string>>();
            if (table1 != null && table1.Data.Any())
            {
                foreach (Dictionary<string, string> item in table1.Data)
                {
                    ret.Data.Add(item);
                }
            }
            if (table2 != null && table2.Data.Any())
            {
                foreach (Dictionary<string, string> item in table2.Data)
                {
                    ret.Data.Add(item);
                }
            }
            return ret;
        }

        private TableDto CreateTable(string title, Func<List<List<string>>?> createHeader, params (string key, string value)[] rows)
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

        #endregion

        #region ====== 获取图表数据 ======

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
                case 0:
                    return await GetChartData_0(ret);
                case 1:
                    return await GetChartDataAsync<XT_310_SL_4_ReceiveData>(systemInfo);
                case 2:
                    return await GetChartDataAsync<XT_307_SL_4_ReceiveData>(systemInfo);
                case 3:
                    return await GetChartDataAsync<XT_314_SL_4_ReceiveData>(systemInfo);
                case 4:
                    return await GetChartDataAsync<XT_109_SL_4_ReceiveData>(systemInfo);
                case 5:
                    return await GetChartDataAsync<XT_108_SL_4_ReceiveData>(systemInfo);
                case 6:
                    return await GetChartDataAsync<XT_121_SL_4_ReceiveData>(systemInfo);
                case 7:
                    return await GetChartDataAsync<XT_202_SL_4_ReceiveData>(systemInfo);
                case 8:
                    return await GetChartDataAsync<XT_103_SL_4_ReceiveData>(systemInfo);
                case 9:
                    return await GetChartDataAsync<XT_112_SL_4_ReceiveData>(systemInfo);
                case 10:
                    return await GetChartDataAsync<XT_119_SL_4_ReceiveData>(systemInfo);
                default:
                    return await Task.FromResult(ret);
            }
        }

        private async Task<List<ChartDataDto>> GetChartData_0(List<ChartDataDto> ret)
        {
            // 获取今天的日期（时间部分为 00:00:00）
            DateTime today = DateTime.Today;
            // 查询今天的所有数据，并按 CreationTime 降序排列
            List<XT_0_SL_5_ReceiveData> todayData = await _sqlSugarClient
                .Queryable<XT_0_SL_5_ReceiveData>()
                .Where(x => x.CreationTime >= today && x.CreationTime < today.AddDays(1)) // 今天 00:00:00 ~ 23:59:59
                .OrderBy(x => x.CreationTime)
                .ToListAsync();
            // 获取采集电压
            List<ChartDataPointDto> cdps = new List<ChartDataPointDto>();
            foreach (XT_0_SL_5_ReceiveData item in todayData)
            {
                ChartDataPointDto cdp = new ChartDataPointDto();
                cdp.Time = item.CreationTime.ToString("HH:mm");
                cdp.Value = item.PowerVoltageRead;
                cdp.Value2 = item.PowerCurrentRead;
                cdps.Add(cdp);
            }
            ret.Add(new ChartDataDto()
            {
                Name = $"电源{i}",
                Data = cdps
            });
            return ret;
        }


        public async Task<List<ChartDataDto>> GetChartDataAsync<T>(SystemInfo systemInfo) where T : class, IPowerSupplyData
        {
            var ret = new List<ChartDataDto>();

            DateTime today = DateTime.Today;

            // 查询今天的所有数据
            var todayData = await _sqlSugarClient.Queryable<T>()
                .Where(x => x.CreationTime >= today && x.CreationTime < today.AddDays(1))
                .OrderBy(x => x.CreationTime)
                .ToListAsync();

            var latestData = todayData.LastOrDefault();
            if (latestData == null) return ret;

            return GenerateChartData(todayData, latestData);
        }

        private List<ChartDataDto> GenerateChartData<T>(List<T> dataList, T latestData) where T : IPowerSupplyData
        {
            var result = new List<ChartDataDto>();

            for (int i = 1; i <= latestData.PowerSupplyCount; i++)
            {
                var cdps = dataList.Select(item => new ChartDataPointDto
                {
                    Time = item.CreationTime.ToString("HH:mm"),
                    Value = GetPowerValue(item, i, isVoltage: true),
                    Value2 = GetPowerValue(item, i, isVoltage: false)
                }).ToList();

                result.Add(new ChartDataDto
                {
                    Name = $"电源{i}",
                    Data = cdps
                });
            }

            return result;
        }

        private float GetPowerValue<T>(T item, int powerIndex, bool isVoltage) where T : IPowerSupplyData
        {
            return powerIndex switch
            {
                1 => isVoltage ? item.Power1VoltageRead : item.Power1CurrentRead,
                2 => isVoltage ? item.Power2VoltageRead : item.Power2CurrentRead,
                3 => isVoltage ? item.Power3VoltageRead : item.Power3CurrentRead,
                4 => isVoltage ? item.Power4VoltageRead : item.Power4CurrentRead,
                5 => isVoltage ? item.Power5VoltageRead : item.Power5CurrentRead,
                6 => isVoltage ? item.Power6VoltageRead : item.Power6CurrentRead,
                7 => isVoltage ? item.Power7VoltageRead : item.Power7CurrentRead,
                8 => isVoltage ? item.Power8VoltageRead : item.Power8CurrentRead,
                _ => 0f
            };
        }
        #endregion
    }
}

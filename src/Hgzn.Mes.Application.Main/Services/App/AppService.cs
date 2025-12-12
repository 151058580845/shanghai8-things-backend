using Hgzn.Mes.Application.Main.Dtos.App;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.App.IService;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.ExperimentData;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
using Microsoft.Extensions.Logging;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.App
{
    public class AppService : IAppService
    {
        protected readonly IConnectionMultiplexer _connectionMultiplexer;
        protected readonly ILogger _logger;
        protected readonly ISqlSugarClient _sqlSugarClient;
        private readonly ITestDataService _testDataService;
        // key是设备ID,value是设备异常信息
        private Dictionary<string, List<string>> _abnormalEquipDic = new Dictionary<string, List<string>>();
        private readonly RedisHelper _redisHelper;
        private const string EquipHealthStatusRedisKey = "equipHealthStatus";
        private SystemInfoManager _sysMgr; // 系统信息管理器
        private List<SystemInfo> _systemInfoList;
        private const double NumberOfSecondsPerMonth = 22 * 8 * 60 * 60;

        /// <summary>
        /// 成本计算缓存 - 用于批量计算时避免重复查询
        /// </summary>
        private class CostCalculationCache
        {
            public List<AssetData> AllAssetData { get; set; } = new();
            public Dictionary<Guid, List<Guid>> RoomEquipmentMap { get; set; } = new();
            public Dictionary<string, Dictionary<DateTime, uint>> EquipmentRuntimeCache { get; set; } = new();
            public List<TestDataReadDto> AllTasks { get; set; } = new();
        }

        public AppService(ISqlSugarClient client,
        IConnectionMultiplexer connectionMultiplexer,
        ITestDataService testDataService,
        RedisHelper redisHelper,
        IEquipLedgerService equipLedgerService,
        IBaseConfigService baseConfigService,
        ILogger<AppService> logger)
        {
            _sqlSugarClient = client;
            _connectionMultiplexer = connectionMultiplexer;
            _testDataService = testDataService;
            _redisHelper = redisHelper;
            _sysMgr = new SystemInfoManager(connectionMultiplexer, redisHelper, client, baseConfigService);
            _systemInfoList = _sysMgr.SystemInfos;
            _logger = logger;
        }

        public async Task<ShowSystemDetailDto> GetTestDetailAsync(ShowSystemDetailQueryDto showSystemDetailQueryDto)
        {
            await _sysMgr.SnapshootHomeData();

            ShowSystemDetailDto read = new ShowSystemDetailDto();
            // 优化：直接获取特定系统的最新试验数据，避免加载所有数据
            TestDataReadDto? currentTestInSystem = await _testDataService.GetCurrentLatestBySystemNameAsync(showSystemDetailQueryDto.systemName);
            TestDataReadDto? featureTestInSystem = await _testDataService.GetFeatureLatestBySystemNameAsync(showSystemDetailQueryDto.systemName);

            #region 人员展示 (已关联数据库)

            List<ExperimenterDto> experimentersData = new List<ExperimenterDto>
            {
                new ExperimenterDto
                {
                    System = "项目办",
                    Person = string.IsNullOrEmpty(currentTestInSystem?.ReqDep) ? "---" : currentTestInSystem.ReqDep
                }
            };

            var roles = new[]
            {
                new { System = "申请调度", Value = currentTestInSystem?.ReqManager },
                new { System = "制导控制专业代表", Value = currentTestInSystem?.GncResp },
                new { System = "仿真试验专业代表", Value = currentTestInSystem?.SimuResp },
                new { System = "仿真试验参与人员", Value = currentTestInSystem?.SimuStaff }
            };

            foreach (var role in roles)
            {
                if (string.IsNullOrEmpty(role.Value))
                {
                    experimentersData.Add(new ExperimenterDto { System = role.System, Person = "---" });
                }
                else
                {
                    string[] persons = role.Value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (string? person in persons.Any() ? persons : new[] { "---" })
                    {
                        experimentersData.Add(new ExperimenterDto { System = role.System, Person = person });
                    }
                }
            }
            read.ExperimentersData = experimentersData;

            #endregion

            #region 图表展示 (已关联数据库)

            // 获取图标数据
            List<ChartDataDto> chartDataPointDto = await _sysMgr.GetChartDataPointDto(_sysMgr.SystemInfos.FirstOrDefault(x => x.Name == showSystemDetailQueryDto.systemName)!);
            read.ChartData = chartDataPointDto;

            #endregion

            #region 异常设备,包括计量到期设备 (已关联数据库)

            // 异常设备
            List<AbnormalDeviceDto> abnormalDeviceDtos = new List<AbnormalDeviceDto>();
            List<Abnormal> sysAbnormals = _sysMgr.Abnormals.Where(x => x.SystemInfo.Name == showSystemDetailQueryDto.systemName).ToList();
            foreach (Abnormal abnormal in sysAbnormals)
            {
                if (abnormal.AbnormalDescription != null && abnormal.AbnormalDescription.Any())
                {
                    foreach (string ad in abnormal.AbnormalDescription)
                    {
                        abnormalDeviceDtos.Add(new AbnormalDeviceDto
                        {
                            System = abnormal.SystemInfo?.Name,
                            Device = abnormal.EquipName,
                            Value = ad,
                            EquipAssetNumber = abnormal.EquipAssetNumber,
                            Time = abnormal.UntilDays?.ToString(),
                        });
                    }
                }
                else
                {
                    abnormalDeviceDtos.Add(new AbnormalDeviceDto
                    {
                        System = abnormal.SystemInfo?.Name,
                        Device = abnormal.EquipName,
                        EquipAssetNumber = abnormal.EquipAssetNumber,
                        Time = abnormal.UntilDays?.ToString(),
                    });
                }
            }
            read.AbnormalDeviceListData = abnormalDeviceDtos;

            #endregion

            #region 任务计划 (已关联数据库)

            // 在本系统中的当前试验计划
            string currentTestName = "无";
            int currentTestActivatedDay = 0;
            string currentTestDevPhase = "无";
            string currentTestTaskEndTime = "无";
            double finishingRate = 0;
            if (currentTestInSystem != null)
            {
                currentTestName = currentTestInSystem.TaskName;
                if (DateTime.TryParse(currentTestInSystem.TaskStartTime, out DateTime startTime))
                    currentTestActivatedDay = (DateTime.Now.ToLocalTime() - startTime).Days;
                currentTestDevPhase = currentTestInSystem.DevPhase;
                currentTestTaskEndTime = DateTime.Parse(currentTestInSystem.TaskEndTime!).Date.ToString("yyyy-MM-dd");
                finishingRate = Math.Round((double)currentTestActivatedDay / (DateTime.Parse(currentTestInSystem.TaskEndTime) - DateTime.Parse(currentTestInSystem.TaskStartTime)).Days, 2) * 100;
            }
            // 在本系统中的后续试验计划（已在上面优化获取）
            string featureTestName = "无";
            int leftUntilToday = 0;
            if (featureTestInSystem != null)
            {
                featureTestName = featureTestInSystem.TaskName;
                if (DateTime.TryParse(featureTestInSystem.TaskStartTime, out DateTime startTime))
                    leftUntilToday = (startTime - DateTime.Now.ToLocalTime()).Days;
            }

            read.CurrentTask = new TaskDetailDto()
            {
                Title = "当前试验任务",
                Details = new List<List<string>>()
                {
                    new List<string>(){ "任务名称", currentTestName },
                    new List<string>(){ "已开展天数", currentTestActivatedDay + "天" },
                    new List<string>(){ "研制阶段", currentTestDevPhase },
                    new List<string>(){ "结束日期", currentTestTaskEndTime }
                },
                FinishingRate = finishingRate,
            };
            read.FollowTask = new TaskDetailDto()
            {
                Title = "后续试验任务",
                Details = new List<List<string>>()
                {
                    new List<string>(){ "任务名称", featureTestName},
                    new List<string>(){ "距今天数", leftUntilToday + "天" }
                }
            };

            #endregion

            #region 数据列表以及物理量 (已关联数据库)

            // Item1是展示在详情页的健康信息表格,Item2是详情页健康信息的详情(所有物理量的表格)
            // 这里会返回两个Tuple<TableDto, TableDto>,因为详情页右边会有展示两个系统的健康信息,随便哪个写上面或写下面都行
            List<TableDto> queue1 = new List<TableDto>();
            List<TableDto> queue2 = new List<TableDto>();
            List<TableDto> queue3 = new List<TableDto>();
            TableDto queueDetail1 = new TableDto();
            TableDto queueDetail2 = new TableDto();
            TableDto queueDetail3 = new TableDto();
            List<Tuple<TableDto, TableDto>> tables = await _sysMgr.GetTableDtos(_sysMgr.SystemInfos.FirstOrDefault(x => x.Name == showSystemDetailQueryDto.systemName)!);
            if (tables != null && tables.Count > 0 && tables[0] != null && tables[0].Item1 != null && tables[0].Item2 != null)
            {
                queue1.Add(tables[0].Item1);
                queueDetail1 = tables[0].Item2;
            }
            if (tables != null && tables.Count > 1 && tables[1] != null && tables[1].Item1 != null && tables[1].Item2 != null)
            {
                queue2.Add(tables[1].Item1);
                queueDetail2 = tables[1].Item2;
            }
            if (tables != null && tables.Count > 2 && tables[2] != null && tables[2].Item1 != null && tables[2].Item2 != null)
            {
                queue3.Add(tables[2].Item1);
                queueDetail3 = tables[2].Item2;
            }
            read.Queue = queue1;
            read.QueueDetail = queueDetail1;
            read.Queue2 = queue2;
            read.Queue2Detail = queueDetail2;
            read.Queue3 = queue3;
            read.Queue3Detail = queueDetail3;

            #endregion

            #region 产品列表 (已关联数据库)

            List<TableDto> productReadDto = new List<TableDto>();
            List<TableDto> deviceReadDto = new List<TableDto>();
            TableDto td = new TableDto()
            {
                Title = "产品列表",
                Header = new List<List<string>>()
                    {
                        new List<string> { "name", "名称" },
                        new List<string> { "code", "编号" },
                        new List<string> { "status", "技术状态" },
                    },
                Data = new List<Dictionary<string, string>>()
            };
            if (currentTestInSystem != null)
            {
                if (currentTestInSystem?.UUT != null && currentTestInSystem.UUT.Any())
                {
                    foreach (TestDataProductReadDto item in currentTestInSystem.UUT)
                    {
                        td.Data.Add(new Dictionary<string, string>()
                        {
                            { "name" , item.Name! },
                            { "code" , item.Code! },
                            { "status" , item.TechnicalStatus! },
                        });
                    }
                }
            }
            productReadDto.Add(td);

            // UST 设备列表
            TableDto ustTd = new TableDto()
            {
                Title = "UST设备列表",
                Header = new List<List<string>>()
                    {
                        new List<string> { "name", "资产名称" },
                        new List<string> { "validityPeriod", "有效期" },
                    },
                Data = new List<Dictionary<string, string>>()
            };
            if (currentTestInSystem != null)
            {
                if (currentTestInSystem?.UST != null && currentTestInSystem.UST.Any())
                {
                    foreach (TestDataUSTReadDto item in currentTestInSystem.UST)
                    {
                        // 格式化有效期，只显示到日期
                        var validityPeriod = item.ValidityPeriod;
                        if (!string.IsNullOrEmpty(validityPeriod) && DateTime.TryParse(validityPeriod, out var date))
                        {
                            validityPeriod = date.ToString("yyyy-MM-dd");
                        }

                        ustTd.Data.Add(new Dictionary<string, string>()
                        {
                            { "name" , item.Name! },
                            { "validityPeriod" , validityPeriod! },
                        });
                    }
                }
            }
            deviceReadDto.Add(ustTd);
            read.ProductList = productReadDto;
            read.DeviceList = deviceReadDto;

            #endregion

            #region 该系统设备在线率

            string? roomName = _sysMgr.SystemInfos.Where(x => x.Name == showSystemDetailQueryDto.systemName).FirstOrDefault()?.RoomNumber;
            List<EquipConnect> connectEquips = new List<EquipConnect>();
            try
            {

                connectEquips = _sqlSugarClient.Queryable<EquipConnect>()
                   .Includes(el => el.EquipLedger)
                   .Includes(el => el.EquipLedger!.Room).ToList();
                connectEquips = connectEquips.Where(x => roomName != null && x.EquipLedger != null && x.EquipLedger != null && x.EquipLedger.Room != null && x.EquipLedger.Room.Name == roomName).ToList();
            }
            catch (Exception) { }
            
            // 使用 Redis 中的实际在线状态计算在线率（而不是数据库中的 State 字段）
            int onlineCount = 0;
            foreach (var connectEquip in connectEquips)
            {
                if (connectEquip.EquipLedger != null)
                {
                    bool isOnline = await _sysMgr.GetEquipOnline(connectEquips, connectEquip.EquipLedger.Id);
                    if (isOnline)
                        onlineCount++;
                }
            }
            int workingRateData = connectEquips.Count == 0 ? 0 : (int)((double)onlineCount / connectEquips.Count() * 100);
            int offlineRateData = 100 - workingRateData;
            read.OnlineRateData = new OnlineRateData() { WorkingRateData = workingRateData, FreeRateData = 0, OfflineRateData = offlineRateData };

            #endregion

            #region 摄像头数据 (已关联数据库)

            read.CameraData = await _sysMgr.GetCameraData(showSystemDetailQueryDto.systemName);

            #endregion

            #region 温湿度计数据

            int iTemperature = 0;
            int iHumidity = 0;
            if (RKData.RoomId_TemperatureAndHumidness.ContainsKey(TestEquipData.HygrothermographRoom[TestEquipData.GetSystemRoom(showSystemDetailQueryDto.systemName!)]))
            {
                iTemperature = (int)RKData.RoomId_TemperatureAndHumidness[TestEquipData.HygrothermographRoom[TestEquipData.GetSystemRoom(showSystemDetailQueryDto.systemName!)]].Item1;
                iHumidity = (int)RKData.RoomId_TemperatureAndHumidness[TestEquipData.HygrothermographRoom[TestEquipData.GetSystemRoom(showSystemDetailQueryDto.systemName!)]].Item2;
            }
            read.Temperature = iTemperature;
            read.Humidity = iHumidity;

            #endregion

            return read;
        }

        public async Task<ShowSystemHomeDataDto> GetTestListAsync()
        {
            var stopwatch = global::System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("[GetTestListAsync] 开始执行");

            await _sysMgr.SnapshootHomeData();
            _logger.LogInformation("[GetTestListAsync] SnapshootHomeData 完成，耗时: {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

            _abnormalEquipDic = new Dictionary<string, List<string>>();
            IEnumerable<TestDataReadDto> current = await _testDataService.GetCurrentListByTestAsync();
            IEnumerable<TestDataReadDto> feature = await _testDataService.GetFeatureListByTestAsync();
            IEnumerable<TestDataReadDto> history = await _testDataService.GetHistoryListByTestAsync();
            _logger.LogInformation("[GetTestListAsync] 获取任务列表完成，耗时: {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

            ShowSystemHomeDataDto testRead = new ShowSystemHomeDataDto();
            List<EquipConnect> connectEquips = _sqlSugarClient.Queryable<EquipConnect>()
                .Includes(el => el.EquipLedger)
                .Includes(el => el.EquipLedger!.Room).ToList();
            _logger.LogInformation("[GetTestListAsync] 查询设备连接信息完成，耗时: {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            // 试验系统设备数据

            // *** 环境数据
            testRead.EnvironmentData = new EnvironmentData() { Humidity = 101, Temperature = 101 };

            // ========== 性能优化：批量预加载数据 ==========
            var batchLoadStopwatch = global::System.Diagnostics.Stopwatch.StartNew();
            
            // 1. 构建系统名称到当前任务的映射字典（避免循环中重复查找）
            Dictionary<string, TestDataReadDto> currentTaskBySystemName = current
                .GroupBy(x => NormalizeSystemName(x.SysName))
                .ToDictionary(g => g.Key, g => g.First());
            
            // 2. 批量获取所有设备的在线状态（避免循环中重复查询 Redis）
            Dictionary<Guid, bool> equipOnlineStatus = new Dictionary<Guid, bool>();
            IDatabase redisDb = _connectionMultiplexer.GetDatabase();
            foreach (var connectEquip in connectEquips)
            {
                if (connectEquip.EquipLedger != null)
                {
                    string key = string.Format(CacheKeyFormatter.EquipState, connectEquip.EquipLedger.Id, connectEquip.Id);
                    var value = await redisDb.StringGetAsync(key);
                    equipOnlineStatus[connectEquip.EquipLedger.Id] = value == 3;
                }
            }
            _logger.LogInformation("[GetTestListAsync] 批量获取设备在线状态完成，耗时: {ElapsedMs}ms", batchLoadStopwatch.ElapsedMilliseconds);
            
            // 3. 批量获取系统转台的运行时间（避免循环中重复查询 Redis）
            Dictionary<int, uint> systemRunTimeCache = new Dictionary<int, uint>();
            foreach (var sysInfo in _systemInfoList)
            {
                string runTimeKey = string.Format(CacheKeyFormatter.EquipRunTime, sysInfo.SystemNum, 3, sysInfo.TurntableEquipId);
                uint runTime = await ReceiveHelper.GetLast30DaysRunningTimeAsync(_connectionMultiplexer, runTimeKey);
                systemRunTimeCache[sysInfo.SystemNum] = runTime;
            }
            _logger.LogInformation("[GetTestListAsync] 批量获取系统转台运行时间完成，耗时: {ElapsedMs}ms", batchLoadStopwatch.ElapsedMilliseconds);
            
            // 4. 批量获取关键设备的运行时间（避免嵌套循环中重复查询 Redis）
            Dictionary<(byte systemNum, Guid equipId), uint> keyDeviceRunTimeCache = new Dictionary<(byte, Guid), uint>();
            Dictionary<(byte systemNum, Guid equipId), uint> keyDeviceTodayRunTimeCache = new Dictionary<(byte, Guid), uint>();
            HashSet<Guid> keyDeviceIds = new HashSet<Guid>();
            DateTime todayForCache = DateTime.Now.Date;
            string todayStr = todayForCache.ToString("yyyy-MM-dd");
            IDatabase redisDatabase = _connectionMultiplexer.GetDatabase();
            foreach (var sysInfo in _systemInfoList)
            {
                foreach (var kd in sysInfo.keyDevices)
                {
                    keyDeviceIds.Add(kd.EquipId);
                    string runTimeKey = string.Format(CacheKeyFormatter.EquipRunTime, sysInfo.SystemNum, kd.EquipTypeNum, kd.EquipId);
                    uint runTime = await ReceiveHelper.GetLast30DaysRunningTimeAsync(_connectionMultiplexer, runTimeKey);
                    keyDeviceRunTimeCache[(sysInfo.SystemNum, kd.EquipId)] = runTime;
                    
                    // 同时获取今天的运行时间，供关键设备利用率计算使用
                    var todayRunningSeconds = await redisDatabase.HashGetAsync($"{runTimeKey}:days", todayStr);
                    if (todayRunningSeconds.HasValue && uint.TryParse(todayRunningSeconds, out uint todaySeconds))
                    {
                        keyDeviceTodayRunTimeCache[(sysInfo.SystemNum, kd.EquipId)] = todaySeconds;
                    }
                }
            }
            _logger.LogInformation("[GetTestListAsync] 批量获取关键设备运行时间完成，耗时: {ElapsedMs}ms", batchLoadStopwatch.ElapsedMilliseconds);
            
            // 5. 批量获取关键设备的台账信息（资产编号和型号）
            Dictionary<Guid, (string? assetNumber, string? model)> keyDeviceLedgerCache = new Dictionary<Guid, (string?, string?)>();
            if (keyDeviceIds.Any())
            {
                var keyDeviceLedgers = await _sqlSugarClient.Queryable<EquipLedger>()
                    .Where(x => keyDeviceIds.Contains(x.Id))
                    .Select(x => new { x.Id, x.AssetNumber, x.Model })
                    .ToListAsync();
                
                foreach (var ledger in keyDeviceLedgers)
                {
                    keyDeviceLedgerCache[ledger.Id] = (ledger.AssetNumber, ledger.Model);
                }
            }
            _logger.LogInformation("[GetTestListAsync] 批量预加载数据完成，总耗时: {ElapsedMs}ms", batchLoadStopwatch.ElapsedMilliseconds);
            // ========== 性能优化结束 ==========

            #region 试验系统列表数据 (已关联数据库)

            var systemListStopwatch = global::System.Diagnostics.Stopwatch.StartNew();
            // *** 试验系统列表数据
            List<SystemDeviceData> list = new List<SystemDeviceData>();
            foreach (SystemInfo sysInfo in _systemInfoList)
            {
                // 获取系统温湿度,这个数据本来是打算从后台内存中读取的,现在修改为根据MQTT推送的消息,在前端获取,所以现在随便传个数字过去就行,在前端改
                int iTemperature = 0;
                int iHumidity = 0;
                if (RKData.RoomId_TemperatureAndHumidness.ContainsKey(TestEquipData.HygrothermographRoom[sysInfo.RoomId]))
                {
                    iTemperature = (int)RKData.RoomId_TemperatureAndHumidness[TestEquipData.HygrothermographRoom[sysInfo.RoomId]].Item1;
                    iHumidity = (int)RKData.RoomId_TemperatureAndHumidness[TestEquipData.HygrothermographRoom[sysInfo.RoomId]].Item2;
                }
                // 获取系统是否存在异常
                string status = "正常";
                RedisTreeNode sysTreeNode = await _redisHelper.FindTreeNodeByPathAsync(_sysMgr.EquipHealthStatusRedisTree, $"{EquipHealthStatusRedisKey}:{sysInfo.SystemNum}");
                if (sysTreeNode != null)
                {
                    // 获取该系统下所有类型的所有设备健康状态
                    List<RedisTreeNode> setTypeNodes = await _redisHelper.FindTreeNodesByTypeAsync(sysTreeNode, RedisKeyType.Set);
                    foreach (RedisTreeNode item in setTypeNodes)
                    {
                        IEnumerable<string> abnormalInfos = await _redisHelper.GetTreeNodeChildrenValuesAsync(item);
                        if (abnormalInfos.Any())
                            status = "异常";
                        if (_abnormalEquipDic.ContainsKey(item.Name) && _abnormalEquipDic[item.Name] != null)
                            _abnormalEquipDic[item.Name].AddRange(abnormalInfos.ToList());
                        else
                            _abnormalEquipDic[item.Name] = abnormalInfos.ToList();
                    }
                }
                // 型号名称
                string typeName = "空闲";
                // 任务名称
                string taskName = "无";
                // 当前试验天数
                int finishingDays = 0;
                // 使用缓存的字典查询，避免循环中重复查找
                string normalizedSystemName = NormalizeSystemName(sysInfo.Name);
                if (currentTaskBySystemName.TryGetValue(normalizedSystemName, out TestDataReadDto? currentTestInSystem))
                {
                    // 型号名称
                    string ttypeName = currentTestInSystem.ProjectName!;
                    if (!string.IsNullOrEmpty(ttypeName))
                        typeName = ttypeName;
                    // 任务名称
                    string tTaskName = currentTestInSystem.TaskName!;
                    if (!string.IsNullOrEmpty(tTaskName))
                        taskName = tTaskName;
                    // 当前试验天数
                    if (DateTime.TryParse(currentTestInSystem.TaskStartTime, out DateTime startTime))
                        finishingDays = (DateTime.Now.ToLocalTime() - startTime).Days;
                }

                list.Add(new SystemDeviceData()
                {
                    Name = sysInfo.Name,
                    SystemAbbreviationName = sysInfo.SystemAbbreviationName,
                    Quantity = sysInfo.AllEquip.Count(),
                    Temperature = iTemperature,
                    Humidity = iHumidity,
                    Status = status,
                    RoomId = sysInfo.RoomId.ToString(),
                    TypeName = typeName,
                    CurrentTaskName = taskName,
                    FinishingDays = finishingDays,
                });
            }
            testRead.SystemDeviceList = list;
            _logger.LogInformation("[GetTestListAsync] 试验系统列表数据完成，耗时: {ElapsedMs}ms", systemListStopwatch.ElapsedMilliseconds);

            #endregion

            #region 试验系统利用率数据 (已关联数据库)

            var utilizationStopwatch = global::System.Diagnostics.Stopwatch.StartNew();
            // 试验系统利用率数据
            // 系统利用率算法:将这个系统转台按月工作时间累加(秒) / 22*8*60*60
            testRead.DeviceListData = new List<DeviceUtilizationData>();
            for (int i = 0; i < _systemInfoList.Count; i++)
            {
                // 使用缓存的运行时间，避免循环中重复查询 Redis
                uint runTime = systemRunTimeCache.TryGetValue(_systemInfoList[i].SystemNum, out uint cachedRunTime) ? cachedRunTime : 0;
                int utilization = (int)Math.Round((double)((double)runTime * 100 / NumberOfSecondsPerMonth), 0);
                int idle = 100 - utilization;
                // 使用缓存的状态，避免循环中重复查询 Redis
                bool isOnline = equipOnlineStatus.TryGetValue(_systemInfoList[i].TurntableEquipId, out bool online) ? online : false;
                string status = isOnline ? "在线" : "离线";
                testRead.DeviceListData.Add(new DeviceUtilizationData()
                {
                    Name = _systemInfoList[i].Name,
                    Idle = idle,
                    Status = status,
                    Utilization = utilization,
                });
            }
            _logger.LogInformation("[GetTestListAsync] 试验系统利用率数据完成，耗时: {ElapsedMs}ms", utilizationStopwatch.ElapsedMilliseconds);

            #endregion

            #region 实验任务数据 (已关联数据库)

            var experimentDataStopwatch = global::System.Diagnostics.Stopwatch.StartNew();
            // 当前试验任务数据组织
            List<Dictionary<string, object>> currentListDic = new List<Dictionary<string, object>>();
            foreach (TestDataReadDto item in current)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>
                {
                    { "name", item.TaskName },
                    { "startend", $"{item.TaskStartTime} - {item.TaskEndTime}" },
                    { "sysname", item.SysName },
                    { "person", item.SimuResp }
                };
                currentListDic.Add(dic);
            }
            // 未来试验任务数据组织
            List<Dictionary<string, object>> featureListDic = new List<Dictionary<string, object>>();
            foreach (TestDataReadDto item in feature)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>
                {
                    { "name", item.TaskName },
                    { "startend", $"{item.TaskStartTime} - {item.TaskEndTime}" },
                    { "sysname", item.SysName },
                    { "person", item.SimuResp }
                };
                featureListDic.Add(dic);
            }
            // 历史试验任务数据组织
            List<Dictionary<string, object>> historyListDic = new List<Dictionary<string, object>>();
            foreach (TestDataReadDto item in history)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>
                {
                    { "name", item.TaskName },
                    { "startend", $"{item.TaskStartTime} - {item.TaskEndTime}"},
                    { "sysname", item.SysName },
                    { "person", item.SimuResp }
                };
                historyListDic.Add(dic);
            }
            testRead.ExperimentData = new List<ExperimentData>();
            testRead.ExperimentData.Add(new ExperimentData()
            {
                Title = "当前试验任务",
                Headers = new List<TableHeader>()
                {
                    new TableHeader() { Field = "name", Label = "任务名称" },
                    new TableHeader() { Field = "startend", Label = "试验档期" },
                    new TableHeader() { Field = "sysname", Label = "仿真系统" },
                    new TableHeader() { Field = "person", Label = "负责人" },
                },
                Data = currentListDic
            });
            testRead.ExperimentData.Add(new ExperimentData()
            {
                Title = "后续试验任务",
                Headers = new List<TableHeader>()
                {
                    new TableHeader() { Field = "name", Label = "任务名称" },
                    new TableHeader() { Field = "startend", Label = "试验档期" },
                    new TableHeader() { Field = "sysname", Label = "仿真系统" },
                    new TableHeader() { Field = "person", Label = "负责人" },
                },
                Data = featureListDic
            });
            testRead.ExperimentData.Add(new ExperimentData()
            {
                Title = "历史试验任务",
                Headers = new List<TableHeader>()
                {
                    new TableHeader() { Field = "name", Label = "任务名称" },
                    new TableHeader() { Field = "startend", Label = "试验档期" },
                    new TableHeader() { Field = "sysname", Label = "仿真系统" },
                    new TableHeader() { Field = "person", Label = "负责人" },
                },
                Data = historyListDic
            });
            _logger.LogInformation("[GetTestListAsync] 实验任务数据完成，耗时: {ElapsedMs}ms", experimentDataStopwatch.ElapsedMilliseconds);

            #endregion

            #region 异常信息列表,包括计量到期设备 (已关联数据库)

            // 异常信息列表, 这里需求改了,就显示每个系统的名字,和异常数量
            testRead.AbnormalDeviceList = new List<AbnormalDeviceData>();
            foreach (SystemInfo item in _systemInfoList)
            {
                testRead.AbnormalDeviceList.Add(new AbnormalDeviceData()
                {
                    System = item.Name,
                    AbnormalCount = item.AbnormalCount,
                    Time = DateTime.Now.ToLocalTime().ToString(),
                });
            }
            testRead.AbnormalDeviceList = testRead.AbnormalDeviceList.OrderByDescending(x => x.AbnormalCount).ToList();

            #endregion

            #region 在线率 和 故障率 (已关联数据库)

            var rateDataStopwatch = global::System.Diagnostics.Stopwatch.StartNew();
            // *** 在线设备状态统计（在线率）
            // 从所有系统的 LiveDevices 中获取活跃设备
            HashSet<Guid> liveDeviceIds = new HashSet<Guid>();
            foreach (SystemInfo item in _systemInfoList)
            {
                foreach (LDevice liveDevice in item.LiveDevices)
                {
                    liveDeviceIds.Add(liveDevice.EquipId);
                }
            }
            int liveDeviceCount = liveDeviceIds.Count;
            
            // 从 EquipDailyRuntime 表格中统计曾经活跃过的设备（去重设备ID）
            var allActiveDeviceIds = await _sqlSugarClient.Queryable<EquipDailyRuntime>()
                .Select(x => x.EquipId)
                .Distinct()
                .ToListAsync();
            
            // 计算所有活跃设备的并集（曾经活跃过的设备 + 当前活跃设备）去重
            HashSet<Guid> allDeviceIdsForRate = new HashSet<Guid>(liveDeviceIds);
            var deviceIdsList = allDeviceIdsForRate.Take(20).Select(id => id.ToString()).ToList();
            _logger.LogInformation("[GetTestListAsync] 在线设备 前20个设备ID: {DeviceIds}", string.Join(", ", deviceIdsList));
            foreach (var deviceId in allActiveDeviceIds)
            {
                allDeviceIdsForRate.Add(deviceId);
            }
            deviceIdsList = allDeviceIdsForRate.Take(20).Select(id => id.ToString()).ToList();
            _logger.LogInformation("[GetTestListAsync] 合并历史设备后 前20个设备ID: {DeviceIds}", string.Join(", ", allDeviceIdsForRate));
            int totalActiveDeviceCount = allDeviceIdsForRate.Count;
            
            // 计算在线率：活跃设备数 / ((曾经活跃过的设备数 + 活跃设备数) 的去重)
            int workingRateData = totalActiveDeviceCount == 0 ? 0 : (int)((double)liveDeviceCount / totalActiveDeviceCount * 100);
            int offlineRateData = 100 - workingRateData;
            testRead.OnlineRateData = new OnlineRateData() { WorkingRateData = workingRateData, FreeRateData = 0, OfflineRateData = offlineRateData };
            
            // 保存这些数据供设备状态统计详细数据使用
            // liveDeviceIds 和 allActiveDeviceIds 将在下面的设备状态统计详细数据部分使用
            // *** 在线设备状态统计（故障率）
            // 统计有异常的系统（包括运行异常和计量到期）
            int abnormalSysCount = testRead.AbnormalDeviceList.Where(x => x.AbnormalCount > 0).Count();
            int sysCount = testRead.AbnormalDeviceList.Count();
            int breakdownData = sysCount == 0 ? 0 : (int)((double)abnormalSysCount / (double)sysCount * 100);
            int healthData = 100 - breakdownData;
            testRead.FailureRateData = new FailureRateData() { BreakdownData = breakdownData, HealthData = healthData, PreferablyData = 0 };
            _logger.LogInformation("[GetTestListAsync] 在线率和故障率完成，耗时: {ElapsedMs}ms", rateDataStopwatch.ElapsedMilliseconds);

            #endregion

            #region 设备状态统计详细数据 (已关联数据库)

            var equipmentStateStopwatch = global::System.Diagnostics.Stopwatch.StartNew();
            // *** 设备状态统计详细数据
            // Redis 里的数据是每天凌晨用另一个线程更新到数据库的，也就是说今天的数据还不会更新上去
            // 所以获取全部设备应该是 liveDeviceIds（今天的活跃设备，来自 Redis）加上已经存在数据库里的 allActiveDeviceIds（历史活跃设备）
            // 合并并去重
            // 根据设备ID查询台账信息
            List<EquipmentData> equipDatas = new List<EquipmentData>();
            if (allDeviceIdsForRate.Any())
            {
                // 先查询完整的 EquipLedger 实体（包括 Room），然后在内存中处理
                var equipLedgers = await _sqlSugarClient.Queryable<EquipLedger>()
                    .Where(x => allDeviceIdsForRate.Contains(x.Id))
                    .Includes(x => x.Room)
                    .ToListAsync();
                
                int index = 1;
                foreach (var equipId in allDeviceIdsForRate)
                {
                    var ledger = equipLedgers.FirstOrDefault(x => x.Id == equipId);
                    if (ledger != null)
                    {
                        // 判断设备是否在线：在 LiveDevices 中就是在线
                        bool isOnline = liveDeviceIds.Contains(equipId);
                        string state = isOnline ? "在线" : "离线";
                        
                        equipDatas.Add(new EquipmentData()
                        {
                            Index = index++,
                            Code = ledger.AssetNumber ?? "",
                            Name = ledger.EquipName ?? "",
                            Location = ledger.Room?.Name ?? "",
                            State = state,
                            Health = _abnormalEquipDic.ContainsKey(equipId.ToString()) ? "异常" : "健康"
                        });
                    }
                }
            }

            testRead.EquipmentState = new EquipmentState()
            {
                Headers = new List<string[]>()
            {
                new string[] { "index", "序号" },
                new string[] { "code", "设备编号" },
                new string[] { "name", "设备名称" },
                new string[] { "location", "设备所在位置" },
                new string[] { "state", "设备在线状态" },
                new string[] { "health", "设备健康" },
            },
                Data = equipDatas
            };
            _logger.LogInformation("[GetTestListAsync] 设备状态统计详细数据完成，耗时: {ElapsedMs}ms", equipmentStateStopwatch.ElapsedMilliseconds);

            #endregion

            #region 关键设备利用率 (已关联数据库)

            var keyDeviceStopwatch = global::System.Diagnostics.Stopwatch.StartNew();
            // 关键设备利用率数据
            testRead.KeyDeviceList = new List<KeyDeviceData>();
            
            DateTime now = DateTime.Now;
            DateTime currentMonthStart = new DateTime(now.Year, now.Month, 1);
            DateTime today = now.Date;
            // 计算这个月已经过去的天数（今天是这个月的第几天）
            int daysPassedInMonth = now.Day;
            // 这个月已经过去的总时间 = 已过去天数 * 8 * 60 * 60秒
            uint totalSecondsInMonth = (uint)(daysPassedInMonth * 8 * 60 * 60);
            
            foreach (SystemInfo item in _systemInfoList)
            {
                foreach (SDevice kd in item.keyDevices)
                {
                    uint totalRunTime = 0;
                    
                    // 1. 从 EquipDailyRuntime 获取这个月（今天以前）的运行时间
                    var dbRuntimeData = await _sqlSugarClient.Queryable<EquipDailyRuntime>()
                        .Where(x => x.EquipId == kd.EquipId)
                        .Where(x => x.RecordDate >= currentMonthStart && x.RecordDate < today)
                        .Select(x => new { x.RecordDate, x.RunningSeconds })
                        .ToListAsync();
                    
                    if (dbRuntimeData.Any())
                    {
                        // 按日期分组，取每天的最大秒数（因为RunningSeconds是累积值）
                        var groupedData = dbRuntimeData
                            .GroupBy(x => x.RecordDate.Date)
                            .Select(g => new { g.Key.Date, MaxSeconds = g.Max(x => x.RunningSeconds) })
                            .ToList();
                        
                        totalRunTime += (uint)groupedData.Sum(x => x.MaxSeconds);
                    }
                    
                    // 2. 从 Redis 获取今天的运行时间（复用已缓存的 keyDeviceTodayRunTimeCache）
                    var cacheKey = (item.SystemNum, kd.EquipId);
                    if (keyDeviceTodayRunTimeCache.TryGetValue(cacheKey, out uint todaySeconds))
                    {
                        totalRunTime += todaySeconds;
                    }
                    
                    // 3. 计算利用率：总运行时间 / 这个月的总时间
                    int utilization = totalSecondsInMonth == 0 ? 0 : (int)Math.Round((double)((double)totalRunTime * 100 / totalSecondsInMonth), 0);
                    int idle = 100 - utilization;

                    // 使用缓存的台账信息，避免重复查询数据库
                    var ledgerInfo = keyDeviceLedgerCache.TryGetValue(kd.EquipId, out var cachedLedger) ? cachedLedger : (null, null);

                    testRead.KeyDeviceList.Add(new KeyDeviceData()
                    {
                        Name = kd.EquipName,
                        Utilization = utilization,
                        Idle = idle,
                        Breakdown = 0,
                        AssetNumber = ledgerInfo.assetNumber,
                        Model = ledgerInfo.model,
                    });
                }
            }
            _logger.LogInformation("[GetTestListAsync] 关键设备利用率完成，耗时: {ElapsedMs}ms", keyDeviceStopwatch.ElapsedMilliseconds);

            #endregion

            #region 按系统统计试验时间 (SystemTestTimes)

            var systemTestTimesStopwatch = global::System.Diagnostics.Stopwatch.StartNew();
            testRead.SystemTestTimes = await CalculateSystemTestTimes(current, history);
            _logger.LogInformation("[GetTestListAsync] 按系统统计试验时间完成，耗时: {ElapsedMs}ms", systemTestTimesStopwatch.ElapsedMilliseconds);

            #endregion

            #region 按型号统计试验时间 (TypeTestTimes)

            var typeTestTimesStopwatch = global::System.Diagnostics.Stopwatch.StartNew();
            testRead.TypeTestTimes = await CalculateTypeTestTimes();
            _logger.LogInformation("[GetTestListAsync] 按型号统计试验时间完成，耗时: {ElapsedMs}ms", typeTestTimesStopwatch.ElapsedMilliseconds);

            #endregion

            #region 按系统统计试验成本 (SystemTestCost)

            var systemTestCostStopwatch = global::System.Diagnostics.Stopwatch.StartNew();
            testRead.SystemTestCost = await CalculateSystemTestCost(current, history);
            _logger.LogInformation("[GetTestListAsync] 按系统统计试验成本完成，耗时: {ElapsedMs}ms", systemTestCostStopwatch.ElapsedMilliseconds);

            #endregion

            #region 按型号统计试验成本 (TypeTestCost)

            var typeTestCostStopwatch = global::System.Diagnostics.Stopwatch.StartNew();
            testRead.TypeTestCost = await CalculateTypeTestCost(current, history);
            _logger.LogInformation("[GetTestListAsync] 按型号统计试验成本完成，耗时: {ElapsedMs}ms", typeTestCostStopwatch.ElapsedMilliseconds);

            #endregion

            #region 摄像头数据 (已关联数据库)

            var cameraDataStopwatch = global::System.Diagnostics.Stopwatch.StartNew();
            testRead.CameraData = await _sysMgr.GetCameraData("home");
            _logger.LogInformation("[GetTestListAsync] 摄像头数据完成，耗时: {ElapsedMs}ms", cameraDataStopwatch.ElapsedMilliseconds);

            #endregion

            #region 各个系统的当前任务名称和已开展的天数



            #endregion

            stopwatch.Stop();
            _logger.LogInformation("[GetTestListAsync] 总耗时: {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            return testRead;
        }

        /// <summary>
        /// 计算按系统统计的试验时间
        /// </summary>
        /// <param name="currentTasks">当前试验任务</param>
        /// <param name="historyTasks">历史试验任务</param>
        /// <returns></returns>
        private async Task<SystemTestTimes> CalculateSystemTestTimes(
            IEnumerable<TestDataReadDto> currentTasks,
            IEnumerable<TestDataReadDto> historyTasks)
        {
            try
            {
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;
                SystemTestTimes systemTestTimes = new SystemTestTimes
                {
                    Times = new Dictionary<string, List<DateTimeRange>>(),
                    SystemMonthlyWorkDays = new Dictionary<string, Dictionary<NaturalMonth, int>>(),
                    TimePerMonth = new Dictionary<NaturalMonth, int>(),
                    CurrentMonthTotalSystemTestDays = 0,
                    CurrentYearTotalSystemTestDays = 0
                };

                // 初始化月度统计（不再使用去重的日期集合）
                Dictionary<NaturalMonth, int> monthlySystemDays = new Dictionary<NaturalMonth, int>();
                for (int month = 1; month <= 12; month++)
                {
                    monthlySystemDays[(NaturalMonth)month] = 0;
                }

                // 合并当前和历史试验任务
                List<TestDataReadDto> allTasks = new List<TestDataReadDto>();
                allTasks.AddRange(currentTasks);
                allTasks.AddRange(historyTasks);

                // 筛选出当前年度的试验任务
                allTasks = allTasks.Where(x =>
                {
                    if (DateTime.TryParse(x.TaskStartTime, out DateTime start) &&
                        DateTime.TryParse(x.TaskEndTime, out DateTime end))
                    {
                        return start.Year == currentYear || end.Year == currentYear;
                    }
                    return false;
                }).ToList();

                // 遍历所有系统
                for (int systemId = 1; systemId <= 10; systemId++)
                {
                    string systemName = TestEquipData.GetSystemName(systemId);
                    string roomIdStr = TestEquipData.GetRoomId(systemId);

                    if (!Guid.TryParse(roomIdStr, out Guid roomId))
                        continue;

                    // 获取该系统下的所有设备
                    List<EquipLedger> systemEquips = await _sqlSugarClient.Queryable<EquipLedger>()
                        .Where(x => x.RoomId == roomId && !x.SoftDeleted)
                        .ToListAsync();

                    // 从传入的试验任务中筛选该系统的任务
                    List<TestDataReadDto> systemTasks = allTasks.Where(t => SystemNameEquals(t.SysName, systemName)).ToList();

                    if (!systemTasks.Any())
                    {
                        // 没有试验任务，初始化该系统的月度统计
                        systemTestTimes.SystemMonthlyWorkDays[systemName] = new Dictionary<NaturalMonth, int>();
                        for (int month = 1; month <= 12; month++)
                        {
                            systemTestTimes.SystemMonthlyWorkDays[systemName][(NaturalMonth)month] = 0;
                        }
                        continue;
                    }

                    // 初始化该系统的月度工作天数统计
                    systemTestTimes.SystemMonthlyWorkDays[systemName] = new Dictionary<NaturalMonth, int>();
                    for (int month = 1; month <= 12; month++)
                    {
                        systemTestTimes.SystemMonthlyWorkDays[systemName][(NaturalMonth)month] = 0;
                    }

                    List<DateTime> workingDates = new List<DateTime>();

                    // 遍历所有试验任务，按新算法计算工作日期
                    foreach (var task in systemTasks)
                    {
                        if (!DateTime.TryParse(task.TaskStartTime, out DateTime start) ||
                            !DateTime.TryParse(task.TaskEndTime, out DateTime end))
                        {
                            continue;
                        }

                        // 只统计过去或当前正在进行的计划周期
                        if (start.Date > DateTime.Now.Date) continue;

                        // 限制结束时间不早于开始时间
                        if (end < start) continue;

                        // 查询该任务期间的真实运行数据
                        uint taskSeconds = 0;
                        if (systemEquips.Any())
                        {
                            // 先获取所有数据，然后按设备和日期分组取最大值（因为RunningSeconds是累积值）
                            var runtimeData = await _sqlSugarClient.Queryable<EquipDailyRuntime>()
                                .Where(x => systemEquips.Select(e => e.Id).Contains(x.EquipId))
                                .Where(x => x.RecordDate >= start.Date && x.RecordDate <= end.Date)
                                .Select(x => new { x.EquipId, x.RecordDate, x.RunningSeconds })
                                .ToListAsync();

                            // 按设备和日期分组，取每天的最大秒数
                            var groupedData = runtimeData
                                .GroupBy(x => new { x.EquipId, x.RecordDate.Date })
                                .Select(g => new { g.Key.Date, MaxSeconds = g.Max(x => x.RunningSeconds) })
                                .ToList();

                            taskSeconds = (uint)groupedData.Sum(x => x.MaxSeconds);

                            if (taskSeconds > 0)
                            {
                                // 有真实运行数据，使用已分组的数据收集有运行的日期
                                var actualWorkingDates = groupedData
                                    .Where(g => g.MaxSeconds > 0)
                                    .Select(g => g.Date)
                                    .Distinct()
                                    .ToList();

                                workingDates.AddRange(actualWorkingDates);
                            }
                        }

                        if (taskSeconds == 0)
                        {
                            // 无真实运行数据，按计划天数计算
                            DateTime planEndDate = end.Date;
                            // 如果计划还没结束，不能超过今天
                            if (planEndDate > DateTime.Now.Date)
                            {
                                planEndDate = DateTime.Now.Date;
                            }
                            
                            // 历史计划按整个计划周期算，当前计划截止到今天
                            for (DateTime date = start.Date; date <= planEndDate; date = date.AddDays(1))
                            {
                                workingDates.Add(date);
                            }
                        }
                    }

                    // 去重并排序
                    workingDates = workingDates.Distinct().OrderBy(d => d).ToList();

                    if (workingDates.Any())
                    {
                        // 生成连续时间段
                        List<DateTimeRange> timeRanges = GenerateContinuousTimeRanges(workingDates);
                        systemTestTimes.Times[systemName] = timeRanges;

                        // 按月统计该系统的工作天数
                        foreach (DateTime date in workingDates)
                        {
                            NaturalMonth month = (NaturalMonth)date.Month;
                            systemTestTimes.SystemMonthlyWorkDays[systemName][month]++;
                        }
                    }
                }

                // 计算每月所有系统的总工作天数（每个系统的天数相加）
                foreach (var systemName in systemTestTimes.SystemMonthlyWorkDays.Keys)
                {
                    foreach (var month in systemTestTimes.SystemMonthlyWorkDays[systemName].Keys)
                    {
                        monthlySystemDays[month] += systemTestTimes.SystemMonthlyWorkDays[systemName][month];
                    }
                }
                systemTestTimes.TimePerMonth = monthlySystemDays;

                // 计算当前月总试验天数（每个系统的天数相加）
                int currentMonthTotalDays = 0;
                foreach (var systemName in systemTestTimes.SystemMonthlyWorkDays.Keys)
                {
                    currentMonthTotalDays += systemTestTimes.SystemMonthlyWorkDays[systemName][(NaturalMonth)currentMonth];
                }
                systemTestTimes.CurrentMonthTotalSystemTestDays = currentMonthTotalDays;

                // 计算年度总试验天数（每个系统的天数相加）
                int currentYearTotalDays = 0;
                foreach (var systemName in systemTestTimes.SystemMonthlyWorkDays.Keys)
                {
                    foreach (var month in systemTestTimes.SystemMonthlyWorkDays[systemName].Keys)
                    {
                        currentYearTotalDays += systemTestTimes.SystemMonthlyWorkDays[systemName][month];
                    }
                }
                systemTestTimes.CurrentYearTotalSystemTestDays = currentYearTotalDays;

                return systemTestTimes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "计算系统试验时间统计失败");
                return new SystemTestTimes();
            }
        }

        /// <summary>
        /// 计算按型号统计的试验时间
        /// </summary>
        /// <returns></returns>
        private async Task<TypeTestTimes> CalculateTypeTestTimes()
        {
            try
            {
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;
                TypeTestTimes typeTestTimes = new TypeTestTimes
                {
                    Times = new Dictionary<string, List<DateTimeRange>>(),
                    TypeMonthlyWorkDays = new Dictionary<string, Dictionary<NaturalMonth, int>>(),
                    TimePerMonth = new Dictionary<NaturalMonth, int>(),
                    CurrentMonthTotalTypeTestDays = 0,
                    CurrentYearTotalTypeTestDays = 0
                };

                // 获取所有试验计划
                List<TestData> allTestData = await _sqlSugarClient.Queryable<TestData>()
                    .Where(x => !string.IsNullOrEmpty(x.TaskStartTime) && !string.IsNullOrEmpty(x.TaskEndTime))
                    .ToListAsync();
                
                // 筛选出当前年度的试验计划
                allTestData = allTestData.Where(x =>
                {
                    if (DateTime.TryParse(x.TaskStartTime, out DateTime start) &&
                        DateTime.TryParse(x.TaskEndTime, out DateTime end))
                    {
                        return start.Year == currentYear || end.Year == currentYear;
                    }
                    return false;
                }).ToList();

                // ========== 性能优化：批量预加载数据 ==========
                // 批量加载所有房间的设备映射
                Dictionary<Guid, List<EquipLedger>> roomEquipCache = new Dictionary<Guid, List<EquipLedger>>();
                var allRoomIds = new List<Guid>();
                for (int systemId = 1; systemId <= 10; systemId++)
                {
                    var roomIdStr = TestEquipData.GetRoomId(systemId);
                    if (Guid.TryParse(roomIdStr, out Guid roomId))
                    {
                        allRoomIds.Add(roomId);
                    }
                }
                if (allRoomIds.Any())
                {
                    var allEquips = await _sqlSugarClient.Queryable<EquipLedger>()
                        .Where(x => allRoomIds.Contains(x.RoomId.Value) && !x.SoftDeleted)
                        .ToListAsync();
                    
                    foreach (var roomId in allRoomIds.Distinct())
                    {
                        roomEquipCache[roomId] = allEquips.Where(e => e.RoomId == roomId).ToList();
                    }
                }

                // 批量加载所有设备的运行时间数据（只加载当年的数据）
                var allEquipIds = roomEquipCache.Values.SelectMany(e => e.Select(x => x.Id)).Distinct().ToList();
                Dictionary<string, Dictionary<DateTime, uint>> equipmentRuntimeCache = new Dictionary<string, Dictionary<DateTime, uint>>();
                if (allEquipIds.Any())
                {
                    var runtimeData = await _sqlSugarClient.Queryable<EquipDailyRuntime>()
                        .Where(x => allEquipIds.Contains(x.EquipId))
                        .Where(x => x.RecordDate.Year == currentYear)
                        .Select(x => new
                        {
                            x.EquipId,
                            x.RecordDate,
                            x.RunningSeconds
                        })
                        .ToListAsync();

                    // 按设备和日期分组，取每天的最大秒数（因为RunningSeconds是累积值）
                    var groupedData = runtimeData
                        .GroupBy(x => new { x.EquipId, x.RecordDate.Date })
                        .Select(g => new
                        {
                            g.Key.EquipId,
                            g.Key.Date,
                            MaxSeconds = g.Max(x => x.RunningSeconds)
                        })
                        .ToList();

                    foreach (var data in groupedData)
                    {
                        string key = $"{data.EquipId}_{data.Date:yyyy-MM-dd}";
                        if (!equipmentRuntimeCache.ContainsKey(key))
                        {
                            equipmentRuntimeCache[key] = new Dictionary<DateTime, uint>();
                        }
                        equipmentRuntimeCache[key][data.Date] = data.MaxSeconds;
                    }
                }
                // ========== 性能优化结束 ==========

                // 初始化月度统计（不再使用去重的日期集合）
                Dictionary<NaturalMonth, int> monthlyTypeDays = new Dictionary<NaturalMonth, int>();
                for (int month = 1; month <= 12; month++)
                {
                    monthlyTypeDays[(NaturalMonth)month] = 0;
                }

                // 按型号分组统计
                IEnumerable<IGrouping<string, TestData>> typeGroups = allTestData.GroupBy(x => x.ProjectName ?? "未知型号");

                foreach (IGrouping<string, TestData> typeGroup in typeGroups)
                {
                    string projectName = typeGroup.Key;
                    List<DateTime> typeWorkingDates = new List<DateTime>();

                    // 初始化该型号的月度工作天数统计
                    typeTestTimes.TypeMonthlyWorkDays[projectName] = new Dictionary<NaturalMonth, int>();
                    for (int month = 1; month <= 12; month++)
                    {
                        typeTestTimes.TypeMonthlyWorkDays[projectName][(NaturalMonth)month] = 0;
                    }

                    // 遍历该型号的所有试验任务
                    foreach (TestData testData in typeGroup)
                    {
                        if (!DateTime.TryParse(testData.TaskStartTime, out DateTime start) ||
                            !DateTime.TryParse(testData.TaskEndTime, out DateTime end))
                        {
                            continue;
                        }

                        // 只统计过去或当前正在进行的计划周期
                        if (start.Date > DateTime.Now.Date) continue;

                        // 限制结束时间不早于开始时间
                        if (end < start) continue;

                        // 根据系统名称找到对应的房间ID
                        Guid roomId = GetRoomIdBySystemName(testData.SysName);
                        if (roomId == Guid.Empty)
                            continue;

                        // 从缓存获取该系统下的所有设备
                        if (!roomEquipCache.TryGetValue(roomId, out List<EquipLedger> systemEquips))
                            systemEquips = new List<EquipLedger>();

                        // 从缓存查询该任务期间的真实运行数据
                        uint taskSeconds = 0;
                        if (systemEquips.Any())
                        {
                            foreach (var equipId in systemEquips.Select(e => e.Id))
                            {
                                for (DateTime date = start.Date; date <= end.Date; date = date.AddDays(1))
                                {
                                    string key = $"{equipId}_{date:yyyy-MM-dd}";
                                    if (equipmentRuntimeCache.TryGetValue(key, out var dateMap) &&
                                        dateMap.TryGetValue(date, out uint seconds))
                                    {
                                        taskSeconds += seconds;
                                    }
                                }
                            }
                        }

                        if (taskSeconds > 0)
                        {
                            // 有真实运行数据，从缓存中获取实际运行日期
                            HashSet<DateTime> actualWorkingDates = new HashSet<DateTime>();
                            foreach (var equipId in systemEquips.Select(e => e.Id))
                            {
                                for (DateTime date = start.Date; date <= end.Date; date = date.AddDays(1))
                                {
                                    string key = $"{equipId}_{date:yyyy-MM-dd}";
                                    if (equipmentRuntimeCache.TryGetValue(key, out var dateMap) &&
                                        dateMap.TryGetValue(date, out uint seconds) && seconds > 0)
                                    {
                                        actualWorkingDates.Add(date);
                                    }
                                }
                            }
                            typeWorkingDates.AddRange(actualWorkingDates);
                        }
                        else
                        {
                            // 无真实运行数据，按计划天数计算
                            DateTime planEndDate = end.Date;
                            // 如果计划还没结束，不能超过今天
                            if (planEndDate > DateTime.Now.Date)
                            {
                                planEndDate = DateTime.Now.Date;
                            }
                            
                            // 历史计划按整个计划周期算，当前计划截止到今天
                            for (DateTime date = start.Date; date <= planEndDate; date = date.AddDays(1))
                            {
                                typeWorkingDates.Add(date);
                            }
                        }
                    }

                    // 去重并排序
                    typeWorkingDates = typeWorkingDates.Distinct().OrderBy(d => d).ToList();

                    if (typeWorkingDates.Any())
                    {
                        // 生成连续时间段
                        List<DateTimeRange> timeRanges = GenerateContinuousTimeRanges(typeWorkingDates);
                        typeTestTimes.Times[projectName] = timeRanges;

                        // 按月统计该型号的工作天数
                        foreach (DateTime date in typeWorkingDates)
                        {
                            NaturalMonth month = (NaturalMonth)date.Month;
                            typeTestTimes.TypeMonthlyWorkDays[projectName][month]++;
                        }
                    }
                }

                // 计算每月所有型号的总工作天数（每个型号的天数相加）
                foreach (var projectName in typeTestTimes.TypeMonthlyWorkDays.Keys)
                {
                    foreach (var month in typeTestTimes.TypeMonthlyWorkDays[projectName].Keys)
                    {
                        monthlyTypeDays[month] += typeTestTimes.TypeMonthlyWorkDays[projectName][month];
                    }
                }
                typeTestTimes.TimePerMonth = monthlyTypeDays;

                // 计算当前月总试验天数（每个型号的天数相加）
                int currentMonthTotalDays = 0;
                foreach (var projectName in typeTestTimes.TypeMonthlyWorkDays.Keys)
                {
                    currentMonthTotalDays += typeTestTimes.TypeMonthlyWorkDays[projectName][(NaturalMonth)currentMonth];
                }
                typeTestTimes.CurrentMonthTotalTypeTestDays = currentMonthTotalDays;

                // 计算年度总试验天数（每个型号的天数相加）
                int currentYearTotalDays = 0;
                foreach (var projectName in typeTestTimes.TypeMonthlyWorkDays.Keys)
                {
                    foreach (var month in typeTestTimes.TypeMonthlyWorkDays[projectName].Keys)
                    {
                        currentYearTotalDays += typeTestTimes.TypeMonthlyWorkDays[projectName][month];
                    }
                }
                typeTestTimes.CurrentYearTotalTypeTestDays = currentYearTotalDays;

                return typeTestTimes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "计算型号试验时间统计失败");
                return new TypeTestTimes();
            }
        }

        /// <summary>
        /// 计算按系统统计的试验成本
        /// </summary>
        /// <param name="currentTasks">当前试验任务</param>
        /// <param name="historyTasks">历史试验任务</param>
        /// <returns></returns>
        private async Task<SystemTestCost> CalculateSystemTestCost(
            IEnumerable<TestDataReadDto> currentTasks,
            IEnumerable<TestDataReadDto> historyTasks)
        {
            try
            {
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;
                SystemTestCost systemTestCost = new SystemTestCost
                {
                    Costs = new Dictionary<string, List<CostBreakdown>>(),
                    SystemMonthlyCosts = new Dictionary<string, Dictionary<NaturalMonth, decimal>>(),
                    CostPerMonth = new Dictionary<NaturalMonth, decimal>(),
                    CurrentMonthTotalSystemTestCost = 0,
                    CurrentYearTotalSystemTestCost = 0
                };

                // 合并当前和历史试验任务
                List<TestDataReadDto> allTasks = new List<TestDataReadDto>();
                allTasks.AddRange(currentTasks);
                allTasks.AddRange(historyTasks);

                // 筛选出当前年度的试验任务
                allTasks = allTasks.Where(x =>
                {
                    if (DateTime.TryParse(x.TaskStartTime, out DateTime start) &&
                        DateTime.TryParse(x.TaskEndTime, out DateTime end))
                    {
                        return start.Year == currentYear || end.Year == currentYear;
                    }
                    return false;
                }).ToList();

                // 批量加载缓存
                var cache = await LoadCostCalculationCacheAsync(allTasks);
                var allAssetData = cache.AllAssetData;

                decimal currentMonthTotalCost = 0;
                decimal currentYearTotalCost = 0;
                Dictionary<NaturalMonth, decimal> monthlyTotalCosts = new Dictionary<NaturalMonth, decimal>();

                // 初始化月度总成本
                for (int month = 1; month <= 12; month++)
                {
                    monthlyTotalCosts[(NaturalMonth)month] = 0;
                }

                foreach (AssetData assetData in allAssetData)
                {
                    string systemName = assetData.SystemName;
                    List<CostBreakdown> systemCosts = new List<CostBreakdown>();

                    // 初始化该系统的月度成本统计
                    systemTestCost.SystemMonthlyCosts[systemName] = new Dictionary<NaturalMonth, decimal>();
                    for (int month = 1; month <= 12; month++)
                    {
                        systemTestCost.SystemMonthlyCosts[systemName][(NaturalMonth)month] = 0;
                    }

                    // 计算每个月的成本
                    for (int month = 1; month <= 12; month++)
                    {
                        CostBreakdown monthCost = await CalculateMonthlySystemCost(assetData, currentYear, month, allTasks, cache);
                        systemCosts.Add(monthCost);

                        // 计算该系统该月的总成本
                        decimal totalMonthlyCost = (monthCost.FactoryUsageFee ?? 0) +
                                             (monthCost.EquipmentUsageFee ?? 0) +
                                             (monthCost.LaborCost ?? 0) +
                                             (monthCost.ElectricityCost ?? 0) +
                                             (monthCost.FuelPowerCost ?? 0) +
                                             (monthCost.EquipmentMaintenanceCost ?? 0) +
                                             (monthCost.SystemIdleCost ?? 0);

                        // 记录该系统该月的总成本
                        systemTestCost.SystemMonthlyCosts[systemName][(NaturalMonth)month] = totalMonthlyCost;

                        // 累计所有系统该月的总成本
                        monthlyTotalCosts[(NaturalMonth)month] += totalMonthlyCost;

                        if (month == currentMonth)
                        {
                            currentMonthTotalCost += totalMonthlyCost;
                        }
                        currentYearTotalCost += totalMonthlyCost;
                    }

                    systemTestCost.Costs[systemName] = systemCosts;
                }

                systemTestCost.CostPerMonth = monthlyTotalCosts;
                systemTestCost.CurrentMonthTotalSystemTestCost = Math.Round(currentMonthTotalCost, 2);
                systemTestCost.CurrentYearTotalSystemTestCost = Math.Round(currentYearTotalCost, 2);

                return systemTestCost;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "计算系统试验成本统计失败");
                return new SystemTestCost();
            }
        }

        /// <summary>
        /// 计算按型号统计的试验成本
        /// </summary>
        /// <param name="currentTasks">当前试验任务</param>
        /// <param name="historyTasks">历史试验任务</param>
        /// <returns></returns>
        private async Task<TypeTestCost> CalculateTypeTestCost(
            IEnumerable<TestDataReadDto> currentTasks,
            IEnumerable<TestDataReadDto> historyTasks)
        {
            try
            {
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;
                TypeTestCost typeTestCost = new TypeTestCost
                {
                    Costs = new Dictionary<string, List<CostBreakdown>>(),
                    TypeMonthlyCosts = new Dictionary<string, Dictionary<NaturalMonth, decimal>>(),
                    CostPerMonth = new Dictionary<NaturalMonth, decimal>(),
                    CurrentMonthTotalTypeTestCost = 0,
                    CurrentYearTotalTypeTestCost = 0
                };

                // 合并当前和历史试验任务
                List<TestDataReadDto> allTasks = new List<TestDataReadDto>();
                allTasks.AddRange(currentTasks);
                allTasks.AddRange(historyTasks);

                // 获取所有试验计划
                List<TestData> allTestData = await _sqlSugarClient.Queryable<TestData>()
                    .Where(x => !string.IsNullOrEmpty(x.TaskStartTime) && !string.IsNullOrEmpty(x.TaskEndTime))
                    .ToListAsync();

                // 筛选出当前年度的试验计划
                List<TestData> validTestData = allTestData
                    .Where(x =>
                    {
                        if (DateTime.TryParse(x.TaskStartTime, out DateTime start) &&
                            DateTime.TryParse(x.TaskEndTime, out DateTime end))
                        {
                            return start.Year == currentYear || end.Year == currentYear;
                        }
                        return false;
                    })
                    .ToList();

                // ========== 性能优化：批量预加载数据 ==========
                // 1. 批量加载所有资产数据
                Dictionary<string, AssetData> assetDataCache = (await _sqlSugarClient.Queryable<AssetData>()
                    .Includes(x => x.Projects)
                    .ToListAsync())
                    .ToDictionary(x => x.SystemName, x => x);

                // 2. 批量加载所有房间的设备映射
                Dictionary<Guid, List<EquipLedger>> roomEquipCache = new Dictionary<Guid, List<EquipLedger>>();
                var allRoomIds = new List<Guid>();
                for (int systemId = 1; systemId <= 10; systemId++)
                {
                    var roomIdStr = TestEquipData.GetRoomId(systemId);
                    if (Guid.TryParse(roomIdStr, out Guid roomId))
                    {
                        allRoomIds.Add(roomId);
                    }
                }
                if (allRoomIds.Any())
                {
                    var allEquips = await _sqlSugarClient.Queryable<EquipLedger>()
                        .Where(x => allRoomIds.Contains(x.RoomId.Value) && !x.SoftDeleted)
                        .ToListAsync();
                    
                    foreach (var roomId in allRoomIds.Distinct())
                    {
                        roomEquipCache[roomId] = allEquips.Where(e => e.RoomId == roomId).ToList();
                    }
                }

                // 3. 批量加载所有设备的运行时间数据（只加载当年的数据）
                var allEquipIds = roomEquipCache.Values.SelectMany(e => e.Select(x => x.Id)).Distinct().ToList();
                Dictionary<string, Dictionary<DateTime, uint>> equipmentRuntimeCache = new Dictionary<string, Dictionary<DateTime, uint>>();
                if (allEquipIds.Any())
                {
                    var runtimeData = await _sqlSugarClient.Queryable<EquipDailyRuntime>()
                        .Where(x => allEquipIds.Contains(x.EquipId))
                        .Where(x => x.RecordDate.Year == currentYear)
                        .Select(x => new
                        {
                            x.EquipId,
                            x.RecordDate,
                            x.RunningSeconds
                        })
                        .ToListAsync();

                    // 按设备和日期分组，取每天的最大秒数（因为RunningSeconds是累积值）
                    var groupedData = runtimeData
                        .GroupBy(x => new { x.EquipId, x.RecordDate.Date })
                        .Select(g => new
                        {
                            g.Key.EquipId,
                            g.Key.Date,
                            MaxSeconds = g.Max(x => x.RunningSeconds)
                        })
                        .ToList();

                    foreach (var data in groupedData)
                    {
                        string key = $"{data.EquipId}_{data.Date:yyyy-MM-dd}";
                        if (!equipmentRuntimeCache.ContainsKey(key))
                        {
                            equipmentRuntimeCache[key] = new Dictionary<DateTime, uint>();
                        }
                        equipmentRuntimeCache[key][data.Date] = data.MaxSeconds;
                    }
                }
                // ========== 性能优化结束 ==========

                decimal currentMonthTotalCost = 0;
                decimal currentYearTotalCost = 0;
                Dictionary<NaturalMonth, decimal> monthlyTotalCosts = new Dictionary<NaturalMonth, decimal>();

                // 初始化月度总成本
                for (int month = 1; month <= 12; month++)
                {
                    monthlyTotalCosts[(NaturalMonth)month] = 0;
                }

                // 按型号分组统计成本
                IEnumerable<IGrouping<string, TestData>> typeGroups = validTestData.GroupBy(x => x.ProjectName ?? "未知型号");

                foreach (IGrouping<string, TestData> typeGroup in typeGroups)
                {
                    string projectName = typeGroup.Key;
                    List<CostBreakdown> typeCosts = new List<CostBreakdown>();

                    // 初始化该型号的月度成本统计
                    typeTestCost.TypeMonthlyCosts[projectName] = new Dictionary<NaturalMonth, decimal>();
                    for (int month = 1; month <= 12; month++)
                    {
                        typeTestCost.TypeMonthlyCosts[projectName][(NaturalMonth)month] = 0;
                    }

                    // 计算每个月的成本
                    for (int month = 1; month <= 12; month++)
                    {
                        CostBreakdown monthCost = await CalculateMonthlyTypeCostWithCache(typeGroup, currentYear, month, allTasks, assetDataCache, roomEquipCache, equipmentRuntimeCache);
                        typeCosts.Add(monthCost);

                        // 计算该型号该月的总成本
                        decimal totalMonthlyCost = (monthCost.FactoryUsageFee ?? 0) +
                                             (monthCost.EquipmentUsageFee ?? 0) +
                                             (monthCost.LaborCost ?? 0) +
                                             (monthCost.ElectricityCost ?? 0) +
                                             (monthCost.FuelPowerCost ?? 0) +
                                             (monthCost.EquipmentMaintenanceCost ?? 0) +
                                             (monthCost.SystemIdleCost ?? 0);

                        // 记录该型号该月的总成本
                        typeTestCost.TypeMonthlyCosts[projectName][(NaturalMonth)month] = totalMonthlyCost;

                        // 累计所有型号该月的总成本
                        monthlyTotalCosts[(NaturalMonth)month] += totalMonthlyCost;

                        if (month == currentMonth)
                        {
                            currentMonthTotalCost += totalMonthlyCost;
                        }
                        currentYearTotalCost += totalMonthlyCost;
                    }

                    typeTestCost.Costs[projectName] = typeCosts;
                }

                typeTestCost.CostPerMonth = monthlyTotalCosts;
                typeTestCost.CurrentMonthTotalTypeTestCost = Math.Round(currentMonthTotalCost, 2);
                typeTestCost.CurrentYearTotalTypeTestCost = Math.Round(currentYearTotalCost, 2);

                return typeTestCost;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "计算型号试验成本统计失败");
                return new TypeTestCost();
            }
        }

        /// <summary>
        /// 生成连续时间段
        /// </summary>
        /// <param name="dates">排序后的日期列表</param>
        /// <returns>连续时间段列表</returns>
        private List<DateTimeRange> GenerateContinuousTimeRanges(List<DateTime> dates)
        {
            List<DateTimeRange> ranges = new List<DateTimeRange>();
            if (!dates.Any())
                return ranges;

            DateTime start = dates[0];
            DateTime end = dates[0];

            for (int i = 1; i < dates.Count; i++)
            {
                if ((dates[i] - end).Days == 1) // 连续的日期
                {
                    end = dates[i];
                }
                else // 不连续，创建一个时间段
                {
                    ranges.Add(new DateTimeRange { Start = start, End = end });
                    start = dates[i];
                    end = dates[i];
                }
            }

            // 添加最后一个时间段
            ranges.Add(new DateTimeRange { Start = start, End = end });
            return ranges;
        }

        /// <summary>
        /// 计算系统某个月的成本分解（按新算法：基于试验计划周期）
        /// </summary>
        /// <param name="assetData">资产数据</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="allTasks">所有试验任务（当前+历史）</param>
        /// <param name="cache">成本计算缓存</param>
        /// <returns>月度成本分解</returns>
        private Task<CostBreakdown> CalculateMonthlySystemCost(AssetData assetData, int year, int month, List<TestDataReadDto> allTasks, CostCalculationCache cache)
        {  
            try
            {
                const decimal ELECTRICITY_UNIT_PRICE = 1m; // 电费单价，单位：元/千瓦时

                CostBreakdown costBreakdown = new CostBreakdown
                {
                    NaturalMonth = (NaturalMonth)month
                };

                // 获取该系统在指定月份的工作天数和工作时长
                Guid roomId = GetRoomIdBySystemName(assetData.SystemName);
                if (roomId == Guid.Empty)
                    return Task.FromResult(costBreakdown);

                // 从缓存获取设备列表
                if (!cache.RoomEquipmentMap.TryGetValue(roomId, out var systemEquipIds))
                {
                    systemEquipIds = new List<Guid>();
                }

                // 从传入的试验任务中筛选该系统的任务
                List<TestDataReadDto> systemTasks = allTasks.Where(t => SystemNameEquals(t.SysName, assetData.SystemName)).ToList();

                // 指定月份的起止时间
                DateTime monthStart = new DateTime(year, month, 1);
                DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);
                
                // 如果月份起始时间就在未来，直接返回空成本
                if (monthStart > DateTime.Now.Date)
                {
                    return Task.FromResult(costBreakdown);
                }
                
                if (monthEnd > DateTime.Now.Date)
                    monthEnd = DateTime.Now.Date;

                int workingDays = 0;
                decimal totalWorkingHours = 0;
                HashSet<DateTime> workingDates = new HashSet<DateTime>();

                // 遍历所有试验任务，计算该月的工作天数和工作时长
                foreach (var task in systemTasks)
                {
                    if (!DateTime.TryParse(task.TaskStartTime, out DateTime taskStart) ||
                        !DateTime.TryParse(task.TaskEndTime, out DateTime taskEnd))
                    {
                        continue;
                    }

                    // 只统计过去或当前正在进行的计划周期
                    if (taskStart.Date > DateTime.Now.Date) continue;

                    // 计算任务与该月的交集
                    DateTime start = taskStart.Date > monthStart ? taskStart.Date : monthStart;
                    DateTime end = taskEnd.Date < monthEnd ? taskEnd.Date : monthEnd;

                    // 如果任务与该月没有交集，跳过
                    if (start > end) continue;

                    // 从缓存查询该任务在该月期间的真实运行数据
                    uint taskSeconds = 0;
                    if (systemEquipIds.Any())
                    {
                        taskSeconds = GetEquipmentRuntimeFromCache(systemEquipIds, start, end, cache);
                    }

                    if (taskSeconds > 0)
                    {
                        // 有真实运行数据，从缓存中获取实际运行日期
                        HashSet<DateTime> actualWorkingDates = new HashSet<DateTime>();
                        foreach (var equipId in systemEquipIds)
                        {
                            for (DateTime date = start.Date; date <= end.Date; date = date.AddDays(1))
                            {
                                string key = $"{equipId}_{date:yyyy-MM-dd}";
                                if (cache.EquipmentRuntimeCache.TryGetValue(key, out var dateMap) &&
                                    dateMap.TryGetValue(date, out uint seconds) && seconds > 0)
                                {
                                    actualWorkingDates.Add(date);
                                }
                            }
                        }

                        foreach (var date in actualWorkingDates)
                        {
                            workingDates.Add(date);
                        }
                        totalWorkingHours += Math.Round((decimal)taskSeconds / 3600, 2);
                    }
                    else
                    {
                        // 无真实运行数据，按任务期间每天8小时计算
                        for (DateTime date = start; date <= end; date = date.AddDays(1))
                        {
                            workingDates.Add(date);
                        }
                        int days = (int)(end - start).TotalDays + 1;
                        totalWorkingHours += days * 8;
                    }
                }

                workingDays = workingDates.Count;
                
                // 计算该月实际天数（如果是当前月份，只计算到今天）
                DateTime actualMonthEnd = monthEnd;
                if (month == DateTime.Now.Month && year == DateTime.Now.Year)
                {
                    actualMonthEnd = DateTime.Now.Date;
                }
                int actualDaysInMonth = (int)(actualMonthEnd - monthStart).TotalDays + 1;
                int idleDays = actualDaysInMonth - workingDays;

                // 计算当前年已经过去的天数（用于年度成本分摊）
                DateTime yearStart = new DateTime(year, 1, 1);
                DateTime yearEnd = year == DateTime.Now.Year ? DateTime.Now.Date : new DateTime(year, 12, 31);
                int daysPassedInYear = (int)(yearEnd - yearStart).TotalDays + 1;

                // 计算各项固定成本的日费率
                decimal dailyFactoryFee = (assetData.FactoryUsageFee ?? 0) / daysPassedInYear;
                decimal dailyEquipmentFee = (assetData.EquipmentUsageFee ?? 0) / daysPassedInYear;
                decimal dailyMaintenanceFee = (assetData.EquipmentMaintenanceCost ?? 0) / daysPassedInYear;
                
                // 计算该自然月的总天数（用于成本分摊）
                int naturalMonthDays = DateTime.DaysInMonth(year, month);
                // 如果是当前月份，只计算到今天
                int daysForCostAllocation = (month == DateTime.Now.Month && year == DateTime.Now.Year) 
                    ? actualDaysInMonth 
                    : naturalMonthDays;
                
                costBreakdown.FactoryUsageFee = Math.Round(dailyFactoryFee * daysForCostAllocation, 2);
                costBreakdown.EquipmentUsageFee = Math.Round(dailyEquipmentFee * daysForCostAllocation, 2);
                costBreakdown.EquipmentMaintenanceCost = Math.Round(dailyMaintenanceFee * daysForCostAllocation, 2);

                // 人力成本：根据该月试验任务的人员数量和工作时长计算
                int staffCount = 0;
                foreach (var task in systemTasks)
                {
                    if (!DateTime.TryParse(task.TaskStartTime, out DateTime taskStart) ||
                        !DateTime.TryParse(task.TaskEndTime, out DateTime taskEnd))
                    {
                        continue;
                    }

                    // 只统计过去或当前正在进行的计划周期
                    if (taskStart.Date > DateTime.Now.Date) continue;

                    // 计算任务与该月的交集
                    DateTime start = taskStart.Date > monthStart ? taskStart.Date : monthStart;
                    DateTime end = taskEnd.Date < monthEnd ? taskEnd.Date : monthEnd;

                    // 如果任务与该月没有交集，跳过
                    if (start > end) continue;

                    // 计算人员岗位数量
                    int taskStaffCount = 0;
                    if (!string.IsNullOrEmpty(task.SimuResp)) taskStaffCount += 1; // 专业代表
                    if (!string.IsNullOrEmpty(task.SimuStaff))
                    {
                        var staffNames = task.SimuStaff.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        taskStaffCount += staffNames.Length; // 参与人员
                    }
                    staffCount += taskStaffCount;
                }

                if (assetData.LaborCostPerHour.HasValue && assetData.LaborCostPerHour.Value > 0 && staffCount > 0 && totalWorkingHours > 0)
                {
                    costBreakdown.LaborCost = Math.Round(staffCount * totalWorkingHours * assetData.LaborCostPerHour.Value, 2);
                }
                else
                {
                    costBreakdown.LaborCost = 0;
                }

                // 电费：调整后工作时长 × 系统能耗 × 电费单价
                if (assetData.SystemEnergyConsumption.HasValue && totalWorkingHours > 0)
                {
                    costBreakdown.ElectricityCost = Math.Round(totalWorkingHours * assetData.SystemEnergyConsumption.Value * ELECTRICITY_UNIT_PRICE, 2);
                }
                else
                {
                    costBreakdown.ElectricityCost = 0;
                }

                // 燃料动力费：燃料动力费单价 × 调整后工作时长
                if (assetData.FuelPowerCostPerHour.HasValue && assetData.FuelPowerCostPerHour.Value > 0 && totalWorkingHours > 0)
                {
                    costBreakdown.FuelPowerCost = Math.Round(assetData.FuelPowerCostPerHour.Value * totalWorkingHours, 2);
                }
                else
                {
                    costBreakdown.FuelPowerCost = 0;
                }

                // 系统空置成本（空置天数的成本）
                // 使用与固定成本相同的日费率
                if (idleDays > 0)
                {
                    costBreakdown.SystemIdleCost = Math.Round(idleDays * (dailyFactoryFee + dailyEquipmentFee + dailyMaintenanceFee), 2);
                }
                else
                {
                    costBreakdown.SystemIdleCost = 0;
                }

                return Task.FromResult(costBreakdown);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "计算系统月度成本失败: {SystemName}, {Year}-{Month}", assetData.SystemName, year, month);
                return Task.FromResult(new CostBreakdown { NaturalMonth = (NaturalMonth)month });
            }
        }

        /// <summary>
        /// 计算型号某个月的成本分解（使用缓存，避免重复查询）
        /// </summary>
        private Task<CostBreakdown> CalculateMonthlyTypeCostWithCache(
            IGrouping<string, TestData> typeGroup, 
            int year, 
            int month, 
            List<TestDataReadDto> allTasks,
            Dictionary<string, AssetData> assetDataCache,
            Dictionary<Guid, List<EquipLedger>> roomEquipCache,
            Dictionary<string, Dictionary<DateTime, uint>> equipmentRuntimeCache)
        {
            try
            {
                const decimal ELECTRICITY_UNIT_PRICE = 1m;

                CostBreakdown costBreakdown = new CostBreakdown
                {
                    NaturalMonth = (NaturalMonth)month
                };

                // 获取该型号在该月涉及的所有系统（去重）
                HashSet<string> involvedSystems = new HashSet<string>();
                foreach (TestData testData in typeGroup)
                {
                    if (!string.IsNullOrEmpty(testData.SysName))
                    {
                        involvedSystems.Add(testData.SysName);
                    }
                }

                DateTime monthStart = new DateTime(year, month, 1);
                DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);
                
                if (monthStart > DateTime.Now.Date)
                {
                    return Task.FromResult(costBreakdown);
                }
                
                if (monthEnd > DateTime.Now.Date)
                    monthEnd = DateTime.Now.Date;

                DateTime actualMonthEnd = monthEnd;
                if (month == DateTime.Now.Month && year == DateTime.Now.Year)
                {
                    actualMonthEnd = DateTime.Now.Date;
                }

                decimal totalFactoryUsageFee = 0;
                decimal totalEquipmentUsageFee = 0;
                decimal totalLaborCost = 0;
                decimal totalElectricityCost = 0;
                decimal totalFuelPowerCost = 0;
                decimal totalEquipmentMaintenanceCost = 0;
                decimal totalSystemIdleCost = 0;

                // 遍历该型号涉及的每个系统
                foreach (string systemName in involvedSystems)
                {
                    // 从缓存获取资产数据
                    if (!assetDataCache.TryGetValue(systemName, out AssetData assetData) || assetData == null)
                        continue;

                    // 获取该系统的房间ID和设备
                    Guid roomId = GetRoomIdBySystemName(systemName);
                    if (roomId == Guid.Empty)
                        continue;

                    if (!roomEquipCache.TryGetValue(roomId, out List<EquipLedger> systemEquips))
                        systemEquips = new List<EquipLedger>();

                    // 从传入的试验任务中筛选该系统且该型号的任务
                    string projectName = typeGroup.Key;
                    List<TestDataReadDto> typeSystemTasks = allTasks
                        .Where(t => SystemNameEquals(t.SysName, systemName) && t.ProjectName == projectName)
                        .ToList();

                    if (!typeSystemTasks.Any())
                        continue;

                    int workingDays = 0;
                    decimal totalWorkingHours = 0;
                    HashSet<DateTime> workingDates = new HashSet<DateTime>();
                    int staffCount = 0;

                    // 遍历该型号在该系统的所有试验任务，计算该月的工作天数和工作时长
                    foreach (var task in typeSystemTasks)
                    {
                        if (!DateTime.TryParse(task.TaskStartTime, out DateTime taskStart) ||
                            !DateTime.TryParse(task.TaskEndTime, out DateTime taskEnd))
                        {
                            continue;
                        }

                        if (taskStart.Date > DateTime.Now.Date) continue;

                        DateTime start = taskStart.Date > monthStart ? taskStart.Date : monthStart;
                        DateTime end = taskEnd.Date < monthEnd ? taskEnd.Date : monthEnd;

                        if (start > end) continue;

                        // 计算人员岗位数量
                        int taskStaffCount = 0;
                        if (!string.IsNullOrEmpty(task.SimuResp)) taskStaffCount += 1;
                        if (!string.IsNullOrEmpty(task.SimuStaff))
                        {
                            var staffNames = task.SimuStaff.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            taskStaffCount += staffNames.Length;
                        }
                        staffCount += taskStaffCount;

                        // 从缓存查询该任务在该月期间的真实运行数据
                        uint taskSeconds = 0;
                        foreach (var equipId in systemEquips.Select(e => e.Id))
                        {
                            for (DateTime date = start.Date; date <= end.Date; date = date.AddDays(1))
                            {
                                string key = $"{equipId}_{date:yyyy-MM-dd}";
                                if (equipmentRuntimeCache.TryGetValue(key, out var dateMap) &&
                                    dateMap.TryGetValue(date, out uint seconds))
                                {
                                    taskSeconds += seconds;
                                }
                            }
                        }

                        if (taskSeconds > 0)
                        {
                            // 有真实运行数据，从缓存中获取实际运行日期
                            HashSet<DateTime> actualWorkingDates = new HashSet<DateTime>();
                            foreach (var equipId in systemEquips.Select(e => e.Id))
                            {
                                for (DateTime date = start.Date; date <= end.Date; date = date.AddDays(1))
                                {
                                    string key = $"{equipId}_{date:yyyy-MM-dd}";
                                    if (equipmentRuntimeCache.TryGetValue(key, out var dateMap) &&
                                        dateMap.TryGetValue(date, out uint seconds) && seconds > 0)
                                    {
                                        actualWorkingDates.Add(date);
                                    }
                                }
                            }

                            foreach (var date in actualWorkingDates)
                            {
                                workingDates.Add(date);
                            }
                            totalWorkingHours += Math.Round((decimal)taskSeconds / 3600, 2);
                        }
                        else
                        {
                            // 无真实运行数据，按任务期间每天8小时计算
                            for (DateTime date = start; date <= end; date = date.AddDays(1))
                            {
                                workingDates.Add(date);
                            }
                            int days = (int)(end - start).TotalDays + 1;
                            totalWorkingHours += days * 8;
                        }
                    }

                    workingDays = workingDates.Count;
                    int actualDaysInMonth = (int)(actualMonthEnd - monthStart).TotalDays + 1;
                    int idleDays = actualDaysInMonth - workingDays;

                    DateTime yearStart = new DateTime(year, 1, 1);
                    DateTime yearEnd = year == DateTime.Now.Year ? DateTime.Now.Date : new DateTime(year, 12, 31);
                    int daysPassedInYear = (int)(yearEnd - yearStart).TotalDays + 1;

                    decimal dailyFactoryFee = (assetData.FactoryUsageFee ?? 0) / daysPassedInYear;
                    decimal dailyEquipmentFee = (assetData.EquipmentUsageFee ?? 0) / daysPassedInYear;
                    decimal dailyMaintenanceFee = (assetData.EquipmentMaintenanceCost ?? 0) / daysPassedInYear;

                    int naturalMonthDays = DateTime.DaysInMonth(year, month);
                    int daysForCostAllocation = (month == DateTime.Now.Month && year == DateTime.Now.Year) 
                        ? actualDaysInMonth 
                        : naturalMonthDays;

                    totalFactoryUsageFee += Math.Round(dailyFactoryFee * daysForCostAllocation, 2);
                    totalEquipmentUsageFee += Math.Round(dailyEquipmentFee * daysForCostAllocation, 2);
                    totalEquipmentMaintenanceCost += Math.Round(dailyMaintenanceFee * daysForCostAllocation, 2);

                    if (assetData.LaborCostPerHour.HasValue && assetData.LaborCostPerHour.Value > 0 && staffCount > 0 && totalWorkingHours > 0)
                    {
                        totalLaborCost += Math.Round(staffCount * totalWorkingHours * assetData.LaborCostPerHour.Value, 2);
                    }

                    if (assetData.SystemEnergyConsumption.HasValue && totalWorkingHours > 0)
                    {
                        totalElectricityCost += Math.Round(totalWorkingHours * assetData.SystemEnergyConsumption.Value * ELECTRICITY_UNIT_PRICE, 2);
                    }

                    if (assetData.FuelPowerCostPerHour.HasValue && assetData.FuelPowerCostPerHour.Value > 0 && totalWorkingHours > 0)
                    {
                        totalFuelPowerCost += Math.Round(assetData.FuelPowerCostPerHour.Value * totalWorkingHours, 2);
                    }

                    if (idleDays > 0)
                    {
                        totalSystemIdleCost += Math.Round(idleDays * (dailyFactoryFee + dailyEquipmentFee + dailyMaintenanceFee), 2);
                    }
                }

                costBreakdown.FactoryUsageFee = Math.Round(totalFactoryUsageFee, 2);
                costBreakdown.EquipmentUsageFee = Math.Round(totalEquipmentUsageFee, 2);
                costBreakdown.LaborCost = Math.Round(totalLaborCost, 2);
                costBreakdown.ElectricityCost = Math.Round(totalElectricityCost, 2);
                costBreakdown.FuelPowerCost = Math.Round(totalFuelPowerCost, 2);
                costBreakdown.EquipmentMaintenanceCost = Math.Round(totalEquipmentMaintenanceCost, 2);
                costBreakdown.SystemIdleCost = Math.Round(totalSystemIdleCost, 2);

                return Task.FromResult(costBreakdown);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "计算型号月度成本失败(缓存版): {ProjectName}, {Year}-{Month}", typeGroup.Key, year, month);
                return Task.FromResult(new CostBreakdown { NaturalMonth = (NaturalMonth)month });
            }
        }

        /// <summary>
        /// 计算型号某个月的成本分解（按新算法：基于试验计划周期）
        /// </summary>
        /// <param name="typeGroup">型号试验数据组</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="allTasks">所有试验任务（当前+历史）</param>
        /// <returns>月度成本分解</returns>
        private async Task<CostBreakdown> CalculateMonthlyTypeCost(IGrouping<string, TestData> typeGroup, int year, int month, List<TestDataReadDto> allTasks)
        {
            try
            {
                const decimal ELECTRICITY_UNIT_PRICE = 1m; // 电费单价，单位：元/千瓦时

                CostBreakdown costBreakdown = new CostBreakdown
                {
                    NaturalMonth = (NaturalMonth)month
                };

                // 获取该型号在该月涉及的所有系统（去重）
                HashSet<string> involvedSystems = new HashSet<string>();
                foreach (TestData testData in typeGroup)
                {
                    if (!string.IsNullOrEmpty(testData.SysName))
                    {
                        involvedSystems.Add(testData.SysName);
                    }
                }

                // 指定月份的起止时间
                DateTime monthStart = new DateTime(year, month, 1);
                DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);
                
                // 如果月份起始时间就在未来，直接返回空成本
                if (monthStart > DateTime.Now.Date)
                {
                    return costBreakdown;
                }
                
                if (monthEnd > DateTime.Now.Date)
                    monthEnd = DateTime.Now.Date;

                // 计算该月实际天数（如果是当前月份，只计算到今天）
                DateTime actualMonthEnd = monthEnd;
                if (month == DateTime.Now.Month && year == DateTime.Now.Year)
                {
                    actualMonthEnd = DateTime.Now.Date;
                }

                decimal totalFactoryUsageFee = 0;
                decimal totalEquipmentUsageFee = 0;
                decimal totalLaborCost = 0;
                decimal totalElectricityCost = 0;
                decimal totalFuelPowerCost = 0;
                decimal totalEquipmentMaintenanceCost = 0;
                decimal totalSystemIdleCost = 0;

                // 遍历该型号涉及的每个系统
                foreach (string systemName in involvedSystems)
                {
                    // 获取系统的资产数据
                    AssetData assetData = await _sqlSugarClient.Queryable<AssetData>()
                        .Where(x => x.SystemName == systemName)
                        .FirstAsync();

                    if (assetData == null)
                        continue;

                    // 获取该系统的房间ID和设备
                    Guid roomId = GetRoomIdBySystemName(systemName);
                    if (roomId == Guid.Empty)
                        continue;

                    List<EquipLedger> systemEquips = await _sqlSugarClient.Queryable<EquipLedger>()
                        .Where(x => x.RoomId == roomId && !x.SoftDeleted)
                        .ToListAsync();

                    // 从传入的试验任务中筛选该系统且该型号的任务
                    string projectName = typeGroup.Key;
                    List<TestDataReadDto> typeSystemTasks = allTasks
                        .Where(t => SystemNameEquals(t.SysName, systemName) && t.ProjectName == projectName)
                        .ToList();

                    if (!typeSystemTasks.Any())
                        continue;

                    int workingDays = 0;
                    decimal totalWorkingHours = 0;
                    HashSet<DateTime> workingDates = new HashSet<DateTime>();
                    int staffCount = 0;

                    // 遍历该型号在该系统的所有试验任务，计算该月的工作天数和工作时长
                    foreach (var task in typeSystemTasks)
                    {
                        if (!DateTime.TryParse(task.TaskStartTime, out DateTime taskStart) ||
                            !DateTime.TryParse(task.TaskEndTime, out DateTime taskEnd))
                        {
                            continue;
                        }

                        // 只统计过去或当前正在进行的计划周期
                        if (taskStart.Date > DateTime.Now.Date) continue;

                        // 计算任务与该月的交集
                        DateTime start = taskStart.Date > monthStart ? taskStart.Date : monthStart;
                        DateTime end = taskEnd.Date < monthEnd ? taskEnd.Date : monthEnd;

                        // 如果任务与该月没有交集，跳过
                        if (start > end) continue;

                        // 计算人员岗位数量
                        int taskStaffCount = 0;
                        if (!string.IsNullOrEmpty(task.SimuResp)) taskStaffCount += 1; // 专业代表
                        if (!string.IsNullOrEmpty(task.SimuStaff))
                        {
                            var staffNames = task.SimuStaff.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            taskStaffCount += staffNames.Length; // 参与人员
                        }
                        staffCount += taskStaffCount;

                        // 查询该任务在该月期间的真实运行数据
                        uint taskSeconds = 0;
                        if (systemEquips.Any())
                        {
                            // 先获取所有数据，然后按设备和日期分组取最大值（因为RunningSeconds是累积值）
                            var runtimeData = await _sqlSugarClient.Queryable<EquipDailyRuntime>()
                                .Where(x => systemEquips.Select(e => e.Id).Contains(x.EquipId))
                                .Where(x => x.RecordDate >= start && x.RecordDate <= end)
                                .Select(x => new { x.EquipId, x.RecordDate, x.RunningSeconds })
                                .ToListAsync();

                            // 按设备和日期分组，取每天的最大秒数
                            var groupedData = runtimeData
                                .GroupBy(x => new { x.EquipId, x.RecordDate.Date })
                                .Select(g => new { g.Key.Date, MaxSeconds = g.Max(x => x.RunningSeconds) })
                                .ToList();

                            taskSeconds = (uint)groupedData.Sum(x => x.MaxSeconds);

                            if (taskSeconds > 0)
                            {
                                // 有真实运行数据，使用已分组的数据收集有运行的日期
                                var actualWorkingDates = groupedData
                                    .Where(g => g.MaxSeconds > 0)
                                    .Select(g => g.Date)
                                    .Distinct()
                                    .ToList();

                                foreach (var date in actualWorkingDates)
                                {
                                    workingDates.Add(date);
                                }
                                totalWorkingHours += Math.Round((decimal)taskSeconds / 3600, 2);
                            }
                        }

                        if (taskSeconds == 0)
                        {
                            // 无真实运行数据，按任务期间每天8小时计算
                            for (DateTime date = start; date <= end; date = date.AddDays(1))
                            {
                                workingDates.Add(date);
                            }
                            int days = (int)(end - start).TotalDays + 1;
                            totalWorkingHours += days * 8;
                        }
                    }

                    workingDays = workingDates.Count;
                    int actualDaysInMonth = (int)(actualMonthEnd - monthStart).TotalDays + 1;
                    int idleDays = actualDaysInMonth - workingDays;

                    // 计算当前年已经过去的天数（用于年度成本分摊）
                    DateTime yearStart = new DateTime(year, 1, 1);
                    DateTime yearEnd = year == DateTime.Now.Year ? DateTime.Now.Date : new DateTime(year, 12, 31);
                    int daysPassedInYear = (int)(yearEnd - yearStart).TotalDays + 1;

                    // 计算各项固定成本的日费率
                    decimal dailyFactoryFee = (assetData.FactoryUsageFee ?? 0) / daysPassedInYear;
                    decimal dailyEquipmentFee = (assetData.EquipmentUsageFee ?? 0) / daysPassedInYear;
                    decimal dailyMaintenanceFee = (assetData.EquipmentMaintenanceCost ?? 0) / daysPassedInYear;

                    // 计算该自然月的总天数（用于成本分摊）
                    int naturalMonthDays = DateTime.DaysInMonth(year, month);
                    // 如果是当前月份，只计算到今天
                    int daysForCostAllocation = (month == DateTime.Now.Month && year == DateTime.Now.Year) 
                        ? actualDaysInMonth 
                        : naturalMonthDays;

                    // 累计各项固定成本（按自然月天数或实际天数分摊）
                    totalFactoryUsageFee += Math.Round(dailyFactoryFee * daysForCostAllocation, 2);
                    totalEquipmentUsageFee += Math.Round(dailyEquipmentFee * daysForCostAllocation, 2);
                    totalEquipmentMaintenanceCost += Math.Round(dailyMaintenanceFee * daysForCostAllocation, 2);

                    // 人力成本：根据该月试验任务的人员数量和工作时长计算
                    if (assetData.LaborCostPerHour.HasValue && assetData.LaborCostPerHour.Value > 0 && staffCount > 0 && totalWorkingHours > 0)
                    {
                        totalLaborCost += Math.Round(staffCount * totalWorkingHours * assetData.LaborCostPerHour.Value, 2);
                    }

                    // 电费：调整后工作时长 × 系统能耗 × 电费单价
                    if (assetData.SystemEnergyConsumption.HasValue && totalWorkingHours > 0)
                    {
                        totalElectricityCost += Math.Round(totalWorkingHours * assetData.SystemEnergyConsumption.Value * ELECTRICITY_UNIT_PRICE, 2);
                    }

                    // 燃料动力费：燃料动力费单价 × 调整后工作时长
                    if (assetData.FuelPowerCostPerHour.HasValue && assetData.FuelPowerCostPerHour.Value > 0 && totalWorkingHours > 0)
                    {
                        totalFuelPowerCost += Math.Round(assetData.FuelPowerCostPerHour.Value * totalWorkingHours, 2);
                    }

                    // 系统空置成本（空置天数的成本）
                    // 使用与固定成本相同的日费率
                    if (idleDays > 0)
                    {
                        totalSystemIdleCost += Math.Round(idleDays * (dailyFactoryFee + dailyEquipmentFee + dailyMaintenanceFee), 2);
                    }
                }

                costBreakdown.FactoryUsageFee = Math.Round(totalFactoryUsageFee, 2);
                costBreakdown.EquipmentUsageFee = Math.Round(totalEquipmentUsageFee, 2);
                costBreakdown.LaborCost = Math.Round(totalLaborCost, 2);
                costBreakdown.ElectricityCost = Math.Round(totalElectricityCost, 2);
                costBreakdown.FuelPowerCost = Math.Round(totalFuelPowerCost, 2);
                costBreakdown.EquipmentMaintenanceCost = Math.Round(totalEquipmentMaintenanceCost, 2);
                costBreakdown.SystemIdleCost = Math.Round(totalSystemIdleCost, 2);

                return costBreakdown;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "计算型号月度成本失败: {ProjectName}, {Year}-{Month}", typeGroup.Key, year, month);
                return new CostBreakdown { NaturalMonth = (NaturalMonth)month };
            }
        }

        /// <summary>
        /// 规范化系统名称（去除换行符、空格等）
        /// </summary>
        /// <param name="systemName">系统名称</param>
        /// <returns>规范化后的系统名称</returns>
        private string NormalizeSystemName(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                return string.Empty;
            
            return systemName
                .Replace("\n", "")
                .Replace("\r", "")
                .Replace("\t", "")
                .Trim();
        }

        /// <summary>
        /// 比较两个系统名称是否相等（忽略换行符、空格等）
        /// </summary>
        /// <param name="name1">系统名称1</param>
        /// <param name="name2">系统名称2</param>
        /// <returns>是否相等</returns>
        private bool SystemNameEquals(string name1, string name2)
        {
            return NormalizeSystemName(name1) == NormalizeSystemName(name2);
        }

        /// <summary>
        /// 根据系统名称获取对应的房间ID
        /// </summary>
        /// <param name="systemName">系统名称</param>
        /// <returns>房间ID</returns>
        private Guid GetRoomIdBySystemName(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                return Guid.Empty;

            // 规范化输入的系统名称
            string normalizedSystemName = NormalizeSystemName(systemName);

            // 根据TestEquipData中的系统名称映射获取房间ID
            for (int systemId = 1; systemId <= 10; systemId++)
            {
                string mappedSystemName = TestEquipData.GetSystemName(systemId);
                // 规范化映射的系统名称
                string normalizedMappedName = NormalizeSystemName(mappedSystemName);
                if (string.Equals(normalizedMappedName, normalizedSystemName, StringComparison.OrdinalIgnoreCase))
                {
                    string roomIdString = TestEquipData.GetRoomId(systemId);
                    if (Guid.TryParse(roomIdString, out Guid roomId))
                    {
                        return roomId;
                    }
                }
            }

            return Guid.Empty; // 未找到对应的房间ID
        }

        /// <summary>
        /// 批量加载成本计算所需的数据缓存
        /// </summary>
        private async Task<CostCalculationCache> LoadCostCalculationCacheAsync(List<TestDataReadDto> allTasks)
        {
            var cache = new CostCalculationCache();
            cache.AllTasks = allTasks;

            // 1. 批量加载所有资产数据
            cache.AllAssetData = await _sqlSugarClient.Queryable<AssetData>()
                .Includes(x => x.Projects)
                .ToListAsync();

            // 2. 批量加载所有房间的设备映射
            var allRoomIds = new List<Guid>();
            for (int systemId = 1; systemId <= 10; systemId++)
            {
                var roomIdStr = TestEquipData.GetRoomId(systemId);
                if (Guid.TryParse(roomIdStr, out Guid roomId))
                {
                    allRoomIds.Add(roomId);
                }
            }

            if (allRoomIds.Any())
            {
                var allEquips = await _sqlSugarClient.Queryable<EquipLedger>()
                    .Where(x => allRoomIds.Contains(x.RoomId.Value) && !x.SoftDeleted)
                    .Select(x => new { x.Id, RoomId = x.RoomId.Value })
                    .ToListAsync();

                foreach (var roomId in allRoomIds.Distinct())
                {
                    cache.RoomEquipmentMap[roomId] = allEquips
                        .Where(e => e.RoomId == roomId)
                        .Select(e => e.Id)
                        .ToList();
                }

                // 3. 批量加载所有设备的运行时间数据（只加载当年的数据）
                var allEquipIds = allEquips.Select(e => e.Id).ToList();
                if (allEquipIds.Any())
                {
                    int currentYear = DateTime.Now.Year;
                    var runtimeData = await _sqlSugarClient.Queryable<EquipDailyRuntime>()
                        .Where(x => allEquipIds.Contains(x.EquipId))
                        .Where(x => x.RecordDate.Year == currentYear)
                        .Select(x => new
                        {
                            x.EquipId,
                            x.RecordDate,
                            x.RunningSeconds
                        })
                        .ToListAsync();

                    // 按设备和日期分组，取每天的最大秒数（因为RunningSeconds是累积值）
                    var groupedData = runtimeData
                        .GroupBy(x => new { x.EquipId, x.RecordDate.Date })
                        .Select(g => new
                        {
                            g.Key.EquipId,
                            g.Key.Date,
                            MaxSeconds = g.Max(x => x.RunningSeconds)
                        })
                        .ToList();

                    foreach (var data in groupedData)
                    {
                        string key = $"{data.EquipId}_{data.Date:yyyy-MM-dd}";
                        if (!cache.EquipmentRuntimeCache.ContainsKey(key))
                        {
                            cache.EquipmentRuntimeCache[key] = new Dictionary<DateTime, uint>();
                        }
                        cache.EquipmentRuntimeCache[key][data.Date] = data.MaxSeconds;
                    }
                }
            }

            return cache;
        }

        /// <summary>
        /// 从缓存中获取设备的运行秒数
        /// </summary>
        private uint GetEquipmentRuntimeFromCache(List<Guid> equipIds, DateTime startDate, DateTime endDate, CostCalculationCache cache)
        {
            uint totalSeconds = 0;
            foreach (var equipId in equipIds)
            {
                for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    string key = $"{equipId}_{date:yyyy-MM-dd}";
                    if (cache.EquipmentRuntimeCache.TryGetValue(key, out var dateMap) &&
                        dateMap.TryGetValue(date, out uint seconds))
                    {
                        totalSeconds += seconds;
                    }
                }
            }
            return totalSeconds;
        }
    }
}

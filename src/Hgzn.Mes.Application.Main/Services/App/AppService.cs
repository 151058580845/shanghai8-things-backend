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
            IEnumerable<TestDataReadDto> current = await _testDataService.GetCurrentListByTestAsync();
            TestDataReadDto currentTestInSystem = current.FirstOrDefault(x => showSystemDetailQueryDto.systemName != null &&
                                                                         x.SysName.Trim() == showSystemDetailQueryDto.systemName.Trim())!;
            IEnumerable<TestDataReadDto> feature = await _testDataService.GetFeatureListByTestAsync();


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
            if (currentTestInSystem != null)
            {
                currentTestName = currentTestInSystem.TaskName;
                if (DateTime.TryParse(currentTestInSystem.TaskStartTime, out DateTime startTime))
                    currentTestActivatedDay = (DateTime.Now.ToLocalTime() - startTime).Days;
                currentTestDevPhase = currentTestInSystem.DevPhase;
                currentTestTaskEndTime = currentTestInSystem.TaskEndTime;
            }
            // 在本系统中的后续试验计划
            TestDataReadDto featureTestInSystem = feature.FirstOrDefault(x => showSystemDetailQueryDto.systemName != null &&
                                                                         x.SysName.Trim() == showSystemDetailQueryDto.systemName.Trim())!;
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
                    new List<string>(){ "当前研制阶段", currentTestDevPhase },
                    new List<string>(){ "当前试验计划结束日期", currentTestTaskEndTime }
                }
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

            #region 摄像头数据 (已关联数据库)

            read.CameraData = await _sysMgr.GetCameraData(showSystemDetailQueryDto.systemName);

            #endregion

            return read;
        }

        public async Task<ShowSystemHomeDataDto> GetTestListAsync()
        {
            await _sysMgr.SnapshootHomeData();
            _abnormalEquipDic = new Dictionary<string, List<string>>();
            IEnumerable<TestDataReadDto> current = await _testDataService.GetCurrentListByTestAsync();
            IEnumerable<TestDataReadDto> feature = await _testDataService.GetFeatureListByTestAsync();
            IEnumerable<TestDataReadDto> history = await _testDataService.GetHistoryListByTestAsync();

            ShowSystemHomeDataDto testRead = new ShowSystemHomeDataDto();
            List<EquipConnect> connectEquips = _sqlSugarClient.Queryable<EquipConnect>()
                .Includes(el => el.EquipLedger)
                .Includes(el => el.EquipLedger!.Room).ToList();
            // 试验系统设备数据

            // *** 环境数据
            testRead.EnvironmentData = new EnvironmentData() { Humidity = 101, Temperature = 101 };

            #region 试验系统列表数据 (已关联数据库)

            // *** 试验系统列表数据
            List<SystemDeviceData> list = new List<SystemDeviceData>();
            foreach (SystemInfo sysInfo in _systemInfoList)
            {
                // 获取系统温湿度,这个数据本来是打算从后台内存中读取的,现在修改为根据MQTT推送的消息,在前端获取,所以现在随便传个数字过去就行,在前端改
                int iTemperature = 0;
                int iHumidity = 0;
                if (RKData.RoomId_TemperatureAndHumidness.ContainsKey(sysInfo.RoomId))
                {
                    iTemperature = (int)RKData.RoomId_TemperatureAndHumidness[sysInfo.RoomId].Item1;
                    iHumidity = (int)RKData.RoomId_TemperatureAndHumidness[sysInfo.RoomId].Item2;
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
                string ttypeName = current.FirstOrDefault(x => x.SysName.Trim() == sysInfo.Name.Trim())?.ProjectName!;
                if (!string.IsNullOrEmpty(ttypeName))
                    typeName = ttypeName;

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
                });
            }
            testRead.SystemDeviceList = list;

            #endregion

            #region 设备状态统计详细数据 (已关联数据库)

            // *** 设备状态统计详细数据
            // 获取设备信息
            List<EquipmentData> equipDatas = new List<EquipmentData>();
            for (int i = 0; i < connectEquips.Count; i++)
            {
                equipDatas.Add(new EquipmentData()
                {
                    Index = i + 1,
                    Code = connectEquips[i].EquipLedger!.EquipCode,
                    Name = connectEquips[i].EquipLedger!.EquipName,
                    Location = connectEquips[i].EquipLedger?.Room?.Name ?? "",
                    State = await _sysMgr.GetEquipOnline(connectEquips, connectEquips[i].EquipLedger!.Id) ? "在线" : "离线",
                    Health = _abnormalEquipDic.ContainsKey(connectEquips[i].EquipLedger!.Id.ToString()) ? "异常" : "健康"
                });
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

            #endregion

            #region 试验系统利用率数据 (已关联数据库)

            // 试验系统利用率数据
            // 系统利用率算法:将这个系统转台按月工作时间累加(秒) / 22*8*60*60
            testRead.DeviceListData = new List<DeviceUtilizationData>();
            for (int i = 0; i < _systemInfoList.Count; i++)
            {
                // 计算利用率
                uint runTime = await _sysMgr.GetRunTime(_systemInfoList[i].SystemNum, 3, _systemInfoList[i].TurntableEquipId);
                int utilization = (int)Math.Round((double)((double)runTime * 100 / NumberOfSecondsPerMonth), 0);
                int idle = 100 - utilization;
                // 获取连接状态
                string status = await _sysMgr.GetEquipOnline(connectEquips, _systemInfoList[i].TurntableEquipId) ? "在线" : "离线";
                testRead.DeviceListData.Add(new DeviceUtilizationData()
                {
                    Name = _systemInfoList[i].Name,
                    Idle = idle,
                    Status = status,
                    Utilization = utilization,
                });
            }

            #endregion

            #region 实验任务数据 (已关联数据库)

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

            // *** 在线设备状态统计（在线率）
            // 获取在线设备
            int onlineCount = connectEquips.Where(x => x.State).Count();
            int workingRateData = (int)((double)onlineCount / connectEquips.Count() * 100);
            int offlineRateData = 100 - workingRateData;
            testRead.OnlineRateData = new OnlineRateData() { WorkingRateData = workingRateData, FreeRateData = 0, OfflineRateData = offlineRateData };
            // *** 在线设备状态统计（故障率）
            int abnormalSysCount = testRead.AbnormalDeviceList.Where(x => x.AbnormalCount > 0).Count();
            int sysCount = testRead.AbnormalDeviceList.Count();
            int breakdownData = (int)((double)abnormalSysCount / (double)sysCount * 100);
            int healthData = 100 - breakdownData;
            testRead.FailureRateData = new FailureRateData() { BreakdownData = breakdownData, HealthData = healthData, PreferablyData = 0 };

            #endregion

            #region 关键设备利用率 (已关联数据库)

            // 关键设备利用率数据
            testRead.KeyDeviceList = new List<KeyDeviceData>();
            foreach (SystemInfo item in _systemInfoList)
            {
                foreach (SDevice kd in item.keyDevices)
                {
                    uint runTime = await _sysMgr.GetRunTime(item.SystemNum, kd.EquipId);
                    int utilization = (int)Math.Round((double)((double)runTime * 100 / NumberOfSecondsPerMonth), 0);
                    int idle = 100 - utilization;

                    testRead.KeyDeviceList.Add(new KeyDeviceData()
                    {
                        Name = kd.EquipName,
                        Utilization = utilization,
                        Idle = idle,
                        Breakdown = 0,
                    });
                }
            }

            #endregion

            #region 按系统统计试验时间 (SystemTestTimes)

            testRead.SystemTestTimes = await CalculateSystemTestTimes();

            #endregion

            #region 按型号统计试验时间 (TypeTestTimes)

            testRead.TypeTestTimes = await CalculateTypeTestTimes();

            #endregion

            #region 按系统统计试验成本 (SystemTestCost)

            testRead.SystemTestCost = await CalculateSystemTestCost();

            #endregion

            #region 按型号统计试验成本 (TypeTestCost)

            testRead.TypeTestCost = await CalculateTypeTestCost();

            #endregion

            #region 摄像头数据 (已关联数据库)

            testRead.CameraData = await _sysMgr.GetCameraData("home");

            #endregion

            return testRead;
        }

        /// <summary>
        /// 计算按系统统计的试验时间
        /// </summary>
        /// <returns></returns>
        private async Task<SystemTestTimes> CalculateSystemTestTimes()
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

                // 获取所有系统的工作日期数据
                HashSet<DateTime> allWorkingDates = new HashSet<DateTime>();
                HashSet<DateTime> currentMonthWorkingDates = new HashSet<DateTime>();
                Dictionary<NaturalMonth, HashSet<DateTime>> monthlyAllSystemWorkingDates = new Dictionary<NaturalMonth, HashSet<DateTime>>();

                // 初始化月度统计
                for (int month = 1; month <= 12; month++)
                {
                    monthlyAllSystemWorkingDates[(NaturalMonth)month] = new HashSet<DateTime>();
                }

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

                    if (!systemEquips.Any())
                        continue;

                    // 获取该系统的工作日期（有任意设备运行的日期）
                    List<DateTime> workingDates = await _sqlSugarClient.Queryable<EquipDailyRuntime>()
                        .Where(x => systemEquips.Select(e => e.Id).Contains(x.EquipId))
                        .Where(x => x.RecordDate.Year == currentYear && x.RunningSeconds > 0)
                        .Select(x => x.RecordDate.Date)
                        .Distinct()
                        .ToListAsync();

                    if (!workingDates.Any())
                    {
                        // 即使没有工作日期，也要初始化该系统的月度统计
                        systemTestTimes.SystemMonthlyWorkDays[systemName] = new Dictionary<NaturalMonth, int>();
                        for (int month = 1; month <= 12; month++)
                        {
                            systemTestTimes.SystemMonthlyWorkDays[systemName][(NaturalMonth)month] = 0;
                        }
                        continue;
                    }

                    // 生成连续时间段
                    List<DateTimeRange> timeRanges = GenerateContinuousTimeRanges(workingDates.OrderBy(d => d).ToList());
                    systemTestTimes.Times[systemName] = timeRanges;

                    // 初始化该系统的月度工作天数统计
                    systemTestTimes.SystemMonthlyWorkDays[systemName] = new Dictionary<NaturalMonth, int>();
                    for (int month = 1; month <= 12; month++)
                    {
                        systemTestTimes.SystemMonthlyWorkDays[systemName][(NaturalMonth)month] = 0;
                    }

                    // 按月统计该系统的工作天数
                    foreach (DateTime date in workingDates)
                    {
                        NaturalMonth month = (NaturalMonth)date.Month;
                        systemTestTimes.SystemMonthlyWorkDays[systemName][month]++;
                        
                        // 累计所有系统的工作日期（去重）
                        allWorkingDates.Add(date);
                        monthlyAllSystemWorkingDates[month].Add(date);
                        
                        if (date.Month == currentMonth)
                        {
                            currentMonthWorkingDates.Add(date);
                        }
                    }
                }

                // 计算每月所有系统的总工作天数（去重后的）
                foreach (var kvp in monthlyAllSystemWorkingDates)
                {
                    systemTestTimes.TimePerMonth[kvp.Key] = kvp.Value.Count;
                }

                systemTestTimes.CurrentMonthTotalSystemTestDays = currentMonthWorkingDates.Count;
                systemTestTimes.CurrentYearTotalSystemTestDays = allWorkingDates.Count;

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

                // 获取当前年度的所有型号试验计划
                List<TestData> currentYearTestData = await _sqlSugarClient.Queryable<TestData>()
                    .Where(x => !string.IsNullOrEmpty(x.TaskStartTime) && !string.IsNullOrEmpty(x.TaskEndTime))
                    .ToListAsync();

                // 筛选出当前年度的试验计划
                List<TestData> validTestData = currentYearTestData
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

                HashSet<DateTime> allWorkingDates = new HashSet<DateTime>();
                HashSet<DateTime> currentMonthWorkingDates = new HashSet<DateTime>();
                Dictionary<NaturalMonth, HashSet<DateTime>> monthlyAllTypeWorkingDates = new Dictionary<NaturalMonth, HashSet<DateTime>>();

                // 初始化月度统计
                for (int month = 1; month <= 12; month++)
                {
                    monthlyAllTypeWorkingDates[(NaturalMonth)month] = new HashSet<DateTime>();
                }

                // 按型号分组统计
                IEnumerable<IGrouping<string, TestData>> typeGroups = validTestData.GroupBy(x => x.ProjectName ?? "未知型号");

                foreach (IGrouping<string, TestData> typeGroup in typeGroups)
                {
                    string projectName = typeGroup.Key;
                    HashSet<DateTime> typeWorkingDates = new HashSet<DateTime>();

                    // 初始化该型号的月度工作天数统计
                    typeTestTimes.TypeMonthlyWorkDays[projectName] = new Dictionary<NaturalMonth, int>();
                    for (int month = 1; month <= 12; month++)
                    {
                        typeTestTimes.TypeMonthlyWorkDays[projectName][(NaturalMonth)month] = 0;
                    }

                    // 获取该型号在所有系统中的工作日期
                    foreach (TestData? testData in typeGroup)
                    {
                        // 根据系统名称找到对应的房间ID
                        Guid roomId = GetRoomIdBySystemName(testData.SysName);
                        if (roomId == Guid.Empty)
                            continue;

                        // 获取该系统下的所有设备
                        List<EquipLedger> systemEquips = await _sqlSugarClient.Queryable<EquipLedger>()
                            .Where(x => x.RoomId == roomId && !x.SoftDeleted)
                            .ToListAsync();

                        if (!systemEquips.Any())
                            continue;

                        // 获取该系统在当前年度的工作日期
                        List<DateTime> workingDates = await _sqlSugarClient.Queryable<EquipDailyRuntime>()
                            .Where(x => systemEquips.Select(e => e.Id).Contains(x.EquipId))
                            .Where(x => x.RecordDate.Year == currentYear && x.RunningSeconds > 0)
                            .Select(x => x.RecordDate.Date)
                            .Distinct()
                            .ToListAsync();

                        foreach (DateTime date in workingDates)
                        {
                            typeWorkingDates.Add(date);
                        }
                    }

                    if (typeWorkingDates.Any())
                    {
                        // 生成连续时间段
                        List<DateTimeRange> timeRanges = GenerateContinuousTimeRanges(typeWorkingDates.OrderBy(d => d).ToList());
                        typeTestTimes.Times[projectName] = timeRanges;

                        // 按月统计该型号的工作天数
                        foreach (DateTime date in typeWorkingDates)
                        {
                            NaturalMonth month = (NaturalMonth)date.Month;
                            typeTestTimes.TypeMonthlyWorkDays[projectName][month]++;
                            
                            // 累计所有型号的工作日期（去重）
                            allWorkingDates.Add(date);
                            monthlyAllTypeWorkingDates[month].Add(date);
                            
                            if (date.Month == currentMonth)
                            {
                                currentMonthWorkingDates.Add(date);
                            }
                        }
                    }
                }

                // 计算每月所有型号的总工作天数（去重后的）
                foreach (var kvp in monthlyAllTypeWorkingDates)
                {
                    typeTestTimes.TimePerMonth[kvp.Key] = kvp.Value.Count;
                }

                typeTestTimes.CurrentMonthTotalTypeTestDays = currentMonthWorkingDates.Count;
                typeTestTimes.CurrentYearTotalTypeTestDays = allWorkingDates.Count;

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
        /// <returns></returns>
        private async Task<SystemTestCost> CalculateSystemTestCost()
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

                // 获取所有系统的资产数据
                List<AssetData> allAssetData = await _sqlSugarClient.Queryable<AssetData>()
                    .Includes(x => x.Projects)
                    .ToListAsync();

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
                        CostBreakdown monthCost = await CalculateMonthlySystemCost(assetData, currentYear, month);
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
                systemTestCost.CurrentMonthTotalSystemTestCost = (int)Math.Round(currentMonthTotalCost);
                systemTestCost.CurrentYearTotalSystemTestCost = (int)Math.Round(currentYearTotalCost);

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
        /// <returns></returns>
        private async Task<TypeTestCost> CalculateTypeTestCost()
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

                // 获取当前年度的所有型号试验计划
                List<TestData> currentYearTestData = await _sqlSugarClient.Queryable<TestData>()
                    .Where(x => !string.IsNullOrEmpty(x.TaskStartTime) && !string.IsNullOrEmpty(x.TaskEndTime))
                    .ToListAsync();

                // 筛选出当前年度的试验计划
                List<TestData> validTestData = currentYearTestData
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
                        CostBreakdown monthCost = await CalculateMonthlyTypeCost(typeGroup, currentYear, month);
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
                typeTestCost.CurrentMonthTotalTypeTestCost = (int)Math.Round(currentMonthTotalCost);
                typeTestCost.CurrentYearTotalTypeTestCost = (int)Math.Round(currentYearTotalCost);

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
        /// 计算系统某个月的成本分解
        /// </summary>
        /// <param name="assetData">资产数据</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>月度成本分解</returns>
        private async Task<CostBreakdown> CalculateMonthlySystemCost(AssetData assetData, int year, int month)
        {
            try
            {
                CostBreakdown costBreakdown = new CostBreakdown
                {
                    NaturalMonth = (NaturalMonth)month
                };

                // 获取该系统在指定月份的工作天数和工作时长
                Guid roomId = GetRoomIdBySystemName(assetData.SystemName);
                if (roomId == Guid.Empty)
                    return costBreakdown;

                List<EquipLedger> systemEquips = await _sqlSugarClient.Queryable<EquipLedger>()
                    .Where(x => x.RoomId == roomId && !x.SoftDeleted)
                    .ToListAsync();

                if (!systemEquips.Any())
                    return costBreakdown;

                // 获取指定月份的工作数据
                DateTime monthStart = new DateTime(year, month, 1);
                DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);

                List<EquipDailyRuntime> monthlyRuntimeData = await _sqlSugarClient.Queryable<EquipDailyRuntime>()
                    .Where(x => systemEquips.Select(e => e.Id).Contains(x.EquipId))
                    .Where(x => x.RecordDate >= monthStart && x.RecordDate <= monthEnd)
                    .Where(x => x.RunningSeconds > 0)
                    .ToListAsync();

                int workingDays = monthlyRuntimeData.Select(x => x.RecordDate.Date).Distinct().Count();
                decimal totalWorkingHours = Math.Round((decimal)monthlyRuntimeData.Sum(x => x.RunningSeconds) / 3600, 2);
                int daysInMonth = DateTime.DaysInMonth(year, month);
                int idleDays = daysInMonth - workingDays;

                // 计算各项成本（平摊到月）
                costBreakdown.FactoryUsageFee = Math.Round((assetData.FactoryUsageFee ?? 0) / 12, 2);
                costBreakdown.EquipmentUsageFee = Math.Round((assetData.EquipmentUsageFee ?? 0) / 12, 2);
                costBreakdown.LaborCost = Math.Round((assetData.LaborCost ?? 0) / 12, 2);
                costBreakdown.EquipmentMaintenanceCost = Math.Round((assetData.EquipmentMaintenanceCost ?? 0) / 12, 2);

                // 电费基于实际工作时长
                if (assetData.SystemEnergyConsumption.HasValue && totalWorkingHours > 0)
                {
                    costBreakdown.ElectricityCost = Math.Round(totalWorkingHours * assetData.SystemEnergyConsumption.Value * 0.8m, 2);
                }

                // 燃料动力费基于实际工作时长（平摊到月）
                if (assetData.FuelPowerCost.HasValue && assetData.FuelPowerCost.Value > 0)
                {
                    // 计算该月的工作时长占比，然后按比例分配年度燃料动力费
                    decimal monthlyWorkingHoursRatio = totalWorkingHours > 0 ? totalWorkingHours / (365 * 24) : 0; // 简化计算，实际应该基于全年总工作时长
                    costBreakdown.FuelPowerCost = Math.Round(assetData.FuelPowerCost.Value / 12, 2); // 暂时按月份平摊
                }
                else
                {
                    costBreakdown.FuelPowerCost = 0;
                }

                // 系统空置成本（空置天数的成本）
                if (idleDays > 0)
                {
                    decimal dailyFactoryFee = (assetData.FactoryUsageFee ?? 0) / 365;
                    decimal dailyEquipmentFee = (assetData.EquipmentUsageFee ?? 0) / 365;
                    decimal dailyMaintenanceFee = (assetData.EquipmentMaintenanceCost ?? 0) / 365;
                    
                    costBreakdown.SystemIdleCost = Math.Round(idleDays * (dailyFactoryFee + dailyEquipmentFee + dailyMaintenanceFee), 2);
                }

                return costBreakdown;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "计算系统月度成本失败: {SystemName}, {Year}-{Month}", assetData.SystemName, year, month);
                return new CostBreakdown { NaturalMonth = (NaturalMonth)month };
            }
        }

        /// <summary>
        /// 计算型号某个月的成本分解
        /// </summary>
        /// <param name="typeGroup">型号试验数据组</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>月度成本分解</returns>
        private async Task<CostBreakdown> CalculateMonthlyTypeCost(IGrouping<string, TestData> typeGroup, int year, int month)
        {
            try
            {
                CostBreakdown costBreakdown = new CostBreakdown
                {
                    NaturalMonth = (NaturalMonth)month
                };

                decimal totalFactoryUsageFee = 0;
                decimal totalEquipmentUsageFee = 0;
                decimal totalLaborCost = 0;
                decimal totalElectricityCost = 0;
                decimal totalFuelPowerCost = 0;
                decimal totalEquipmentMaintenanceCost = 0;
                decimal totalSystemIdleCost = 0;

                // 遍历该型号在各个系统的试验
                foreach (TestData testData in typeGroup)
                {
                    // 获取系统的资产数据
                    AssetData assetData = await _sqlSugarClient.Queryable<AssetData>()
                        .Where(x => x.SystemName == testData.SysName)
                        .FirstAsync();

                    if (assetData == null)
                        continue;

                    // 计算该系统在指定月份的成本
                    CostBreakdown systemMonthlyCost = await CalculateMonthlySystemCost(assetData, year, month);

                    // 累计各项成本
                    totalFactoryUsageFee += systemMonthlyCost.FactoryUsageFee ?? 0;
                    totalEquipmentUsageFee += systemMonthlyCost.EquipmentUsageFee ?? 0;
                    totalLaborCost += systemMonthlyCost.LaborCost ?? 0;
                    totalElectricityCost += systemMonthlyCost.ElectricityCost ?? 0;
                    totalFuelPowerCost += systemMonthlyCost.FuelPowerCost ?? 0;
                    totalEquipmentMaintenanceCost += systemMonthlyCost.EquipmentMaintenanceCost ?? 0;
                    totalSystemIdleCost += systemMonthlyCost.SystemIdleCost ?? 0;
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
        /// 根据系统名称获取对应的房间ID
        /// </summary>
        /// <param name="systemName">系统名称</param>
        /// <returns>房间ID</returns>
        private Guid GetRoomIdBySystemName(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                return Guid.Empty;

            // 根据TestEquipData中的系统名称映射获取房间ID
            for (int systemId = 1; systemId <= 10; systemId++)
            {
                string mappedSystemName = TestEquipData.GetSystemName(systemId);
                if (string.Equals(mappedSystemName, systemName, StringComparison.OrdinalIgnoreCase))
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
    }
}

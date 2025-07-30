using Hgzn.Mes.Application.Main.Dtos.App;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.App.IService;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
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
        protected readonly ISqlSugarClient _sqlSugarClient;
        private readonly ITestDataService _testDataService;
        // key是设备ID,value是设备异常信息
        private Dictionary<string, List<string>> _abnormalEquipDic = new Dictionary<string, List<string>>();
        private readonly RedisHelper _redisHelper;
        private const string EquipHealthStatusRedisKey = "equipHealthStatus";
        private SystemInfoManager _sysMgr; // 系统信息管理器
        private List<SystemInfo> _systemInfoList;
        private const double NumberOfSecondsPerMonth = 22 * 8 * 60 * 60;
        private IEquipLedgerService _equipLedgerService;
        private int EquipCount = 50;

        public AppService(ISqlSugarClient client,
        IConnectionMultiplexer connectionMultiplexer,
        ITestDataService testDataService,
        RedisHelper redisHelper,
        IEquipLedgerService equipLedgerService)
        {
            _sqlSugarClient = client;
            _connectionMultiplexer = connectionMultiplexer;
            _testDataService = testDataService;
            _redisHelper = redisHelper;
            _equipLedgerService = equipLedgerService;
            _sysMgr = new SystemInfoManager(connectionMultiplexer, redisHelper, client);
            _systemInfoList = _sysMgr.SystemInfos;
        }

        public async Task<ShowSystemDetailDto> GetTestDetailAsync(ShowSystemDetailQueryDto showSystemDetailQueryDto)
        {
            await Simulation(); // 模拟数据接收
            await _sysMgr.SnapshootHomeData();

            ShowSystemDetailDto read = new ShowSystemDetailDto();
            IEnumerable<TestDataReadDto> current = await _testDataService.GetCurrentListByTestAsync();
            TestDataReadDto currentTestInSystem = current.FirstOrDefault(x => x.SysName == showSystemDetailQueryDto.systemName)!;
            IEnumerable<TestDataReadDto> feature = await _testDataService.GetFeatureListByTestAsync();


            #region 人员展示 (已关联数据库)

            if (currentTestInSystem != null)
            {
                // 申请调度
                List<string> reqManagers = currentTestInSystem.ReqManager.Split(',').ToList();
                // 制导控制专业代表
                List<string> gncResps = currentTestInSystem.GncResp.Split(',').ToList();
                // 试验专业代表
                List<string> simuResps = currentTestInSystem.SimuResp.Split(',').ToList();
                // 试验参与人员
                List<string> simuStaffs = currentTestInSystem.SimuStaff.Split(',').ToList();
                List<ExperimenterDto> experimentersDatas = new List<ExperimenterDto>();
                foreach (string item in reqManagers)
                {
                    experimentersDatas.Add(new ExperimenterDto() { System = "申请调度", Person = item });
                }
                foreach (string item in gncResps)
                {
                    experimentersDatas.Add(new ExperimenterDto() { System = "制导控制专业代表", Person = item });
                }
                foreach (string item in simuResps)
                {
                    experimentersDatas.Add(new ExperimenterDto() { System = "试验专业代表", Person = item });
                }
                foreach (string item in simuStaffs)
                {
                    experimentersDatas.Add(new ExperimenterDto() { System = "试验参与人员", Person = item });
                }
                read.ExperimentersData = experimentersDatas;
            }

            #endregion

            #region 图表展示 (已关联数据库)

            // 获取图标数据
            var chartDataPointDto = await _sysMgr.GetChartDataPointDto(_sysMgr.SystemInfos.FirstOrDefault(x => x.Name == showSystemDetailQueryDto.systemName)!);
            read.ChartData = chartDataPointDto;

            #endregion

            #region 异常设备,包括计量到期设备 (已关联数据库)

            // 异常设备
            List<AbnormalDeviceDto> abnormalDeviceDtos = new List<AbnormalDeviceDto>();
            List<Abnormal> sysAbnormals = _sysMgr.Abnormals.Where(x => x.SystemInfo.Name == showSystemDetailQueryDto.systemName).ToList();
            foreach (Abnormal abnormal in sysAbnormals)
            {
                foreach (string ad in abnormal.AbnormalDescription)
                {
                    abnormalDeviceDtos.Add(new AbnormalDeviceDto
                    {
                        System = abnormal.SystemInfo.Name,
                        Device = await _equipLedgerService.GetEquipName(abnormal.EquipId),
                        Value = ad,
                        Time = "5",
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
            TestDataReadDto featureTestInSystem = feature.FirstOrDefault(x => x.SysName == showSystemDetailQueryDto.systemName)!;
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
            TableDto queueDetail1 = new TableDto();
            TableDto queueDetail2 = new TableDto();
            List<Tuple<TableDto, TableDto>> tables = await _sysMgr.GetTableDtos(_sysMgr.SystemInfos.FirstOrDefault(x => x.Name == showSystemDetailQueryDto.systemName)!);
            if (tables != null && tables.Count > 0 && tables[0] != null && tables[0].Item1 != null && tables[0].Item2 != null)
            {
                queue1.Add(tables[0].Item1);
                queueDetail1 = tables[0].Item2;
            }
            if (tables != null && tables.Count > 1 && tables[1] != null && tables[1].Item1 != null && tables[1].Item2 != null)
            {
                queue2.Add(tables[1].Item1);
                read.Queue2Detail = tables[1].Item2;
            }
            read.Queue = queue1;
            read.QueueDetail = queueDetail1;
            read.Queue2 = queue2;
            read.Queue2Detail = queueDetail2;

            #endregion

            #region 产品列表 (已关联数据库)

            List<TableDto> productReadDto = new List<TableDto>();
            if (currentTestInSystem != null)
            {
                TableDto td = new TableDto()
                {
                    Title = "产品列表",
                    Header = new List<List<string>>()
                {
                    new List<string> { "name", "产品名称" },
                    new List<string> { "code", "产品编号" },
                    new List<string> { "status", "技术状态" },
                },
                    Data = new List<Dictionary<string, string>>()
                };
                foreach (TestDataProductReadDto item in currentTestInSystem.UUT)
                {
                    td.Data.Add(new Dictionary<string, string>()
                {
                    { "name" , item.Name },
                    { "code" , item.Code },
                    { "status" , item.TechnicalStatus },
                });
                }
                productReadDto.Add(td);
            }
            read.ProductList = productReadDto;

            #endregion

            return read;
        }

        public async Task<ShowSystemHomeDataDto> GetTestListAsync()
        {
            await Simulation(); // 模拟数据接收
            await _sysMgr.SnapshootHomeData();
            _abnormalEquipDic = new Dictionary<string, List<string>>();

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
                list.Add(new SystemDeviceData()
                {
                    Name = sysInfo.Name,
                    Quantity = 10,
                    Temperature = iTemperature,
                    Humidity = iHumidity,
                    Status = status,
                    RoomId = sysInfo.RoomId.ToString(),
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
                    Index = i,
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

            #region 实验任务数据 (已关联数据库)

            IEnumerable<TestDataReadDto> current = await _testDataService.GetCurrentListByTestAsync();
            IEnumerable<TestDataReadDto> feature = await _testDataService.GetFeatureListByTestAsync();
            IEnumerable<TestDataReadDto> history = await _testDataService.GetHistoryListByTestAsync();
            // 当前试验任务数据组织
            List<Dictionary<string, object>> currentListDic = new List<Dictionary<string, object>>();
            foreach (TestDataReadDto item in current)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>
                {
                    { "name", item.TaskName },
                    { "start", item.TaskStartTime },
                    { "end", item.TaskEndTime },
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
                    { "start", item.TaskStartTime },
                    { "end", item.TaskEndTime },
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
                    { "start", item.TaskStartTime },
                    { "end", item.TaskEndTime },
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
                    new TableHeader() { Field = "start", Label = "计划开始时间" },
                    new TableHeader() { Field = "end", Label = "计划结束时间" },
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
                    new TableHeader() { Field = "start", Label = "实际开始时间" },
                    new TableHeader() { Field = "end", Label = "实际开始时间" },
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
                    new TableHeader() { Field = "start", Label = "计划开始时间" },
                    new TableHeader() { Field = "end", Label = "计划结束时间" },
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

            // *** 在线设备状态统计（在线率）
            // 获取在线设备
            int onlineCount = connectEquips.Where(x => x.ConnectState).Count();
            int workingRateData = (int)((double)onlineCount / (double)EquipCount * 100);
            int offlineRateData = 100 - workingRateData;
            testRead.OnlineRateData = new OnlineRateData() { WorkingRateData = workingRateData, FreeRateData = 0, OfflineRateData = offlineRateData };
            // *** 在线设备状态统计（故障率）
            int abnormalSysCount = testRead.AbnormalDeviceList.Where(x => x.AbnormalCount > 0).Count();
            int sysCount = testRead.AbnormalDeviceList.Count();
            int breakdownData = (int)((double)abnormalSysCount / (double)sysCount * 100);
            int healthData = 100 - breakdownData;
            testRead.FailureRateData = new FailureRateData() { BreakdownData = breakdownData, HealthData = healthData, PreferablyData = 0 };

            // 关键设备利用率数据
            testRead.KeyDeviceList = new List<KeyDeviceData>();
            foreach (SystemInfo item in _systemInfoList)
            {
                foreach (KeyDevice kd in item.keyDevices)
                {
                    uint runTime = await _sysMgr.GetRunTime(item.SystemNum, kd.EquipTypeNum, kd.EquipId);
                    int utilization = (int)Math.Round((double)((double)runTime * 100 / NumberOfSecondsPerMonth), 0);
                    int idle = 100 - utilization;

                    testRead.KeyDeviceList.Add(new KeyDeviceData()
                    {
                        Name = item.Name,
                        Utilization = utilization,
                        Idle = idle,
                        Breakdown = 0,
                    });
                }

            }

            return testRead;
        }

        public async Task Simulation()
        {
            await ReceiveHelper.ExceptionRecordToRedis(_connectionMultiplexer, 1, 3, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"), new List<string>() { "这个设备这也坏了", "这个设备那也坏了" }, new DateTime(2025, 3, 15, 8, 30, 0), 555);
            await ReceiveHelper.ExceptionRecordToRedis(_connectionMultiplexer, 1, 3, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"), new List<string>() { "这个设备这也坏了", "这个设备那也坏了" }, new DateTime(2025, 6, 15, 8, 30, 0), 12345);
            await ReceiveHelper.ExceptionRecordToRedis(_connectionMultiplexer, 1, 3, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"), new List<string>() { "这个设备这也坏了", "这个设备那也坏了" }, new DateTime(2025, 7, 16, 8, 30, 0), 20000);
            await ReceiveHelper.ExceptionRecordToRedis(_connectionMultiplexer, 1, 3, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"), new List<string>() { "这个设备这也坏了", "这个设备那也坏了" }, new DateTime(2025, 7, 17, 8, 30, 0), 30000);
            await ReceiveHelper.ExceptionRecordToRedis(_connectionMultiplexer, 1, 3, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, Guid.Parse("1e14e36a-ec3a-4358-aca4-8e655a252f54"), new List<string>() { "这个设备这也坏了", "这个设备那也坏了" }, new DateTime(2025, 7, 17, 9, 30, 0), 300000);
        }
    }
}

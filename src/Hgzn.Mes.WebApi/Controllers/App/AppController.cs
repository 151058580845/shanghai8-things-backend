using Hgzn.Mes.Application.Main.Dtos.App;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Shared.Extensions;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace Hgzn.Mes.WebApi.Controllers.App;


/// <summary>
/// 大屏使用
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AppController : ControllerBase
{
    private IEquipLedgerService _equipLedgerService;
    private ITestDataService _testDataService;
    private IDictionaryInfoService _dictionaryInfoService;
    private IRoomService _roomService;

    public AppController(IEquipLedgerService equipLedgerService,
        ITestDataService testDataService,
        IDictionaryInfoService dictionaryInfoService,
        IRoomService roomService)
    {
        _equipLedgerService = equipLedgerService;
        _testDataService = testDataService;
        _dictionaryInfoService = dictionaryInfoService;
        _roomService = roomService;
    }

    /// <summary>
    /// 获取试验系统主页数据
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("test/list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<ShowSystemHomeDataDto>> GetTestListAsync()
    {
        List<Application.Main.Dtos.Base.NameValueDto> testList = await _dictionaryInfoService.GetNameValueByTypeAsync("TestSystem");
        ShowSystemHomeDataDto testRead = new ShowSystemHomeDataDto();
        // 试验系统设备数据

        // 环境数据
        testRead.EnvironmentData = new EnvironmentData() { Humidity = 101, Temperature = 101 };
        // 试验系统列表数据
        List<SystemDeviceData> list = new List<SystemDeviceData>();
        foreach (Application.Main.Dtos.Base.NameValueDto test in testList)
        {
            list.Add(new SystemDeviceData()
            {
                Name = test.Value,
                Quantity = 10,
                Temperature = 99,
                Humidity = 50,
                Status = "掉了一颗螺丝",
            });
        }
        testRead.SystemDeviceList = list;
        // 在线设备状态统计（在线率）
        testRead.OnlineRateData = new OnlineRateData() { WorkingRateData = 50, FreeRateData = 40, OfflineRateData = 10 };
        // 在线设备状态统计（故障率）
        testRead.FailureRateData = new FailureRateData() { BreakdownData = 20, HealthData = 50, PreferablyData = 30 };
        // 设备状态统计详细数据
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
            Data = new List<EquipmentData>()
            {
                new EquipmentData()
                {
                    Index = 111,
                    Code = "这是code",
                    Name = "这是name",
                    Location = "这是Location",
                    State = "这是State",
                    Health = "这是Health"
                },
            }
        };

        // 试验系统利用率数据
        testRead.DeviceListData = new List<DeviceUtilizationData>();
        for (int i = 0; i < testList.Count; i++)
        {
            testRead.DeviceListData.Add(new DeviceUtilizationData()
            {
                Name = testList[i].Name,
                Idle = 20,
                Status = "离线",
                Utilization = 80,
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

        // 异常信息列表
        testRead.AbnormalDeviceList = new List<AbnormalDeviceData>();
        foreach (var item in testList)
        {
            testRead.AbnormalDeviceList.Add(new AbnormalDeviceData()
            {
                System = item.Name,
                Device = "这是一个设备名",
                Value = "这个设备正在发疯",
                Time = DateTime.Now.ToLocalTime().ToString(),
            });
        }
        // 关键设备利用率数据
        testRead.KeyDeviceList = new List<KeyDeviceData>();
        foreach (var item in testList)
        {
            testRead.KeyDeviceList.Add(new KeyDeviceData()
            {
                Name = item.Name,
                Utilization = 20,
                Idle = 30,
                Breakdown = 50,
            });
        }
        return testRead.Wrap();
    }

    /// <summary>
    /// 获取试验系统详情页数据
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("test/detail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<ShowSystemDetailDto>> GetTestDetailAsync(ShowSystemDetailQueryDto showSystemDetailQueryDto)
    {
        ShowSystemDetailDto read = new ShowSystemDetailDto();
        read.ExperimentersData = new List<ExperimenterDto>()
        {
            new ExperimenterDto()
            {
                System = "这是System",
                Person = "这是Person"
            }
        };
        read.ChartData = new List<ChartDataDto>()
        {
            new ChartDataDto()
            {
                Name = "这是某个设备",
                Data = new List<ChartDataPointDto>()
                {
                    new ChartDataPointDto()
                    {
                        Time = "00:00",
                        Value = 10,
                    },
                    new ChartDataPointDto()
                    {
                        Time = "01:00",
                        Value = 50,
                    },
                    new ChartDataPointDto()
                    {
                        Time = "02:00",
                        Value = 20,
                    },
                    new ChartDataPointDto()
                    {
                        Time = "03:00",
                        Value = 50,
                    },
                    new ChartDataPointDto()
                    {
                        Time = "04:00",
                        Value = 60,
                    },
                    new ChartDataPointDto()
                    {
                        Time = "05:00",
                        Value = 10,
                    },
                    new ChartDataPointDto()
                    {
                        Time = "06:00",
                        Value = 90,
                    },
                    new ChartDataPointDto()
                    {
                        Time = "07:00",
                        Value = 30,
                    },
                    new ChartDataPointDto()
                    {
                        Time = "08:00",
                        Value = 40,
                    },
                    new ChartDataPointDto()
                    {
                        Time = "09:00",
                        Value = 10,
                    },
                    new ChartDataPointDto()
                    {
                        Time = "10:00",
                        Value = 0,
                    },
                    new ChartDataPointDto()
                    {
                        Time = "11:00",
                        Value = 50,
                    },
                }
            }
        };
        read.AbnormalDeviceListData = new List<AbnormalDeviceDto>()
        {
            new AbnormalDeviceDto()
            {
                System = showSystemDetailQueryDto.systemName,
                Device = "这是设备名称",
                Value = "这个设备正在发疯",
                Time = "5:20"
            }
        };
        read.CurrentTask = new TaskDetailDto()
        {
            Title = "这是当前试验的Title",
            Details = new List<List<string>>()
            {
                new List<string>(){ "这是当前试验的第一行", "这是第一行的内容" },
                new List<string>(){ "这是当前试验的第二行", "这是第二行的内容" },
                new List<string>(){ "这是当前试验的第三行", "这是第三行的内容" },
                new List<string>(){ "这是当前试验的第四行", "这是第四行的内容" }
            }
        };
        read.FollowTask = new TaskDetailDto()
        {
            Title = "这是后续试验的Title",
            Details = new List<List<string>>()
            {
                new List<string>(){ "这是后续试验的第一行", "这是第一行的内容" },
                new List<string>(){ "这是后续试验的第二行", "这是第二行的内容" }
            }
        };
        read.Queue = new List<TableDto>()
        {
            new TableDto()
            {
                Title = "这是队列的Title",
                Header = new List<List<string>>()
                {
                    new List<string> { "name", "阵列名称" },
                    new List<string> { "control", "阵列控制状态" },
                    new List<string> { "power", "阵列电压状态" }
                },
                Data = new List<Dictionary<string, string>>()
                {
                    new Dictionary<string, string>()
                    {
                        { "name", "任务1" },
                        { "control" , "正常得很" },
                        { "power" , "也很正常" }
                    }
                }
            }
        };
        read.ProductList = new List<TableDto>()
        {
            new TableDto()
            {
                Title = "这是产品队列的Title",
                Header = new List<List<string>>()
                {
                    new List<string> { "name", "产品名称" },
                    new List<string> { "code", "产品编号" },
                    new List<string> { "status", "产品技术状态" }
                },
                Data = new List<Dictionary<string, string>>()
                {
                    new Dictionary<string, string>()
                    {
                        { "name", "产品1" },
                        { "code" , "编号1" },
                        { "status" , "正常" }
                    }
                }
            }
        };
        return read.Wrap();
    }





















    /// <summary>
    /// 获取试验系统列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("test/list2")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<EquipLedgerTestReadDto>> GetTestListAsync2()
    {
        var testList = await _dictionaryInfoService.GetNameValueByTypeAsync("TestSystem");
        var testRead = new EquipLedgerTestReadDto();
        var list = new List<TestListReadDto>();
        foreach (var test in testList)
        {
            list.Add(new TestListReadDto()
            {
                TestName = test.Value
            });
        }

        foreach (var entity in list)
        {
            var rooms = await _roomService.GetRoomListByTestName(entity.TestName);
            var ids = await rooms.Select(x => x.Id).ToListAsync();
            var equips = (await _equipLedgerService.GetEquipsListByRoomAsync(ids)).ToList();
            if (equips.Count != 0)
            {
                entity.EquipCount = equips.Count(t => t.DeviceStatus == DeviceStatus.Normal);
                entity.Rate = equips.Count(t => t.DeviceStatus == DeviceStatus.Normal);
                entity.HealthRate = equips.Count(t => t.DeviceStatus == DeviceStatus.Normal);
            }
        }

        testRead.TestList = list;
        testRead.TestErrorList = new List<TestErrorListReadDto>();
        testRead.UpRate = 0;
        testRead.DownRate = 0;
        testRead.NormalCount = 0;
        testRead.FreeCount = 0;
        testRead.LeaveCount = 0;
        testRead.HealthCount = 0;
        testRead.BetterCount = 0;
        testRead.ErrorCount = 0;
        return testRead.Wrap();
    }

    /// <summary>
    /// 获取试验系统中的计划
    /// </summary>
    /// <param name="testName"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("test/list")]
    [AllowAnonymous]
    public async Task<ResponseWrapper<TestDataAppReadDto>> GetListByTestAsync(string testName)
    {
        var entity = new TestDataAppReadDto();
        entity.TestList = await (await _testDataService.GetListByTestAsync(testName)).ToListAsync();
        entity.NormalCount = 0;
        entity.FreeCount = 0;
        entity.LeaveCount = 0;
        entity.HealthCount = 0;
        entity.BetterCount = 0;
        entity.ErrorCount = 0;
        return entity.Wrap();
    }
}
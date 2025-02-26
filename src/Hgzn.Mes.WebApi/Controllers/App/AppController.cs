using Hgzn.Mes.Application.Main.Dtos.App;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Shared.Extensions;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    /// 获取试验系统列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("test/list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<EquipLedgerTestReadDto>> GetTestListAsync()
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
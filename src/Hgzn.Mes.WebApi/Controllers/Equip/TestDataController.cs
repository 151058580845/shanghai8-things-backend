using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Equip;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class TestDataController : ControllerBase
{
    private readonly ITestDataService _testDataService;
    private readonly IBaseConfigService _baseConfigService;

    public TestDataController(ITestDataService testDataService, IBaseConfigService baseConfigService)
    {
        _testDataService = testDataService;
        _baseConfigService = baseConfigService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("list")]
    [Authorize(Policy = $"equip:testdata:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<IEnumerable<TestDataReadDto>>> GetListAsync(TestDataQueryDto queryDto)
        => (await _testDataService.GetListAsync(queryDto)).Wrap();

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("page")]
    [Authorize(Policy = $"equip:testdata:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<PaginatedList<TestDataReadDto>>> GetPaginatedListAsync(TestDataQueryDto queryDto)
        => (await _testDataService.GetPaginatedListAsync(queryDto)).Wrap();

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"equip:testdata:{ScopeMethodType.Edit}")]
    public async Task<ResponseWrapper<TestDataReadDto>> CreateAsync(TestDataCreateDto input) =>
        (await _testDataService.CreateAsync(input)).Wrap();

    /// <summary>
    /// 导入
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("import")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"equip:testdata:{ScopeMethodType.Add}")]
    public async Task<ResponseWrapper<int>> CreateAsync(IEnumerable<TestDataCreateDto> inputs)
    {
        var addCount = await _testDataService.CreateAsync(inputs);
        return addCount.Wrap();
    }

    /// <summary>
    /// Api批量导入
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("importByApi")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"equip:testdata:{ScopeMethodType.Add}")]
    public async Task<ResponseWrapper<int>> CreateAsync()
    {
        var url = await _baseConfigService.GetValueByKeyAsync("import_plan_url");
        return (await _testDataService.GetDataFromThirdPartyAsync(url)).Wrap();
    }


    [HttpGet]
    [Route("getTestData")]
    [Authorize]
    public List<TestDataCreateDto> GetTestData(string apiUrl)
    {
        var detilData = new List<TestDataProductCreateDto>();
        detilData.Add(new TestDataProductCreateDto()
        {
            Name = "ces0",
            Code = "123",
            TechnicalStatus = ""
        });

        TestDataCreateDto testDataCreateDto = new TestDataCreateDto()
        {
            SysName = "ce",
            ProjectName = "ce",
            TaskName = "ce",
            DevPhase = "ce",
            TaskStartTime = "2025-01-01",
            TaskEndTime = "2025-01-01",
            ReqDep = "ce",
            ReqManager = "ce",
            ReqManagerCode = "ce",
            GncResp = "ce",
            GncRespCode = "ce",
            SimuStaff = "ce",
            QncResp = "ce",
            UUT = detilData
        };

        var allData = new List<TestDataCreateDto>();
        allData.Add(testDataCreateDto);

        return allData;
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"equip:testdata:{ScopeMethodType.Remove}")]
    public async Task<ResponseWrapper<int>> DeleteAsync(Guid id) =>
        (await _testDataService.DeleteAsync(id)).Wrap();

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"equip:testdata:{ScopeMethodType.Edit}")]
    public async Task<ResponseWrapper<TestDataReadDto>> UpdateAsync(Guid id, TestDataUpdateDto input) =>
        (await _testDataService.UpdateAsync(id, input)).Wrap();

    /// <summary>
    /// 获取
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"equip:testdata:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<TestDataReadDto>> GetAsync(Guid id) =>
        (await _testDataService.GetAsync(id)).Wrap();
}
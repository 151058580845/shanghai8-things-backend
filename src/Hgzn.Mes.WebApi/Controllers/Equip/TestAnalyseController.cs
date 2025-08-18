using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Equip;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class TestAnalyseController : ControllerBase
{
    private readonly ITestAnalyseServer _testAnalyseServer;

    public TestAnalyseController(ITestAnalyseServer testAnalyseServer)
    {
        _testAnalyseServer = testAnalyseServer;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("list")]
    [Authorize(Policy = $"equip:testdata:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<IEnumerable<TestAnalyseReadDto>>> GetListAsync(TestAnalyseQueryDto queryDto)
    => (await _testAnalyseServer.GetListAsync(queryDto)).Wrap();

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("page")]
    [Authorize(Policy = $"equip:testdata:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<PaginatedList<TestAnalyseReadDto>>> GetPaginatedListAsync(TestAnalyseQueryDto queryDto)
        => (await _testAnalyseServer.GetPaginatedListAsync(queryDto)).Wrap();
}

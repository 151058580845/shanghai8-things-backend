using Hgzn.Mes.Application.Main.Dtos.Audit;
using Hgzn.Mes.Application.Main.Services.Audit;
using Hgzn.Mes.Application.Main.Services.Audit.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Audit;

[Route("api/[controller]")]
[ApiController]
public class OperatorLogController : ControllerBase
{
    private readonly IOperLogService _operLogService;

    public OperatorLogController(IOperLogService operLogService)
    {
        _operLogService = operLogService;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("page")]
    [Authorize(Policy = $"audit:operatorlog:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<PaginatedList<OperatorLogReadDto>>> GetPaginatedListAsync(OperatorLogQueryDto queryDto) =>
        (await _operLogService.GetPaginatedListAsync(queryDto)).Wrap();

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("list")]
    [Authorize(Policy = $"audit:operatorlog:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<IEnumerable<OperatorLogReadDto>?>> GetListAsync(OperatorLogQueryDto queryDto) =>
        (await _operLogService.GetListAsync(queryDto)).Wrap()!;

    /// <summary>
    ///     删除
    /// </summary>
    /// <param name="guids"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("deletePage")]
    public async Task<ResponseWrapper<int>> DeleteAsync(List<Guid>? guids)
    {
        var dCount = 0;
        if (guids != null && guids.Count() > 0)
        {
            foreach (var logId in guids)
            {
                dCount += await _operLogService.DeleteAsync(logId);
            }
        }

        return dCount.Wrap();
    }

    /// <summary>
    ///     删除全部日志
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("deleteAll")]
    public async Task<ResponseWrapper<int>> DeleteAllAsync() =>
     (await _operLogService.DeleteAllLoginfo()).Wrap();

}
using Hgzn.Mes.Application.Main.Dtos.Audit;
using Hgzn.Mes.Application.Main.Services.Audit.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.WebApi.Utilities;
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
    public async Task<ResponseWrapper<PaginatedList<OperatorLogReadDto>>> GetPaginatedListAsync(OperatorLogQueryDto queryDto) =>
        (await _operLogService.GetPaginatedListAsync(queryDto)).Wrap();

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("list")]
    public async Task<ResponseWrapper<IEnumerable<OperatorLogReadDto>?>> GetListAsync(OperatorLogQueryDto queryDto) =>
        (await _operLogService.GetListAsync(queryDto)).Wrap()!;
}
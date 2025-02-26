using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Extensions;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Equip;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class EquipLedgerHistoryController : ControllerBase
{
    private readonly IEquipLedgerHistoryService _equipLedgerHistoryService;
    private readonly IEquipLedgerService _equipLedgerService;

    public EquipLedgerHistoryController(IEquipLedgerHistoryService equipLedgerHistoryService,
        IEquipLedgerService equipLedgerService)
    {
        _equipLedgerHistoryService = equipLedgerHistoryService;
        _equipLedgerService = equipLedgerService;
    }
    
    /// <summary>
    ///     分页查询
    ///     auth: anonymous
    /// </summary>
    /// <param name="input">用于验证用户身份</param>
    /// <returns>更换密码状态</returns>
    [HttpPost]
    [Route("page")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"equip:equipledgerhistory:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<PaginatedList<EquipLedgerHistoryReadDto>>> GetPaginatedListAsync(EquipLedgerHistoryQueryDto input) =>
        (await _equipLedgerHistoryService.GetPaginatedListAsync(input)).Wrap();
    /// <summary>
    ///     分页查询
    ///     auth: anonymous
    /// </summary>
    /// <param name="input">用于验证用户身份</param>
    /// <returns>更换密码状态</returns>
    [HttpPost]
    [Route("list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"equip:equipledgerhistory:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<IEnumerable<EquipLedgerHistoryReadDto>>> GetListAsync(EquipLedgerHistoryQueryDto input) =>
        (await _equipLedgerHistoryService.GetListAsync(input)).Wrap();
}
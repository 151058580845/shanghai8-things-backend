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

    /// <summary>
    /// 手持端巡检记录上传
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("app/store")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<int>> PostAppStoreAsync(List<EquipLedgerHistoryCreateDto> list)
    {
        var result = (await _equipLedgerHistoryService.CreateAsync(list)).Wrap();
        var dictionary =  list.Where(t=>t.RoomId != null).ToDictionary(t => t.EquipId, s => s.RoomId.Value);
        var equipList = await _equipLedgerService.UpdateEquipRoomId(dictionary);
        return equipList.Wrap();
    }

    /// <summary>
    /// 手持端上传搜索结果
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("app/search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<int>> PostAppSearchAsync(List<EquipLedgerHistoryCreateDto> list) =>
        (await _equipLedgerHistoryService.CreateAsync(list)).Wrap();
}
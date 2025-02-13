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
public class EquipNoticeController:ControllerBase
{
    private IEquipNoticeService _equipNoticeService;

    public EquipNoticeController(IEquipNoticeService equipNoticeService)
    {
        _equipNoticeService = equipNoticeService;
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
    [Authorize(Policy = $"equip:equipnotice:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<PaginatedList<EquipNoticeReadDto>>> GetPaginatedListAsync(EquipNoticeQueryDto input) =>
        (await _equipNoticeService.GetPaginatedListAsync(input)).Wrap();
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
    [Authorize(Policy = $"equip:equipnotice:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<IEnumerable<EquipNoticeReadDto>>> GetListAsync(EquipNoticeQueryDto input) =>
        (await _equipNoticeService.GetListAsync(input)).Wrap();

}
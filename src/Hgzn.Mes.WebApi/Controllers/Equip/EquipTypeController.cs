using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Equip;

[Route("api/[controller]")]
[ApiController]
public class EquipTypeController : ControllerBase
{
    private readonly IEquipTypeService _equipTypeService;

    public EquipTypeController(IEquipTypeService equipTypeService)
    {
        _equipTypeService = equipTypeService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("list")]
    public async Task<ResponseWrapper<PaginatedList<EquipTypeReadDto>?>> GetListAsync(EquipTypeQueryDto queryDto)
        => (await _equipTypeService.GetPaginatedListAsync(queryDto)).Wrap();

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:code:{ScopeMethodType.Edit}")]
    public async Task<ResponseWrapper<EquipTypeReadDto?>> CreateAsync(EquipTypeCreateDto input) =>
        (await _equipTypeService.CreateAsync(input)).Wrap();

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:code:{ScopeMethodType.Remove}")]
    public async Task<ResponseWrapper<int>> DeleteAsync(Guid id) =>
        (await _equipTypeService.DeleteAsync(id)).Wrap();

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
    [Authorize(Policy = $"system:code:{ScopeMethodType.Edit}")]
    public async Task<ResponseWrapper<EquipTypeReadDto?>> UpdateAsync(Guid id, EquipTypeUpdateDto input) =>
        (await _equipTypeService.UpdateAsync(id, input)).Wrap();

    /// <summary>
    /// 获取
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:code:{ScopeMethodType.Add}")]
    public async Task<ResponseWrapper<EquipTypeReadDto?>> GetAsync(Guid id) =>
        (await _equipTypeService.GetAsync(id)).Wrap();
}
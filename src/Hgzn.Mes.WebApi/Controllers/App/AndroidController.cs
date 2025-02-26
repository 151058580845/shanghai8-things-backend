using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.App;

/// <summary>
/// 安卓设备使用
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AndroidController:ControllerBase
{
    private readonly IEquipLedgerService _equipLedgerService;
    private readonly IEquipLedgerHistoryService _equipLedgerHistoryService;
    private readonly ILocationLabelService _locationLabelService;

    public AndroidController(
        IEquipLedgerService equipLedgerService,
        IEquipLedgerHistoryService equipLedgerHistoryService,
        ILocationLabelService locationLabelService)
    {
        _equipLedgerService = equipLedgerService;
        _equipLedgerHistoryService = equipLedgerHistoryService;
        _locationLabelService = locationLabelService;
    }

    /// <summary>
    ///     手持端获取设备rfid绑定关系
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("label/equip")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<PaginatedList<EquipLocationLabelReadDto>>> GetEquipLabelAsync(int pageIndex = 1, int pageSize = 100) =>
        (await _locationLabelService.GetEquipLabelAsync(pageIndex, pageSize)).Wrap();
    
    /// <summary>
    ///     手持端获取房间rfid绑定关系
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("label/room")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<PaginatedList<RoomLocationLabelReadDto>>> GetRoomLabelAsync(int pageIndex = 1, int pageSize = 100) =>
        (await _locationLabelService.GetRoomLabelAsync(pageIndex, pageSize)).Wrap();
    
    /// <summary>
    /// 手持端获取搜索目标
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<IEnumerable<EquipLedgerSearchReadDto>>> GetAppSearchAsync() =>
        (await _equipLedgerService.GetAppSearchAsync()).Wrap();
    
    /// <summary>
    /// 手持端巡检记录上传
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("store")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<int>> PostAppStoreAsync(List<EquipLedgerHistoryCreateDto> list)
    {
        var result = (await _equipLedgerHistoryService.CreateAsync(list)).Wrap();
        var dictionary =  list.Where(t=>t.RoomId != null).ToDictionary(t => t.EquipCode, s => s.RoomId.Value);
        var equipList = await _equipLedgerService.UpdateEquipRoomId(dictionary);
        return equipList.Wrap();
    }

    /// <summary>
    /// 手持端上传搜索结果
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<int>> PostAppSearchAsync(List<EquipLedgerHistoryCreateDto> list) =>
        (await _equipLedgerHistoryService.CreateAsync(list)).Wrap();
}
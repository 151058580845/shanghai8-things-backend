using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
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

    public AndroidController(IEquipLedgerService equipLedgerService, IEquipLedgerHistoryService equipLedgerHistoryService)
    {
        _equipLedgerService = equipLedgerService;
        _equipLedgerHistoryService = equipLedgerHistoryService;
    }
    
    /// <summary>
    /// 手持端获取设备rfid绑定关系
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("model")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<IEnumerable<EquipLedgerSearchReadDto>>> GetModelAsync() =>
        (await _equipLedgerService.GetAppSearchAsync()).Wrap();
    
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
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.System;

[Route("api/[controller]")]
[ApiController]
public class LocationController : ControllerBase
{
    private readonly IBuildingService _buildingService;
    private readonly IFloorService _floorService;
    private readonly IRoomService _roomService;

    public LocationController(IBuildingService buildingService, IFloorService floorService, IRoomService roomService)
    {
        _buildingService = buildingService;
        _floorService = floorService;
        _roomService = roomService;
    }

    #region Building

    /// <summary>
    ///     获取
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("building/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:building:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<BuildingReadDto>> GetBuildingAsync(Guid id) =>
        (await _buildingService.GetAsync(id)).Wrap();

    /// <summary>
    ///     删除
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("building/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:building:{ScopeMethodType.Remove}")]
    public async Task<ResponseWrapper<int>> DeleteBuildingAsync(Guid id) =>
        (await _buildingService.DeleteAsync(id)).Wrap();

    /// <summary>
    ///     新增
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("building/create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:building:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<BuildingReadDto>> CreateBuildingAsync(BuildingCreateDto dto) =>
        (await _buildingService.CreateAsync(dto)).Wrap();

    /// <summary>
    ///     更新
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("building/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:building:{ScopeMethodType.Edit}")]
    public async Task<ResponseWrapper<BuildingReadDto>> UpdateBuildingAsync(Guid id, BuildingUpdateDto input) =>
        (await _buildingService.UpdateAsync(id, input)).Wrap();

    /// <summary>
    ///     分页查询
    ///     auth: anonymous
    /// </summary>
    [HttpPost]
    [Route("building/page")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:building:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<PaginatedList<BuildingReadDto>>> GetBuildingPaginatedListAsync(
        BuildingQueryDto input) =>
        (await _buildingService.GetPaginatedListAsync(input)).Wrap();

    /// <summary>
    ///     分页查询
    ///     auth: anonymous
    /// </summary>
    [HttpPost]
    [Route("building/list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:building:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<IEnumerable<BuildingReadDto>>> GetBuildingListAsync(BuildingQueryDto? input) =>
        (await _buildingService.GetListAsync(input)).Wrap();

    #endregion


    #region Floor

    /// <summary>
    ///     获取
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("floor/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:floor:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<FloorReadDto>> GetFloorAsync(Guid id) =>
        (await _floorService.GetAsync(id)).Wrap();

    /// <summary>
    ///     删除
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("floor/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:floor:{ScopeMethodType.Remove}")]
    public async Task<ResponseWrapper<int>> DeleteFloorAsync(Guid id) =>
        (await _floorService.DeleteAsync(id)).Wrap();

    /// <summary>
    ///     新增
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("floor/create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:floor:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<FloorReadDto>> CreateFloorAsync(FloorCreateDto dto) =>
        (await _floorService.CreateAsync(dto)).Wrap();

    /// <summary>
    ///     更新
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("floor/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:floor:{ScopeMethodType.Edit}")]
    public async Task<ResponseWrapper<FloorReadDto>> UpdateFloorAsync(Guid id, FloorUpdateDto input) =>
        (await _floorService.UpdateAsync(id, input)).Wrap();

    /// <summary>
    ///     分页查询
    ///     auth: anonymous
    /// </summary>
    [HttpPost]
    [Route("floor/page")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:floor:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<PaginatedList<FloorReadDto>>> GetFloorPaginatedListAsync(
        FloorQueryDto input) =>
        (await _floorService.GetPaginatedListAsync(input)).Wrap();

    /// <summary>
    ///     分页查询
    ///     auth: anonymous
    /// </summary>
    [HttpPost]
    [Route("floor/list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:floor:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<IEnumerable<FloorReadDto>>> GetFloorListAsync(FloorQueryDto? input) =>
        (await _floorService.GetListAsync(input)).Wrap();

    #endregion

    #region Room

    /// <summary>
    ///     获取
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("room/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:room:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<RoomReadDto>> GetRoomAsync(Guid id) =>
        (await _roomService.GetAsync(id)).Wrap();

    /// <summary>
    ///     删除
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("room/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:room:{ScopeMethodType.Remove}")]
    public async Task<ResponseWrapper<int>> DeleteRoomAsync(Guid id) =>
        (await _roomService.DeleteAsync(id)).Wrap();

    /// <summary>
    ///     新增
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("room/create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:room:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<RoomReadDto>> CreateFloorAsync(RoomCreateDto dto) =>
        (await _roomService.CreateAsync(dto)).Wrap();

    /// <summary>
    ///     更新
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("room/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:room:{ScopeMethodType.Edit}")]
    public async Task<ResponseWrapper<RoomReadDto>> UpdateRoomAsync(Guid id, RoomUpdateDto input) =>
        (await _roomService.UpdateAsync(id, input)).Wrap();

    /// <summary>
    ///     分页查询
    ///     auth: anonymous
    /// </summary>
    /// <param name="input">用于验证用户身份</param>
    /// <returns>更换密码状态</returns>
    [HttpPost]
    [Route("room/page")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:room:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<PaginatedList<RoomReadDto>>> GetRoomPaginatedListAsync(RoomQueryDto input) =>
        (await _roomService.GetPaginatedListAsync(input)).Wrap();

    /// <summary>
    ///     分页查询
    ///     auth: anonymous
    /// </summary>
    /// <param name="input">用于验证用户身份</param>
    /// <returns>更换密码状态</returns>
    [HttpPost]
    [Route("room/list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:room:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<IEnumerable<RoomReadDto>>> GetRoomListAsync(RoomQueryDto? input) =>
        (await _roomService.GetListAsync(input)).Wrap();

    #endregion
}
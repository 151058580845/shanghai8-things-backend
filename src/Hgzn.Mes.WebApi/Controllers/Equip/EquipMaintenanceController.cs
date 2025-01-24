using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Equip
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipMaintenanceController : ControllerBase
    {
        private readonly IEquipMaintenancePlanService _equipMaintenancePlanService;
        private readonly IEquipMaintenanceRecordsService _equipMaintenanceRecordsService;
        private readonly IEquipMaintenanceTaskService _equipMaintenanceTaskService;

        public EquipMaintenanceController(IEquipMaintenancePlanService equipMaintenancePlanService, IEquipMaintenanceRecordsService equipMaintenanceRecordsService, IEquipMaintenanceTaskService equipMaintenanceTaskService)
        {
            this._equipMaintenancePlanService = equipMaintenancePlanService;
            this._equipMaintenanceRecordsService = equipMaintenanceRecordsService;
            this._equipMaintenanceTaskService = equipMaintenanceTaskService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("plan")]
        public async Task<ResponseWrapper<PaginatedList<EquipMaintenancePlanReadDto>?>> GetPaginatedListAsync(EquipMaintenancePlanQueryDto queryDto)
        => (await _equipMaintenancePlanService.GetPaginatedListAsync(queryDto)).Wrap();

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
        [Authorize(Policy = $"system:code:{ScopeMethodType.Add}")]
        public async Task<ResponseWrapper<EquipMaintenancePlanReadDto?>> EquipMaintenancePlanCreateAsync(EquipMaintenancePlanCreateDto input) =>
            (await _equipMaintenancePlanService.CreateAsync(input)).Wrap();

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
            (await _equipMaintenancePlanService.DeleteAsync(id)).Wrap();

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
        public async Task<ResponseWrapper<EquipMaintenancePlanReadDto?>> EquipMaintenancePlanUpdateAsync(Guid id, EquipMaintenancePlanUpdateDto input) =>
            (await _equipMaintenancePlanService.UpdateAsync(id, input)).Wrap();

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:code:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<EquipMaintenancePlanReadDto?>> EquipMaintenancePlanGetAsync(Guid id) =>
            (await _equipMaintenancePlanService.GetAsync(id)).Wrap();

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("record")]
        public async Task<ResponseWrapper<PaginatedList<EquipMaintenanceRecordsReadDto>?>> GetPaginatedListAsync(EquipMaintenanceRecordsQueryDto queryDto)
        => (await _equipMaintenanceRecordsService.GetPaginatedListAsync(queryDto)).Wrap();

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
        [Authorize(Policy = $"system:code:{ScopeMethodType.Add}")]
        public async Task<ResponseWrapper<EquipMaintenanceRecordsReadDto?>> EquipMaintenanceRecordsCreateAsync(EquipMaintenanceRecordsCreateDto input) =>
            (await _equipMaintenanceRecordsService.CreateAsync(input)).Wrap();

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
        public async Task<ResponseWrapper<int>> EquipMaintenanceRecordsDeleteAsync(Guid id) =>
            (await _equipMaintenanceRecordsService.DeleteAsync(id)).Wrap();

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
        public async Task<ResponseWrapper<EquipMaintenanceRecordsReadDto?>> EquipMaintenanceRecordsUpdateAsync(Guid id, EquipMaintenanceRecordsUpdateDto input) =>
            (await _equipMaintenanceRecordsService.UpdateAsync(id, input)).Wrap();

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:code:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<EquipMaintenanceRecordsReadDto?>> EquipMaintenanceRecordsGetAsync(Guid id) =>
            (await _equipMaintenanceRecordsService.GetAsync(id)).Wrap();

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("task")]
        public async Task<ResponseWrapper<PaginatedList<EquipMaintenanceTaskReadDto>?>> GetPaginatedListAsync(EquipMaintenanceTaskQueryDto queryDto)
        => (await _equipMaintenanceTaskService.GetPaginatedListAsync(queryDto)).Wrap();

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
        [Authorize(Policy = $"system:code:{ScopeMethodType.Add}")]
        public async Task<ResponseWrapper<EquipMaintenanceTaskReadDto?>> EquipMaintenanceTaskCreateAsync(EquipMaintenanceTaskCreateDto input) =>
            (await _equipMaintenanceTaskService.CreateAsync(input)).Wrap();

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
        public async Task<ResponseWrapper<int>> EquipMaintenanceTaskDeleteAsync(Guid id) =>
            (await _equipMaintenanceTaskService.DeleteAsync(id)).Wrap();

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
        public async Task<ResponseWrapper<EquipMaintenanceTaskReadDto?>> EquipMaintenanceTaskUpdateAsync(Guid id, EquipMaintenanceTaskUpdateDto input) =>
            (await _equipMaintenanceTaskService.UpdateAsync(id, input)).Wrap();

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:code:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<EquipMaintenanceTaskReadDto?>> EquipMaintenanceTaskGetAsync(Guid id) =>
            (await _equipMaintenanceTaskService.GetAsync(id)).Wrap();
    }
}

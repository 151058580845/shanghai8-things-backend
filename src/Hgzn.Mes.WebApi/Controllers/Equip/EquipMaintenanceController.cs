using Hgzn.Mes.Application.Main.Dtos.Equip;
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

        public EquipMaintenanceController()
        {
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("plan/page")]
        public async Task<ResponseWrapper<PaginatedList<EquipMaintenancePlanReadDto>>> GetPaginatedListAsync(EquipMaintenancePlanQueryDto queryDto)
        => (await _equipMaintenancePlanService.GetPaginatedListAsync(queryDto)).Wrap();

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("plan/list")]
        public async Task<ResponseWrapper<IEnumerable<EquipMaintenancePlanReadDto>?>> GetListAsync(EquipMaintenancePlanQueryDto queryDto)
            => (await _equipMaintenancePlanService.GetListAsync(queryDto)).Wrap()!;

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("plan/create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipmaintenanceplan:{ScopeMethodType.Add}")]
        public async Task<ResponseWrapper<EquipMaintenancePlanReadDto>> EquipMaintenancePlanCreateAsync(EquipMaintenancePlanCreateDto input) =>
            (await _equipMaintenancePlanService.CreateAsync(input)).Wrap();

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("plan/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipmaintenanceplan:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<int>> DeleteAsync(Guid id) =>
            (await _equipMaintenancePlanService.DeleteAsync(id)).Wrap();

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("plan/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipmaintenanceplan:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<EquipMaintenancePlanReadDto>> EquipMaintenancePlanUpdateAsync(Guid id, EquipMaintenancePlanUpdateDto input) =>
            (await _equipMaintenancePlanService.UpdateAsync(id, input)).Wrap();

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("plan/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipmaintenanceplan:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<EquipMaintenancePlanReadDto>> EquipMaintenancePlanGetAsync(Guid id) =>
            (await _equipMaintenancePlanService.GetAsync(id)).Wrap();

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("record/page")]
        public async Task<ResponseWrapper<PaginatedList<EquipMaintenanceRecordsReadDto>>> GetPaginatedListAsync(EquipMaintenanceRecordsQueryDto queryDto)
        => (await _equipMaintenanceRecordsService.GetPaginatedListAsync(queryDto)).Wrap();

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("record/list")]
        public async Task<ResponseWrapper<IEnumerable<EquipMaintenanceRecordsReadDto>>> GetListAsync(EquipMaintenanceRecordsQueryDto queryDto)
            => (await _equipMaintenanceRecordsService.GetListAsync(queryDto)).Wrap();

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("record/create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipmaintenancerecords:{ScopeMethodType.Add}")]
        public async Task<ResponseWrapper<EquipMaintenanceRecordsReadDto>> EquipMaintenanceRecordsCreateAsync(EquipMaintenanceRecordsCreateDto input) =>
            (await _equipMaintenanceRecordsService.CreateAsync(input)).Wrap();

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("record/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipmaintenancerecords:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<int>> EquipMaintenanceRecordsDeleteAsync(Guid id) =>
            (await _equipMaintenanceRecordsService.DeleteAsync(id)).Wrap();

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("record/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipmaintenancerecords:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<EquipMaintenanceRecordsReadDto>> EquipMaintenanceRecordsUpdateAsync(Guid id, EquipMaintenanceRecordsUpdateDto input) =>
            (await _equipMaintenanceRecordsService.UpdateAsync(id, input)).Wrap();

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("record/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipmaintenancerecords:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<EquipMaintenanceRecordsReadDto>> EquipMaintenanceRecordsGetAsync(Guid id) =>
            (await _equipMaintenanceRecordsService.GetAsync(id)).Wrap();

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("task/page")]
        public async Task<ResponseWrapper<PaginatedList<EquipMaintenanceTaskReadDto>>> GetPaginatedListAsync(EquipMaintenanceTaskQueryDto queryDto)
        => (await _equipMaintenanceTaskService.GetPaginatedListAsync(queryDto)).Wrap();

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("task/list")]
        public async Task<ResponseWrapper<IEnumerable<EquipMaintenanceTaskReadDto>?>> GetListAsync(EquipMaintenanceTaskQueryDto queryDto)
            => (await _equipMaintenanceTaskService.GetListAsync(queryDto)).Wrap()!;

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("task/create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipmaintenanceretask:{ScopeMethodType.Add}")]
        public async Task<ResponseWrapper<EquipMaintenanceTaskReadDto>> EquipMaintenanceTaskCreateAsync(EquipMaintenanceTaskCreateDto input) =>
            (await _equipMaintenanceTaskService.CreateAsync(input)).Wrap();

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("task/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipmaintenanceretask:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<int>> EquipMaintenanceTaskDeleteAsync(Guid id) =>
            (await _equipMaintenanceTaskService.DeleteAsync(id)).Wrap();

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("task/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipmaintenanceretask:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<EquipMaintenanceTaskReadDto>> EquipMaintenanceTaskUpdateAsync(Guid id, EquipMaintenanceTaskUpdateDto input) =>
            (await _equipMaintenanceTaskService.UpdateAsync(id, input)).Wrap();

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("task/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipmaintenanceretask:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<EquipMaintenanceTaskReadDto>> EquipMaintenanceTaskGetAsync(Guid id) =>
            (await _equipMaintenanceTaskService.GetAsync(id)).Wrap();
    }
}

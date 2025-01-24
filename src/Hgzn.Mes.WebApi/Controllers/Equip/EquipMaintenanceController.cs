using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.WebApi.Utilities;
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("PlanList")]
        public async Task<ResponseWrapper<PaginatedList<EquipMaintenancePlanReadDto>>> GetPaginatedListAsync(EquipMaintenancePlanQueryDto queryDto)
        => (await _equipMaintenancePlanService.GetPaginatedListAsync(queryDto)).Wrap();

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("RecordsList")]
        public async Task<ResponseWrapper<PaginatedList<EquipMaintenanceRecordsReadDto>>> GetPaginatedListAsync(EquipMaintenanceRecordsQueryDto queryDto)
        => (await _equipMaintenanceRecordsService.GetPaginatedListAsync(queryDto)).Wrap();

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("TaskList")]
        public async Task<ResponseWrapper<PaginatedList<EquipMaintenanceTaskReadDto>>> GetPaginatedListAsync(EquipMaintenanceTaskQueryDto queryDto)
        => (await _equipMaintenanceTaskService.GetPaginatedListAsync(queryDto)).Wrap();
    }
}

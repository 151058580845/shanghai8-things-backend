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

        public EquipMaintenanceController(
            IEquipMaintenancePlanService equipMaintenancePlanService,
            IEquipMaintenanceRecordsService equipMaintenanceRecordsService,
            IEquipMaintenanceTaskService equipMaintenanceTaskService)
        {
            _equipMaintenancePlanService = equipMaintenancePlanService;
            _equipMaintenanceRecordsService = equipMaintenanceRecordsService;
            _equipMaintenanceTaskService = equipMaintenanceTaskService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("plan")]
        public async Task<ResponseWrapper<PaginatedList<EquipMaintenancePlanReadDto>?>> GetPaginatedListAsync(EquipMaintenancePlanQueryDto queryDto)
        => (await _equipMaintenancePlanService.GetPaginatedListAsync(queryDto)).Wrap();

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("record")]
        public async Task<ResponseWrapper<PaginatedList<EquipMaintenanceRecordsReadDto>?>> GetPaginatedListAsync(EquipMaintenanceRecordsQueryDto queryDto)
        => (await _equipMaintenanceRecordsService.GetPaginatedListAsync(queryDto)).Wrap();

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("task")]
        public async Task<ResponseWrapper<PaginatedList<EquipMaintenanceTaskReadDto>?>> GetPaginatedListAsync(EquipMaintenanceTaskQueryDto queryDto)
        => (await _equipMaintenanceTaskService.GetPaginatedListAsync(queryDto)).Wrap();
    }
}

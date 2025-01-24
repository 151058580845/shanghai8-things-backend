using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Equip
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipMaintenancePlanController : ControllerBase
    {
        //private readonly IEquipMaintenancePlanService _equipMaintenancePlanService;

        //public EquipMaintenancePlanController(IEquipMaintenancePlanService equipMaintenancePlanService)
        //{
        //    _equipMaintenancePlanService = equipMaintenancePlanService;
        //}

        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[Route("list")]
        //public async Task<ResponseWrapper<PaginatedList<EquipMaintenancePlanReadDto>>> GetPaginatedListAsync(EquipMaintenancePlanQueryDto queryDto)
        //=> (await _equipItemsService.GetPaginatedListAsync(queryDto)).Wrap();
    }
}

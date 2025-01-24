using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Equip
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipMaintenancePlanController : ControllerBase
    {
        private readonly IEquipMaintenancePlanService _equipMaintenancePlanService;

        public EquipMaintenancePlanController(IEquipMaintenancePlanService equipMaintenancePlanService)
        {
            _equipMaintenancePlanService = equipMaintenancePlanService;
        }
    }
}

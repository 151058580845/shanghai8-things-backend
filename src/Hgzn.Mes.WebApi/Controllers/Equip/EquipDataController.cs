using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Equip
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipDataController : ControllerBase
    {
        private readonly IEquipDataService _equipDataService;

        public EquipDataController(IEquipDataService equipDataService)
        {
            _equipDataService = equipDataService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("list")]
        public async Task<ResponseWrapper<PaginatedList<EquipDataReadDto>>> GetPaginatedListAsync(EquipDataQueryDto queryDto)
        => (await _equipDataService.GetPaginatedListAsync(queryDto)).Wrap();
    }
}

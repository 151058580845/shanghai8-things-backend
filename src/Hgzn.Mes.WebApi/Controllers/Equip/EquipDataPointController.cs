using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Equip
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipDataPointController : ControllerBase
    {
        private readonly IEquipDataPointService _equipDataPointService;

        public EquipDataPointController(IEquipDataPointService equipDataPointService)
        {
            _equipDataPointService = equipDataPointService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("list")]
        public async Task<ResponseWrapper<PaginatedList<EquipDataPointReadDto>>> GetPaginatedListAsync(EquipDataPointQueryDto queryDto)
        => (await _equipDataPointService.GetPaginatedListAsync(queryDto)).Wrap();
    }
}

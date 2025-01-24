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
    public class EquipItemsController : ControllerBase
    {
        private readonly IEquipItemsService _equipItemsService;

        public EquipItemsController(IEquipItemsService equipItemsService)
        {
            _equipItemsService = equipItemsService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("list")]
        public async Task<ResponseWrapper<PaginatedList<EquipItemsReadDto>>> GetPaginatedListAsync(EquipItemsQueryDto queryDto)
        => (await _equipItemsService.GetPaginatedListAsync(queryDto)).Wrap();


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("Enums")]
        public async Task<ResponseWrapper<EquipItemsEnumReadDto>> GetEnumsAsync()
        => (await _equipItemsService.GetEnumsAsync()).Wrap()!;
    }
}

using Hgzn.Mes.Application.Dtos.Equip;
using Hgzn.Mes.Application.Services.Equip.IService;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Equip;

[Route("Api/[controller]")]
[ApiController]
public class EquipTypeController:BaseController
{
    private readonly IEquipTypeService _equipTypeService;

    public EquipTypeController(IEquipTypeService equipTypeService)
    {
        this._equipTypeService = equipTypeService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("list")]
    public async Task<ResponseWrapper<IEnumerable<EquipTypeReadDto>>> GetListAsync(EquipTypeQueryDto queryDto)
        => Success(await _equipTypeService.GetListAsync(queryDto));
}
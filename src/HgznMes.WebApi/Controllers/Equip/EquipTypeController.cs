using HgznMes.Application.Dtos.Equip;
using HgznMes.Application.Services.Equip.IService;
using HgznMes.WebApi.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace HgznMes.WebApi.Controllers.Equip;

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
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Equip;

[Route("api/[controller]")]
[ApiController]
public class EquipTypeController : ControllerBase
{
    private readonly IEquipTypeService _equipTypeService;

    public EquipTypeController(IEquipTypeService equipTypeService)
    {
        _equipTypeService = equipTypeService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("list")]
    public async Task<ResponseWrapper<PaginatedList<EquipTypeReadDto>?>> GetListAsync(EquipTypeQueryDto queryDto)
        => (await _equipTypeService.GetPaginatedListAsync(queryDto)).Wrap();

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    [Route("")]
    public async Task<ResponseWrapper<EquipTypeReadDto>> CreateAsync(EquipTypeCreateDto createDto)
    => (await _equipTypeService.CreateAsync(createDto)).Wrap()!;
}
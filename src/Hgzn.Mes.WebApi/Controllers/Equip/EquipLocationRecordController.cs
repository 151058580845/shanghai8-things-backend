using Hgzn.Mes.Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Hgzn.Mes.WebApi.Utilities;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;

namespace Hgzn.Mes.WebApi.Controllers.Equip;

[Route("api/[controller]")]
[ApiController]
public class EquipLocationRecordController(IEquipLocationRecordService _service) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("page")]
    public async Task<ResponseWrapper<PaginatedList<EquipLocationRecordReadDto>>> GetPaginatedListAsync(EquipLocationRecordQueryDto queryDto) => (await _service.GetPaginatedListAsync(queryDto)).Wrap();
}

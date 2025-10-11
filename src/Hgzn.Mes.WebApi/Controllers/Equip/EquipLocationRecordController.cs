using Hgzn.Mes.Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Hgzn.Mes.WebApi.Utilities;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Application.Main.Services.Equip;

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

    /// <summary>
    /// 获取设备台账分页列表（用于Rfid跟踪历史页面）
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("ledger/page")]
    public async Task<ResponseWrapper<PaginatedList<EquipLedgerReadDto>>> GetEquipLedgerPaginatedListAsync(EquipLedgerQueryDto queryDto)
    {
        var service = _service as EquipLocationRecordService;
        return (await service!.GetEquipLedgerPaginatedListAsync(queryDto)).Wrap();
    }
}

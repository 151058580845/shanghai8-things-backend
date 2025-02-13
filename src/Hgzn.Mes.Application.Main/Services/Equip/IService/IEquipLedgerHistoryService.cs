using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService;

public interface IEquipLedgerHistoryService : ICrudAppService<
    EquipLedgerHistory, Guid,
    EquipLedgerHistoryReadDto, EquipLedgerHistoryQueryDto,
    EquipLedgerHistoryCreateDto>
{
    Task<int> CreateAsync(List<EquipLedgerHistoryCreateDto> list);
}
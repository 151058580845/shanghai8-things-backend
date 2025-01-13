using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService;

public interface IEquipLedgerService : ICrudAppService<EquipLedger, Guid,EquipLedgerQueryDto,
     EquipLedgerReadDto, EquipLedgerCreateDto, EquipLedgerUpdateDto>
{
}
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService;

public interface IEquipLocationRecordService : ICrudAppService<
    EquipLocationRecord, Guid,
    EquipLocationRecordReadDto, EquipLocationRecordQueryDto>
{
}

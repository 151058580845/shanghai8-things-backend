using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService;

public interface IEquipNoticeService : ICrudAppService<
    EquipNotice, Guid,
    EquipNoticeReadDto, EquipNoticeQueryDto,
    EquipNoticeCreateDto>
{
    
}
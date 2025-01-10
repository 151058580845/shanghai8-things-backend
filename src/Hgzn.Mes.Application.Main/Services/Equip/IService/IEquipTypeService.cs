using Hgzn.Mes.Application.BaseS;
using Hgzn.Mes.Application.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;

namespace Hgzn.Mes.Application.Services.Equip.IService;

public interface IEquipTypeService:ICrudAppService<EquipType
    ,EquipTypeReadDto,EquipTypeReadDto,Guid,EquipTypeQueryDto,EquipTypeCreateDto,EquipTypeUpdateDto>
{
    
}
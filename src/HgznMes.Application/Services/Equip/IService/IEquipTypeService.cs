using HgznMes.Application.BaseS;
using HgznMes.Application.Dtos.Equip;
using HgznMes.Domain.Entities.Equip.EquipManager;

namespace HgznMes.Application.Services.Equip.IService;

public interface IEquipTypeService:ICrudAppService<EquipTypeAggregateRoot
    ,EquipTypeReadDto,EquipTypeReadDto,Guid,EquipTypeQueryDto,EquipTypeCreateDto,EquipTypeUpdateDto>
{
    
}
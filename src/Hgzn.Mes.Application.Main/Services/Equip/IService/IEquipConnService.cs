using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService;

public interface IEquipConnectService:ICrudAppService<EquipConnect
    ,Guid,EquipConnectQueryDto,EquipConnectReadDto,EquipConnectCreateDto,EquipConnectUpdateDto>
{
    
}
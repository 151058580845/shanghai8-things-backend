using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipConnService:CrudAppServiceSugar<EquipConnect
    ,Guid,EquipConnectQueryDto,EquipConnectReadDto,EquipConnectCreateDto,EquipConnectUpdateDto>
{
    public override Task<IEnumerable<EquipConnectReadDto>> GetListAsync(EquipConnectQueryDto queryDto)
    {
        throw new NotImplementedException();
    }
}
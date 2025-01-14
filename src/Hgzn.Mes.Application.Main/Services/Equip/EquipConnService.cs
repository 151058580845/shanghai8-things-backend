using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Shared;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipConnService:CrudAppServiceSugar<EquipConnect
    ,Guid,EquipConnectQueryDto,EquipConnectReadDto,EquipConnectCreateDto,EquipConnectUpdateDto>
{
    public override Task<PaginatedList<EquipConnectReadDto>> GetListAsync(EquipConnectQueryDto queryDto)
    {
        throw new NotImplementedException();
    }
}
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Infrastructure.SqlSugarContext;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipConnService:CrudAppServiceSugar<EquipConnect
    ,EquipConnectReadDto,EquipConnectReadDto,Guid,EquipConnectQueryDto,EquipConnectCreateDto,EquipConnectUpdateDto>
{
    public EquipConnService(SqlSugarContext dbContext) : base(dbContext)
    {
    }

    public override Task<IEnumerable<EquipConnectReadDto>> GetListAsync(EquipConnectQueryDto input)
    {
        throw new NotImplementedException();
    }
}
using Hgzn.Mes.Application.Dtos.Equip;
using Hgzn.Mes.Application.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Infrastructure.SqlSugarContext;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipTypeService : CrudAppServiceSugar<EquipType
        , EquipTypeReadDto, EquipTypeReadDto, Guid, EquipTypeQueryDto, EquipTypeCreateDto, EquipTypeUpdateDto>,
    IEquipTypeService
{
    public EquipTypeService(SqlSugarContext dbContext) : base(dbContext)
    {
    }

    public override async Task<IEnumerable<EquipTypeReadDto>> GetListAsync(EquipTypeQueryDto input)
    {
        var entities = await Queryable()
            .Where(m => input.TypeName == null || input.TypeName == m.TypeName)
            .Where(m => input.TypeCode == null || input.TypeCode == m.TypeCode)
            .OrderBy(m => m.OrderNum)
            .ToListAsync();
        return Mapper.Map<IEnumerable<EquipTypeReadDto>>(entities);
    }
}
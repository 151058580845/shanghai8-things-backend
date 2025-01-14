using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipTypeService : CrudAppServiceSugar<EquipType, Guid, EquipTypeQueryDto,
    EquipTypeReadDto, EquipTypeCreateDto, EquipTypeUpdateDto>,
    IEquipTypeService
{
    public override async Task<PaginatedList<EquipTypeReadDto>> GetListAsync(EquipTypeQueryDto dto)
    {
        var entities = await Queryable()
            .Where(m => dto.TypeName == null || dto.TypeName == m.TypeName)
            .Where(m => dto.TypeCode == null || dto.TypeCode == m.TypeCode)
            .OrderBy(m => m.OrderNum)
            .ToListAsync();
        return Mapper.Map<PaginatedList<EquipTypeReadDto>>(entities);
    }
}
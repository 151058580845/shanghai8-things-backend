using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipTypeService : SugarCrudAppService<
    EquipType, Guid,
    EquipTypeReadDto, EquipTypeQueryDto,
    EquipTypeCreateDto, EquipTypeUpdateDto>,
    IEquipTypeService
{
    public override Task<IEnumerable<EquipTypeReadDto>> GetListAsync(EquipTypeQueryDto? queryDto)
    {
        throw new NotImplementedException();
    }

    public override async Task<PaginatedList<EquipTypeReadDto>> GetPaginatedListAsync(EquipTypeQueryDto dto)
    {
        var entities = await Queryable
            .Where(m => dto.TypeName == null || dto.TypeName == m.TypeName)
            .Where(m => dto.TypeCode == null || dto.TypeCode == m.TypeCode)
            .OrderBy(m => m.OrderNum)
            .ToListAsync();
        return Mapper.Map<PaginatedList<EquipTypeReadDto>>(entities);
    }
}
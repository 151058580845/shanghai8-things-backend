using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipTypeService : SugarCrudAppService<
    EquipType, Guid,
    EquipTypeReadDto, EquipTypeQueryDto,
    EquipTypeCreateDto, EquipTypeUpdateDto>,
    IEquipTypeService
{
    public override async Task<IEnumerable<EquipTypeReadDto>> GetListAsync(EquipTypeQueryDto? dto)
    {
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(dto.TypeName) ,m => dto.TypeName == m.TypeName)
            .WhereIF(!string.IsNullOrEmpty(dto.TypeCode),m => dto.TypeCode == m.TypeCode)
            .OrderBy(m => m.OrderNum)
            .ToListAsync();
        return Mapper.Map<IEnumerable<EquipTypeReadDto>>(entities);
    }

    public override async Task<PaginatedList<EquipTypeReadDto>> GetPaginatedListAsync(EquipTypeQueryDto dto)
    {
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(dto.TypeName) ,m => dto.TypeName == m.TypeName)
            .WhereIF(!string.IsNullOrEmpty(dto.TypeCode),m => dto.TypeCode == m.TypeCode)
            .OrderBy(m => m.OrderNum)
            .ToPaginatedListAsync(dto.PageIndex, dto.PageSize);
        return Mapper.Map<PaginatedList<EquipTypeReadDto>>(entities);
    }
}
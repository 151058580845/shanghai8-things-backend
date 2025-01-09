using Hgzn.Mes.Application.BaseS;
using Hgzn.Mes.Application.Dtos.Equip;
using Hgzn.Mes.Application.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Hgzn.Mes.Application.Services.Equip;

public class EquipTypeService:CrudAppService<EquipType
    ,EquipTypeReadDto,EquipTypeReadDto,Guid,EquipTypeQueryDto,EquipTypeCreateDto,EquipTypeUpdateDto>,IEquipTypeService
{
    private readonly ApiDbContext _context;
    public EquipTypeService(ApiDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public override async Task<IEnumerable<EquipTypeReadDto>> GetListAsync(EquipTypeQueryDto input)
    {
       var entities = await _context.EquipTypes.Where(m => input.TypeName == null || input.TypeName == m.TypeName)
            .Where(m => input.TypeCode == null || input.TypeCode == m.TypeCode)
            .OrderBy(m => m.OrderNum)
            .ToListAsync();
       return Mapper.Map<IEnumerable<EquipTypeReadDto>>(entities);
    }
}
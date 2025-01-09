using HgznMes.Application.BaseS;
using HgznMes.Application.Dtos.Equip;
using HgznMes.Application.Services.Equip.IService;
using HgznMes.Domain.Entities.Equip.EquipManager;
using HgznMes.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HgznMes.Application.Services.Equip;

public class EquipTypeService:CrudAppService<EquipTypeAggregateRoot
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
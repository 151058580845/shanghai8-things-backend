using Hgzn.Mes.Application.BaseS;
using Hgzn.Mes.Application.Dtos.Base;
using Hgzn.Mes.Application.Dtos.Equip;
using Hgzn.Mes.Application.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Infrastructure.DbContexts;
using Hgzn.Mes.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Hgzn.Mes.Application.Services.Equip;

public class EquipLedgerService:CrudAppService<EquipLedger
    ,EquipLedgerReadDto,EquipLedgerReadDto,Guid,EquipLedgerQueryDto,EquipLedgerCreateDto,EquipLedgerUpdateDto>,IEquipLedgerService
{
    private readonly ApiDbContext _context;

    public EquipLedgerService(ApiDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EquipLedgerReadDto>> GetEquipsListAsync(EquipLedgerQueryDto query)
    {
       var entities = await _context.EquipLedgers
           .Where(m=> query.EquipName == null || m.EquipName.Contains(query.EquipName))
           .Where(m=> query.TypeId == null || m.TypeId.Equals(query.TypeId))
           .Where(m=> query.RoomId == null || m.RoomId.Equals(query.RoomId))
           .Where(m=> query.StartTime == null || m.CreationTime >= query.StartTime)
           .Where(m=> query.EndTime == null || m.CreationTime <= query.EndTime)
           .Include(t=>t.Room)
            .OrderByDescending(m => m.OrderNum)
            .ToPaginatedListAsync(query.PageIndex, query.PageSize);
       return Mapper.Map<IEnumerable<EquipLedgerReadDto>>(entities);
    }

    public async Task<EquipLedger> GetEquipByIpAsync(string ipAddress)
    {
        return await _context.EquipLedgers.FirstAsync(t => t.IpAddress == ipAddress);

    }

    public async Task<IEnumerable<NameValueDto>> GetNameValueListAsync()
    {
        var entities = await _context.EquipLedgers
            .Select(t => new NameValueDto
            {
                Name = t.EquipName,
                Value = t.Id.ToString()
            }).ToListAsync();
        return entities;
    }

    public override Task<IEnumerable<EquipLedgerReadDto>> GetListAsync(EquipLedgerQueryDto input)
    {
        throw new NotImplementedException();
    }
}
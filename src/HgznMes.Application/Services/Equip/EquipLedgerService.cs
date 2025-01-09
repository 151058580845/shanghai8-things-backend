using HgznMes.Application.BaseS;
using HgznMes.Application.Dtos.Base;
using HgznMes.Application.Dtos.Equip;
using HgznMes.Application.Services.Equip.IService;
using HgznMes.Domain.Entities.Equip.EquipManager;
using HgznMes.Infrastructure.DbContexts;
using HgznMes.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace HgznMes.Application.Services.Equip;

public class EquipLedgerService:CrudAppService<EquipLedgerAggregateRoot
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

    public async Task<EquipLedgerAggregateRoot> GetEquipByIpAsync(string ipAddress)
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
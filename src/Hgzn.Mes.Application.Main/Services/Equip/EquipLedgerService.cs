using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipLedgerService : CrudAppServiceSugar<EquipLedger, Guid, EquipLedgerQueryDto,
    EquipLedgerReadDto, EquipLedgerCreateDto,
    EquipLedgerUpdateDto>,
    IEquipLedgerService
{

    public async Task<EquipLedger> GetEquipByIpAsync(string ipAddress)
    {
        return await Queryable().FirstAsync(t => t.IpAddress == ipAddress);
    }

    public async Task<IEnumerable<NameValueDto>> GetNameValueListAsync()
    {
        var entities = await Queryable()
            .Select(t => new NameValueDto
            {
                Id = t.Id,
                Name = t.EquipName,
                Value = t.Id.ToString()
            }).ToListAsync();
        return entities;
    }

    public async Task<IEnumerable<EquipLedgerReadDto>> GetEquipsListAsync(string? equipCode, string? equipName)
    {
        var entities = await DbContext.Queryable<EquipLedger>()
             .WhereIF(!equipCode.IsNullOrEmpty(), t => t.EquipCode == equipCode)
             .WhereIF(!equipName.IsNullOrEmpty(), t => t.EquipName == equipName)
             .ToListAsync();
        return Mapper.Map<IEnumerable<EquipLedgerReadDto>>(entities);
    }

    public override async Task<IEnumerable<EquipLedgerReadDto>> GetListAsync(EquipLedgerQueryDto query)
    {
        var entities = await Queryable()
            .Where(m => query.EquipName == null || m.EquipName.Contains(query.EquipName))
            .Where(m => query.TypeId == null || m.TypeId.Equals(query.TypeId))
            .Where(m => query.RoomId == null || m.RoomId.Equals(query.RoomId))
            .Where(m => query.StartTime == null || m.CreationTime >= query.StartTime)
            .Where(m => query.EndTime == null || m.CreationTime <= query.EndTime)
            .Includes(t => t.Room)
            .OrderByDescending(m => m.OrderNum)
            .ToPageListAsync(query.PageIndex, query.PageSize);
        return Mapper.Map<PaginatedList<EquipLedgerReadDto>>(entities);
    }
}
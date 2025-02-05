using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Utilities;
using Hgzn.Mes.Infrastructure.Utilities;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipLedgerService : SugarCrudAppService<
    EquipLedger, Guid,
    EquipLedgerReadDto, EquipLedgerQueryDto,
    EquipLedgerCreateDto, EquipLedgerUpdateDto>,
    IEquipLedgerService
{
    public async Task<EquipLedger> GetEquipByIpAsync(string ipAddress)
    {
        return await Queryable.FirstAsync(t => t.IpAddress == ipAddress);
    }

    public async Task<IEnumerable<NameValueDto>> GetNameValueListAsync()
    {
        var entities = await Queryable
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
             .WhereIF(!string.IsNullOrEmpty(equipCode), t => t.EquipCode == equipCode)
             .WhereIF(!string.IsNullOrEmpty(equipName), t => t.EquipName == equipName)
             .ToListAsync();
        return Mapper.Map<IEnumerable<EquipLedgerReadDto>>(entities);
    }

    public override async Task<PaginatedList<EquipLedgerReadDto>> GetPaginatedListAsync(EquipLedgerQueryDto query)
    {
        var entities = await Queryable
            .Where(m =>string.IsNullOrEmpty(query.EquipName) || m.EquipName.Contains(query.EquipName))
            .Where(m => query.TypeId.IsGuidEmpty() || m.TypeId.Equals(query.TypeId))
            .Where(m => query.RoomId.IsGuidEmpty() || m.RoomId.Equals(query.RoomId))
            .Where(m => query.StartTime == null || m.CreationTime >= query.StartTime)
            .Where(m => query.EndTime == null || m.CreationTime <= query.EndTime)
            .Includes(t => t.Room)
            .OrderByDescending(m => m.OrderNum)
            .ToPaginatedListAsync(query.PageIndex, query.PageSize);
        return Mapper.Map<PaginatedList<EquipLedgerReadDto>>(entities);
    }

    public override async Task<IEnumerable<EquipLedgerReadDto>> GetListAsync(EquipLedgerQueryDto? query = null)
    {
        var entities = await Queryable
            .Where(m =>query != null && (string.IsNullOrEmpty(query.EquipName) || m.EquipName.Contains(query.EquipName)))
            .Where(m => query != null && (query.TypeId.IsGuidEmpty() || m.TypeId.Equals(query.TypeId)))
            .Where(m => query != null && (query.RoomId.IsGuidEmpty() || m.RoomId.Equals(query.RoomId)))
            .Where(m => query != null && (query.StartTime == null || m.CreationTime >= query.StartTime))
            .Where(m => query != null && (query.EndTime == null || m.CreationTime <= query.EndTime))
            .Includes(t => t.Room)
            .OrderByDescending(m => m.OrderNum)
            .ToListAsync();
        return Mapper.Map<IEnumerable<EquipLedgerReadDto>>(entities);
    }

    public async Task<IEnumerable<RfidEquipReadDto>> GetRfidEquipsListAsync(Guid equipId)
    {
        List<RfidEquipReadDto> list = await DbContext.Queryable<RfidEquipReadDto>().Where(t => t.EquipId == equipId).ToListAsync();
        return list;
    }
}
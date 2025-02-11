using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Utilities;
using Hgzn.Mes.Infrastructure.Utilities;
using StackExchange.Redis;

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
            .WhereIF(!string.IsNullOrEmpty(query.EquipName),m => m.EquipName.Contains(query.EquipName!))
            .WhereIF(!string.IsNullOrEmpty(query.EquipCode),m => m.EquipName.Contains(query.EquipCode!))
            .WhereIF(!query.TypeId.IsGuidEmpty(),m => m.TypeId.Equals(query.TypeId))
            .WhereIF(!query.RoomId.IsGuidEmpty(),m => m.RoomId.Equals(query.RoomId))
            .WhereIF(query.StartTime != null ,m => m.CreationTime >= query.StartTime)
            .WhereIF(query.EndTime != null ,m => m.CreationTime <= query.EndTime)
            .Includes(t => t.Room)
            .OrderByDescending(m => m.OrderNum)
            .ToPaginatedListAsync(query.PageIndex, query.PageSize);
        return Mapper.Map<PaginatedList<EquipLedgerReadDto>>(entities);
    }

    public override async Task<IEnumerable<EquipLedgerReadDto>> GetListAsync(EquipLedgerQueryDto? query = null)
    {
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(query.EquipName),m => m.EquipName.Contains(query.EquipName!))
            .WhereIF(!string.IsNullOrEmpty(query.EquipCode),m => m.EquipName.Contains(query.EquipCode!))
            .WhereIF(!query.TypeId.IsGuidEmpty(),m => m.TypeId.Equals(query.TypeId))
            .WhereIF(!query.RoomId.IsGuidEmpty(),m => m.RoomId.Equals(query.RoomId))
            .WhereIF(query.StartTime != null ,m => m.CreationTime >= query.StartTime)
            .WhereIF(query.EndTime != null ,m => m.CreationTime <= query.EndTime)
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
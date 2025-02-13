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

    public Task<int> UpdateEquipRoomId(Dictionary<string, Guid> equipIds)
    {
        var keys = equipIds.Keys.ToArray();
        var list = Queryable.Where(t => keys.Contains(t.EquipCode)).ToList();
        foreach (var equipLedger in list)
        {
            equipLedger.RoomId = equipIds[equipLedger.EquipCode];   
        }

        return DbContext.Updateable(list).ExecuteCommandAsync();
    }


    /// <summary>
    /// 获取设备列表
    /// </summary>
    /// <param name="equipIds"></param>
    /// <returns></returns>
    public async Task<IEnumerable<EquipLedgerReadDto>> GetEquipsListInIdsAsync(List<Guid> equipIds)
    {
        var entities = await Queryable.Where(t => equipIds.Contains(t.Id)).ToListAsync();
        return Mapper.Map<IEnumerable<EquipLedgerReadDto>>(entities);
    }

    /// <summary>
    /// 获取待搜索记录
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<EquipLedgerSearchReadDto>> GetAppSearchAsync()
    {
        var entities = await Queryable.Where(t => t.State == false).Includes(t=>t.EquipType)
            .Select<EquipLedgerSearchReadDto>(t=>new EquipLedgerSearchReadDto()
            {
                Id = t.Id,
                EquipCode = t.EquipCode,
                EquipName = t.EquipName,
                TypeId = t.TypeId,
                TypeName = t.EquipType!.TypeName,
                Model = t.Model,
                RoomId = t.RoomId,
                AssetNumber = t.AssetNumber
            }).ToListAsync();
        return entities;
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
            .Includes(t=>t.EquipType)
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
            .Includes(t=>t.EquipType)
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
using Hgzn.Mes.Application.BaseS;
using Hgzn.Mes.Application.Dtos.Base;
using Hgzn.Mes.Application.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services;
using Hgzn.Mes.Application.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Infrastructure.DbContexts;
using Hgzn.Mes.Infrastructure.SqlSugarContext;
using Hgzn.Mes.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;
using SqlSugar;

namespace Hgzn.Mes.Application.Services.Equip;

public class EquipLedgerService : CrudAppServiceSugar<EquipLedger
        , EquipLedgerReadDto, EquipLedgerReadDto, Guid, EquipLedgerQueryDto, EquipLedgerCreateDto,
        EquipLedgerUpdateDto>,
    IEquipLedgerService
{
    public EquipLedgerService(SqlSugarContext context) : base(context)
    {
    }

    public async Task<IEnumerable<EquipLedgerReadDto>> GetEquipsListAsync(EquipLedgerQueryDto query)
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
        return Mapper.Map<IEnumerable<EquipLedgerReadDto>>(entities);
    }

    public async Task<EquipLedger> GetEquipByIpAsync(string ipAddress)
    {
        return await Queryable().FirstAsync(t => t.IpAddress == ipAddress);
    }

    public async Task<IEnumerable<NameValueDto>> GetNameValueListAsync()
    {
        var entities = await Queryable()
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
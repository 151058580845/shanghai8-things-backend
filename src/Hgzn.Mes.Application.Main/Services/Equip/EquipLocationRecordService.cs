using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.Equip;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Extensions;
using Hgzn.Mes.Domain.Shared.Utilities;
using Hgzn.Mes.Infrastructure.Utilities;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipLocationRecordService : SugarCrudAppService<
        EquipLocationRecord, Guid,
        EquipLocationRecordReadDto, EquipLocationRecordQueryDto>,
    IEquipLocationRecordService
{
    public override async Task<PaginatedList<EquipLocationRecordReadDto>> GetPaginatedListAsync(EquipLocationRecordQueryDto queryDto)
    {
        var guid = !string.IsNullOrEmpty(queryDto.EquipId) ? Guid.Parse(queryDto.EquipId) : Guid.Empty;
        var guid_room = !string.IsNullOrEmpty(queryDto.RoomId) ? Guid.Parse(queryDto.RoomId) : Guid.Empty;
        var entities = await Queryable.WhereIF(guid != Guid.Empty, x => x.EquipId == guid)
                .WhereIF(guid_room != Guid.Empty, x => x.RoomId == guid_room)
                .WhereIF(queryDto.StartDateTime != null && queryDto.EndDateTime != null, x => x.DateTime >= queryDto.StartDateTime && x.DateTime <= queryDto.EndDateTime)
                .OrderByDescending(x => x.DateTime)
                .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
        return Mapper.Map<PaginatedList<EquipLocationRecordReadDto>>(entities);
    }

    /// <summary>
    /// 获取设备台账分页列表（用于Rfid跟踪历史页面）
    /// 此方法复制自 EquipLedgerService.GetPaginatedListAsync，避免修改原方法影响其他功能
    /// 注意：此接口会排除读写器(RfidReaderType)和发卡器(RfidIssuerType)类型的设备
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>设备台账分页列表</returns>
    public async Task<PaginatedList<EquipLedgerReadDto>> GetEquipLedgerPaginatedListAsync(EquipLedgerQueryDto query)
    {
        var queryable = DbContext.Queryable<EquipLedger>()
            .Where(m => !string.IsNullOrEmpty(m.EquipName) && !string.IsNullOrEmpty(m.AssetNumber)) // 过滤没有设备名称或资产编号的记录
            .Where(m => m.TypeId != EquipType.RfidIssuerType.Id && m.TypeId != EquipType.RfidReaderType.Id) // 排除读写器和发卡器
            .WhereIF(!string.IsNullOrEmpty(query.EquipCode), m => m.EquipCode.Contains(query.EquipCode!))
            .WhereIF(!string.IsNullOrEmpty(query.AssetNumber), m => m.AssetNumber!.Contains(query.AssetNumber))
            .WhereIF(query.ResponsibleUserId is not null, m => m.ResponsibleUserId.Equals(query.ResponsibleUserId))
            .WhereIF(!string.IsNullOrEmpty(query.Query),
                m => m.EquipName.Contains(query.Query!) || m.Model!.Contains(query.Query!))
            .WhereIF(!query.TypeId.IsNullableGuidEmpty(), m => m.TypeId.Equals(query.TypeId))
            .WhereIF(query.NoRfidDevice == true, m => m.TypeId == null ||
                                                      (m.TypeId != EquipType.RfidIssuerType.Id &&
                                                       m.TypeId != EquipType.RfidReaderType.Id))
            .WhereIF(!query.RoomId.IsNullableGuidEmpty(), m => m.RoomId.Equals(query.RoomId))
            .WhereIF(query.StartTime != null, m => m.CreationTime >= query.StartTime)
            .WhereIF(query.EndTime != null, m => m.CreationTime <= query.EndTime)
            .WhereIF(query.State != null, m => m.State == query.State);

        if (query.BindingTagCount is not null)
        {
            queryable = queryable.Includes(eq => eq.Labels);
            queryable = query.BindingTagCount == -1
                ? queryable.Where(eq => SqlFunc.Subqueryable<LocationLabel>()
                    .Where(l => l.EquipLedgerId == eq.Id)
                    .Count() > 0)
                : queryable.Where(eq => SqlFunc.Subqueryable<LocationLabel>()
                    .Where(l => l.EquipLedgerId == eq.Id)
                    .Count() == 0);
        }

        var entities = await queryable
            .Includes(t => t.Room)
            .Includes(t => t.EquipType)
            .OrderByDescending(m => m.OrderNum)
            .OrderByDescending(m => m.CreationTime)
            .ToPaginatedListAsync(query.PageIndex, query.PageSize);
        return Mapper.Map<PaginatedList<EquipLedgerReadDto>>(entities);
    }

    #region
    public Task<int> DeleteAsync(Guid key)
    {
        throw new NotImplementedException();
    }

    public Task<EquipLocationRecordReadDto?> GetAsync(Guid key)
    {
        throw new NotImplementedException();
    }

    public Task<EquipLocationRecordReadDto?> UpdateStateAsync(Guid key, bool state)
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<EquipLocationRecordReadDto>> GetListAsync(EquipLocationRecordQueryDto? queryDto = null)
    {
        throw new NotImplementedException();
    }
    #endregion
}

using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;

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

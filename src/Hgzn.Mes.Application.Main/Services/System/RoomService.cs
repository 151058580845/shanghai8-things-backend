using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Location;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Utilities;
using Hgzn.Mes.Infrastructure.Utilities;
using Newtonsoft.Json;

namespace Hgzn.Mes.Application.Main.Services.System;

public class RoomService : SugarCrudAppService<
        Room, Guid,
        RoomReadDto, RoomQueryDto,
        RoomCreateDto, RoomUpdateDto>,
    IRoomService
{
    public override async Task<IEnumerable<RoomReadDto>> GetListAsync(RoomQueryDto? queryDto = null)
    {
        var entities = await Queryable
            .WhereIF(!queryDto.ParentId.IsGuidEmpty(),x=> x.ParentId == queryDto.ParentId)
            .WhereIF(queryDto != null && !string.IsNullOrEmpty(queryDto.Name), x=> x.Name.Contains(queryDto!.Name!))      
            // .Includes(t=>t.Rooms)
            .OrderBy(x=>x.OrderNum)
            .ToListAsync();
        return Mapper.Map<IEnumerable<Room>, IEnumerable<RoomReadDto>>(entities);
    }

    public override async Task<PaginatedList<RoomReadDto>> GetPaginatedListAsync(RoomQueryDto queryDto)
    {
        var entities = await Queryable
            .WhereIF(!queryDto.ParentId.IsGuidEmpty(),x=> x.ParentId == queryDto.ParentId)
            .WhereIF(!string.IsNullOrEmpty(queryDto.Name), x=> x.Name!.Contains(queryDto.Name!))
            // .Includes(t=>t.Rooms)
            .OrderBy(x=>x.OrderNum)
            .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
        return Mapper.Map<PaginatedList<Room>, PaginatedList<RoomReadDto>>(entities);
    }

    public override async Task<RoomReadDto?> GetAsync(Guid key)
    {
        var entity = await base.GetAsync(key);
        var qrCode = Mapper.Map<RoomQrCode>(entity);
        //生成二维码
        entity.QrCode = await QrCodeHelper.GetOrCreateQrCode(entity.Id.ToString(), JsonConvert.SerializeObject(qrCode), "Room");
        return entity;
    }
}
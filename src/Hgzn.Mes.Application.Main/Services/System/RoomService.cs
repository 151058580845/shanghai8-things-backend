using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Dictionary;
using Hgzn.Mes.Domain.Entities.System.Location;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Extensions;
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
            .WhereIF(!queryDto.ParentId.IsGuidEmpty(), x => x.ParentId == queryDto.ParentId)
            .WhereIF(queryDto != null && !string.IsNullOrEmpty(queryDto.Name), x => x.Name.Contains(queryDto!.Name!))
            // .Includes(t=>t.Rooms)
            .OrderBy(x => x.OrderNum)
            .ToListAsync();
        return Mapper.Map<IEnumerable<Room>, IEnumerable<RoomReadDto>>(entities);
    }

    public override async Task<PaginatedList<RoomReadDto>> GetPaginatedListAsync(RoomQueryDto queryDto)
    {
        var entities = await Queryable
            .WhereIF(!queryDto.ParentId.IsGuidEmpty(), x => x.ParentId == queryDto.ParentId)
            .WhereIF(!string.IsNullOrEmpty(queryDto.Name), x => x.Name!.Contains(queryDto.Name!))
            // .Includes(t=>t.Rooms)
            .OrderBy(x => x.OrderNum)
            .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
        return Mapper.Map<PaginatedList<Room>, PaginatedList<RoomReadDto>>(entities);
    }

    public async Task<IEnumerable<NameValueListDto>> GetRoomListAllAsync(RoomQueryDto? input)
    {
        var list1 = await DbContext.Queryable<Building>().Select<NameValueListDto>(t => new NameValueListDto()
        {
            Id = t.Id,
            Name = t.Name,
            Value = t.Code,
        }).ToListAsync();
        var list2 = await DbContext.Queryable<Floor>().Select<NameValueListDto>(t => new NameValueListDto()
        {
            Id = t.Id,
            Name = t.Name,
            Value = t.Code,
            ParentId = t.ParentId
        }).ToListAsync();
        var list3 = await DbContext.Queryable<Room>().Select<NameValueListDto>(t => new NameValueListDto()
        {
            Id = t.Id,
            Name = t.Name,
            Value = t.Code,
            ParentId = t.ParentId
        }).ToListAsync();
        foreach (var floor in list2)
        {
            floor.Children.AddRange(await list3.Where(t => t.ParentId == floor.Id).ToListAsync());
        }

        foreach (var build in list1)
        {
            build.Children.AddRange(await list2.Where(t => t.ParentId == build.Id).ToListAsync());
        }

        return list1;
    }

    public override async Task<RoomReadDto?> GetAsync(Guid key)
    {
        var entity = await base.GetAsync(key);
        var qrCode = Mapper.Map<RoomQrCode>(entity);
        //生成二维码
        entity.QrCode =
            await QrCodeHelper.GetOrCreateQrCode(entity.ToString(), JsonConvert.SerializeObject(qrCode), "Room");
        return entity;
    }
}
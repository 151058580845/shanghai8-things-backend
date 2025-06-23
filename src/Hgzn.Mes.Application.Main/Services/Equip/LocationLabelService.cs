using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enum;
using Hgzn.Mes.Domain.Shared.Exceptions;
using Hgzn.Mes.Infrastructure.Utilities;

namespace Hgzn.Mes.Application.Main.Services.Equip
{
    public class LocationLabelService :
        SugarCrudAppService<LocationLabel, Guid,
        LocationLabelReadDto, LocationLabelQueryDto,
        LocationLabelCreateDto, LocationLabelUpdateDto>,
        ILocationLabelService
    {
        public override async Task<IEnumerable<LocationLabelReadDto>> GetListAsync(LocationLabelQueryDto? queryDto = null)
        {
            var query = queryDto is null ? Queryable : Queryable
                .Where(ll => ll.Type == queryDto.LabelType)
                .WhereIF(!string.IsNullOrEmpty(queryDto?.TagId), ll => queryDto!.TagId == ll.TagId)
                .Includes(ll => ll.EquipLedger)
                .Includes(ll => ll.Room)
                .WhereIF(!string.IsNullOrEmpty(queryDto?.AssetNumber), ll => ll.EquipLedger!.AssetNumber == queryDto!.AssetNumber)
                .WhereIF(!string.IsNullOrEmpty(queryDto?.Query),
                    ll => ll.EquipLedger!.EquipName.Contains(queryDto!.Query!) ||
                    ll.EquipLedger!.Model!.Contains(queryDto!.Query!));

            var entities = await query
                .OrderByDescending(m => m.CreationTime)
                .ToArrayAsync();
            return Mapper.Map<IEnumerable<LocationLabelReadDto>>(entities);
        }

        public override async Task<PaginatedList<LocationLabelReadDto>> GetPaginatedListAsync(LocationLabelQueryDto queryDto)
        {
            var entities = await Queryable
                .Where(ll => ll.Type == queryDto.LabelType)
                .WhereIF(!string.IsNullOrEmpty(queryDto?.TagId), ll => queryDto!.TagId == ll.TagId)
                .Includes(ll => ll.EquipLedger)
                .Includes(ll => ll.Room)
                .WhereIF(!string.IsNullOrEmpty(queryDto?.AssetNumber), ll => ll.EquipLedger!.AssetNumber == queryDto!.AssetNumber)
                .WhereIF(!string.IsNullOrEmpty(queryDto?.Query), 
                    ll => ll.EquipLedger!.EquipName.Contains(queryDto!.Query!) ||
                    ll.EquipLedger!.Model!.Contains(queryDto!.Query!))
                .OrderByDescending(m => m.CreationTime)
                .ToPaginatedListAsync(queryDto!.PageIndex, queryDto.PageSize); 
            return Mapper.Map<PaginatedList<LocationLabelReadDto>>(entities);
        }

        public async Task<int> DeleteRangesAsync(IEnumerable<Guid> ids)
        {
            var count = await DbContext.Deleteable<LocationLabel>()
                .Where(ll => ids.Contains(ll.Id))
                .ExecuteCommandAsync();
            return count;
        }
        public async Task<PaginatedList<EquipLocationLabelReadDto>> GetEquipLabelAsync(int pageIndex, int pageSize)
        {
            var dtos = await DbContext.Queryable<LocationLabel>()
                .Where(ll => ll.Type == LabelType.Equip)
                .Includes(ll => ll.EquipLedger, el => el!.EquipType)
                .OrderByDescending(ll => ll.LastModificationTime)
                .Select(ll => new EquipLocationLabelReadDto
                {
                    Id = ll.Id,
                    AssetNumber = ll.EquipLedger == null ? null : ll.EquipLedger.AssetNumber,
                    EquipName = ll.EquipLedger == null ? null : ll.EquipLedger.EquipName,
                    Model = ll.EquipLedger == null ? null : ll.EquipLedger.Model,
                    EquipTypeName = (ll.EquipLedger == null || ll.EquipLedger.EquipType == null) ? null : ll.EquipLedger.EquipType.TypeName,
                    Tid = ll.TagId
                })
                .ToPaginatedListAsync(pageIndex, pageSize);
            return dtos;
        }

        public async Task<IEnumerable<EquipLocationLabelReadDto>> FindEquipLabelAsync(Guid equipId)
        {
            var dtos = await DbContext.Queryable<LocationLabel>()
                .Where(ll => ll.EquipLedgerId == equipId)
                .Includes(ll => ll.EquipLedger, el => el!.EquipType)
                .OrderByDescending(ll => ll.LastModificationTime)
                .Select(ll => new EquipLocationLabelReadDto
                {
                    Id = ll.Id,
                    AssetNumber = ll.EquipLedger!.AssetNumber,
                    EquipName = ll.EquipLedger.EquipName,
                    Model =  ll.EquipLedger.Model,
                    EquipTypeName = ll.EquipLedger.EquipType!.TypeName,
                    Tid = ll.TagId
                })
                .ToArrayAsync();
            return dtos;
        }

        public async Task<PaginatedList<RoomLocationLabelReadDto>> GetRoomLabelAsync(int pageIndex, int pageSize)
        {
            var dtos = await DbContext.Queryable<LocationLabel>()
                .Where(ll => ll.Type == LabelType.Room)
                .Includes(ll => ll.Room)
                .OrderByDescending(ll => ll.LastModificationTime)
                .Select(ll => new RoomLocationLabelReadDto
                {
                    RoomId = ll.RoomId,
                    RoomName = ll.EquipLedger == null ? null : ll.EquipLedger.AssetNumber,
                    RoomCode = ll.EquipLedger == null ? null : ll.EquipLedger.EquipName,
                    Tid = ll.TagId
                })
                .ToPaginatedListAsync(pageIndex, pageSize);
            return dtos;
        }

        public async Task<int> BindingLabelsAsync(BindingLabelDto dto)
        {
            if(await Queryable.AnyAsync(ll => dto.Tids.Contains(ll.TagId)))
            {
                throw new NotAcceptableException("tag existes");
            }
            var entities = dto.Tids.Select(bl => new LocationLabel
            {
                Type = dto.LabelType,
                TagId = bl,
                EquipLedgerId = dto.EquipLedgerId,
                RoomId = dto.RoomId,
            }).ToArray();
            var index = await DbContext.Insertable(entities).ExecuteCommandAsync();
            return index;
        }
    }
}

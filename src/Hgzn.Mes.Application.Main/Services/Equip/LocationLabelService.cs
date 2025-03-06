using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enum;
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
            var entities = await Queryable
                .WhereIF(!string.IsNullOrEmpty(queryDto?.TagId), ll => queryDto!.TagId == ll.TagId)
                .OrderBy(m => m.CreationTime)
                .ToArrayAsync();
            return Mapper.Map<IEnumerable<LocationLabelReadDto>>(entities);
        }

        public override async Task<PaginatedList<LocationLabelReadDto>> GetPaginatedListAsync(LocationLabelQueryDto queryDto)
        {
            var entities = await Queryable
                .Where(ll => ll.Type == queryDto.LabelType)
                .WhereIF(!string.IsNullOrEmpty(queryDto?.TagId), ll => queryDto!.TagId == ll.TagId)
                .Includes(ll => ll.EquipLedger)
                .WhereIF(!string.IsNullOrEmpty(queryDto?.Query), ll => ll.EquipLedger!.EquipName.Contains(queryDto!.Query!))
                .OrderBy(m => m.CreationTime)
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
            var entities = dto.Tids.Select(bl => new LocationLabel
            {
                Type = dto.LabelType,
                TagId = bl,
                EquipLedgerId = dto.EquipLedgerId,
                RoomId = dto.RoomId,
            });
            return await DbContext.Insertable<LocationLabel>(entities).ExecuteCommandAsync();
        }
    }
}

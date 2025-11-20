using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.System.Location;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enum;
using Hgzn.Mes.Domain.Shared.Exceptions;
using Hgzn.Mes.Infrastructure.Utilities;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services.Equip
{
    public class LocationLabelService :
        SugarCrudAppService<LocationLabel, Guid,
            LocationLabelReadDto, LocationLabelQueryDto,
            LocationLabelCreateDto, LocationLabelUpdateDto>,
        ILocationLabelService
    {
        public override async Task<IEnumerable<LocationLabelReadDto>> GetListAsync(
            LocationLabelQueryDto? queryDto = null)
        {
            var query = queryDto is null
                ? Queryable
                : Queryable
                    .Where(ll => ll.Type == queryDto.LabelType)
                    .WhereIF(!string.IsNullOrEmpty(queryDto?.TagId), ll => queryDto!.TagId == ll.TagId)
                    .Includes(ll => ll.EquipLedger)
                    .Includes(ll => ll.Room)
                    .WhereIF(queryDto?.FilterEquipType == true,
                        ll => ll.EquipLedger!.TypeId != EquipType.RfidIssuerType.Id &&
                              ll.EquipLedger!.TypeId != EquipType.RfidReaderType.Id)
                    .WhereIF(!string.IsNullOrEmpty(queryDto?.AssetNumber),
                        ll => ll.EquipLedger!.AssetNumber == queryDto!.AssetNumber)
                    .WhereIF(!string.IsNullOrEmpty(queryDto?.Query),
                        ll => ll.EquipLedger!.EquipName.Contains(queryDto!.Query!) ||
                              ll.EquipLedger!.Model!.Contains(queryDto!.Query!));

            var entities = await query
                .OrderByDescending(m => m.CreationTime)
                .ToArrayAsync();
            return Mapper.Map<IEnumerable<LocationLabelReadDto>>(entities);
        }

        public async Task<IEnumerable<LocationLabelReadDto>> QueryByDeviceTypes(IEnumerable<Guid>? typeIds)
        {
            var targets = (typeIds ??
                           await DbContext.Queryable<EquipType>()
                               .Where(et => et.Id != EquipType.RfidIssuerType.Id &&
                                            et.Id != EquipType.RfidReaderType.Id)
                               .Select(et => et.Id)
                               .ToArrayAsync()).ToList();
            var entities = await Queryable
                .Includes(ll => ll.EquipLedger)
                .WhereIF(targets is not null && targets.Count != 0,
                    ll => targets!.Contains(ll.EquipLedger!.TypeId!.Value))
                .ToArrayAsync();
            return Mapper.Map<IEnumerable<LocationLabelReadDto>>(entities);
        }

        public async Task<PaginatedList<EquipLedgerSearchReadDto>> GetEquipBindLabelAsync(int pageIndex = 1, int pageSize = 100)
        {
           var dtos =  await DbContext.Queryable<EquipLedger>().Includes(eq => eq.Labels)
                .Where(eq => SqlFunc.Subqueryable<LocationLabel>()
                    .Where(l => l.EquipLedgerId == eq.Id)
                    .Count() > 0)
                .Select(ll => new EquipLedgerSearchReadDto
                {
                    AssetNumber = ll.AssetNumber,
                    EquipName = ll.EquipName,
                    Model = ll.Model,
                    ResponsibleUserId = ll.ResponsibleUserId,
                    ResponsibleUserName = ll.ResponsibleUserName
                })
                .Distinct()
                .ToPaginatedListAsync(pageIndex, pageSize);
            return dtos;
        }

        public override async Task<PaginatedList<LocationLabelReadDto>> GetPaginatedListAsync(
            LocationLabelQueryDto queryDto)
        {
            // 先使用子查询获取符合条件的 LocationLabel ID
            var labelIdsQuery = DbContext.Queryable<LocationLabel>()
                .Where(ll => ll.Type == queryDto.LabelType)
                .WhereIF(!string.IsNullOrEmpty(queryDto?.TagId), ll => queryDto!.TagId == ll.TagId);
            
            // 根据标签类型决定搜索字段
            if (!string.IsNullOrEmpty(queryDto?.Query))
            {
                if (queryDto.LabelType == LabelType.Room)
                {
                    // 房间标签：搜索房间名称和房间编号
                    var roomIds = await DbContext.Queryable<Room>()
                        .Where(r => (r.Name != null && r.Name.Contains(queryDto.Query!)) ||
                                   (r.Code != null && r.Code.Contains(queryDto.Query!)))
                        .Select(r => r.Id)
                        .ToListAsync();
                    labelIdsQuery = labelIdsQuery.Where(ll => ll.RoomId != null && roomIds.Contains(ll.RoomId.Value));
                }
                else
                {
                    // 设备标签：搜索设备名称和型号
                    var equipIds = await DbContext.Queryable<EquipLedger>()
                        .Where(e => (e.EquipName != null && e.EquipName.Contains(queryDto.Query!)) ||
                                   (e.Model != null && e.Model.Contains(queryDto.Query!)))
                        .Select(e => e.Id)
                        .ToListAsync();
                    labelIdsQuery = labelIdsQuery.Where(ll => ll.EquipLedgerId != null && equipIds.Contains(ll.EquipLedgerId.Value));
                }
            }
            
            // 获取符合条件的 ID 列表
            var labelIds = await labelIdsQuery.Select(ll => ll.Id).ToListAsync();
            
            // 根据其他条件进一步过滤
            if (queryDto?.FilterEquipType == true || !string.IsNullOrEmpty(queryDto?.AssetNumber))
            {
                var equipFilterIds = await DbContext.Queryable<LocationLabel>()
                    .LeftJoin<EquipLedger>((ll, equip) => ll.EquipLedgerId == equip.Id)
                    .Where((ll, equip) => labelIds.Contains(ll.Id))
                    .WhereIF(queryDto?.FilterEquipType == true,
                        (ll, equip) => equip.TypeId == null ||
                              (equip.TypeId != EquipType.RfidIssuerType.Id &&
                               equip.TypeId != EquipType.RfidReaderType.Id))
                    .WhereIF(!string.IsNullOrEmpty(queryDto?.AssetNumber),
                        (ll, equip) => equip.AssetNumber == queryDto!.AssetNumber)
                    .Select((ll, equip) => ll.Id)
                    .ToListAsync();
                labelIds = labelIds.Where(id => equipFilterIds.Contains(id)).ToList();
            }
            
            // 使用 ID 列表查询完整数据，包含关联数据
            var entities = await Queryable
                .Where(ll => labelIds.Contains(ll.Id))
                .Includes(ll => ll.EquipLedger)
                .Includes(ll => ll.Room)
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
                    AssetNumber = ll.EquipLedger.AssetNumber,
                    EquipName = ll.EquipLedger.EquipName,
                    Model = ll.EquipLedger.Model,
                    EquipTypeName = ll.EquipLedger.EquipType.TypeName,
                    Tid = ll.TagId,
                    ResponsibleUserId = ll.EquipLedger.ResponsibleUserId,
                    ResponsibleUserName = ll.EquipLedger.ResponsibleUserName
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
                    Model = ll.EquipLedger.Model,
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
                    RoomName = ll.Room.Name,
                    RoomCode = ll.Room.Code,
                    Tid = ll.TagId
                })
                .ToPaginatedListAsync(pageIndex, pageSize);
            return dtos;
        }

        public async Task<int> BindingLabelsAsync(BindingLabelDto dto)
        {
            if (await Queryable.AnyAsync(ll => dto.Tids.Contains(ll.TagId)))
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
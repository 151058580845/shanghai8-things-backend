using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip;
using Hgzn.Mes.Domain.Shared;
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
                .WhereIF(!string.IsNullOrEmpty(queryDto?.TagId), ll => queryDto!.TagId == ll.TagId)
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
    }
}

using Hgzn.Mes.Application.Main.Dtos.Basic;
using Hgzn.Mes.Application.Main.Services.Basic.IService;
using Hgzn.Mes.Domain.Entities.Basic;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;

namespace Hgzn.Mes.Application.Main.Services.Basic
{
    public class UnitService : SugarCrudAppService<
    Unit, Guid,
    UnitReadDto, UnitQueryDto,
    UnitCreateDto, UnitUpdateDto
        >, IUnitService
    {

        public async override Task<IEnumerable<UnitReadDto>> GetListAsync(UnitQueryDto? queryDto = null)
        {
            var users = await Queryable
          .WhereIF(!string.IsNullOrEmpty(queryDto.Code), x => x.Code.Contains(queryDto.Code))
          .WhereIF(!string.IsNullOrEmpty(queryDto.Name), x => x.Name.Contains(queryDto.Name))
           .OrderBy(x => x.CreationTime)
           .ToArrayAsync();

            return Mapper.Map<IEnumerable<UnitReadDto>>(users);
        }

        public async override Task<PaginatedList<UnitReadDto>> GetPaginatedListAsync(UnitQueryDto input)
        {
            var entities = await Queryable
         .WhereIF(!string.IsNullOrEmpty(input.Code), x => x.Code.Contains(input.Code))
          .WhereIF(!string.IsNullOrEmpty(input.Name), x => x.Name.Contains(input.Name))
           .OrderBy(x => x.CreationTime)
           .ToPaginatedListAsync(input.PageIndex, input.PageSize);
            return Mapper.Map<PaginatedList<UnitReadDto>>(entities);
        }
    }
}

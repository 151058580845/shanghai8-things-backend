using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Config;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;

namespace Hgzn.Mes.Application.Main.Services.System
{
    public class BaseConfigService : SugarCrudAppService<
            BaseConfig, Guid,
            BaseConfigReadDto, BaseConfigQueryDto,
            BaseConfigCreateDto, BaseConfigUpdateDto>,
        IBaseConfigService
    {
        public override async Task<IEnumerable<BaseConfigReadDto>> GetListAsync(BaseConfigQueryDto? queryDto = null)
        {
            var entities = await Queryable
                .WhereIF(!string.IsNullOrEmpty(queryDto?.ConfigName), x => x.ConfigName.Contains(queryDto!.ConfigName!))
                .WhereIF(!string.IsNullOrEmpty(queryDto?.ConfigKey), x => x.ConfigKey.Contains(queryDto!.ConfigKey!))
                .ToListAsync();

            return Mapper.Map<IEnumerable<BaseConfig>, IEnumerable<BaseConfigReadDto>>(entities);
        }

        public override async Task<PaginatedList<BaseConfigReadDto>> GetPaginatedListAsync(BaseConfigQueryDto queryDto)
        {
            var entities = await Queryable
                .WhereIF(!string.IsNullOrEmpty(queryDto.ConfigName), x => x.ConfigName.Contains(queryDto!.ConfigName!))
                .WhereIF(!string.IsNullOrEmpty(queryDto?.ConfigKey), x => x.ConfigKey.Contains(queryDto!.ConfigKey!))
                .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);

            return Mapper.Map<PaginatedList<BaseConfig>, PaginatedList<BaseConfigReadDto>>(entities);
        }

        public async Task<string?> GetValueByKeyAsync(string key)
        {
            var entity = await Queryable.FirstAsync(t => t.ConfigKey == key);
            return entity?.ConfigValue;
        }
    }
}
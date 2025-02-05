using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.Equip
{
    public class CollectionConfigService : SugarCrudAppService<
    CollectionConfig, Guid,
    CollectionConfigReadDto, CollectionConfigQueryDto,
    CollectionConfigCreateDto, CollectionConfigUpdateDto>,
    ICollectionConfigService
    {
        public async override Task<IEnumerable<CollectionConfigReadDto>> GetListAsync(CollectionConfigQueryDto? queryDto = null)
        {
            RefAsync<int> total = 0;
            List<CollectionConfig> entities = await Queryable
                .ToListAsync();
            return Mapper.Map<IEnumerable<CollectionConfigReadDto>>(entities);
        }

        public async override Task<PaginatedList<CollectionConfigReadDto>> GetPaginatedListAsync(CollectionConfigQueryDto queryDto)
        {
            RefAsync<int> total = 0;
            PaginatedList<CollectionConfig> entities = await Queryable
                .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
            return Mapper.Map<PaginatedList<CollectionConfigReadDto>>(entities);
        }
    }
}

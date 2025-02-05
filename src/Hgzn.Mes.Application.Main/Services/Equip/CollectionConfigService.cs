using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
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
        public override Task<IEnumerable<CollectionConfigReadDto>> GetListAsync(CollectionConfigQueryDto? queryDto = null)
        {
            throw new NotImplementedException();
        }

        public async override Task<PaginatedList<CollectionConfigReadDto>> GetPaginatedListAsync(CollectionConfigQueryDto queryDto)
        {
            RefAsync<int> total = 0;
            List<CollectionConfig> entities = await Queryable
                .ToPageListAsync(queryDto.PageIndex, queryDto.PageSize, total);
            List<CollectionConfigReadDto> map = Mapper.Map<List<CollectionConfigReadDto>>(entities);
            return new PaginatedList<CollectionConfigReadDto>(map, total, queryDto.PageIndex, queryDto.PageSize);
        }
    }
}

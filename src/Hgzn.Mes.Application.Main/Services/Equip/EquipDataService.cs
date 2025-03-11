using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Utilities;
using Hgzn.Mes.Infrastructure.Utilities;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.Equip
{
    public class EquipDataService : SugarCrudAppService<
    EquipData, Guid,
    EquipDataReadDto, EquipDataQueryDto,
    EquipDataCreateDto, EquipDataUpdateDto>,
    IEquipDataService
    {
        public async override Task<IEnumerable<EquipDataReadDto>> GetListAsync(EquipDataQueryDto? queryDto = null)
        {
            RefAsync<int> total = 0;
            var entities = await Queryable.WhereIF(queryDto != null && !queryDto.TypeId.IsNullableGuidEmpty(), t => t.TypeId == queryDto.TypeId)
                .ToListAsync();
            return Mapper.Map<IEnumerable<EquipDataReadDto>>(entities);
        }

        public async override Task<PaginatedList<EquipDataReadDto>> GetPaginatedListAsync(EquipDataQueryDto queryDto)
        {
            RefAsync<int> total = 0;
            var entities = await Queryable.WhereIF(!queryDto.TypeId.IsNullableGuidEmpty(), t => t.TypeId == queryDto.TypeId)
                .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
            return Mapper.Map<PaginatedList<EquipDataReadDto>>(entities);
        }
    }
}

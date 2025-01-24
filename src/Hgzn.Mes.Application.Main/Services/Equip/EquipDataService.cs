using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Utilities;
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
        public override Task<IEnumerable<EquipDataReadDto>> GetListAsync(EquipDataQueryDto? queryDto = null)
        {
            throw new NotImplementedException();
        }

        public async override Task<PaginatedList<EquipDataReadDto>> GetPaginatedListAsync(EquipDataQueryDto queryDto)
        {
            RefAsync<int> total = 0;
            var entities = await Queryable.WhereIF(!queryDto.TypeId.IsGuidEmpty(), t => t.TypeId == queryDto.TypeId)
                .ToPageListAsync(queryDto.PageIndex, queryDto.PageSize, total);
            List<EquipDataReadDto> map = Mapper.Map<List<EquipDataReadDto>>(entities);
            return new PaginatedList<EquipDataReadDto>(map, total, queryDto.PageIndex, queryDto.PageSize);
        }
    }
}

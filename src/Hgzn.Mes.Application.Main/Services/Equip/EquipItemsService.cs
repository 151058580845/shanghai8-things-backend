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
    public class EquipItemsService : SugarCrudAppService<
    EquipItems, Guid,
    EquipItemsReadDto, EquipItemsQueryDto,
    EquipItemsCreateDto, EquipItemsUpdateDto>,
    IEquipItemsService
    {
        public async Task<EquipItemsEnumReadDto> GetEnumsAsync()
        {
            return await Task.FromResult(new EquipItemsEnumReadDto()
            {
                EquipMaintenanceType = EnumHelper.GetEnumNames<EquipMaintenanceType>(),
            });
        }

        public async override Task<IEnumerable<EquipItemsReadDto>> GetListAsync(EquipItemsQueryDto? queryDto = null)
        {
            throw new NotImplementedException();
        }

        public async override Task<PaginatedList<EquipItemsReadDto>> GetPaginatedListAsync(EquipItemsQueryDto queryDto)
        {
            RefAsync<int> total = 0;
            var entities = await Queryable
                .WhereIF(!string.IsNullOrEmpty(queryDto.ItemCode), x => x.ItemCode.Contains(queryDto.ItemCode))
                .WhereIF(!string.IsNullOrEmpty(queryDto.ItemName), x => x.ItemName.Contains(queryDto.ItemName))
                .ToListAsync();

            List<EquipItemsReadDto> map = Mapper.Map<List<EquipItemsReadDto>>(entities);
            return new PaginatedList<EquipItemsReadDto>(map, total, queryDto.PageIndex, queryDto.PageSize);
        }
    }
}

using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;
using SqlSugar;

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
            RefAsync<int> total = 0;
            List<EquipItems> entities = await Queryable
                .WhereIF(queryDto != null && !string.IsNullOrEmpty(queryDto.ItemCode), x => x.ItemCode.Contains(queryDto.ItemCode))
                .WhereIF(queryDto != null && !string.IsNullOrEmpty(queryDto.ItemName), x => x.ItemName.Contains(queryDto.ItemName))
                .ToListAsync();

            return Mapper.Map<IEnumerable<EquipItemsReadDto>>(entities);
        }

        public async override Task<PaginatedList<EquipItemsReadDto>> GetPaginatedListAsync(EquipItemsQueryDto queryDto)
        {
            RefAsync<int> total = 0;
            var entities = await Queryable
                .WhereIF(!string.IsNullOrEmpty(queryDto.ItemCode), x => x.ItemCode.Contains(queryDto.ItemCode))
                .WhereIF(!string.IsNullOrEmpty(queryDto.ItemName), x => x.ItemName.Contains(queryDto.ItemName))
                .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);

            return Mapper.Map<PaginatedList<EquipItemsReadDto>>(entities);
        }
    }
}

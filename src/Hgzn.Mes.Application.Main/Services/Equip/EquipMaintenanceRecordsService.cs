using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipMaintenance;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services.Equip
{
    public class EquipMaintenanceRecordsServicev : SugarCrudAppService<
    EquipMaintenanceRecords, Guid,
    EquipMaintenanceRecordsReadDto, EquipMaintenanceRecordsQueryDto,
    EquipMaintenanceRecordsCreateDto, EquipMaintenanceRecordsUpdateDto>,
    IEquipMaintenanceRecordsService
    {
        public async override Task<IEnumerable<EquipMaintenanceRecordsReadDto>> GetListAsync(EquipMaintenanceRecordsQueryDto? queryDto = null)
        {
            RefAsync<int> total = 0;
            var entities = await Queryable
                .ToListAsync();
            return Mapper.Map<IEnumerable<EquipMaintenanceRecordsReadDto>>(entities);
        }

        public async override Task<PaginatedList<EquipMaintenanceRecordsReadDto>> GetPaginatedListAsync(EquipMaintenanceRecordsQueryDto queryDto)
        {
            RefAsync<int> total = 0;
            var entities = await Queryable
                .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
            return Mapper.Map<PaginatedList<EquipMaintenanceRecordsReadDto>>(entities);
        }
    }
}

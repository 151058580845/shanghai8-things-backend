using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipMaintenance;
using Hgzn.Mes.Domain.Shared;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.Equip
{
    public class EquipMaintenanceRecordsServicev : SugarCrudAppService<
    EquipMaintenanceRecords, Guid,
    EquipMaintenanceRecordsReadDto, EquipMaintenanceRecordsQueryDto,
    EquipMaintenanceRecordsCreateDto, EquipMaintenanceRecordsUpdateDto>,
    IEquipMaintenanceRecordsService
    {
        public override Task<IEnumerable<EquipMaintenanceRecordsReadDto>> GetListAsync(EquipMaintenanceRecordsQueryDto? queryDto = null)
        {
            throw new NotImplementedException();
        }

        public async override Task<PaginatedList<EquipMaintenanceRecordsReadDto>> GetPaginatedListAsync(EquipMaintenanceRecordsQueryDto queryDto)
        {
            RefAsync<int> total = 0;
            var entities = await Queryable
                .ToPageListAsync(queryDto.PageIndex, queryDto.PageSize, total);
            List<EquipMaintenanceRecordsReadDto> map = Mapper.Map<List<EquipMaintenanceRecordsReadDto>>(entities);
            return new PaginatedList<EquipMaintenanceRecordsReadDto>(map, total, queryDto.PageIndex, queryDto.PageSize);
        }
    }
}

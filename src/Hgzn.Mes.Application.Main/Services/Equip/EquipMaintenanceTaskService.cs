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
    public class EquipMaintenanceTaskService : SugarCrudAppService<
    EquipMaintenanceTask, Guid,
    EquipMaintenanceTaskReadDto, EquipMaintenanceTaskQueryDto,
    EquipMaintenanceTaskCreateDto, EquipMaintenanceTaskUpdateDto>,
    IEquipMaintenanceTaskService
    {
        public override Task<IEnumerable<EquipMaintenanceTaskReadDto>> GetListAsync(EquipMaintenanceTaskQueryDto? queryDto = null)
        {
            throw new NotImplementedException();
        }

        public async override Task<PaginatedList<EquipMaintenanceTaskReadDto>> GetPaginatedListAsync(EquipMaintenanceTaskQueryDto queryDto)
        {
            RefAsync<int> total = 0;
            var entities = await Queryable
                .Where(t => t.State == queryDto.State)
                .ToListAsync();
            List<EquipMaintenanceTaskReadDto> map = Mapper.Map<List<EquipMaintenanceTaskReadDto>>(entities);
            return new PaginatedList<EquipMaintenanceTaskReadDto>(map, total, queryDto.PageIndex, queryDto.PageSize);

        }
    }
}

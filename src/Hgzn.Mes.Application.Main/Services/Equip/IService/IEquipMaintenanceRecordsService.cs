using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipMaintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService
{
    public interface IEquipMaintenanceRecordsService : ICrudAppService<
    EquipMaintenanceRecords, Guid,
    EquipMaintenanceRecordsReadDto, EquipMaintenanceRecordsQueryDto,
    EquipMaintenanceRecordsCreateDto, EquipMaintenanceRecordsUpdateDto>
    {
    }
}

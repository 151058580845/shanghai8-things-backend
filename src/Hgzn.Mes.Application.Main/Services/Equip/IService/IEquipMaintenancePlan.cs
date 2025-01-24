using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService
{
    public interface IEquipMaintenancePlan
    {
        Task PlanSetEquipAsync(List<Guid> planIds, List<Guid> equipIds);
        Task PlanSetItemsAsync(List<Guid> planIds, List<Guid> itemIds);
    }
}

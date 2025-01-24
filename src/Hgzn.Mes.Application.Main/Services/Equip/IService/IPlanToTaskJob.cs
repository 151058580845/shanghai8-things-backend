using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService
{
    public interface IPlanToTaskJob
    {
        Task SchedulePlanInsertAsync(Guid planId, DateTime startTime, DateTime endTime, int interval);

        Task ScheduleNoticeDeleteAsync(Guid planId);
    }
}

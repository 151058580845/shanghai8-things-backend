using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Jobs.RecurringTask
{
    public class DeleteRFIDStaleDataEveryDayJob
    {
        private readonly ILogger<DeleteRFIDStaleDataEveryDayJob> _logger;
        private readonly SqlSugarContext _dbContext;
        public DeleteRFIDStaleDataEveryDayJob(ILogger<DeleteRFIDStaleDataEveryDayJob> logger, SqlSugarContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public bool Execute()
        {
            _dbContext.DbContext.Deleteable<EquipLocationRecord>().Where(x => DateTime.Now - x.CreationTime > TimeSpan.FromDays(7));
            return true;
        }
    }
}

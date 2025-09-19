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

        /// <summary>
        /// 执行RFID过期数据清理任务
        /// 删除创建时间超过7天的设备位置记录
        /// </summary>
        /// <returns>执行是否成功</returns>
        public async Task<bool> ExecuteAsync()
        {
            try
            {
                _logger.LogInformation("开始执行RFID过期数据清理任务");
                
                // 计算7天前的时间点
                var cutoffDate = DateTime.Now.AddDays(-7);
                _logger.LogInformation($"准备删除创建时间早于 {cutoffDate:yyyy-MM-dd HH:mm:ss} 的设备位置记录");

                // 先查询要删除的记录数量（用于日志记录）
                var recordsToDeleteCount = await _dbContext.DbContext
                    .Queryable<EquipLocationRecord>()
                    .Where(x => x.CreationTime < cutoffDate)
                    .CountAsync();

                if (recordsToDeleteCount == 0)
                {
                    _logger.LogInformation("没有找到需要删除的过期数据");
                    return true;
                }

                // 执行删除操作
                var deletedCount = await _dbContext.DbContext
                    .Deleteable<EquipLocationRecord>()
                    .Where(x => x.CreationTime < cutoffDate)
                    .ExecuteCommandAsync();

                _logger.LogInformation($"RFID过期数据清理完成，成功删除 {deletedCount} 条记录");
                
                // 验证删除结果
                if (deletedCount != recordsToDeleteCount)
                {
                    _logger.LogWarning($"删除数量不一致：预期删除 {recordsToDeleteCount} 条，实际删除 {deletedCount} 条");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "执行RFID过期数据清理任务时发生异常");
                return false;
            }
        }

        /// <summary>
        /// 同步执行方法（保持向后兼容）
        /// </summary>
        /// <returns>执行是否成功</returns>
        public bool Execute()
        {
            try
            {
                return ExecuteAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "执行RFID过期数据清理任务时发生异常");
                return false;
            }
        }
    }
}

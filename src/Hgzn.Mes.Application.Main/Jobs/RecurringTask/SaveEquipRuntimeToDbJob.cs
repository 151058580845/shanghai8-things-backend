using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Hgzn.Mes.Infrastructure.Utilities;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Jobs.RecurringTask
{
    /// <summary>
    /// 每日将Redis中的设备运行时长数据保存到数据库的定时任务
    /// </summary>
    public class SaveEquipRuntimeToDbJob
    {
        private readonly ILogger<SaveEquipRuntimeToDbJob> _logger;
        private readonly SqlSugarContext _dbContext;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly RedisHelper _redisHelper;

        public SaveEquipRuntimeToDbJob(
            ILogger<SaveEquipRuntimeToDbJob> logger, 
            SqlSugarContext dbContext,
            IConnectionMultiplexer connectionMultiplexer,
            RedisHelper redisHelper)
        {
            _logger = logger;
            _dbContext = dbContext;
            _connectionMultiplexer = connectionMultiplexer;
            _redisHelper = redisHelper;
        }

        /// <summary>
        /// 执行设备运行时长数据持久化任务
        /// </summary>
        /// <returns>执行是否成功</returns>
        public async Task<bool> ExecuteAsync()
        {
            try
            {
                _logger.LogInformation("开始执行设备运行时长数据持久化任务");
                
                var yesterday = DateTime.Now.AddDays(-1).Date;
                var yesterdayStr = yesterday.ToString("yyyy-MM-dd");
                
                _logger.LogInformation($"准备保存 {yesterdayStr} 的设备运行时长数据");

                // 获取所有设备的运行时长数据
                var runtimeRecords = await GetAllEquipRuntimeFromRedis(yesterdayStr);
                
                if (!runtimeRecords.Any())
                {
                    _logger.LogInformation($"没有找到 {yesterdayStr} 的设备运行时长数据");
                    return true;
                }

                // 检查数据库中是否已存在该日期的记录
                var existingRecords = await _dbContext.DbContext
                    .Queryable<EquipDailyRuntime>()
                    .Where(x => x.RecordDate.Date == yesterday)
                    .ToListAsync();

                var newRecords = new List<EquipDailyRuntime>();
                var updateRecords = new List<EquipDailyRuntime>();

                foreach (var record in runtimeRecords)
                {
                    var existing = existingRecords.FirstOrDefault(x => 
                        x.EquipId == record.EquipId && 
                        x.SystemNumber == record.SystemNumber &&
                        x.DeviceTypeNumber == record.DeviceTypeNumber);

                    if (existing != null)
                    {
                        // 更新现有记录
                        existing.RunningSeconds = record.RunningSeconds;
                        existing.Remark = $"更新于 {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                        updateRecords.Add(existing);
                    }
                    else
                    {
                        // 创建新记录
                        newRecords.Add(record);
                    }
                }

                // 批量插入新记录
                if (newRecords.Any())
                {
                    await _dbContext.DbContext.Insertable(newRecords).ExecuteCommandAsync();
                    _logger.LogInformation($"成功插入 {newRecords.Count} 条新的运行时长记录");
                }

                // 批量更新现有记录
                if (updateRecords.Any())
                {
                    await _dbContext.DbContext.Updateable(updateRecords).ExecuteCommandAsync();
                    _logger.LogInformation($"成功更新 {updateRecords.Count} 条现有的运行时长记录");
                }

                _logger.LogInformation($"设备运行时长数据持久化完成，共处理 {runtimeRecords.Count} 条记录");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "执行设备运行时长数据持久化任务时发生异常");
                return false;
            }
        }

        /// <summary>
        /// 从Redis获取所有设备的运行时长数据
        /// </summary>
        /// <param name="dateStr">日期字符串 yyyy-MM-dd</param>
        /// <returns>设备运行时长记录列表</returns>
        private async Task<List<EquipDailyRuntime>> GetAllEquipRuntimeFromRedis(string dateStr)
        {
            var records = new List<EquipDailyRuntime>();
            var redisDb = _connectionMultiplexer.GetDatabase();

            try
            {
                // 获取所有设备的运行时长Redis键
                var endPoints = _connectionMultiplexer.GetEndPoints();
                if (!endPoints.Any())
                {
                    _logger.LogWarning("没有可用的Redis端点");
                    return records;
                }

                var server = _connectionMultiplexer.GetServer(endPoints.First());
                var keys = server.Keys(pattern: "equipRunTime:*:days").ToArray();

                foreach (var key in keys)
                {
                    try
                    {
                        // 检查键是否为空
                        if (string.IsNullOrEmpty(key))
                        {
                            continue;
                        }

                        // 解析Redis键获取系统编号、设备类型编号、设备ID
                        if (!ParseRuntimeKey(key, out byte systemNumber, out byte deviceTypeNumber, out Guid equipId))
                        {
                            continue;
                        }

                        // 获取指定日期的运行时长
                        var runningSeconds = await redisDb.HashGetAsync(key, dateStr);
                        
                        if (runningSeconds.HasValue && uint.TryParse(runningSeconds, out uint seconds) && seconds > 0)
                        {
                            records.Add(new EquipDailyRuntime
                            {
                                Id = Guid.NewGuid(),
                                EquipId = equipId,
                                SystemNumber = systemNumber,
                                DeviceTypeNumber = deviceTypeNumber,
                                RecordDate = DateTime.Parse(dateStr),
                                RunningSeconds = seconds,
                                DataSource = "Redis",
                                CreationTime = DateTime.Now,
                                Remark = $"从Redis自动同步，原始键: {key}"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"处理Redis键 {key} 时发生异常");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "从Redis获取设备运行时长数据时发生异常");
            }

            return records;
        }

        /// <summary>
        /// 解析运行时长Redis键
        /// </summary>
        /// <param name="key">Redis键</param>
        /// <param name="systemNumber">系统编号</param>
        /// <param name="deviceTypeNumber">设备类型编号</param>
        /// <param name="equipId">设备ID</param>
        /// <returns>解析是否成功</returns>
        private bool ParseRuntimeKey(string key, out byte systemNumber, out byte deviceTypeNumber, out Guid equipId)
        {
            systemNumber = 0;
            deviceTypeNumber = 0;
            equipId = Guid.Empty;

            try
            {
                // 键格式: equipRunTime:{systemNumber}:{deviceTypeNumber}:{equipId}:days
                var parts = key.Split(':');
                if (parts.Length >= 5 && parts[0] == "equipRunTime" && parts[4] == "days")
                {
                    systemNumber = byte.Parse(parts[1]);
                    deviceTypeNumber = byte.Parse(parts[2]);
                    equipId = Guid.Parse(parts[3]);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"解析Redis键失败: {key}");
            }

            return false;
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
                _logger.LogError(ex, "执行设备运行时长数据持久化任务时发生异常");
                return false;
            }
        }
    }
}

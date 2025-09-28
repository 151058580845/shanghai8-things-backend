using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SqlSugar;
using Newtonsoft.Json;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// 基于数据库的MQTT离线消息存储实现
    /// 重要数据持久化存储，不轻易清理
    /// </summary>
    public class MqttOfflineDatabaseStorage : IMqttOfflineDatabaseStorage, IMqttOfflineStorage
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ILogger<MqttOfflineDatabaseStorage> _logger;

        public MqttOfflineDatabaseStorage(ISqlSugarClient sqlSugarClient, ILogger<MqttOfflineDatabaseStorage> logger)
        {
            _sqlSugarClient = sqlSugarClient;
            _logger = logger;
            
            // 确保表存在
            _ = Task.Run(EnsureTableExistsAsync);
        }

        private async Task EnsureTableExistsAsync()
        {
            try
            {
                _sqlSugarClient.CodeFirst.InitTables(typeof(MqttOfflineMessageEntity));
                
                // 创建索引以提高查询性能
                await _sqlSugarClient.Ado.ExecuteCommandAsync(@"
                    CREATE INDEX IF NOT EXISTS idx_mqtt_offline_equip_id ON mqtt_offline_messages(equip_id);
                    CREATE INDEX IF NOT EXISTS idx_mqtt_offline_created_time ON mqtt_offline_messages(created_time);
                    CREATE INDEX IF NOT EXISTS idx_mqtt_offline_is_sent ON mqtt_offline_messages(is_sent);
                    CREATE INDEX IF NOT EXISTS idx_mqtt_offline_priority ON mqtt_offline_messages(priority);
                    CREATE INDEX IF NOT EXISTS idx_mqtt_offline_retry_count ON mqtt_offline_messages(retry_count);
                    CREATE INDEX IF NOT EXISTS idx_mqtt_offline_sent_time ON mqtt_offline_messages(sent_time);
                ");
                
                _logger.LogInformation("MQTT离线消息表初始化完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "初始化MQTT离线消息表失败");
            }
        }

        public async Task AddOfflineMessageAsync(IMqttOfflineMessage message)
        {
            try
            {
                var entity = MqttOfflineMessageEntity.FromOfflineMessage(message);
                await _sqlSugarClient.Insertable(entity).ExecuteCommandAsync();
                
                _logger.LogDebug($"Added offline message {message.Id} to database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add offline message {message.Id} to database");
                throw;
            }
        }

        public async Task AddOfflineMessagesBatchAsync(List<IMqttOfflineMessage> messages)
        {
            try
            {
                var entities = messages.Select(msg => MqttOfflineMessageEntity.FromOfflineMessage(msg)).ToList();
                await _sqlSugarClient.Insertable(entities).ExecuteCommandAsync();
                
                _logger.LogDebug($"Added {messages.Count} offline messages to database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add {messages.Count} offline messages to database");
                throw;
            }
        }

        public async Task<List<IMqttOfflineMessage>> GetPendingMessagesAsync(int maxCount = 100, Guid? equipId = null)
        {
            try
            {
                var query = _sqlSugarClient.Queryable<MqttOfflineMessageEntity>()
                    .Where(x => !x.IsSent && x.RetryCount < x.MaxRetryCount);

                if (equipId.HasValue)
                {
                    query = query.Where(x => x.EquipId == equipId.Value);
                }

                var entities = await query
                    .OrderBy(x => x.Priority, OrderByType.Asc) // 按优先级排序
                    .OrderBy(x => x.CreatedTime, OrderByType.Asc) // 相同优先级按时间排序
                    .Take(maxCount)
                    .ToListAsync();

                var messages = entities.Select(e => e.ToOfflineMessage()).ToList();
                
                _logger.LogDebug($"Retrieved {messages.Count} pending messages from database");
                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pending messages from database");
                return new List<IMqttOfflineMessage>();
            }
        }

        public async Task MarkMessageAsSentAsync(Guid messageId)
        {
            try
            {
                await _sqlSugarClient.Updateable<MqttOfflineMessageEntity>()
                    .SetColumns(x => new MqttOfflineMessageEntity 
                    { 
                        IsSent = true, 
                        SentTime = DateTime.UtcNow 
                    })
                    .Where(x => x.Id == messageId)
                    .ExecuteCommandAsync();
                
                _logger.LogDebug($"Marked message {messageId} as sent in database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to mark message {messageId} as sent in database");
            }
        }

        public async Task MarkMessagesAsSentAsync(List<Guid> messageIds)
        {
            try
            {
                await _sqlSugarClient.Updateable<MqttOfflineMessageEntity>()
                    .SetColumns(x => new MqttOfflineMessageEntity 
                    { 
                        IsSent = true, 
                        SentTime = DateTime.UtcNow 
                    })
                    .Where(x => messageIds.Contains(x.Id))
                    .ExecuteCommandAsync();
                
                _logger.LogDebug($"Marked {messageIds.Count} messages as sent in database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to mark {messageIds.Count} messages as sent in database");
            }
        }

        public async Task IncrementRetryCountAsync(Guid messageId)
        {
            try
            {
                await _sqlSugarClient.Updateable<MqttOfflineMessageEntity>()
                    .SetColumns(x => new MqttOfflineMessageEntity 
                    { 
                        RetryCount = x.RetryCount + 1,
                        LastRetryTime = DateTime.UtcNow
                    })
                    .Where(x => x.Id == messageId)
                    .ExecuteCommandAsync();
                
                _logger.LogDebug($"Incremented retry count for message {messageId} in database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to increment retry count for message {messageId} in database");
            }
        }

        public async Task<DatabaseStorageStats> GetStorageStatsAsync(Guid? equipId = null)
        {
            try
            {
                var query = _sqlSugarClient.Queryable<MqttOfflineMessageEntity>();
                
                if (equipId.HasValue)
                {
                    query = query.Where(x => x.EquipId == equipId.Value);
                }

                var stats = new DatabaseStorageStats();

                // 总消息数
                stats.TotalMessageCount = await query.CountAsync();

                // 待发送消息数
                stats.PendingMessageCount = await query.Where(x => !x.IsSent && x.RetryCount < x.MaxRetryCount).CountAsync();

                // 已发送消息数
                stats.SentMessageCount = await query.Where(x => x.IsSent).CountAsync();

                // 失败消息数
                stats.FailedMessageCount = await query.Where(x => !x.IsSent && x.RetryCount >= x.MaxRetryCount).CountAsync();

                // 总存储大小
                stats.TotalStorageSizeBytes = await query.SumAsync(x => x.PayloadSize);

                // 时间范围
                var timeStats = await query
                    .GroupBy(x => 1)
                    .Select(x => new 
                    {
                        MinTime = SqlFunc.AggregateMin(x.CreatedTime),
                        MaxTime = SqlFunc.AggregateMax(x.CreatedTime),
                        AvgSize = SqlFunc.AggregateAvg(x.PayloadSize)
                    })
                    .FirstAsync();

                stats.OldestMessageTime = timeStats.MinTime;
                stats.NewestMessageTime = timeStats.MaxTime;
                stats.AverageMessageSizeBytes = timeStats.AvgSize;

                // 按优先级统计
                var priorityStats = await query
                    .GroupBy(x => x.Priority)
                    .Select(x => new { Priority = x.Priority, Count = SqlFunc.AggregateCount(x.Priority) })
                    .ToListAsync();

                stats.MessageCountByPriority = priorityStats.ToDictionary(x => x.Priority, x => x.Count);

                // 按设备统计
                var equipStats = await query
                    .Where(x => x.EquipId.HasValue)
                    .GroupBy(x => x.EquipId)
                    .Select(x => new { EquipId = x.EquipId, Count = SqlFunc.AggregateCount(x.EquipId) })
                    .ToListAsync();

                stats.MessageCountByEquip = equipStats.ToDictionary(x => x.EquipId!.Value, x => x.Count);

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get storage stats from database");
                return new DatabaseStorageStats();
            }
        }

        public async Task<int> DeleteSentMessagesAsync(DateTime sentBefore)
        {
            try
            {
                var deletedCount = await _sqlSugarClient.Deleteable<MqttOfflineMessageEntity>()
                    .Where(x => x.IsSent && x.SentTime < sentBefore)
                    .ExecuteCommandAsync();
                
                _logger.LogInformation($"Deleted {deletedCount} sent messages before {sentBefore:yyyy-MM-dd HH:mm:ss}");
                return deletedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete sent messages from database");
                return 0;
            }
        }

        public async Task<List<IMqttOfflineMessage>> GetMessagesByTimeRangeAsync(DateTime startTime, DateTime endTime, Guid? equipId = null)
        {
            try
            {
                var query = _sqlSugarClient.Queryable<MqttOfflineMessageEntity>()
                    .Where(x => x.CreatedTime >= startTime && x.CreatedTime <= endTime);

                if (equipId.HasValue)
                {
                    query = query.Where(x => x.EquipId == equipId.Value);
                }

                var entities = await query
                    .OrderBy(x => x.CreatedTime)
                    .ToListAsync();

                return entities.Select(e => e.ToOfflineMessage()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get messages by time range from database");
                return new List<IMqttOfflineMessage>();
            }
        }

        public async Task<List<IMqttOfflineMessage>> GetPendingMessagesByEquipAsync(Guid equipId)
        {
            try
            {
                var entities = await _sqlSugarClient.Queryable<MqttOfflineMessageEntity>()
                    .Where(x => x.EquipId == equipId && !x.IsSent && x.RetryCount < x.MaxRetryCount)
                    .OrderBy(x => x.Priority, OrderByType.Asc)
                    .OrderBy(x => x.CreatedTime, OrderByType.Asc)
                    .ToListAsync();

                return entities.Select(e => e.ToOfflineMessage()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get pending messages for equip {equipId} from database");
                return new List<IMqttOfflineMessage>();
            }
        }

        public async Task UpdateMessagePriorityAsync(Guid messageId, int newPriority)
        {
            try
            {
                await _sqlSugarClient.Updateable<MqttOfflineMessageEntity>()
                    .SetColumns(x => new MqttOfflineMessageEntity { Priority = newPriority })
                    .Where(x => x.Id == messageId)
                    .ExecuteCommandAsync();
                
                _logger.LogDebug($"Updated priority for message {messageId} to {newPriority} in database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update priority for message {messageId} in database");
            }
        }

        public async Task<List<IMqttOfflineMessage>> GetFailedMessagesAsync(int maxCount = 100)
        {
            try
            {
                var entities = await _sqlSugarClient.Queryable<MqttOfflineMessageEntity>()
                    .Where(x => !x.IsSent && x.RetryCount >= x.MaxRetryCount)
                    .OrderBy(x => x.CreatedTime)
                    .Take(maxCount)
                    .ToListAsync();

                return entities.Select(e => e.ToOfflineMessage()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get failed messages from database");
                return new List<IMqttOfflineMessage>();
            }
        }

        #region IMqttOfflineStorage 接口实现

        public async Task<List<IMqttOfflineMessage>> GetPendingMessagesAsync(int maxCount = 100)
        {
            return await GetPendingMessagesAsync(maxCount, null);
        }

        public async Task DeleteExpiredMessagesAsync(DateTime expiredBefore)
        {
            try
            {
                var deletedCount = await _sqlSugarClient.Deleteable<MqttOfflineMessageEntity>()
                    .Where(x => x.CreatedTime < expiredBefore)
                    .ExecuteCommandAsync();

                _logger.LogInformation($"Deleted {deletedCount} expired messages from database (before {expiredBefore:yyyy-MM-dd HH:mm:ss})");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete expired messages from database");
                throw;
            }
        }

        public async Task ClearAllMessagesAsync()
        {
            try
            {
                var deletedCount = await _sqlSugarClient.Deleteable<MqttOfflineMessageEntity>()
                    .ExecuteCommandAsync();

                _logger.LogWarning($"Cleared all {deletedCount} messages from database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear all messages from database");
                throw;
            }
        }

        #endregion
    }
}

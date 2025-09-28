using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// 增强的基于Redis的MQTT离线消息存储实现，包含内存保护机制
    /// </summary>
    public class MqttOfflineStorageEnhanced : IMqttOfflineStorageEnhanced
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger<MqttOfflineStorageEnhanced> _logger;
        private readonly IDatabase _database;
        
        private const string OFFLINE_MESSAGE_KEY_PREFIX = "mqtt:offline:message:";
        private const string OFFLINE_MESSAGE_QUEUE_KEY = "mqtt:offline:queue";
        private const string STORAGE_STATS_KEY = "mqtt:offline:stats";
        
        // 默认存储限制
        private int _maxMessageCount = 10000; // 最大消息数量
        private long _maxStorageSizeBytes = 100 * 1024 * 1024; // 100MB
        private int _maxStorageDays = 7; // 最大存储天数
        private int _emergencyCleanupThreshold = 8000; // 紧急清理阈值

        public MqttOfflineStorageEnhanced(IConnectionMultiplexer connectionMultiplexer, ILogger<MqttOfflineStorageEnhanced> logger)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
            _database = _connectionMultiplexer.GetDatabase();
        }

        public async Task AddOfflineMessageAsync(IMqttOfflineMessage message)
        {
            try
            {
                // 检查存储空间
                if (!await IsStorageSpaceAvailableAsync(message.Payload.Length))
                {
                    // 尝试清理空间
                    await PerformStorageCleanupAsync();
                    
                    // 再次检查
                    if (!await IsStorageSpaceAvailableAsync(message.Payload.Length))
                    {
                        _logger.LogWarning($"Storage space insufficient, dropping message {message.Id}");
                        return;
                    }
                }

                var messageJson = JsonConvert.SerializeObject(message);
                var messageKey = $"{OFFLINE_MESSAGE_KEY_PREFIX}{message.Id}";
                
                // 存储消息详情
                await _database.StringSetAsync(messageKey, messageJson, TimeSpan.FromDays(_maxStorageDays));
                
                // 添加到优先级队列
                await _database.SortedSetAddAsync(OFFLINE_MESSAGE_QUEUE_KEY, message.Id.ToString(), message.Priority);
                
                // 更新统计信息
                await UpdateStorageStatsAsync();
                
                _logger.LogDebug($"Added offline message {message.Id} to storage");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add offline message {message.Id}");
                throw;
            }
        }

        public async Task<List<IMqttOfflineMessage>> GetPendingMessagesAsync(int maxCount = 100)
        {
            try
            {
                var messages = new List<IMqttOfflineMessage>();
                
                // 限制单次获取数量，避免内存问题
                var actualMaxCount = Math.Min(maxCount, 1000);
                
                var messageIds = await _database.SortedSetRangeByRankAsync(
                    OFFLINE_MESSAGE_QUEUE_KEY, 
                    0, 
                    actualMaxCount - 1, 
                    Order.Ascending);
                
                if (messageIds.Length == 0)
                    return messages;

                // 批量获取消息内容，避免一次性加载过多数据
                var batchSize = 100;
                for (int i = 0; i < messageIds.Length; i += batchSize)
                {
                    var batch = messageIds.Skip(i).Take(batchSize).ToArray();
                    var keys = batch.Select(id => new RedisKey($"{OFFLINE_MESSAGE_KEY_PREFIX}{id}")).ToArray();
                    var messageJsons = await _database.StringGetAsync(keys);
                    
                    foreach (var messageJson in messageJsons)
                    {
                        if (messageJson.HasValue)
                        {
                            try
                            {
                                var message = JsonConvert.DeserializeObject<MqttOfflineMessage>(messageJson);
                                if (message != null && !message.IsSent && message.RetryCount < message.MaxRetryCount)
                                {
                                    messages.Add(message);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, $"Failed to deserialize offline message: {messageJson}");
                            }
                        }
                    }
                    
                    // 避免阻塞太久
                    if (i + batchSize < messageIds.Length)
                    {
                        await Task.Delay(10);
                    }
                }
                
                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pending messages");
                return new List<IMqttOfflineMessage>();
            }
        }

        public async Task MarkMessageAsSentAsync(Guid messageId)
        {
            try
            {
                var messageKey = $"{OFFLINE_MESSAGE_KEY_PREFIX}{messageId}";
                var messageJson = await _database.StringGetAsync(messageKey);
                
                if (messageJson.HasValue)
                {
                    var message = JsonConvert.DeserializeObject<MqttOfflineMessage>(messageJson);
                    if (message != null)
                    {
                        message.IsSent = true;
                        message.SentTime = DateTime.UtcNow;
                        
                        var updatedJson = JsonConvert.SerializeObject(message);
                        await _database.StringSetAsync(messageKey, updatedJson, TimeSpan.FromDays(_maxStorageDays));
                    }
                }
                
                // 从队列中移除
                await _database.SortedSetRemoveAsync(OFFLINE_MESSAGE_QUEUE_KEY, messageId.ToString());
                
                _logger.LogDebug($"Marked message {messageId} as sent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to mark message {messageId} as sent");
            }
        }

        public async Task IncrementRetryCountAsync(Guid messageId)
        {
            try
            {
                var messageKey = $"{OFFLINE_MESSAGE_KEY_PREFIX}{messageId}";
                var messageJson = await _database.StringGetAsync(messageKey);
                
                if (messageJson.HasValue)
                {
                    var message = JsonConvert.DeserializeObject<MqttOfflineMessage>(messageJson);
                    if (message != null)
                    {
                        message.RetryCount++;
                        
                        var updatedJson = JsonConvert.SerializeObject(message);
                        await _database.StringSetAsync(messageKey, updatedJson, TimeSpan.FromDays(_maxStorageDays));
                        
                        // 如果超过最大重试次数，从队列中移除
                        if (message.RetryCount >= message.MaxRetryCount)
                        {
                            await _database.SortedSetRemoveAsync(OFFLINE_MESSAGE_QUEUE_KEY, messageId.ToString());
                            _logger.LogWarning($"Message {messageId} exceeded max retry count ({message.MaxRetryCount})");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to increment retry count for message {messageId}");
            }
        }

        public async Task DeleteExpiredMessagesAsync(DateTime expiredBefore)
        {
            try
            {
                var expiredMessageIds = new List<string>();
                var batchSize = 1000;
                
                // 分批处理，避免内存问题
                var allMessageIds = await _database.SortedSetRangeByRankAsync(OFFLINE_MESSAGE_QUEUE_KEY);
                
                for (int i = 0; i < allMessageIds.Length; i += batchSize)
                {
                    var batch = allMessageIds.Skip(i).Take(batchSize).ToArray();
                    
                    foreach (var messageId in batch)
                    {
                        var messageKey = $"{OFFLINE_MESSAGE_KEY_PREFIX}{messageId}";
                        var messageJson = await _database.StringGetAsync(messageKey);
                        
                        if (messageJson.HasValue)
                        {
                            var message = JsonConvert.DeserializeObject<MqttOfflineMessage>(messageJson);
                            if (message != null && message.CreatedTime < expiredBefore)
                            {
                                expiredMessageIds.Add(messageId);
                            }
                        }
                    }
                    
                    // 避免阻塞太久
                    if (i + batchSize < allMessageIds.Length)
                    {
                        await Task.Delay(10);
                    }
                }
                
                // 批量删除过期消息
                var deleteBatchSize = 100;
                for (int i = 0; i < expiredMessageIds.Count; i += deleteBatchSize)
                {
                    var deleteBatch = expiredMessageIds.Skip(i).Take(deleteBatchSize).ToArray();
                    
                    foreach (var messageId in deleteBatch)
                    {
                        await _database.KeyDeleteAsync($"{OFFLINE_MESSAGE_KEY_PREFIX}{messageId}");
                        await _database.SortedSetRemoveAsync(OFFLINE_MESSAGE_QUEUE_KEY, messageId);
                    }
                    
                    await Task.Delay(10);
                }
                
                if (expiredMessageIds.Count > 0)
                {
                    _logger.LogInformation($"Deleted {expiredMessageIds.Count} expired offline messages");
                    await UpdateStorageStatsAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete expired messages");
            }
        }

        public async Task ClearAllMessagesAsync()
        {
            try
            {
                var allMessageIds = await _database.SortedSetRangeByRankAsync(OFFLINE_MESSAGE_QUEUE_KEY);
                
                // 分批删除，避免阻塞
                var batchSize = 100;
                for (int i = 0; i < allMessageIds.Length; i += batchSize)
                {
                    var batch = allMessageIds.Skip(i).Take(batchSize).ToArray();
                    
                    foreach (var messageId in batch)
                    {
                        await _database.KeyDeleteAsync($"{OFFLINE_MESSAGE_KEY_PREFIX}{messageId}");
                    }
                    
                    await Task.Delay(10);
                }
                
                await _database.KeyDeleteAsync(OFFLINE_MESSAGE_QUEUE_KEY);
                await _database.KeyDeleteAsync(STORAGE_STATS_KEY);
                _logger.LogInformation("Cleared all offline messages");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear all messages");
            }
        }

        public async Task<OfflineStorageStats> GetStorageStatsAsync()
        {
            try
            {
                var stats = new OfflineStorageStats();
                
                var allMessageIds = await _database.SortedSetRangeByRankAsync(OFFLINE_MESSAGE_QUEUE_KEY);
                stats.TotalMessageCount = allMessageIds.Length;
                
                long totalSize = 0;
                var pendingCount = 0;
                var sentCount = 0;
                var expiredCount = 0;
                var oldestTime = DateTime.MaxValue;
                var newestTime = DateTime.MinValue;
                var priorityCounts = new Dictionary<int, int>();
                
                // 分批处理统计
                var batchSize = 500;
                for (int i = 0; i < allMessageIds.Length; i += batchSize)
                {
                    var batch = allMessageIds.Skip(i).Take(batchSize).ToArray();
                    var keys = batch.Select(id => new RedisKey($"{OFFLINE_MESSAGE_KEY_PREFIX}{id}")).ToArray();
                    var messageJsons = await _database.StringGetAsync(keys);
                    
                    foreach (var messageJson in messageJsons)
                    {
                        if (messageJson.HasValue)
                        {
                            var message = JsonConvert.DeserializeObject<MqttOfflineMessage>(messageJson);
                            if (message != null)
                            {
                                totalSize += message.Payload.Length;
                                
                                if (message.IsSent)
                                    sentCount++;
                                else
                                    pendingCount++;
                                
                                if (message.CreatedTime.AddDays(_maxStorageDays) < DateTime.UtcNow)
                                    expiredCount++;
                                
                                if (message.CreatedTime < oldestTime)
                                    oldestTime = message.CreatedTime;
                                if (message.CreatedTime > newestTime)
                                    newestTime = message.CreatedTime;
                                
                                if (!priorityCounts.ContainsKey(message.Priority))
                                    priorityCounts[message.Priority] = 0;
                                priorityCounts[message.Priority]++;
                            }
                        }
                    }
                    
                    await Task.Delay(10);
                }
                
                stats.TotalStorageSizeBytes = totalSize;
                stats.PendingMessageCount = pendingCount;
                stats.SentMessageCount = sentCount;
                stats.ExpiredMessageCount = expiredCount;
                stats.OldestMessageTime = oldestTime == DateTime.MaxValue ? DateTime.MinValue : oldestTime;
                stats.NewestMessageTime = newestTime == DateTime.MinValue ? DateTime.MinValue : newestTime;
                stats.MessageCountByPriority = priorityCounts;
                stats.AverageMessageSizeBytes = allMessageIds.Length > 0 ? (double)totalSize / allMessageIds.Length : 0;
                stats.IsStorageNearLimit = allMessageIds.Length > _emergencyCleanupThreshold || totalSize > _maxStorageSizeBytes * 0.8;
                
                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get storage stats");
                return new OfflineStorageStats();
            }
        }

        public async Task<bool> IsStorageSpaceAvailableAsync(int messageSize)
        {
            var stats = await GetStorageStatsAsync();
            
            // 检查消息数量限制
            if (stats.TotalMessageCount >= _maxMessageCount)
                return false;
            
            // 检查存储大小限制
            if (stats.TotalStorageSizeBytes + messageSize > _maxStorageSizeBytes)
                return false;
            
            return true;
        }

        public async Task<int> CleanupOldestMessagesAsync(int targetCount)
        {
            try
            {
                var allMessageIds = await _database.SortedSetRangeByRankAsync(OFFLINE_MESSAGE_QUEUE_KEY);
                
                if (allMessageIds.Length <= targetCount)
                    return 0;
                
                var messagesToDelete = allMessageIds.Length - targetCount;
                var deletedCount = 0;
                
                // 按创建时间排序，删除最旧的消息
                var messageInfos = new List<(string Id, DateTime CreatedTime)>();
                
                var batchSize = 100;
                for (int i = 0; i < allMessageIds.Length; i += batchSize)
                {
                    var batch = allMessageIds.Skip(i).Take(batchSize).ToArray();
                    var keys = batch.Select(id => new RedisKey($"{OFFLINE_MESSAGE_KEY_PREFIX}{id}")).ToArray();
                    var messageJsons = await _database.StringGetAsync(keys);
                    
                    for (int j = 0; j < batch.Length; j++)
                    {
                        if (messageJsons[j].HasValue)
                        {
                            var message = JsonConvert.DeserializeObject<MqttOfflineMessage>(messageJsons[j]);
                            if (message != null)
                            {
                                messageInfos.Add((batch[j], message.CreatedTime));
                            }
                        }
                    }
                    
                    await Task.Delay(10);
                }
                
                // 按创建时间排序，删除最旧的消息
                var sortedMessages = messageInfos.OrderBy(x => x.CreatedTime).Take(messagesToDelete).ToList();
                
                foreach (var (id, _) in sortedMessages)
                {
                    await _database.KeyDeleteAsync($"{OFFLINE_MESSAGE_KEY_PREFIX}{id}");
                    await _database.SortedSetRemoveAsync(OFFLINE_MESSAGE_QUEUE_KEY, id);
                    deletedCount++;
                }
                
                if (deletedCount > 0)
                {
                    _logger.LogInformation($"Cleaned up {deletedCount} oldest messages");
                    await UpdateStorageStatsAsync();
                }
                
                return deletedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cleanup oldest messages");
                return 0;
            }
        }

        public async Task SetStorageLimitsAsync(int maxMessageCount, int maxStorageSizeMB, int maxStorageDays)
        {
            _maxMessageCount = maxMessageCount;
            _maxStorageSizeBytes = maxStorageSizeMB * 1024L * 1024L;
            _maxStorageDays = maxStorageDays;
            _emergencyCleanupThreshold = (int)(maxMessageCount * 0.8);
            
            _logger.LogInformation($"Updated storage limits: MaxMessages={maxMessageCount}, MaxSize={maxStorageSizeMB}MB, MaxDays={maxStorageDays}");
        }

        public async Task<int> EmergencyCleanupAsync(int priorityThreshold)
        {
            try
            {
                var allMessageIds = await _database.SortedSetRangeByRankAsync(OFFLINE_MESSAGE_QUEUE_KEY);
                var deletedCount = 0;
                
                var batchSize = 100;
                for (int i = 0; i < allMessageIds.Length; i += batchSize)
                {
                    var batch = allMessageIds.Skip(i).Take(batchSize).ToArray();
                    var keys = batch.Select(id => new RedisKey($"{OFFLINE_MESSAGE_KEY_PREFIX}{id}")).ToArray();
                    var messageJsons = await _database.StringGetAsync(keys);
                    
                    for (int j = 0; j < batch.Length; j++)
                    {
                        if (messageJsons[j].HasValue)
                        {
                            var message = JsonConvert.DeserializeObject<MqttOfflineMessage>(messageJsons[j]);
                            if (message != null && message.Priority > priorityThreshold)
                            {
                                await _database.KeyDeleteAsync($"{OFFLINE_MESSAGE_KEY_PREFIX}{batch[j]}");
                                await _database.SortedSetRemoveAsync(OFFLINE_MESSAGE_QUEUE_KEY, batch[j]);
                                deletedCount++;
                            }
                        }
                    }
                    
                    await Task.Delay(10);
                }
                
                if (deletedCount > 0)
                {
                    _logger.LogWarning($"Emergency cleanup: deleted {deletedCount} low-priority messages (priority > {priorityThreshold})");
                    await UpdateStorageStatsAsync();
                }
                
                return deletedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to perform emergency cleanup");
                return 0;
            }
        }

        private async Task PerformStorageCleanupAsync()
        {
            try
            {
                var stats = await GetStorageStatsAsync();
                
                // 如果接近限制，执行清理
                if (stats.IsStorageNearLimit)
                {
                    // 1. 删除过期消息
                    await DeleteExpiredMessagesAsync(DateTime.UtcNow.AddDays(-_maxStorageDays));
                    
                    // 2. 如果仍然接近限制，删除低优先级消息
                    var updatedStats = await GetStorageStatsAsync();
                    if (updatedStats.IsStorageNearLimit)
                    {
                        await EmergencyCleanupAsync(2); // 删除优先级大于2的消息
                    }
                    
                    // 3. 如果仍然接近限制，删除最旧的消息
                    updatedStats = await GetStorageStatsAsync();
                    if (updatedStats.IsStorageNearLimit)
                    {
                        var targetCount = (int)(_maxMessageCount * 0.7);
                        await CleanupOldestMessagesAsync(targetCount);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to perform storage cleanup");
            }
        }

        private async Task UpdateStorageStatsAsync()
        {
            try
            {
                var stats = await GetStorageStatsAsync();
                var statsJson = JsonConvert.SerializeObject(stats);
                await _database.StringSetAsync(STORAGE_STATS_KEY, statsJson, TimeSpan.FromHours(1));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update storage stats");
            }
        }
    }
}

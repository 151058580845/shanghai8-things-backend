using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// 针对高频消息优化的离线存储实现
    /// 每秒1条消息 = 3600条/小时 = 86400条/天
    /// </summary>
    public class MqttOfflineStorageHighFrequency : IMqttOfflineStorageEnhanced
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger<MqttOfflineStorageHighFrequency> _logger;
        private readonly IDatabase _database;
        
        private const string OFFLINE_MESSAGE_KEY_PREFIX = "mqtt:offline:message:";
        private const string OFFLINE_MESSAGE_QUEUE_KEY = "mqtt:offline:queue";
        private const string STORAGE_STATS_KEY = "mqtt:offline:stats";
        private const string BATCH_OPERATION_KEY = "mqtt:offline:batch";
        
        // 高频消息优化配置
        private int _maxMessageCount = 50000; // 增加到5万条（约14小时的消息）
        private long _maxStorageSizeBytes = 200 * 1024 * 1024; // 200MB
        private int _maxStorageDays = 2; // 减少到2天，避免积累太多
        private int _emergencyCleanupThreshold = 40000; // 80%阈值
        private int _batchSize = 200; // 增大批处理大小
        private int _maxFetchCount = 2000; // 增大单次获取数量
        
        // 内存优化：使用内存缓存减少Redis访问
        private readonly ConcurrentQueue<IMqttOfflineMessage> _memoryCache = new();
        private readonly object _cacheLock = new object();
        private const int MEMORY_CACHE_SIZE = 1000; // 内存缓存1000条消息
        private DateTime _lastFlushTime = DateTime.UtcNow;

        public MqttOfflineStorageHighFrequency(IConnectionMultiplexer connectionMultiplexer, ILogger<MqttOfflineStorageHighFrequency> logger)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
            _database = _connectionMultiplexer.GetDatabase();
            
            // 启动定时刷新内存缓存到Redis
            _ = Task.Run(FlushMemoryCachePeriodically);
        }

        public async Task AddOfflineMessageAsync(IMqttOfflineMessage message)
        {
            try
            {
                // 检查存储空间
                if (!await IsStorageSpaceAvailableAsync(message.Payload.Length))
                {
                    await PerformAggressiveCleanupAsync();
                    
                    if (!await IsStorageSpaceAvailableAsync(message.Payload.Length))
                    {
                        _logger.LogWarning($"Storage space insufficient, dropping message {message.Id}");
                        return;
                    }
                }

                // 高频场景：优先使用内存缓存
                lock (_cacheLock)
                {
                    _memoryCache.Enqueue(message);
                    
                    // 如果缓存满了，立即刷新
                    if (_memoryCache.Count >= MEMORY_CACHE_SIZE)
                    {
                        _ = Task.Run(FlushMemoryCacheToRedis);
                    }
                }
                
                _logger.LogDebug($"Added offline message {message.Id} to cache");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add offline message {message.Id}");
                throw;
            }
        }

        private async Task FlushMemoryCacheToRedis()
        {
            var messagesToFlush = new List<IMqttOfflineMessage>();
            
            lock (_cacheLock)
            {
                while (_memoryCache.TryDequeue(out var message))
                {
                    messagesToFlush.Add(message);
                }
            }
            
            if (messagesToFlush.Count == 0)
                return;

            try
            {
                // 批量写入Redis，提高性能
                var tasks = new List<Task>();
                var batchSize = 50; // 每批50条
                
                for (int i = 0; i < messagesToFlush.Count; i += batchSize)
                {
                    var batch = messagesToFlush.Skip(i).Take(batchSize).ToList();
                    tasks.Add(FlushBatchToRedis(batch));
                }
                
                await Task.WhenAll(tasks);
                _lastFlushTime = DateTime.UtcNow;
                
                _logger.LogDebug($"Flushed {messagesToFlush.Count} messages to Redis");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to flush memory cache to Redis");
                
                // 如果Redis写入失败，将消息放回缓存
                lock (_cacheLock)
                {
                    foreach (var message in messagesToFlush)
                    {
                        _memoryCache.Enqueue(message);
                    }
                }
            }
        }

        private async Task FlushBatchToRedis(List<IMqttOfflineMessage> batch)
        {
            var messageKeys = new List<RedisKey>();
            var messageValues = new List<RedisValue>();
            var queueEntries = new List<SortedSetEntry>();
            
            foreach (var message in batch)
            {
                var messageJson = JsonConvert.SerializeObject(message);
                var messageKey = $"{OFFLINE_MESSAGE_KEY_PREFIX}{message.Id}";
                
                messageKeys.Add(messageKey);
                messageValues.Add(messageJson);
                queueEntries.Add(new SortedSetEntry(message.Id.ToString(), message.Priority));
            }
            
            // 批量写入消息内容
            var keyValuePairs = messageKeys.Zip(messageValues, (key, value) => new KeyValuePair<RedisKey, RedisValue>(key, value)).ToArray();
            await _database.StringSetAsync(keyValuePairs);
            
            // 批量写入队列
            await _database.SortedSetAddAsync(OFFLINE_MESSAGE_QUEUE_KEY, queueEntries.ToArray());
        }

        private async Task FlushMemoryCachePeriodically()
        {
            while (true)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5)); // 每5秒刷新一次
                    
                    // 如果缓存有数据且距离上次刷新超过5秒，就刷新
                    if (_memoryCache.Count > 0 && DateTime.UtcNow - _lastFlushTime > TimeSpan.FromSeconds(5))
                    {
                        await FlushMemoryCacheToRedis();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in periodic memory cache flush");
                }
            }
        }

        public async Task<List<IMqttOfflineMessage>> GetPendingMessagesAsync(int maxCount = 100)
        {
            try
            {
                var messages = new List<IMqttOfflineMessage>();
                
                // 高频场景：增大单次获取数量
                var actualMaxCount = Math.Min(maxCount, _maxFetchCount);
                
                var messageIds = await _database.SortedSetRangeByRankAsync(
                    OFFLINE_MESSAGE_QUEUE_KEY, 
                    0, 
                    actualMaxCount - 1, 
                    Order.Ascending);
                
                if (messageIds.Length == 0)
                    return messages;

                // 使用更大的批次大小处理
                var batchSize = _batchSize;
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
                                _logger.LogWarning(ex, $"Failed to deserialize offline message");
                            }
                        }
                    }
                    
                    // 减少延迟，提高处理速度
                    if (i + batchSize < messageIds.Length)
                    {
                        await Task.Delay(5);
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
                
                await _database.SortedSetRemoveAsync(OFFLINE_MESSAGE_QUEUE_KEY, messageId.ToString());
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
                        
                        if (message.RetryCount >= message.MaxRetryCount)
                        {
                            await _database.SortedSetRemoveAsync(OFFLINE_MESSAGE_QUEUE_KEY, messageId.ToString());
                            _logger.LogWarning($"Message {messageId} exceeded max retry count");
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
                
                // 高频场景：分批处理，但使用更大的批次
                var allMessageIds = await _database.SortedSetRangeByRankAsync(OFFLINE_MESSAGE_QUEUE_KEY);
                var batchSize = 500; // 增大批次大小
                
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
                            if (message != null && message.CreatedTime < expiredBefore)
                            {
                                expiredMessageIds.Add(batch[j]);
                            }
                        }
                    }
                    
                    // 减少延迟
                    if (i + batchSize < allMessageIds.Length)
                    {
                        await Task.Delay(5);
                    }
                }
                
                // 批量删除过期消息
                var deleteBatchSize = 200;
                for (int i = 0; i < expiredMessageIds.Count; i += deleteBatchSize)
                {
                    var deleteBatch = expiredMessageIds.Skip(i).Take(deleteBatchSize).ToArray();
                    
                    var deleteKeys = deleteBatch.Select(id => new RedisKey($"{OFFLINE_MESSAGE_KEY_PREFIX}{id}")).ToArray();
                    await _database.KeyDeleteAsync(deleteKeys);
                    
                    var queueValues = deleteBatch.Select(id => (RedisValue)id).ToArray();
                    await _database.SortedSetRemoveAsync(OFFLINE_MESSAGE_QUEUE_KEY, queueValues);
                }
                
                if (expiredMessageIds.Count > 0)
                {
                    _logger.LogInformation($"Deleted {expiredMessageIds.Count} expired offline messages");
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
                
                // 批量删除
                var batchSize = 200;
                for (int i = 0; i < allMessageIds.Length; i += batchSize)
                {
                    var batch = allMessageIds.Skip(i).Take(batchSize).ToArray();
                    var keys = batch.Select(id => new RedisKey($"{OFFLINE_MESSAGE_KEY_PREFIX}{id}")).ToArray();
                    await _database.KeyDeleteAsync(keys);
                    
                    await Task.Delay(5);
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
                stats.TotalMessageCount = allMessageIds.Length + _memoryCache.Count; // 包含内存缓存
                stats.PendingMessageCount = allMessageIds.Length + _memoryCache.Count;
                
                // 简化的统计，避免遍历所有消息
                stats.TotalStorageSizeBytes = allMessageIds.Length * 1024; // 估算：每条消息约1KB
                stats.OldestMessageTime = DateTime.UtcNow.AddDays(-_maxStorageDays);
                stats.NewestMessageTime = DateTime.UtcNow;
                stats.AverageMessageSizeBytes = 1024; // 估算
                stats.IsStorageNearLimit = stats.TotalMessageCount > _emergencyCleanupThreshold;
                
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
            
            if (stats.TotalMessageCount >= _maxMessageCount)
                return false;
            
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
                
                // 高频场景：直接删除最旧的消息，不进行复杂排序
                var messageIdsToDelete = allMessageIds.Take(messagesToDelete).ToArray();
                
                var batchSize = 200;
                for (int i = 0; i < messageIdsToDelete.Length; i += batchSize)
                {
                    var batch = messageIdsToDelete.Skip(i).Take(batchSize).ToArray();
                    var keys = batch.Select(id => new RedisKey($"{OFFLINE_MESSAGE_KEY_PREFIX}{id}")).ToArray();
                    await _database.KeyDeleteAsync(keys);
                    
                    var queueValues = batch.Select(id => (RedisValue)id).ToArray();
                    await _database.SortedSetRemoveAsync(OFFLINE_MESSAGE_QUEUE_KEY, queueValues);
                }
                
                _logger.LogInformation($"Cleaned up {messagesToDelete} oldest messages");
                return messagesToDelete;
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
                
                var batchSize = 200;
                for (int i = 0; i < allMessageIds.Length; i += batchSize)
                {
                    var batch = allMessageIds.Skip(i).Take(batchSize).ToArray();
                    var keys = batch.Select(id => new RedisKey($"{OFFLINE_MESSAGE_KEY_PREFIX}{id}")).ToArray();
                    var messageJsons = await _database.StringGetAsync(keys);
                    
                    var idsToDelete = new List<string>();
                    
                    for (int j = 0; j < batch.Length; j++)
                    {
                        if (messageJsons[j].HasValue)
                        {
                            var message = JsonConvert.DeserializeObject<MqttOfflineMessage>(messageJsons[j]);
                            if (message != null && message.Priority > priorityThreshold)
                            {
                                idsToDelete.Add(batch[j]);
                            }
                        }
                    }
                    
                    if (idsToDelete.Count > 0)
                    {
                        var deleteKeys = idsToDelete.Select(id => new RedisKey($"{OFFLINE_MESSAGE_KEY_PREFIX}{id}")).ToArray();
                        await _database.KeyDeleteAsync(deleteKeys);
                        
                        var queueValues = idsToDelete.Select(id => (RedisValue)id).ToArray();
                        await _database.SortedSetRemoveAsync(OFFLINE_MESSAGE_QUEUE_KEY, queueValues);
                        
                        deletedCount += idsToDelete.Count;
                    }
                }
                
                if (deletedCount > 0)
                {
                    _logger.LogWarning($"Emergency cleanup: deleted {deletedCount} low-priority messages");
                }
                
                return deletedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to perform emergency cleanup");
                return 0;
            }
        }

        private async Task PerformAggressiveCleanupAsync()
        {
            try
            {
                _logger.LogInformation("Performing aggressive cleanup for high-frequency scenario...");
                
                // 1. 删除过期消息
                await DeleteExpiredMessagesAsync(DateTime.UtcNow.AddDays(-_maxStorageDays));
                
                // 2. 删除低优先级消息
                await EmergencyCleanupAsync(1); // 只保留最高优先级消息
                
                // 3. 删除最旧的消息，保留最新的50%
                var stats = await GetStorageStatsAsync();
                if (stats.TotalMessageCount > _maxMessageCount * 0.5)
                {
                    var targetCount = (int)(_maxMessageCount * 0.3); // 只保留30%的消息
                    await CleanupOldestMessagesAsync(targetCount);
                }
                
                // 4. 清空内存缓存
                lock (_cacheLock)
                {
                    while (_memoryCache.TryDequeue(out _)) { }
                }
                
                _logger.LogInformation("Aggressive cleanup completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to perform aggressive cleanup");
            }
        }
    }
}

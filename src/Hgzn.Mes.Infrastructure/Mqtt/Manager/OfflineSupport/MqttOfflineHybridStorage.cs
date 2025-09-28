using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using SqlSugar;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// 混合存储策略：Redis缓存 + 数据库持久化
    /// 重要数据双重保护，不轻易丢失
    /// </summary>
    public class MqttOfflineHybridStorage : IMqttOfflineStorage
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ILogger<MqttOfflineHybridStorage> _logger;
        private readonly IDatabase _redisDatabase;
        
        private const string REDIS_KEY_PREFIX = "mqtt:offline:message:";
        private const string REDIS_QUEUE_KEY = "mqtt:offline:queue";
        
        // 内存缓存，减少数据库访问
        private readonly ConcurrentQueue<IMqttOfflineMessage> _memoryCache = new();
        private readonly object _cacheLock = new object();
        private const int MEMORY_CACHE_SIZE = 500; // 内存缓存500条
        private DateTime _lastFlushTime = DateTime.UtcNow;
        
        // 批量处理配置
        private const int BATCH_SIZE = 100;
        private const int FLUSH_INTERVAL_SECONDS = 10;

        public MqttOfflineHybridStorage(
            IConnectionMultiplexer connectionMultiplexer,
            ISqlSugarClient sqlSugarClient,
            ILogger<MqttOfflineHybridStorage> logger)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _sqlSugarClient = sqlSugarClient;
            _logger = logger;
            _redisDatabase = _connectionMultiplexer.GetDatabase();
            
            // 启动定时刷新任务
            _ = Task.Run(FlushMemoryCachePeriodically);
        }

        public async Task AddOfflineMessageAsync(IMqttOfflineMessage message)
        {
            try
            {
                // 1. 立即写入数据库（持久化）
                var entity = MqttOfflineMessageEntity.FromOfflineMessage(message, GetEquipIdFromMessage(message));
                await _sqlSugarClient.Insertable(entity).ExecuteCommandAsync();
                
                // 2. 添加到内存缓存
                lock (_cacheLock)
                {
                    _memoryCache.Enqueue(message);
                    
                    // 如果缓存满了，立即刷新
                    if (_memoryCache.Count >= MEMORY_CACHE_SIZE)
                    {
                        _ = Task.Run(FlushMemoryCacheToRedis);
                    }
                }
                
                _logger.LogDebug($"Added offline message {message.Id} to hybrid storage");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add offline message {message.Id} to hybrid storage");
                throw;
            }
        }

        public async Task<List<IMqttOfflineMessage>> GetPendingMessagesAsync(int maxCount = 100)
        {
            try
            {
                var messages = new List<IMqttOfflineMessage>();
                
                // 1. 优先从Redis获取（快速访问）
                var redisMessages = await GetPendingMessagesFromRedis(maxCount);
                messages.AddRange(redisMessages);
                
                // 2. 如果Redis中没有足够的消息，从数据库获取
                if (messages.Count < maxCount)
                {
                    var remainingCount = maxCount - messages.Count;
                    var dbMessages = await GetPendingMessagesFromDatabase(remainingCount);
                    
                    // 去重（避免Redis和数据库中的重复消息）
                    var existingIds = messages.Select(m => m.Id).ToHashSet();
                    var newMessages = dbMessages.Where(m => !existingIds.Contains(m.Id)).ToList();
                    
                    messages.AddRange(newMessages);
                    
                    // 将数据库中的消息同步到Redis
                    if (newMessages.Count > 0)
                    {
                        await SyncMessagesToRedis(newMessages);
                    }
                }
                
                _logger.LogDebug($"Retrieved {messages.Count} pending messages from hybrid storage");
                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pending messages from hybrid storage");
                return new List<IMqttOfflineMessage>();
            }
        }

        public async Task MarkMessageAsSentAsync(Guid messageId)
        {
            try
            {
                // 1. 更新数据库
                await _sqlSugarClient.Updateable<MqttOfflineMessageEntity>()
                    .SetColumns(x => new MqttOfflineMessageEntity 
                    { 
                        IsSent = true, 
                        SentTime = DateTime.UtcNow 
                    })
                    .Where(x => x.Id == messageId)
                    .ExecuteCommandAsync();
                
                // 2. 从Redis中移除
                await _redisDatabase.KeyDeleteAsync($"{REDIS_KEY_PREFIX}{messageId}");
                await _redisDatabase.SortedSetRemoveAsync(REDIS_QUEUE_KEY, messageId.ToString());
                
                _logger.LogDebug($"Marked message {messageId} as sent in hybrid storage");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to mark message {messageId} as sent in hybrid storage");
            }
        }

        public async Task IncrementRetryCountAsync(Guid messageId)
        {
            try
            {
                // 1. 更新数据库
                await _sqlSugarClient.Updateable<MqttOfflineMessageEntity>()
                    .SetColumns(x => new MqttOfflineMessageEntity 
                    { 
                        RetryCount = x.RetryCount + 1,
                        LastRetryTime = DateTime.UtcNow
                    })
                    .Where(x => x.Id == messageId)
                    .ExecuteCommandAsync();
                
                // 2. 更新Redis中的消息
                var messageJson = await _redisDatabase.StringGetAsync($"{REDIS_KEY_PREFIX}{messageId}");
                if (messageJson.HasValue)
                {
                    var message = System.Text.Json.JsonSerializer.Deserialize<MqttOfflineMessage>(messageJson);
                    if (message != null)
                    {
                        message.RetryCount++;
                        
                        var updatedJson = System.Text.Json.JsonSerializer.Serialize(message);
                        await _redisDatabase.StringSetAsync($"{REDIS_KEY_PREFIX}{messageId}", updatedJson);
                        
                        // 如果超过最大重试次数，从Redis队列中移除
                        if (message.RetryCount >= message.MaxRetryCount)
                        {
                            await _redisDatabase.SortedSetRemoveAsync(REDIS_QUEUE_KEY, messageId.ToString());
                        }
                    }
                }
                
                _logger.LogDebug($"Incremented retry count for message {messageId} in hybrid storage");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to increment retry count for message {messageId} in hybrid storage");
            }
        }

        public async Task DeleteExpiredMessagesAsync(DateTime expiredBefore)
        {
            try
            {
                // 只删除已发送的消息，保留所有未发送的重要数据
                var deletedCount = await _sqlSugarClient.Deleteable<MqttOfflineMessageEntity>()
                    .Where(x => x.IsSent && x.SentTime < expiredBefore)
                    .ExecuteCommandAsync();
                
                // 清理Redis中的过期消息
                await CleanupExpiredMessagesFromRedis(expiredBefore);
                
                _logger.LogInformation($"Deleted {deletedCount} expired sent messages from hybrid storage");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete expired messages from hybrid storage");
            }
        }

        public async Task ClearAllMessagesAsync()
        {
            try
            {
                // 注意：重要数据不轻易清理，只清理Redis缓存
                await _redisDatabase.KeyDeleteAsync(REDIS_QUEUE_KEY);
                
                // 删除所有Redis中的消息键
                var server = _redisDatabase.Multiplexer.GetServer(_redisDatabase.Multiplexer.GetEndPoints().First());
                var keys = server.Keys(pattern: $"{REDIS_KEY_PREFIX}*");
                await _redisDatabase.KeyDeleteAsync(keys.ToArray());
                
                // 清空内存缓存
                lock (_cacheLock)
                {
                    while (_memoryCache.TryDequeue(out _)) { }
                }
                
                _logger.LogWarning("Cleared all messages from Redis cache (database messages preserved)");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear messages from hybrid storage");
            }
        }

        private async Task<List<IMqttOfflineMessage>> GetPendingMessagesFromRedis(int maxCount)
        {
            try
            {
                var messageIds = await _redisDatabase.SortedSetRangeByRankAsync(
                    REDIS_QUEUE_KEY, 
                    0, 
                    maxCount - 1, 
                    Order.Ascending);
                
                if (messageIds.Length == 0)
                    return new List<IMqttOfflineMessage>();

                var messages = new List<IMqttOfflineMessage>();
                var keys = messageIds.Select(id => new RedisKey($"{REDIS_KEY_PREFIX}{id}")).ToArray();
                var messageJsons = await _redisDatabase.StringGetAsync(keys);
                
                foreach (var messageJson in messageJsons)
                {
                    if (messageJson.HasValue)
                    {
                        try
                        {
                            var message = System.Text.Json.JsonSerializer.Deserialize<MqttOfflineMessage>(messageJson);
                            if (message != null && !message.IsSent && message.RetryCount < message.MaxRetryCount)
                            {
                                messages.Add(message);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to deserialize message from Redis");
                        }
                    }
                }
                
                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pending messages from Redis");
                return new List<IMqttOfflineMessage>();
            }
        }

        private async Task<List<IMqttOfflineMessage>> GetPendingMessagesFromDatabase(int maxCount)
        {
            try
            {
                var entities = await _sqlSugarClient.Queryable<MqttOfflineMessageEntity>()
                    .Where(x => !x.IsSent && x.RetryCount < x.MaxRetryCount)
                    .OrderBy(x => x.Priority, OrderByType.Asc)
                    .OrderBy(x => x.CreatedTime, OrderByType.Asc)
                    .Take(maxCount)
                    .ToListAsync();

                return entities.Select(e => e.ToOfflineMessage()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pending messages from database");
                return new List<IMqttOfflineMessage>();
            }
        }

        private async Task SyncMessagesToRedis(List<IMqttOfflineMessage> messages)
        {
            try
            {
                var tasks = messages.Select(async message =>
                {
                    var messageJson = System.Text.Json.JsonSerializer.Serialize(message);
                    await _redisDatabase.StringSetAsync($"{REDIS_KEY_PREFIX}{message.Id}", messageJson);
                    await _redisDatabase.SortedSetAddAsync(REDIS_QUEUE_KEY, message.Id.ToString(), message.Priority);
                });
                
                await Task.WhenAll(tasks);
                _logger.LogDebug($"Synced {messages.Count} messages to Redis");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync messages to Redis");
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
                await SyncMessagesToRedis(messagesToFlush);
                _lastFlushTime = DateTime.UtcNow;
                
                _logger.LogDebug($"Flushed {messagesToFlush.Count} messages from memory cache to Redis");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to flush memory cache to Redis");
            }
        }

        private async Task FlushMemoryCachePeriodically()
        {
            while (true)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(FLUSH_INTERVAL_SECONDS));
                    
                    // 如果缓存有数据且距离上次刷新超过间隔时间，就刷新
                    if (_memoryCache.Count > 0 && DateTime.UtcNow - _lastFlushTime > TimeSpan.FromSeconds(FLUSH_INTERVAL_SECONDS))
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

        private async Task CleanupExpiredMessagesFromRedis(DateTime expiredBefore)
        {
            try
            {
                var allMessageIds = await _redisDatabase.SortedSetRangeByRankAsync(REDIS_QUEUE_KEY);
                var expiredIds = new List<string>();
                
                foreach (var messageId in allMessageIds)
                {
                    var messageJson = await _redisDatabase.StringGetAsync($"{REDIS_KEY_PREFIX}{messageId}");
                    if (messageJson.HasValue)
                    {
                        var message = System.Text.Json.JsonSerializer.Deserialize<MqttOfflineMessage>(messageJson);
                        if (message != null && message.CreatedTime < expiredBefore)
                        {
                            expiredIds.Add(messageId);
                        }
                    }
                }
                
                foreach (var id in expiredIds)
                {
                    await _redisDatabase.KeyDeleteAsync($"{REDIS_KEY_PREFIX}{id}");
                    await _redisDatabase.SortedSetRemoveAsync(REDIS_QUEUE_KEY, id);
                }
                
                if (expiredIds.Count > 0)
                {
                    _logger.LogDebug($"Cleaned up {expiredIds.Count} expired messages from Redis");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cleanup expired messages from Redis");
            }
        }

        private Guid? GetEquipIdFromMessage(IMqttOfflineMessage message)
        {
            // 从消息主题或内容中提取设备ID
            // 这里需要根据实际的消息格式来实现
            try
            {
                // 示例：从主题中提取设备ID
                if (message.Topic.Contains("/uri/"))
                {
                    var parts = message.Topic.Split('/');
                    var uriIndex = Array.IndexOf(parts, "uri");
                    if (uriIndex >= 0 && uriIndex + 1 < parts.Length)
                    {
                        if (Guid.TryParse(parts[uriIndex + 1], out var equipId))
                        {
                            return equipId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract equip ID from message");
            }
            
            return null;
        }
    }
}

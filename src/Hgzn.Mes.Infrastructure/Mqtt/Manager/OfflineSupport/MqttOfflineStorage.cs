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
    /// 基于Redis的MQTT离线消息存储实现
    /// </summary>
    public class MqttOfflineStorage : IMqttOfflineStorage
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger<MqttOfflineStorage> _logger;
        private readonly IDatabase _database;
        
        private const string OFFLINE_MESSAGE_KEY_PREFIX = "mqtt:offline:message:";
        private const string OFFLINE_MESSAGE_QUEUE_KEY = "mqtt:offline:queue";
        private const int MAX_STORAGE_DAYS = 7; // 最多存储7天的消息

        public MqttOfflineStorage(IConnectionMultiplexer connectionMultiplexer, ILogger<MqttOfflineStorage> logger)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
            _database = _connectionMultiplexer.GetDatabase();
        }

        public async Task AddOfflineMessageAsync(IMqttOfflineMessage message)
        {
            try
            {
                var messageJson = JsonConvert.SerializeObject(message);
                var messageKey = $"{OFFLINE_MESSAGE_KEY_PREFIX}{message.Id}";
                
                // 存储消息详情
                await _database.StringSetAsync(messageKey, messageJson, TimeSpan.FromDays(MAX_STORAGE_DAYS));
                
                // 添加到优先级队列（使用分数作为优先级，数字越小优先级越高）
                await _database.SortedSetAddAsync(OFFLINE_MESSAGE_QUEUE_KEY, message.Id.ToString(), message.Priority);
                
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
                
                // 按优先级获取消息ID（分数越小优先级越高）
                var messageIds = await _database.SortedSetRangeByRankAsync(
                    OFFLINE_MESSAGE_QUEUE_KEY, 
                    0, 
                    maxCount - 1, 
                    Order.Ascending);
                
                if (messageIds.Length == 0)
                    return messages;

                var keys = messageIds.Select(id => new RedisKey($"{OFFLINE_MESSAGE_KEY_PREFIX}{id}")).ToArray();
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
                        await _database.StringSetAsync(messageKey, updatedJson, TimeSpan.FromDays(MAX_STORAGE_DAYS));
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
                        await _database.StringSetAsync(messageKey, updatedJson, TimeSpan.FromDays(MAX_STORAGE_DAYS));
                        
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
                
                // 获取所有消息ID
                var allMessageIds = await _database.SortedSetRangeByRankAsync(OFFLINE_MESSAGE_QUEUE_KEY);
                
                foreach (var messageId in allMessageIds)
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
                
                // 删除过期消息
                foreach (var messageId in expiredMessageIds)
                {
                    await _database.KeyDeleteAsync($"{OFFLINE_MESSAGE_KEY_PREFIX}{messageId}");
                    await _database.SortedSetRemoveAsync(OFFLINE_MESSAGE_QUEUE_KEY, messageId);
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
                
                foreach (var messageId in allMessageIds)
                {
                    await _database.KeyDeleteAsync($"{OFFLINE_MESSAGE_KEY_PREFIX}{messageId}");
                }
                
                await _database.KeyDeleteAsync(OFFLINE_MESSAGE_QUEUE_KEY);
                _logger.LogInformation("Cleared all offline messages");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear all messages");
            }
        }
    }
}

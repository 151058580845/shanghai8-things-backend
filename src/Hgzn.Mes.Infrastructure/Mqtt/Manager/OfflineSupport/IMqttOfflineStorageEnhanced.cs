using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// 增强的MQTT离线消息存储接口，包含内存保护机制
    /// </summary>
    public interface IMqttOfflineStorageEnhanced : IMqttOfflineStorage
    {
        /// <summary>
        /// 获取存储统计信息
        /// </summary>
        /// <returns></returns>
        Task<OfflineStorageStats> GetStorageStatsAsync();

        /// <summary>
        /// 检查存储空间是否充足
        /// </summary>
        /// <param name="messageSize">即将添加的消息大小</param>
        /// <returns></returns>
        Task<bool> IsStorageSpaceAvailableAsync(int messageSize);

        /// <summary>
        /// 清理最旧的消息以释放空间
        /// </summary>
        /// <param name="targetCount">目标消息数量</param>
        /// <returns>清理的消息数量</returns>
        Task<int> CleanupOldestMessagesAsync(int targetCount);

        /// <summary>
        /// 设置存储限制
        /// </summary>
        /// <param name="maxMessageCount">最大消息数量</param>
        /// <param name="maxStorageSizeMB">最大存储大小(MB)</param>
        /// <param name="maxStorageDays">最大存储天数</param>
        Task SetStorageLimitsAsync(int maxMessageCount, int maxStorageSizeMB, int maxStorageDays);

        /// <summary>
        /// 紧急清理：删除低优先级消息
        /// </summary>
        /// <param name="priorityThreshold">优先级阈值，删除优先级大于此值的消息</param>
        /// <returns>清理的消息数量</returns>
        Task<int> EmergencyCleanupAsync(int priorityThreshold);
    }

    /// <summary>
    /// 离线存储统计信息
    /// </summary>
    public class OfflineStorageStats
    {
        public int TotalMessageCount { get; set; }
        public long TotalStorageSizeBytes { get; set; }
        public int PendingMessageCount { get; set; }
        public int SentMessageCount { get; set; }
        public int ExpiredMessageCount { get; set; }
        public DateTime OldestMessageTime { get; set; }
        public DateTime NewestMessageTime { get; set; }
        public Dictionary<int, int> MessageCountByPriority { get; set; } = new Dictionary<int, int>();
        public double AverageMessageSizeBytes { get; set; }
        public bool IsStorageNearLimit { get; set; }
    }
}

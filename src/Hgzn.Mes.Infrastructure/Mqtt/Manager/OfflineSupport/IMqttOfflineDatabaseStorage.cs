using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// 基于数据库的MQTT离线消息存储接口
    /// 重要数据持久化存储，不轻易清理
    /// </summary>
    public interface IMqttOfflineDatabaseStorage
    {
        /// <summary>
        /// 添加离线消息到数据库
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        Task AddOfflineMessageAsync(IMqttOfflineMessage message);

        /// <summary>
        /// 批量添加离线消息
        /// </summary>
        /// <param name="messages">消息列表</param>
        /// <returns></returns>
        Task AddOfflineMessagesBatchAsync(List<IMqttOfflineMessage> messages);

        /// <summary>
        /// 获取待发送的离线消息（按优先级排序）
        /// </summary>
        /// <param name="maxCount">最大数量</param>
        /// <param name="equipId">设备ID（可选）</param>
        /// <returns></returns>
        Task<List<IMqttOfflineMessage>> GetPendingMessagesAsync(int maxCount = 100, Guid? equipId = null);

        /// <summary>
        /// 标记消息为已发送
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <returns></returns>
        Task MarkMessageAsSentAsync(Guid messageId);

        /// <summary>
        /// 批量标记消息为已发送
        /// </summary>
        /// <param name="messageIds">消息ID列表</param>
        /// <returns></returns>
        Task MarkMessagesAsSentAsync(List<Guid> messageIds);

        /// <summary>
        /// 增加消息重试次数
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <returns></returns>
        Task IncrementRetryCountAsync(Guid messageId);

        /// <summary>
        /// 获取存储统计信息
        /// </summary>
        /// <param name="equipId">设备ID（可选）</param>
        /// <returns></returns>
        Task<DatabaseStorageStats> GetStorageStatsAsync(Guid? equipId = null);

        /// <summary>
        /// 删除已发送的消息（可选清理）
        /// </summary>
        /// <param name="sentBefore">发送时间早于此时间的消息</param>
        /// <returns>删除的消息数量</returns>
        Task<int> DeleteSentMessagesAsync(DateTime sentBefore);

        /// <summary>
        /// 获取指定时间范围内的消息
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="equipId">设备ID（可选）</param>
        /// <returns></returns>
        Task<List<IMqttOfflineMessage>> GetMessagesByTimeRangeAsync(DateTime startTime, DateTime endTime, Guid? equipId = null);

        /// <summary>
        /// 获取指定设备的所有待发送消息
        /// </summary>
        /// <param name="equipId">设备ID</param>
        /// <returns></returns>
        Task<List<IMqttOfflineMessage>> GetPendingMessagesByEquipAsync(Guid equipId);

        /// <summary>
        /// 更新消息优先级
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <param name="newPriority">新优先级</param>
        /// <returns></returns>
        Task UpdateMessagePriorityAsync(Guid messageId, int newPriority);

        /// <summary>
        /// 获取失败的消息（重试次数达到上限）
        /// </summary>
        /// <param name="maxCount">最大数量</param>
        /// <returns></returns>
        Task<List<IMqttOfflineMessage>> GetFailedMessagesAsync(int maxCount = 100);
    }

    /// <summary>
    /// 数据库存储统计信息
    /// </summary>
    public class DatabaseStorageStats
    {
        public int TotalMessageCount { get; set; }
        public int PendingMessageCount { get; set; }
        public int SentMessageCount { get; set; }
        public int FailedMessageCount { get; set; }
        public long TotalStorageSizeBytes { get; set; }
        public DateTime OldestMessageTime { get; set; }
        public DateTime NewestMessageTime { get; set; }
        public Dictionary<int, int> MessageCountByPriority { get; set; } = new Dictionary<int, int>();
        public Dictionary<Guid, int> MessageCountByEquip { get; set; } = new Dictionary<Guid, int>();
        public double AverageMessageSizeBytes { get; set; }
        public int MaxRetryCountReached { get; set; }
    }
}

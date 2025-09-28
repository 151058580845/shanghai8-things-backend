using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// MQTT离线消息接口
    /// </summary>
    public interface IMqttOfflineMessage
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        Guid Id { get; set; }
        
        /// <summary>
        /// 主题
        /// </summary>
        string Topic { get; set; }
        
        /// <summary>
        /// 消息内容
        /// </summary>
        byte[] Payload { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreatedTime { get; set; }
        
        /// <summary>
        /// 重试次数
        /// </summary>
        int RetryCount { get; set; }
        
        /// <summary>
        /// 最大重试次数
        /// </summary>
        int MaxRetryCount { get; set; }
        
        /// <summary>
        /// 消息优先级（数字越小优先级越高）
        /// </summary>
        int Priority { get; set; }
        
        /// <summary>
        /// 是否已发送成功
        /// </summary>
        bool IsSent { get; set; }
        
        /// <summary>
        /// 发送时间
        /// </summary>
        DateTime? SentTime { get; set; }
    }

    /// <summary>
    /// MQTT离线消息存储接口
    /// </summary>
    public interface IMqttOfflineStorage
    {
        /// <summary>
        /// 添加离线消息
        /// </summary>
        /// <param name="message">消息</param>
        Task AddOfflineMessageAsync(IMqttOfflineMessage message);
        
        /// <summary>
        /// 获取待发送的离线消息
        /// </summary>
        /// <param name="maxCount">最大数量</param>
        /// <returns>离线消息列表</returns>
        Task<List<IMqttOfflineMessage>> GetPendingMessagesAsync(int maxCount = 100);
        
        /// <summary>
        /// 标记消息为已发送
        /// </summary>
        /// <param name="messageId">消息ID</param>
        Task MarkMessageAsSentAsync(Guid messageId);
        
        /// <summary>
        /// 增加消息重试次数
        /// </summary>
        /// <param name="messageId">消息ID</param>
        Task IncrementRetryCountAsync(Guid messageId);
        
        /// <summary>
        /// 删除过期的消息
        /// </summary>
        /// <param name="expiredBefore">过期时间</param>
        Task DeleteExpiredMessagesAsync(DateTime expiredBefore);
        
        /// <summary>
        /// 清空所有离线消息
        /// </summary>
        Task ClearAllMessagesAsync();
    }
}

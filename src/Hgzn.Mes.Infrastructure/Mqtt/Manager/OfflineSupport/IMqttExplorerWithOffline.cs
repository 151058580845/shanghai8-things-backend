using Hgzn.Mes.Domain.Shared.Enums;
using MQTTnet.Client;
using MQTTnet.Packets;
using System;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// 支持断点续传的MQTT管理器接口
    /// </summary>
    public interface IMqttExplorerWithOffline : IMqttExplorer
    {
        /// <summary>
        /// 发布消息（支持离线存储）
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="payload">消息内容</param>
        /// <param name="priority">消息优先级（数字越小优先级越高）</param>
        /// <param name="maxRetryCount">最大重试次数</param>
        /// <param name="storeOfflineIfDisconnected">连接断开时是否存储到离线队列</param>
        /// <returns></returns>
        Task PublishWithOfflineSupportAsync(string topic, byte[] payload, int priority = 0, int maxRetryCount = 3, bool storeOfflineIfDisconnected = true);

        /// <summary>
        /// 重发所有离线消息
        /// </summary>
        /// <returns>重发的消息数量</returns>
        Task<int> ResendOfflineMessagesAsync();

        /// <summary>
        /// 清理过期的离线消息
        /// </summary>
        /// <param name="expiredBefore">过期时间</param>
        /// <returns></returns>
        Task CleanupExpiredMessagesAsync(DateTime expiredBefore);

        /// <summary>
        /// 获取离线消息统计信息
        /// </summary>
        /// <returns></returns>
        Task<(int pendingCount, int totalCount)> GetOfflineMessageStatsAsync();

        /// <summary>
        /// 连接状态变化事件
        /// </summary>
        event EventHandler<bool> ConnectionStatusChanged;
    }
}

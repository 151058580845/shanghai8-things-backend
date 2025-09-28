using System;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// 高频消息场景的MQTT离线存储配置
    /// 每秒1条消息 = 3600条/小时 = 86400条/天
    /// </summary>
    public class MqttHighFrequencyConfig
    {
        /// <summary>
        /// 最大消息数量（约14小时的消息量）
        /// </summary>
        public int MaxMessageCount { get; set; } = 50000;

        /// <summary>
        /// 最大存储大小(MB) - 增加存储空间
        /// </summary>
        public int MaxStorageSizeMB { get; set; } = 200;

        /// <summary>
        /// 最大存储天数 - 减少存储时间，避免积累过多
        /// </summary>
        public int MaxStorageDays { get; set; } = 2;

        /// <summary>
        /// 紧急清理阈值（80%）
        /// </summary>
        public int EmergencyCleanupThreshold { get; set; } = 40000;

        /// <summary>
        /// 内存缓存大小
        /// </summary>
        public int MemoryCacheSize { get; set; } = 1000;

        /// <summary>
        /// 内存缓存刷新间隔(秒)
        /// </summary>
        public int MemoryCacheFlushIntervalSeconds { get; set; } = 5;

        /// <summary>
        /// 批处理大小 - 增大以提高效率
        /// </summary>
        public int BatchSize { get; set; } = 200;

        /// <summary>
        /// 单次获取消息最大数量
        /// </summary>
        public int MaxFetchCount { get; set; } = 2000;

        /// <summary>
        /// 是否启用内存缓存
        /// </summary>
        public bool EnableMemoryCache { get; set; } = true;

        /// <summary>
        /// 是否启用激进清理策略
        /// </summary>
        public bool EnableAggressiveCleanup { get; set; } = true;

        /// <summary>
        /// 自动清理间隔(分钟) - 更频繁的清理
        /// </summary>
        public int AutoCleanupIntervalMinutes { get; set; } = 10;

        /// <summary>
        /// 内存保护阈值(MB)
        /// </summary>
        public int MemoryProtectionThresholdMB { get; set; } = 100;

        /// <summary>
        /// 是否启用批量操作优化
        /// </summary>
        public bool EnableBatchOptimization { get; set; } = true;

        /// <summary>
        /// 获取最大存储大小(字节)
        /// </summary>
        public long MaxStorageSizeBytes => MaxStorageSizeMB * 1024L * 1024L;

        /// <summary>
        /// 获取内存保护阈值(字节)
        /// </summary>
        public long MemoryProtectionThresholdBytes => MemoryProtectionThresholdMB * 1024L * 1024L;

        /// <summary>
        /// 计算理论最大消息数（基于频率和存储天数）
        /// </summary>
        /// <param name="messagesPerSecond">每秒消息数</param>
        /// <param name="storageDays">存储天数</param>
        /// <returns></returns>
        public static int CalculateMaxMessages(int messagesPerSecond, int storageDays)
        {
            return messagesPerSecond * 3600 * 24 * storageDays; // 秒 * 小时 * 天
        }

        /// <summary>
        /// 计算理论存储大小（基于消息大小和数量）
        /// </summary>
        /// <param name="averageMessageSizeBytes">平均消息大小</param>
        /// <param name="maxMessageCount">最大消息数</param>
        /// <returns>存储大小(MB)</returns>
        public static int CalculateStorageSizeMB(int averageMessageSizeBytes, int maxMessageCount)
        {
            return (int)((averageMessageSizeBytes * maxMessageCount) / (1024.0 * 1024.0));
        }

        /// <summary>
        /// 验证高频配置
        /// </summary>
        public void ValidateForHighFrequency()
        {
            if (MaxMessageCount < 10000)
                throw new ArgumentException("高频场景下MaxMessageCount至少应为10000");

            if (MaxStorageDays > 7)
                throw new ArgumentException("高频场景下MaxStorageDays不应超过7天，避免积累过多消息");

            if (MemoryCacheSize < 100)
                throw new ArgumentException("高频场景下MemoryCacheSize至少应为100");

            if (BatchSize < 50)
                throw new ArgumentException("高频场景下BatchSize至少应为50");

            if (AutoCleanupIntervalMinutes < 5)
                throw new ArgumentException("高频场景下AutoCleanupIntervalMinutes至少应为5分钟");

            // 检查理论消息数量
            var theoreticalMax = CalculateMaxMessages(1, MaxStorageDays); // 每秒1条消息
            if (MaxMessageCount < theoreticalMax * 0.5)
            {
                throw new ArgumentException($"高频场景下MaxMessageCount({MaxMessageCount})过小，建议至少为{theoreticalMax}");
            }

            // 检查存储大小
            var theoreticalSize = CalculateStorageSizeMB(1024, MaxMessageCount); // 假设每条消息1KB
            if (MaxStorageSizeMB < theoreticalSize * 0.8)
            {
                throw new ArgumentException($"高频场景下MaxStorageSizeMB({MaxStorageSizeMB})过小，建议至少为{theoreticalSize}MB");
            }
        }

        /// <summary>
        /// 获取高频场景的推荐配置
        /// </summary>
        /// <param name="messagesPerSecond">每秒消息数</param>
        /// <param name="averageMessageSizeKB">平均消息大小(KB)</param>
        /// <returns></returns>
        public static MqttHighFrequencyConfig GetRecommendedConfig(int messagesPerSecond = 1, int averageMessageSizeKB = 1)
        {
            var config = new MqttHighFrequencyConfig();
            
            // 基于消息频率计算
            var messagesPerHour = messagesPerSecond * 3600;
            var messagesPerDay = messagesPerHour * 24;
            
            // 存储2天的消息，但不超过10万条
            config.MaxMessageCount = Math.Min(messagesPerDay * 2, 100000);
            
            // 基于消息大小计算存储空间
            var averageMessageSizeBytes = averageMessageSizeKB * 1024;
            config.MaxStorageSizeMB = CalculateStorageSizeMB(averageMessageSizeBytes, config.MaxMessageCount);
            
            // 动态调整其他参数
            config.EmergencyCleanupThreshold = (int)(config.MaxMessageCount * 0.8);
            config.MemoryCacheSize = Math.Min(messagesPerHour / 10, 2000); // 缓存10分钟的消息量
            config.BatchSize = Math.Max(messagesPerSecond * 10, 100); // 批量处理10秒的消息量
            config.MaxFetchCount = Math.Max(messagesPerHour / 10, 1000); // 单次获取10分钟的消息量
            
            return config;
        }

        /// <summary>
        /// 输出配置摘要
        /// </summary>
        public string GetConfigSummary()
        {
            return $@"高频MQTT配置摘要:
- 最大消息数: {MaxMessageCount:N0} 条 (约 {MaxMessageCount / 3600:F1} 小时)
- 最大存储: {MaxStorageSizeMB} MB
- 存储天数: {MaxStorageDays} 天
- 内存缓存: {MemoryCacheSize} 条
- 批处理大小: {BatchSize} 条
- 自动清理间隔: {AutoCleanupIntervalMinutes} 分钟
- 内存保护阈值: {MemoryProtectionThresholdMB} MB";
        }
    }
}

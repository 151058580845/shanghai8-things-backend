using System;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// MQTT离线存储配置
    /// </summary>
    public class MqttOfflineStorageConfig
    {
        /// <summary>
        /// 最大消息数量
        /// </summary>
        public int MaxMessageCount { get; set; } = 10000;

        /// <summary>
        /// 最大存储大小(MB)
        /// </summary>
        public int MaxStorageSizeMB { get; set; } = 100;

        /// <summary>
        /// 最大存储天数
        /// </summary>
        public int MaxStorageDays { get; set; } = 7;

        /// <summary>
        /// 紧急清理阈值(消息数量)
        /// </summary>
        public int EmergencyCleanupThreshold { get; set; } = 8000;

        /// <summary>
        /// 存储大小警告阈值(百分比)
        /// </summary>
        public double StorageWarningThresholdPercent { get; set; } = 80.0;

        /// <summary>
        /// 批量处理大小
        /// </summary>
        public int BatchSize { get; set; } = 100;

        /// <summary>
        /// 单次获取消息最大数量
        /// </summary>
        public int MaxFetchCount { get; set; } = 1000;

        /// <summary>
        /// 是否启用自动清理
        /// </summary>
        public bool EnableAutoCleanup { get; set; } = true;

        /// <summary>
        /// 自动清理间隔(小时)
        /// </summary>
        public int AutoCleanupIntervalHours { get; set; } = 1;

        /// <summary>
        /// 是否启用内存保护
        /// </summary>
        public bool EnableMemoryProtection { get; set; } = true;

        /// <summary>
        /// 内存保护阈值(MB)
        /// </summary>
        public int MemoryProtectionThresholdMB { get; set; } = 50;

        /// <summary>
        /// 获取最大存储大小(字节)
        /// </summary>
        public long MaxStorageSizeBytes => MaxStorageSizeMB * 1024L * 1024L;

        /// <summary>
        /// 获取内存保护阈值(字节)
        /// </summary>
        public long MemoryProtectionThresholdBytes => MemoryProtectionThresholdMB * 1024L * 1024L;

        /// <summary>
        /// 验证配置
        /// </summary>
        public void Validate()
        {
            if (MaxMessageCount <= 0)
                throw new ArgumentException("MaxMessageCount must be greater than 0");
            
            if (MaxStorageSizeMB <= 0)
                throw new ArgumentException("MaxStorageSizeMB must be greater than 0");
            
            if (MaxStorageDays <= 0)
                throw new ArgumentException("MaxStorageDays must be greater than 0");
            
            if (EmergencyCleanupThreshold >= MaxMessageCount)
                throw new ArgumentException("EmergencyCleanupThreshold must be less than MaxMessageCount");
            
            if (StorageWarningThresholdPercent < 0 || StorageWarningThresholdPercent > 100)
                throw new ArgumentException("StorageWarningThresholdPercent must be between 0 and 100");
            
            if (BatchSize <= 0)
                throw new ArgumentException("BatchSize must be greater than 0");
            
            if (MaxFetchCount <= 0)
                throw new ArgumentException("MaxFetchCount must be greater than 0");
            
            if (AutoCleanupIntervalHours <= 0)
                throw new ArgumentException("AutoCleanupIntervalHours must be greater than 0");
            
            if (MemoryProtectionThresholdMB <= 0)
                throw new ArgumentException("MemoryProtectionThresholdMB must be greater than 0");
        }
    }
}

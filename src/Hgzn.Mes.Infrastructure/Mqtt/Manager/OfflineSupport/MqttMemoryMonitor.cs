using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// MQTT内存监控服务
    /// </summary>
    public class MqttMemoryMonitor : IDisposable
    {
        private readonly ILogger<MqttMemoryMonitor> _logger;
        private readonly IMqttOfflineStorageEnhanced _offlineStorage;
        private readonly MqttOfflineStorageConfig _config;
        private readonly Timer _memoryCheckTimer;
        
        private volatile bool _isDisposed = false;
        private long _lastMemoryUsage = 0;
        private DateTime _lastCheckTime = DateTime.UtcNow;

        public event EventHandler<MemoryUsageEventArgs> MemoryUsageChanged;
        public event EventHandler<MemoryWarningEventArgs> MemoryWarning;

        public MqttMemoryMonitor(
            ILogger<MqttMemoryMonitor> logger,
            IMqttOfflineStorageEnhanced offlineStorage,
            MqttOfflineStorageConfig config)
        {
            _logger = logger;
            _offlineStorage = offlineStorage;
            _config = config;
            
            // 每5分钟检查一次内存使用情况
            _memoryCheckTimer = new Timer(CheckMemoryUsage, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        }

        private async void CheckMemoryUsage(object state)
        {
            if (_isDisposed)
                return;

            try
            {
                var currentMemory = GC.GetTotalMemory(false);
                var currentTime = DateTime.UtcNow;
                
                // 计算内存变化
                var memoryDelta = currentMemory - _lastMemoryUsage;
                var timeDelta = currentTime - _lastCheckTime;
                
                // 触发内存使用变化事件
                var args = new MemoryUsageEventArgs
                {
                    CurrentMemoryBytes = currentMemory,
                    PreviousMemoryBytes = _lastMemoryUsage,
                    MemoryDeltaBytes = memoryDelta,
                    TimeDelta = timeDelta,
                    Timestamp = currentTime
                };
                
                MemoryUsageChanged?.Invoke(this, args);
                
                // 检查是否需要内存警告
                await CheckMemoryWarningAsync(currentMemory);
                
                // 检查存储统计
                await CheckStorageStatsAsync();
                
                _lastMemoryUsage = currentMemory;
                _lastCheckTime = currentTime;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check memory usage");
            }
        }

        private async Task CheckMemoryWarningAsync(long currentMemory)
        {
            var memoryMB = currentMemory / (1024.0 * 1024.0);
            
            // 如果内存使用超过阈值，触发警告
            if (memoryMB > _config.MemoryProtectionThresholdMB)
            {
                var warningArgs = new MemoryWarningEventArgs
                {
                    CurrentMemoryMB = memoryMB,
                    ThresholdMB = _config.MemoryProtectionThresholdMB,
                    Timestamp = DateTime.UtcNow
                };
                
                MemoryWarning?.Invoke(this, warningArgs);
                
                _logger.LogWarning($"Memory usage warning: {memoryMB:F2}MB > {_config.MemoryProtectionThresholdMB}MB");
                
                // 如果启用了内存保护，执行紧急清理
                if (_config.EnableMemoryProtection)
                {
                    await PerformMemoryProtectionAsync();
                }
            }
        }

        private async Task CheckStorageStatsAsync()
        {
            try
            {
                var stats = await _offlineStorage.GetStorageStatsAsync();
                
                // 如果存储接近限制，记录警告
                if (stats.IsStorageNearLimit)
                {
                    _logger.LogWarning($"Storage near limit: {stats.TotalMessageCount} messages, {stats.TotalStorageSizeBytes / (1024.0 * 1024.0):F2}MB");
                }
                
                // 定期记录存储统计
                _logger.LogDebug($"Storage stats: {stats.TotalMessageCount} total, {stats.PendingMessageCount} pending, {stats.TotalStorageSizeBytes / (1024.0 * 1024.0):F2}MB");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check storage stats");
            }
        }

        private async Task PerformMemoryProtectionAsync()
        {
            try
            {
                _logger.LogInformation("Performing memory protection cleanup...");
                
                var stats = await _offlineStorage.GetStorageStatsAsync();
                
                // 1. 删除过期消息
                await _offlineStorage.DeleteExpiredMessagesAsync(DateTime.UtcNow.AddDays(-_config.MaxStorageDays));
                
                // 2. 删除低优先级消息
                await _offlineStorage.EmergencyCleanupAsync(2);
                
                // 3. 删除最旧的消息
                var targetCount = (int)(_config.MaxMessageCount * 0.5); // 保留50%的消息
                await _offlineStorage.CleanupOldestMessagesAsync(targetCount);
                
                // 4. 强制垃圾回收
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                
                var newStats = await _offlineStorage.GetStorageStatsAsync();
                var newMemory = GC.GetTotalMemory(false);
                
                _logger.LogInformation($"Memory protection completed: {stats.TotalMessageCount - newStats.TotalMessageCount} messages cleaned, memory reduced to {newMemory / (1024.0 * 1024.0):F2}MB");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to perform memory protection cleanup");
            }
        }

        /// <summary>
        /// 获取当前内存使用情况
        /// </summary>
        public MemoryUsageInfo GetCurrentMemoryUsage()
        {
            var process = Process.GetCurrentProcess();
            var workingSet = process.WorkingSet64;
            var managedMemory = GC.GetTotalMemory(false);
            var gen0Collections = GC.CollectionCount(0);
            var gen1Collections = GC.CollectionCount(1);
            var gen2Collections = GC.CollectionCount(2);
            
            return new MemoryUsageInfo
            {
                ProcessWorkingSetBytes = workingSet,
                ManagedMemoryBytes = managedMemory,
                Gen0Collections = gen0Collections,
                Gen1Collections = gen1Collections,
                Gen2Collections = gen2Collections,
                Timestamp = DateTime.UtcNow
            };
        }

        /// <summary>
        /// 强制执行内存保护
        /// </summary>
        public async Task ForceMemoryProtectionAsync()
        {
            await PerformMemoryProtectionAsync();
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _memoryCheckTimer?.Dispose();
            }
        }
    }

    /// <summary>
    /// 内存使用事件参数
    /// </summary>
    public class MemoryUsageEventArgs : EventArgs
    {
        public long CurrentMemoryBytes { get; set; }
        public long PreviousMemoryBytes { get; set; }
        public long MemoryDeltaBytes { get; set; }
        public TimeSpan TimeDelta { get; set; }
        public DateTime Timestamp { get; set; }
        
        public double CurrentMemoryMB => CurrentMemoryBytes / (1024.0 * 1024.0);
        public double PreviousMemoryMB => PreviousMemoryBytes / (1024.0 * 1024.0);
        public double MemoryDeltaMB => MemoryDeltaBytes / (1024.0 * 1024.0);
        public double MemoryChangeRateMBPerMinute => TimeDelta.TotalMinutes > 0 ? MemoryDeltaMB / TimeDelta.TotalMinutes : 0;
    }

    /// <summary>
    /// 内存警告事件参数
    /// </summary>
    public class MemoryWarningEventArgs : EventArgs
    {
        public double CurrentMemoryMB { get; set; }
        public double ThresholdMB { get; set; }
        public DateTime Timestamp { get; set; }
        
        public double OverThresholdMB => CurrentMemoryMB - ThresholdMB;
        public double OverThresholdPercent => (CurrentMemoryMB / ThresholdMB - 1) * 100;
    }

    /// <summary>
    /// 内存使用信息
    /// </summary>
    public class MemoryUsageInfo
    {
        public long ProcessWorkingSetBytes { get; set; }
        public long ManagedMemoryBytes { get; set; }
        public int Gen0Collections { get; set; }
        public int Gen1Collections { get; set; }
        public int Gen2Collections { get; set; }
        public DateTime Timestamp { get; set; }
        
        public double ProcessWorkingSetMB => ProcessWorkingSetBytes / (1024.0 * 1024.0);
        public double ManagedMemoryMB => ManagedMemoryBytes / (1024.0 * 1024.0);
    }
}

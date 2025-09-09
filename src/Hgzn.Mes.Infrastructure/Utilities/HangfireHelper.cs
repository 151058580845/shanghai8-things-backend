using Hangfire.Storage;
using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities
{
    /// <summary>
    /// Hangfire 任务调度帮助类
    /// </summary>
    public class HangfireHelper
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly ILogger<HangfireHelper> _logger;
        private readonly IMonitoringApi _monitoringApi;

        public HangfireHelper(
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager recurringJobManager,
            ILogger<HangfireHelper> logger,
            JobStorage jobStorage)
        {
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
            _logger = logger;
            _monitoringApi = jobStorage.GetMonitoringApi();
        }

        #region 即时任务（Fire-and-Forget）

        /// <summary>
        /// 创建即时任务（立即执行）
        /// </summary>
        public string Enqueue<T>(Expression<Action<T>> methodCall, string queue = "default")
        {
            var jobId = _backgroundJobClient.Enqueue<T>(methodCall);
            _logger.LogInformation("创建即时任务: {JobId}, 队列: {Queue}", jobId, queue);
            return jobId;
        }

        /// <summary>
        /// 创建异步即时任务
        /// </summary>
        public string Enqueue<T>(Expression<Func<T, Task>> methodCall, string queue = "default")
        {
            var jobId = _backgroundJobClient.Enqueue<T>(methodCall);
            _logger.LogInformation("创建异步即时任务: {JobId}, 队列: {Queue}", jobId, queue);
            return jobId;
        }

        #endregion

        #region 延迟任务（Delayed）

        /// <summary>
        /// 创建延迟任务（指定时间间隔后执行）
        /// </summary>
        public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay, string queue = "default")
        {
            var jobId = _backgroundJobClient.Schedule<T>(methodCall, delay);
            _logger.LogInformation("创建延迟任务: {JobId}, 延迟: {Delay}, 队列: {Queue}", jobId, delay, queue);
            return jobId;
        }

        /// <summary>
        /// 创建异步延迟任务
        /// </summary>
        public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay, string queue = "default")
        {
            var jobId = _backgroundJobClient.Schedule<T>(methodCall, delay);
            _logger.LogInformation("创建异步延迟任务: {JobId}, 延迟: {Delay}, 队列: {Queue}", jobId, delay, queue);
            return jobId;
        }

        /// <summary>
        /// 创建定时任务（指定具体时间执行）
        /// </summary>
        public string Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt, string queue = "default")
        {
            var jobId = _backgroundJobClient.Schedule<T>(methodCall, enqueueAt);
            _logger.LogInformation("创建定时任务: {JobId}, 执行时间: {EnqueueAt}, 队列: {Queue}", jobId, enqueueAt, queue);
            return jobId;
        }

        #endregion

        #region 循环任务（Recurring）

        /// <summary>
        /// 创建或更新循环任务
        /// </summary>
        public void AddOrUpdateRecurringJob<T>(string jobId, Expression<Action<T>> methodCall,
            string cronExpression, TimeZoneInfo timeZone = null, string queue = "default")
        {
            _recurringJobManager.AddOrUpdate<T>(jobId, methodCall, cronExpression, timeZone, queue);
            _logger.LogInformation("创建/更新循环任务: {JobId}, Cron: {Cron}, 队列: {Queue}", jobId, cronExpression, queue);
        }

        /// <summary>
        /// 创建或更新异步循环任务
        /// </summary>
        public void AddOrUpdateRecurringJob<T>(string jobId, Expression<Func<T, Task>> methodCall,
            string cronExpression, TimeZoneInfo timeZone = null, string queue = "default")
        {
            _recurringJobManager.AddOrUpdate<T>(jobId, methodCall, cronExpression, timeZone, queue);
            _logger.LogInformation("创建/更新异步循环任务: {JobId}, Cron: {Cron}, 队列: {Queue}", jobId, cronExpression, queue);
        }

        /// <summary>
        /// 立即触发循环任务（不等待下次计划时间）
        /// </summary>
        public void TriggerRecurringJob(string jobId)
        {
            _recurringJobManager.Trigger(jobId);
            _logger.LogInformation("立即触发循环任务: {JobId}", jobId);
        }

        /// <summary>
        /// 移除循环任务
        /// </summary>
        public void RemoveRecurringJob(string jobId)
        {
            _recurringJobManager.RemoveIfExists(jobId);
            _logger.LogInformation("移除循环任务: {JobId}", jobId);
        }

        #endregion

        #region 连续任务（Continuations）

        /// <summary>
        /// 创建连续任务（在指定任务成功后执行）
        /// </summary>
        public string ContinueWith<T>(string parentJobId, Expression<Action<T>> methodCall, string queue = "default")
        {
            var jobId = _backgroundJobClient.ContinueJobWith<T>(parentJobId, methodCall);
            _logger.LogInformation("创建连续任务: {JobId}, 父任务: {ParentJobId}, 队列: {Queue}", jobId, parentJobId, queue);
            return jobId;
        }

        /// <summary>
        /// 创建异步连续任务
        /// </summary>
        public string ContinueWith<T>(string parentJobId, Expression<Func<T, Task>> methodCall, string queue = "default")
        {
            var jobId = _backgroundJobClient.ContinueJobWith<T>(parentJobId, methodCall);
            _logger.LogInformation("创建异步连续任务: {JobId}, 父任务: {ParentJobId}, 队列: {Queue}", jobId, parentJobId, queue);
            return jobId;
        }

        #endregion

        #region 任务管理

        /// <summary>
        /// 删除任务,注意,如果是要移除循环任务需要使用RemoveRecurringJob而不是DeleteJob
        /// </summary>
        public bool DeleteJob(string jobId)
        {
            var result = _backgroundJobClient.Delete(jobId);
            _logger.LogInformation("删除任务: {JobId}, 结果: {Result}", jobId, result);
            return result;
        }

        /// <summary>
        /// 重新排队任务（用于失败任务重试）
        /// </summary>
        public bool RequeueJob(string jobId)
        {
            var result = _backgroundJobClient.Requeue(jobId);
            _logger.LogInformation("重新排队任务: {JobId}, 结果: {Result}", jobId, result);
            return result;
        }

        /// <summary>
        /// 获取任务状态,注意,该方法无法获取循环任务的状态
        /// </summary>
        public JobStatus GetJobStatus(string jobId)
        {
            using var connection = JobStorage.Current.GetConnection();
            var jobData = connection.GetJobData(jobId);

            if (jobData == null)
                return null;

            return new JobStatus
            {
                JobId = jobId,
                State = jobData.State,
                CreatedAt = jobData.CreatedAt,
                Job = jobData.Job?.ToString(),
                LoadException = jobData.LoadException?.Message
            };
        }

        /// <summary>
        /// 获取所有循环任务
        /// </summary>
        public List<RecurringJobInfo> GetAllRecurringJobs()
        {
            var result = new List<RecurringJobInfo>();
            using var connection = JobStorage.Current.GetConnection();
            var recurringJobs = connection.GetRecurringJobs();

            foreach (var job in recurringJobs)
            {
                result.Add(new RecurringJobInfo
                {
                    Id = job.Id,
                    Cron = job.Cron,
                    LastExecution = job.LastExecution,
                    NextExecution = job.NextExecution,
                    LastJobId = job.LastJobId,
                    LastJobState = job.LastJobState,
                    Queue = job.Queue,
                    TimeZoneId = job.TimeZoneId,
                    Error = job.Error
                });
            }

            return result;
        }

        /// <summary>
        /// 获取任务统计信息
        /// </summary>
        public JobStatistics GetJobStatistics()
        {
            try
            {
                var stats = _monitoringApi.GetStatistics();

                return new JobStatistics
                {
                    Enqueued = stats.Enqueued,
                    Failed = stats.Failed,
                    Processing = stats.Processing,
                    Scheduled = stats.Scheduled,
                    Succeeded = stats.Succeeded,
                    Deleted = stats.Deleted,
                    Servers = stats.Servers,
                    Recurring = stats.Recurring,
                    // Retries 字段在 Hangfire 的 Statistics 中不存在，移除或使用其他方式获取
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取任务统计信息失败");
                return new JobStatistics();
            }
        }

        /// <summary>
        /// 获取队列统计信息
        /// </summary>
        public List<QueueInfo> GetQueueStatistics()
        {
            var result = new List<QueueInfo>();

            try
            {
                var queues = _monitoringApi.Queues();
                foreach (var queue in queues)
                {
                    result.Add(new QueueInfo
                    {
                        Name = queue.Name,
                        Length = queue.Length,
                        // Fetched 字段在 QueueStatisticsDto 中不存在，移除或使用其他方式获取
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取队列统计信息失败");
            }

            return result;
        }

        /// <summary>
        /// 获取服务器信息
        /// </summary>
        public List<ServerInfo> GetServers()
        {
            var result = new List<ServerInfo>();

            try
            {
                var servers = _monitoringApi.Servers();
                foreach (var server in servers)
                {
                    result.Add(new ServerInfo
                    {
                        Name = server.Name,
                        // Workers 字段在 ServerDto 中不存在，移除或使用其他方式获取
                        Queues = server.Queues?.ToArray(), // 转换为数组
                        StartedAt = server.StartedAt,
                        Heartbeat = server.Heartbeat
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取服务器信息失败");
            }

            return result;
        }

        /// <summary>
        /// 获取失败任务数量
        /// </summary>
        public long GetFailedCount()
        {
            try
            {
                return _monitoringApi.GetStatistics().Failed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取失败任务数量失败");
                return 0;
            }
        }

        /// <summary>
        /// 获取重试任务数量（通过查询失败队列中的重试任务）
        /// </summary>
        public long GetRetriesCount()
        {
            try
            {
                // 重试任务通常在失败队列中，可以通过查询特定状态的任务来获取
                var failedJobs = _monitoringApi.FailedJobs(0, int.MaxValue);
                return failedJobs.Count(job => job.Value?.InFailedState == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取重试任务数量失败");
                return 0;
            }
        }

        /// <summary>
        /// 获取已获取的任务数量（通过队列信息计算）
        /// </summary>
        public long GetFetchedCount()
        {
            try
            {
                var queues = _monitoringApi.Queues();
                return queues.Sum(queue => queue.Length); // 使用队列长度作为近似值
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取已获取任务数量失败");
                return 0;
            }
        }

        /// <summary>
        /// 获取工作进程数量（通过服务器信息计算）
        /// </summary>
        public int GetWorkersCount()
        {
            try
            {
                var servers = _monitoringApi.Servers();
                return servers.Count; // 服务器数量近似于工作进程数量
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取工作进程数量失败");
                return 0;
            }
        }

        #endregion

        #region 便捷方法

        /// <summary>
        /// 创建每分钟任务
        /// </summary>
        public void AddMinutelyJob<T>(string jobId, Expression<Action<T>> methodCall,
            int second = 0, string queue = "default")
        {
            var cron = $"{second} * * * * *";
            AddOrUpdateRecurringJob<T>(jobId, methodCall, cron, queue: queue);
            _logger.LogInformation("创建每分钟任务: {JobId}, 在每分钟的第 {Second} 秒执行", jobId, second);
        }

        /// <summary>
        /// 创建异步每分钟任务
        /// </summary>
        public void AddMinutelyJob<T>(string jobId, Expression<Func<T, Task>> methodCall,
            int second = 0, string queue = "default")
        {
            var cron = $"{second} * * * * *";
            AddOrUpdateRecurringJob<T>(jobId, methodCall, cron, queue: queue);
            _logger.LogInformation("创建异步每分钟任务: {JobId}, 在每分钟的第 {Second} 秒执行", jobId, second);
        }

        /// <summary>
        /// 创建每秒任务（高频率任务，请谨慎使用）
        /// </summary>
        public void AddSecondlyJob<T>(string jobId, Expression<Action<T>> methodCall,
            string queue = "default", int intervalSeconds = 1)
        {
            if (intervalSeconds < 1) intervalSeconds = 1;

            var cron = intervalSeconds == 1 ? "* * * * * *" : $"*/{intervalSeconds} * * * * *";
            AddOrUpdateRecurringJob<T>(jobId, methodCall, cron, queue: queue);
            _logger.LogInformation("创建每秒任务: {JobId}, 间隔: {Interval}秒", jobId, intervalSeconds);
        }

        /// <summary>
        /// 创建异步每秒任务,默认的轮询周期是15秒,如果需要更高频率的任务,请谨慎使用
        /// </summary>
        public void AddSecondlyJob<T>(string jobId, Expression<Func<T, Task>> methodCall,
            string queue = "default", int intervalSeconds = 1)
        {
            if (intervalSeconds < 1) intervalSeconds = 1;

            var cron = intervalSeconds == 1 ? "* * * * * *" : $"*/{intervalSeconds} * * * * *";
            AddOrUpdateRecurringJob<T>(jobId, methodCall, cron, queue: queue);
            _logger.LogInformation("创建异步每秒任务: {JobId}, 间隔: {Interval}秒", jobId, intervalSeconds);
        }

        /// <summary>
        /// 创建每N秒任务（自定义间隔）,默认的轮询周期是15秒,如果需要更高频率的任务,请谨慎使用
        /// </summary>
        public void AddEveryNSecondsJob<T>(string jobId, Expression<Action<T>> methodCall,
    int seconds, string queue = "default")
        {
            if (seconds < 1) seconds = 1;
            if (seconds > 59) seconds = 59;

            // 使用正确的 Cron 表达式格式
            var cron = $"*/{seconds} * * * * *";

            // 确保使用支持秒的 Cron 表达式
            AddOrUpdateRecurringJob<T>(jobId, methodCall, cron, queue: queue);
            _logger.LogInformation("创建每 {Seconds} 秒任务: {JobId}", seconds, jobId);
        }

        /// <summary>
        /// 创建每日任务
        /// </summary>
        public void AddDailyJob<T>(string jobId, Expression<Action<T>> methodCall,
            int hour = 0, int minute = 0, int second = 0, string queue = "default")
        {
            var cron = $"{second} {minute} {hour} * * *";
            AddOrUpdateRecurringJob<T>(jobId, methodCall, cron, queue: queue);
            _logger.LogInformation("创建每日任务: {JobId}, 时间: {Hour:00}:{Minute:00}:{Second:00}",
                jobId, hour, minute, second);
        }

        /// <summary>
        /// 创建每小时任务
        /// </summary>
        public void AddHourlyJob<T>(string jobId, Expression<Action<T>> methodCall,
            int minute = 0, int second = 0, string queue = "default")
        {
            var cron = $"{second} {minute} * * * *";
            AddOrUpdateRecurringJob<T>(jobId, methodCall, cron, queue: queue);
            _logger.LogInformation("创建每小时任务: {JobId}, 在每小时的 {Minute:00} 分 {Second:00} 秒执行",
                jobId, minute, second);
        }

        /// <summary>
        /// 创建每周任务
        /// </summary>
        public void AddWeeklyJob<T>(string jobId, Expression<Action<T>> methodCall,
            DayOfWeek dayOfWeek, int hour = 0, int minute = 0, int second = 0, string queue = "default")
        {
            var cron = $"{second} {minute} {hour} * * {(int)dayOfWeek}";
            AddOrUpdateRecurringJob<T>(jobId, methodCall, cron, queue: queue);
            _logger.LogInformation("创建每周任务: {JobId}, {DayOfWeek} {Hour:00}:{Minute:00}:{Second:00}",
                jobId, GetChineseDayOfWeek(dayOfWeek), hour, minute, second);
        }

        /// <summary>
        /// 创建每月任务
        /// </summary>
        public void AddMonthlyJob<T>(string jobId, Expression<Action<T>> methodCall,
            int day = 1, int hour = 0, int minute = 0, int second = 0, string queue = "default")
        {
            var cron = $"{second} {minute} {hour} {day} * *";
            AddOrUpdateRecurringJob<T>(jobId, methodCall, cron, queue: queue);
            _logger.LogInformation("创建每月任务: {JobId}, 每月 {Day} 日 {Hour:00}:{Minute:00}:{Second:00}",
                jobId, day, hour, minute, second);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取中文星期几
        /// </summary>
        private static string GetChineseDayOfWeek(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Sunday => "星期日",
                DayOfWeek.Monday => "星期一",
                DayOfWeek.Tuesday => "星期二",
                DayOfWeek.Wednesday => "星期三",
                DayOfWeek.Thursday => "星期四",
                DayOfWeek.Friday => "星期五",
                DayOfWeek.Saturday => "星期六",
                _ => "未知"
            };
        }

        /// <summary>
        /// 验证Cron表达式是否有效
        /// </summary>
        public bool ValidateCronExpression(string cronExpression)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cronExpression))
                    return false;

                var parts = cronExpression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return parts.Length == 5 || parts.Length == 6;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取常用的Cron表达式
        /// </summary>
        public Dictionary<string, string> GetCommonCronExpressions()
        {
            return new Dictionary<string, string>
            {
                ["每秒"] = "* * * * * *",
                ["每5秒"] = "*/5 * * * * *",
                ["每10秒"] = "*/10 * * * * *",
                ["每30秒"] = "*/30 * * * * *",
                ["每分钟"] = "0 * * * * *",
                ["每5分钟"] = "0 */5 * * * *",
                ["每小时"] = "0 0 * * * *",
                ["每天凌晨2点"] = "0 0 2 * * *",
                ["每周一9点"] = "0 0 9 * * 1",
                ["每月1号0点"] = "0 0 0 1 * *"
            };
        }

        #endregion
    }

    #region 数据模型

    /// <summary>
    /// 任务状态信息
    /// </summary>
    public class JobStatus
    {
        public string JobId { get; set; }
        public string State { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Job { get; set; }
        public string LoadException { get; set; }
    }

    /// <summary>
    /// 循环任务信息
    /// </summary>
    public class RecurringJobInfo
    {
        public string Id { get; set; }
        public string Cron { get; set; }
        public DateTime? LastExecution { get; set; }
        public DateTime? NextExecution { get; set; }
        public string LastJobId { get; set; }
        public string LastJobState { get; set; }
        public string Queue { get; set; }
        public string TimeZoneId { get; set; }
        public string Error { get; set; }
    }

    /// <summary>
    /// 任务统计信息
    /// </summary>
    public class JobStatistics
    {
        public long Enqueued { get; set; }
        public long Failed { get; set; }
        public long Processing { get; set; }
        public long Scheduled { get; set; }
        public long Succeeded { get; set; }
        public long Deleted { get; set; }
        public long Servers { get; set; }
        public long Recurring { get; set; }
    }

    /// <summary>
    /// 队列信息
    /// </summary>
    public class QueueInfo
    {
        public string Name { get; set; }
        public long Length { get; set; }
    }

    /// <summary>
    /// 服务器信息
    /// </summary>
    public class ServerInfo
    {
        public string Name { get; set; }
        public string[] Queues { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? Heartbeat { get; set; }
    }

    #endregion
}

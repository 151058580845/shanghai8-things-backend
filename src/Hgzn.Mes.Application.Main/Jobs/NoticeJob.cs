using System.Collections.Concurrent;
using Hangfire;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Entities.System.Notice;
using Hgzn.Mes.Domain.Shared.Enums;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Jobs;

public class NoticeJob : INoticeJob
{
    private readonly ConcurrentDictionary<Guid, string> _dictionary = new();
    private ISqlSugarClient _sqlSugarClient = null!;
    public Task ScheduleNoticeInsertAsync(Guid noticeId, DateTime sendTime)
    {
        if (_dictionary.ContainsKey(noticeId))
        {
            ScheduleNoticeDeleteAsync(noticeId);
        }
        else
        {
            var jobId = BackgroundJob.Schedule(
                () => ExecuteJob(noticeId), sendTime
            );
            _dictionary.TryAdd(noticeId, jobId);
        }

        return Task.CompletedTask;
    }

    public async Task ScheduleNoticeUpdateAsync(Guid noticeId, DateTime sendTime)
    {
        await ScheduleNoticeDeleteAsync(noticeId);
        await ScheduleNoticeInsertAsync(noticeId, sendTime);
    }

    public Task ScheduleNoticeDeleteAsync(Guid noticeId)
    {
        if (_dictionary.TryGetValue(noticeId, out var jobId))
        {
           // var jobDetails = JobStorage.Current.GetMonitoringApi().JobDetails(jobId);
           BackgroundJob.Delete(jobId);
        }
        return Task.CompletedTask;
    }


    private async Task ExecuteJob(Guid noticeId)
    {
        var notice = await _sqlSugarClient.Queryable<NoticeInfo>().FirstAsync(t=>t.Id == noticeId);
        notice.Status = NoticeStatus.Publish;
        var targets = new List<Guid>();
        switch (notice.Target)
        {
            case NoticeTargetType.User:
                targets = await _sqlSugarClient.Queryable<User>()
                    .Where(t => notice.NoticeTargets != null && notice.NoticeTargets.Contains(t.Id))
                    .Select(t=>t.Id)
                    .ToListAsync();
                break;
            case NoticeTargetType.Role:
                break;
            case NoticeTargetType.Dept:
                break;
            case NoticeTargetType.Post:
                break;
            case NoticeTargetType.Group:
                break;
            case NoticeTargetType.Organization:
                break;
            case NoticeTargetType.Team:
                break;
            case NoticeTargetType.AllUsers:
                break;
            case NoticeTargetType.Custom:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        foreach (var target in targets)
        {
            _sqlSugarClient.Insertable(new NoticeTarget
            {
                Id = Guid.NewGuid(),
                NoticeId = noticeId,
                NoticeTargetType = notice.Target,
                NoticeObjectId = target,
                NoticeTime = DateTime.Now.ToLocalTime()
            });
        }
        // //发送给对应用户
        // foreach (var userId in sendUsers)
        // {
        //     await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", "测试中");
        // }
        //保存回数据库
        await _sqlSugarClient.Updateable(notice).ExecuteCommandAsync();
    }
}
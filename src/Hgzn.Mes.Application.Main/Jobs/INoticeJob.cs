namespace Hgzn.Mes.Application.Main.Jobs;

public interface INoticeJob
{
    Task ScheduleNoticeInsertAsync(Guid noticeId, DateTime sendTime);
    Task ScheduleNoticeUpdateAsync(Guid noticeId, DateTime sendTime);
    Task ScheduleNoticeDeleteAsync(Guid noticeId);
}
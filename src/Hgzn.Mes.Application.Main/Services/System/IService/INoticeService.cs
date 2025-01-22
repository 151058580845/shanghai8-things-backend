using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Notice;
using NoticeInfo = Hgzn.Mes.Domain.Entities.System.Notice.NoticeInfo;

namespace Hgzn.Mes.Application.Main.Services.System.IService;

public interface INoticeService : ICrudAppService<
    NoticeInfo, Guid,
    NoticeReadDto, NoticeQueryDto,
    NoticeCreateDto, NoticeUpdateDto>
{
    Task ModifyTargets(Guid outputId, List<NoticeTarget> targetsEntities);
}
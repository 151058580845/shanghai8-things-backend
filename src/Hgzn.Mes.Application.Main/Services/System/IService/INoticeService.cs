using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Notice;
using Notice = Hgzn.Mes.Domain.Entities.System.Notice.Notice;

namespace Hgzn.Mes.Application.Main.Services.System.IService;

public interface INoticeService : ICrudAppService<Notice
    , Guid, NoticeQueryDto, NoticeReadDto, NoticeCreateDto, NoticeUpdateDto>
{
    Task ModifyTargets(Guid outputId, List<NoticeTarget> targetsEntities);
}
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Notice;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;
using NoticeInfo = Hgzn.Mes.Domain.Entities.System.Notice.NoticeInfo;

namespace Hgzn.Mes.Application.Main.Services.System;

/// <summary>
/// 通知信息
/// </summary>
public class NoticeService : SugarCrudAppService<
        NoticeInfo, Guid,
        NoticeReadDto, NoticeQueryDto,
        NoticeCreateDto, NoticeUpdateDto>,
    INoticeService
{
    public override async Task<IEnumerable<NoticeReadDto>> GetListAsync(NoticeQueryDto? input)
    {
        var entities = await Queryable
            .WhereIF(input is { Type: not null }, x => input != null && x.NoticeShowType == input.Type)
            .WhereIF(!string.IsNullOrEmpty(input?.Title), x => input != null && x.Title.Contains(input.Title!))
            .WhereIF(input is { StartTime: not null, EndTime: not null },
                x => input != null && x.CreationTime >= input.StartTime && x.CreationTime <= input.EndTime)
            .ToListAsync();
        return Mapper.Map<IEnumerable<NoticeReadDto>>(entities);
    }

    public override async Task<PaginatedList<NoticeReadDto>> GetPaginatedListAsync(NoticeQueryDto input)
    {
        var entities = await Queryable.WhereIF(input.Type is not null, x => x.NoticeShowType == input.Type)
            .WhereIF(!string.IsNullOrEmpty(input.Title), x => x.Title.Contains(input.Title!))
            .WhereIF(input.StartTime is not null && input.EndTime is not null,
                x => x.CreationTime >= input.StartTime && x.CreationTime <= input.EndTime)
            .ToPaginatedListAsync(input.PageIndex, input.PageSize);
        return Mapper.Map<PaginatedList<NoticeReadDto>>(entities);
    }

    /// <summary>
    /// 创建一个通知
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public override async Task<NoticeReadDto> CreateAsync(NoticeCreateDto dto)
    {
        var output = await base.CreateAsync(dto);
        var list = new List<NoticeTarget>();
        if (dto is { TargetType: not null, TargetIds: not null })
            list.AddRange(dto.TargetIds.Select(id => new NoticeTarget()
            {
                NoticeId = output.Id,
                NoticeTargetType = dto.TargetType.Value,
                NoticeObjectId = id,
                NoticeTime = output.SendTime,
            }));

        await ModifyTargets(output.Id, list);
        return output;
    }

    /// <summary>
    /// 修改通知
    /// </summary>
    /// <param name="key"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public override async Task<NoticeReadDto?> UpdateAsync(Guid key, NoticeUpdateDto dto)
    {
        var output = await base.UpdateAsync(key, dto);
        if (output == null)
        {
            throw new ArgumentNullException(nameof(output));
        }

        var list = new List<NoticeTarget>();
        if (dto is { TargetType: not null, TargetIds: not null })
            list.AddRange(dto.TargetIds.Select(id => new NoticeTarget()
            {
                NoticeId = output.Id,
                NoticeTargetType = dto.TargetType.Value,
                NoticeObjectId = id,
                NoticeTime = output.SendTime,
            }));
        await ModifyTargets(output.Id, list);
        return output;
    }

    public override async Task<int> DeleteAsync(Guid key)
    {
        //级联删除
        var result = await DbContext.DeleteNav<NoticeInfo>(t => t.Id == key)
            .Include(t => t.NoticeTargets)
            .ExecuteCommandAsync();
        return result ? 1 : 0;
    }

    /// <summary>
    /// 修改通知目标和时间
    /// </summary>
    /// <param name="outputId"></param>
    /// <param name="targetsEntities"></param>
    public async Task ModifyTargets(Guid outputId, List<NoticeTarget> targetsEntities)
    {
        try
        {
            await DbContext.Ado.BeginTranAsync();
            DbContext.Deleteable<NoticeTarget>().Where(t => t.NoticeId == outputId);
            DbContext.Insertable(targetsEntities);
            await DbContext.Ado.CommitTranAsync();
        }
        catch
        {
            await DbContext.Ado.RollbackTranAsync();
            throw;
        }
    }
}
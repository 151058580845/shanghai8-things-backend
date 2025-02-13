using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Application.Main.Dtos.System;

public class NoticeDto
{
}

public class NoticeReadDto : ReadDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public NoticeType Type { get; set; }
    public string Content { get; set; } = null!;
    public bool IsDeleted { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public int OrderNum { get; set; }
    public bool State { get; set; }

    public NoticeTimeType NoticeTimeType { get; set; }

    public NoticeTargetType Target { get; set; }
    public DateTime SendTime { get; set; }
}

public class NoticeCreateDto : CreateDto
{
    public string Title { get; set; } = null!;
    public NoticeType Type { get; set; }
    public string Content { get; set; } = null!;
    public int OrderNum { get; set; }
    public bool State { get; set; }

    public DateTime? SendTime { get; set; }

    /// <summary>
    /// target_groups：接收范围（部门、角色、特定用户列表）。
    /// </summary>
    public NoticeTargetType? TargetType { get; set; }

    public List<Guid>? TargetIds { get; set; }
}

public class NoticeUpdateDto : UpdateDto
{
    public string? Title { get; set; }
    public NoticeType? Type { get; set; }
    public string? Content { get; set; }
    public int? OrderNum { get; set; }
    public bool? State { get; set; }

    public DateTime SendTime { get; set; } = DateTime.Now;

    /// <summary>
    /// target_groups：接收范围（部门、角色、特定用户列表）。
    /// </summary>
    public NoticeTargetType? TargetType { get; set; }

    public List<Guid>? TargetIds { get; set; }
}

public class NoticeQueryDto : PaginatedQueryDto
{
    public string? Title { get; set; }
    // public NoticeShow? Type { get; set; }
}
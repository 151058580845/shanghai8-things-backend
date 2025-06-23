using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.System.Resource;

public class Attachment : UniversalEntity, ISoftDelete, IAudited
{
    public string FileName { get; set; } = null!;
    public string? FileType { get; set; }
    public string? Description { get; set; } 
    public string? Version { get; set; } 
    public string? VersionDescription { get; set; } 
    public int? Count { get; set; }
    public bool SoftDeleted { get; set; }
    public DateTime? DeleteTime { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
    public int CreatorLevel { get; set; }
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public bool State { get; set; } = true;
    public static string[] ForbidExtension { get; set; } = ["exe", "dll"];

}
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.System.Resource;

public class Attachment : UniversalEntity, ISoftDelete, IAudited
{
    public bool SoftDeleted { get; set; }
    public DateTime? DeleteTime { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    
    
    /// <summary>
    /// 文件大小 
    ///</summary>
    public decimal FileSize { get; set; }
    /// <summary>
    /// 文件名 
    ///</summary>
    public string? FileName { get; set; }
    /// <summary>
    /// 文件路径 
    ///</summary>
    public string? FilePath { get; set; }
}
using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Basic;

[Description("联系人")]
public class Contact: UniversalEntity, IAudited
{
    /// <summary>
    /// 父类主键
    /// </summary>
    public Guid ParentId { get; set; }

    /// <summary>
    /// 联系人名称
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 联系人电话
    /// </summary>
    public string Phone { get; set; } = null!;

    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
}
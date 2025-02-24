using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Basic;

[Description("地址管理")]
public class AddressB: UniversalEntity, IAudited
{
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    
    /// <summary>
    /// 父类主键
    /// </summary>
    public Guid ParentId { get; set; }

    /// <summary>
    /// 联系人名称
    /// </summary>
    public string Address { get; set; } = null!;
}
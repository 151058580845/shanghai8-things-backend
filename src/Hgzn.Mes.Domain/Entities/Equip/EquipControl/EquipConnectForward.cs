using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using System.ComponentModel;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipControl;

/// <summary>
/// 设备绑定表
/// </summary>
[Description("设备绑定表")]
public class EquipConnectForward : UniversalEntity, ICreationAudited
{
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; } = default!;

    [Description("源链接")]
    public Guid OriginatorId { get; set; }

    [Description("目标链接")]
    public Guid TargetId { get; set; }
}
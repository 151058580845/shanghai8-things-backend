using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Base;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipControl;

/// <summary>
/// 设备绑定表
/// </summary>
[Table("EquipConnect")]
[Description("设备绑定表")]
public class EquipConnectForward : UniversalEntity ,ICreationAudited
{
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }=default!;
    [Description( "源链接")]
    public Guid OriginatorId { get; set; }
    [Description( "目标链接")]
    public Guid TargetId { get; set; }
}


using System.ComponentModel;
using HgznMes.Domain.Entities.System.Base;
using HgznMes.Domain.Entities.System.Base.Audited;

namespace HgznMes.Domain.Entities.Equip.EquipControl;

/// <summary>
/// 设备绑定表
/// </summary>
public class EquipConnectForwardEntity:UniversalEntity,ICreationAudited
{
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }=default!;
    [Description( "源链接")]
    public Guid OriginatorId { get; set; }
    [Description( "目标链接")]
    public Guid TargetId { get; set; }
}
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared.Enums;
using System.ComponentModel;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipControl;

/// <summary>
/// 设备采集信息配置表
/// </summary>
[Description("设备采集信息配置表")]
public class EquipConnect : UniversalEntity, ISoftDelete, IState, IOrder, IAudited
{
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public bool State { get; set; } = true;

    [Description("数据名称")]
    public string? Name { get; set; }

    [Description("数据编号")]
    public string? Code { get; set; }

    [Description("设备Id")]
    public Guid EquipId { get; set; }

    [Description("采集协议")]
    public Protocol ProtocolEnum { get; set; }

    [Description("连接参数")]
    public string? ConnectStr { get; set; }

    [Description("采集频率")]
    public Guid? CollectionConfigId { get; set; }

    /// <summary>
    /// 扩展字段：
    /// RfidReaderClient模式下 0采集数据，1绑定标签 2解绑标签
    /// </summary>
    [Description("采集扩展字段")]
    public int? CollectionExtension { get; set; }

    // [Navigate(NavigateType.OneToOne, nameof(CollectionConfigId))]
    // public CollectionConfig? CollectionConfig { get; set; }

    // [Navigate(NavigateType.OneToOne, nameof(EquipId))]
    public EquipLedger? EquipLedger { get; set; }

    // [Navigate(NavigateType.OneToMany, nameof(EquipConnectForwardEntity.OriginatorId))]
    public IEnumerable<EquipConnectForward>? ForwardEntities { get; set; }

    [Description("转发频率")]
    public int? ForwardRate { get; set; }

    public bool SoftDeleted { get; set; }
    public DateTime? DeleteTime { get; set; }
    public int OrderNum { get; set; }
}
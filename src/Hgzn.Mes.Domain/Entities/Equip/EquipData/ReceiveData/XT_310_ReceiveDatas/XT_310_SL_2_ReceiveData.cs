using System.ComponentModel;
using Hgzn.Mes.Domain.Attributes;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_310_ReceiveDatas;

/// <summary>
/// 310_雷达转台
/// </summary>
public class XT_310_SL_2_ReceiveData : UniversalEntity, IAudited
{
    [TableNotShow]
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }

    [Description("仿真试验系统识别编码")]
    [TableNotShow]
    public byte SimuTestSysld { get; set; }

    [Description("设备类型识别编码")]
    [TableNotShow]
    public byte DevTypeld { get; set; }

    [Description("本机识别编码")]
    public string? Compld { get; set; }

    #region 工作模式信息

    [Description("运行状态")]
    [TableNotShow]
    public byte OperationStatus { get; set; }

    [Description("是否接入弹道状态")]
    [TableNotShow]
    public byte IsTrajectoryConnected { get; set; }

    [Description("预留")]
    [TableNotShow]
    public byte Reserved { get; set; }

    #endregion

    #region 健康状态信息

    [Description("状态类型")]
    [TableNotShow]
    public byte StatusType { get; set; }

    [Description("自检状态")]
    [TableNotShow]
    public byte SelfTestStatus { get; set; }

    [Description("运行状态")]
    [TableNotShow]
    public byte HealthOperationStatus { get; set; }

    #endregion

    #region 物理量

    [Description("物理量参数数量")]
    [TableNotShow]
    public uint PhysicalQuantityCount { get; set; }

    [Description("内框位置")]
    public float InnerFramePosition { get; set; }

    [Description("中框位置")]
    public float MiddleFramePosition { get; set; }

    [Description("外框位置")]
    public float OuterFramePosition { get; set; }

    [Description("内框速度")]
    public float InnerFrameVelocity { get; set; }

    [Description("中框速度")]
    public float MiddleFrameVelocity { get; set; }

    [Description("外框速度")]
    public float OuterFrameVelocity { get; set; }

    [Description("内框加速度")]
    public float InnerFrameAcceleration { get; set; }

    [Description("中框加速度")]
    public float MiddleFrameAcceleration { get; set; }

    [Description("外框加速度")]
    public float OuterFrameAcceleration { get; set; }

    #endregion

    [Description("运行时间")]
    [TableNotShow]
    public uint? RunTime { get; set; }

    [TableNotShow]
    public Guid? LastModifierId { get; set; }
    [TableNotShow]
    public DateTime? LastModificationTime { get; set; }
    [TableNotShow]
    public int CreatorLevel { get; set; } = 0;
}

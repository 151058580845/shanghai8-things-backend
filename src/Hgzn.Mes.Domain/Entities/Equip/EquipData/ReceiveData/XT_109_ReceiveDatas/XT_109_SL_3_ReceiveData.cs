using System.ComponentModel;
using Hgzn.Mes.Domain.Attributes;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_109_ReceiveDatas;

/// <summary>
/// 109_红外转台
/// </summary>
public class XT_109_SL_3_ReceiveData : UniversalEntity, IAudited
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

    #region 工作信息模式 3个

    [Description("本地还是远程")]
    [TableNotShow]
    public byte LocalOrRemote { get; set; }

    [Description("工作模式")]
    [TableNotShow]
    public byte WorkPattern { get; set; }

    [Description("预留")]
    [TableNotShow]
    public byte Reserved { get; set; }

    #endregion

    #region 健康状态信息 6个

    [Description("状态类型")]
    [TableNotShow]
    public byte StatusType { get; set; }

    [Description("滚转轴工作状态")]
    [TableNotShow]
    public short RollingAxisOperationStatus { get; set; }
    [Description("俯仰轴工作状态")]
    [TableNotShow]
    public short PitchAxisOperationStatus { get; set; }
    [Description("偏航轴工作状态")]
    [TableNotShow]
    public short YawAxisOperationStatus { get; set; }
    [Description("高低轴工作状态")]
    [TableNotShow]
    public short ElevationAxisOperationStatus { get; set; }
    [Description("方位轴工作状态")]
    [TableNotShow]
    public short AzimuthAxisOperationStatus { get; set; }

    #endregion

    #region 物理量

    // 物理量参数数量
    [Description("物理量参数数量")]
    [TableNotShow]
    public uint PhysicalQuantityCount { get; set; }

    // 三轴转台控制参数
    [Description("三轴转台滚动轴给定")]
    public float ThreeAxisRollGiven { get; set; }
    [Description("三轴转台偏航轴给定")]
    public float ThreeAxisYawGiven { get; set; }
    [Description("三轴转台俯仰轴给定")]
    public float ThreeAxisPitchGiven { get; set; }

    // 两轴转台控制参数
    [Description("两轴转台方位轴给定")]
    public float TwoAxisAzimuthGiven { get; set; }
    [Description("两轴转台高低轴给定")]
    public float TwoAxisPitchGiven { get; set; }

    // 三轴转台反馈参数
    [Description("三轴转台滚动轴反馈")]
    public float ThreeAxisRollFeedback { get; set; }
    [Description("三轴转台偏航轴反馈")]
    public float ThreeAxisYawFeedback { get; set; }
    [Description("三轴转台俯仰轴反馈")]
    public float ThreeAxisPitchFeedback { get; set; }

    // 两轴转台反馈参数
    [Description("两轴转台方位轴反馈")]
    public float TwoAxisAzimuthFeedback1 { get; set; }
    [Description("两轴转台高低轴反馈")]
    public float TwoAxisPitchFeedback { get; set; }

    // 液压系统参数
    [Description("油压")]
    public float OilPressure { get; set; }
    [Description("油温")]
    public float OilTemperature { get; set; }

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

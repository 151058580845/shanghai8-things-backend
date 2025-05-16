using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common
{
    /// <summary>
    /// 通用_状态类型
    /// </summary>
    public enum General_StatusType
    {
        [Description("未获取设备健康状态")]
        DeviceHealthNotAcquired = 0,
        [Description("已获取设备健康状态")]
        DeviceHealthAcquired = 1,
    }

    /// <summary>
    /// 通用_运行状态/自检状态
    /// </summary>
    public enum General_Status
    {
        [Description("正常")]
        Normal,
        [Description("异常")]
        Abnormal,
    }

    /// <summary>
    /// 通用_是否满足要求
    /// </summary>
    public enum General_Require
    {
        [Description("不满足要求")]
        NotSatisfyRequirement,
        [Description("满足要求")]
        SatisfyRequirement
    }

    /// <summary>
    /// SL_3工作状态
    /// </summary>
    public enum SL_3_OperationStatusEnum
    {
        [Description("正常状态")]
        NormalStatus = 0,
        [Description("软件限位")]
        SoftwareLimit = 1,
        [Description("超速")]
        OverSpeed = 2,
        [Description("飞车")]
        Runaway,
        [Description("反馈异常")]
        FeedbackError,
    }

    /// <summary>
    /// XT_202_SL_3工作状态(转台里比较特殊的一台)
    /// </summary>
    public enum XT_202_SL_3_OperationStatusEnum
    {
        [Description("正常状态")]
        Normal = 0,
        [Description("软件正限位")]
        SoftwarePositiveLimit = 1,
        [Description("软件负限位")]
        SoftwareNegativeLimit = 2,
        [Description("超速")]
        OverSpeed = 3,
        [Description("限DA")]
        DA_Limit = 4,
        [Description("飞车")]
        Runaway = 5,
        [Description("锁紧")]
        Locked = 6,
        [Description("寻零失败")]
        HomingFailed = 7,
        [Description("驱动故障")]
        DriveFault = 8,
        [Description("反馈异常")]
        FeedbackError = 9,
        [Description("电开关限位")]
        ElectricalSwitchLimit = 10,
        [Description("相对位置限位")]
        RelativePositionLimit = 11
    }

    /// <summary>
    /// 设备单元状态枚举
    /// </summary>
    public enum UnitStatus
    {
        [Description("主控存储单元状态")]
        MainControlStorageUnit = 0,
        [Description("光纤反射内存单元状态")]
        FiberOpticReflectiveMemoryUnit = 1,
        [Description("频综单元状态")]
        FrequencySynthesizerUnit = 2,
        [Description("下变频单元状态")]
        DownConverterUnit = 3,
        [Description("基带信号处理单元DRFM1状态")]
        BasebandProcessingDRFM1 = 4,
        [Description("基带信号处理单元DRFM2状态")]
        BasebandProcessingDRFM2 = 5,
        [Description("上变频单元1状态")]
        UpConverterUnit1 = 6,
        [Description("上变频单元2状态")]
        UpConverterUnit2 = 7
    }
}

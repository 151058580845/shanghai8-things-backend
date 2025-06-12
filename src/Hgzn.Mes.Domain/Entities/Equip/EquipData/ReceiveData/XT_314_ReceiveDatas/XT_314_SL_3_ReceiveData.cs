using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_314_ReceiveDatas
{
    /// <summary>
    /// 314_红外转台
    /// </summary>
    public class XT_314_SL_3_ReceiveData : UniversalEntity, IAudited
    {
        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }

        [Description("仿真试验系统识别编码")]
        public byte SimuTestSysld { get; set; }

        [Description("设备类型识别编码")]
        public byte DevTypeld { get; set; }

        [Description("本机识别编码")]
        public string? Compld { get; set; }

        #region 工作模式信息

        [Description("本地还是远程")]
        public byte LocalOrRemote { get; set; }

        [Description("工作模式")]
        public byte WorkPattern { get; set; }

        [Description("预留")]
        public byte Reserved { get; set; }

        #endregion

        #region 健康状态信息

        [Description("状态类型")]
        public byte StatusType { get; set; }

        [Description("消旋轴工作状态")]
        public byte RacemizationAxisOperationStatus { get; set; }
        [Description("短臂轴工作状态")]
        public byte ShortArmAxisOperationStatus { get; set; }
        [Description("长臂轴工作状态")]
        public byte LongArmAxisOperationStatus { get; set; }

        #endregion

        #region 物理量

        [Description("物理量参数数量")]
        public uint PhysicalParameterCount { get; set; }

        [Description("旋转轴给定")]
        public float RotationAxisCommand { get; set; }

        [Description("小臂轴给定")]
        public float ForearmAxisCommand { get; set; }

        [Description("大臂轴给定")]
        public float ArmAxisCommand { get; set; }

        [Description("消旋轴反馈")]
        public float DerotationAxisFeedback { get; set; }

        [Description("小臂轴反馈")]
        public float ForearmAxisFeedback { get; set; }

        [Description("大臂轴反馈")]
        public float ArmAxisFeedback { get; set; }
        [Description("周期")]
        public float Period { get; set; }

        #endregion

        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}

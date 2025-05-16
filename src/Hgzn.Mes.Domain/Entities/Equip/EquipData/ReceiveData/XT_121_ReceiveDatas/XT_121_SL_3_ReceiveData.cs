using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_121_ReceiveDatas
{
    /// <summary>
    /// 121_红外转台
    /// </summary>
    public class XT_121_SL_3_ReceiveData : UniversalEntity, IAudited
    {
        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }
        [Description("仿真试验系统识别编码")]
        public byte SimuTestSysld { get; set; }

        [Description("设备类型识别编码")]
        public byte DevTypeld { get; set; }

        [Description("本机识别编码")]
        public string? Compld { get; set; }

        #region 工作信息模式

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

        [Description("工作状态")]
        public byte OperationStatus { get; set; }

        #endregion

        #region 物理量

        // 物理量参数数量
        [Description("物理量参数数量")]
        public uint PhysicalQuantityCount { get; set; }

        // 三轴转台给定值
        [Description("三轴转台滚动轴给定")]
        public float ThreeAxisRollGiven { get; set; }
        [Description("三轴转台偏航轴给定")]
        public float ThreeAxisYawGiven { get; set; }
        [Description("三轴转台俯仰轴给定")]
        public float ThreeAxisPitchGiven { get; set; }

        // 两轴转台给定值
        [Description("两轴转台方位轴给定")]
        public float TwoAxisAzimuthGiven { get; set; }
        [Description("两轴转台俯仰轴给定")]
        public float TwoAxisPitchGiven { get; set; }

        // 三轴转台反馈值
        [Description("三轴转台滚动轴反馈")]
        public float ThreeAxisRollFeedback { get; set; }
        [Description("三轴转台偏航轴反馈")]
        public float ThreeAxisYawFeedback { get; set; }
        [Description("三轴转台俯仰轴反馈")]
        public float ThreeAxisPitchFeedback { get; set; }

        // 两轴转台反馈值
        [Description("两轴转台方位轴反馈")]
        public float TwoAxisAzimuthFeedback { get; set; }
        [Description("两轴转台方位轴反馈")]
        public float TwoAxisPitchFeedback { get; set; }

        #endregion

        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}

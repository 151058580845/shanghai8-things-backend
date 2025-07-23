using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_112_ReceiveDatas
{
    /// <summary>
    /// _雷达转台
    /// </summary>
    public class XT_112_SL_2_ReceiveData : UniversalEntity, IAudited
    {
        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }

        [Description("仿真试验系统识别编码")]
        public byte SimuTestSysld { get; set; }

        [Description("设备类型识别编码")]
        public byte DevTypeld { get; set; }

        [Description("本机识别编码")]
        public string? Compld { get; set; }

        #region 工作模式信息 3个

        [Description("运行状态")]
        public byte OperationStatus { get; set; }

        [Description("是否接入弹道状态")]
        public byte IsTrajectoryConnected { get; set; }

        [Description("预留")]
        public byte Reserved { get; set; }

        #endregion

        #region 健康状态信息 4个

        [Description("状态类型")]
        public byte StatusType { get; set; }

        [Description("滚转轴工作状态")]
        public byte RollingAxisOperationStatus { get; set; }
        [Description("俯仰轴工作状态")]
        public byte PitchAxisOperationStatus { get; set; }
        [Description("偏航轴工作状态")]
        public byte YawAxisOperationStatus { get; set; }

        #endregion

        #region 物理量

        [Description("物理量参数数量")]
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
        public uint? RunTime { get; set; }

        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int CreatorLevel { get; set; } = 0;
    }
}

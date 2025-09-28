using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hgzn.Mes.Domain.Attributes;
using System.ComponentModel;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_103_ReceiveDatas
{
    public class XT_103_SL_1_ReceiveData : UniversalEntity, IAudited
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

        #region 工作模式信息 3个

        [Description("开启/关闭")]
        [TableNotShow]
        public byte TurnOnOrOff { get; set; }

        [Description("本地/远程")]
        [TableNotShow]
        public byte LocalOrRemote { get; set; }

        [Description("预留")]
        [TableNotShow]
        public byte Reserved { get; set; }

        #endregion

        #region 健康状态信息 5个

        [Description("状态类型")]
        [TableNotShow]
        public byte StatusType { get; set; }

        [Description("横移轴状态")]
        [TableNotShow]
        public byte TraverseAxisStatus { get; set; }

        [Description("升降轴状态")]
        [TableNotShow]
        public byte LiftAxisStatus { get; set; }

        [Description("前进轴状态")]
        [TableNotShow]
        public byte AdvanceAxisStatus { get; set; }

        [Description("云台状态")]
        [TableNotShow]
        public byte PanTiltStatus { get; set; }

        #endregion

        #region 物理量

        [Description("物理量参数数量")]
        [TableNotShow]
        public uint PhysicalQuantityCount { get; set; }

        [Description("横移轴位置")]
        public double TraverseAxisPosition { get; set; }

        [Description("横移轴速度")]
        public double TraverseAxisVelocity { get; set; }

        [Description("升降轴位置")]
        public double LiftAxisPosition { get; set; }

        [Description("升降轴速度")]
        public double LiftAxisVelocity { get; set; }

        [Description("前进轴位置")]
        public double AdvanceAxisPosition { get; set; }

        [Description("前进轴速度")]
        public double AdvanceAxisVelocity { get; set; }

        [Description("云台方位轴位置")]
        public double PanTiltAzimuthPosition { get; set; }

        [Description("云台方位轴速度")]
        public double PanTiltAzimuthVelocity { get; set; }

        [Description("云台俯仰轴位置")]
        public double PanTiltElevationPosition { get; set; }

        [Description("云台俯仰轴速度")]
        public double PanTiltElevationVelocity { get; set; }

        [Description("机械阵ex")]
        public double MechanicalArrayEx { get; set; }

        [Description("机械阵bx")]
        public double MechanicalArrayBx { get; set; }

        [Description("机械阵方位偏置")]
        public double MechanicalArrayAzimuthOffset { get; set; }

        [Description("机械阵俯仰偏置")]
        public double MechanicalArrayElevationOffset { get; set; }

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
}

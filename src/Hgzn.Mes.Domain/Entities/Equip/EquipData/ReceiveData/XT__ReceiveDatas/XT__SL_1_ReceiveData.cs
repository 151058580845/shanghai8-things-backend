using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT__ReceiveDatas
{
    /// <summary>
    /// _阵列馈电
    /// </summary>
    public class XT__SL_1_ReceiveData : UniversalEntity, IAudited
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

        [Description("微波/毫米波")]
        public byte MicroWare { get; set; }

        [Description("通道接入")]
        public byte Channel { get; set; }

        [Description("模式是否有效")]
        public byte ModelValid { get; set; }

        [Description("阵面末端极化方式")]
        public byte ArrayEndPolarizationMode { get; set; }

        [Description("阵面末端功率模式")]
        public byte ArrayEndPowerMode { get; set; }

        [Description("阵列通道复用")]
        public byte ArrayChannelMultiplexing { get; set; }

        [Description("通道极化方式1")]
        public byte ChannelPolarizationMode1 { get; set; }

        [Description("通道极化方式2")]
        public byte ChannelPolarizationMode2 { get; set; }

        [Description("通道功率模式")]
        public byte ChannelPowerMode { get; set; }

        [Description("预留")]
        public byte Reserved { get; set; }

        #endregion

        #region 健康状态信息(公共定义)

        //[Description("状态类型")]
        //public byte StateType { get; set; }

        //[Description("自检状态")]
        //public uint SelfTest { get; set; }

        //[Description("电源电压状态")]
        //public uint SupplyVoltageState { get; set; }

        #endregion

        #region 物理量



        #endregion

        [Description("运行时间")]
        public uint? RunTime { get; set; }

        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int CreatorLevel { get; set; } = 0;
    }
}

using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_0_ReceiveDatas
{
    /// <summary>
    /// 移动设备_雷达源
    /// </summary>
    public class XT_0_SL_6_ReceiveDatas : UniversalEntity, IAudited
    {
        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }

        [Description("仿真试验系统识别编码")]
        public byte SimuTestSysld { get; set; }

        [Description("设备类型识别编码")]
        public byte DevTypeld { get; set; }

        [Description("本机识别编码")]
        public string? Compld { get; set; }

        #region 工作模式信息 6个

        [Description("本地还是远程")]
        public byte LocalOrRemote { get; set; }

        [Description("通道使能")]
        public byte ChannelEnabled { get; set; }

        [Description("工作模式")]
        public byte OperationMode { get; set; }

        [Description("测频使能")]
        public byte FrequencyMeasurementEnabled { get; set; }

        [Description("速度/多普勒有效")]
        public byte DopplerDataValid { get; set; }

        [Description("预留")]
        public byte Reserved { get; set; }

        #endregion

        #region 健康状态信息 3个(注意,此处的自检状态类型是ushort)

        [Description("状态类型")]
        public byte StatusType { get; set; }

        [Description("自检状态")]
        public ushort SelfTestStatus { get; set; }

        #endregion

        #region 物理量

        [Description("物理量参数数量")]
        public uint PhysicalParameterCount { get; set; }

        [Description("主控和DRFM板卡1网络连接状态")]
        public byte MainCtrlDrfm1NetStatus { get; set; }

        [Description("主控和DRFM板卡2网络连接状态")]
        public byte MainCtrlDrfm2NetStatus { get; set; }

        [Description("FPGA1连接状态")]
        public byte Fpga1Status { get; set; }

        [Description("FPGA2连接状态")]
        public byte Fpga2Status { get; set; }

        [Description("DSP1连接状态")]
        public byte Dsp1Status { get; set; }

        [Description("DSP2连接状态")]
        public byte Dsp2Status { get; set; }

        [Description("FPGA1和ADC1和DAC1连接状态")]
        public byte Fpga1Adc1Dac1Status { get; set; }

        [Description("FPGA2和ADC2和DAC2连接状态")]
        public byte Fpga2Adc2Dac2Status { get; set; }

        [Description("FPGA1和DSP1的SRIO连接状态")]
        public byte Fpga1Dsp1SrioStatus { get; set; }

        [Description("FPGA2和DSP2的SRIO连接状态")]
        public byte Fpga2Dsp2SrioStatus { get; set; }

        [Description("DRFM板卡1温度")]
        public float Drfm1Temperature { get; set; }

        [Description("DRFM板卡2温度")]
        public float Drfm2Temperature { get; set; }

        [Description("微波控制板状态")]
        public byte MicrowaveCtrlStatus { get; set; }

        [Description("频综单元状态")]
        public byte FrequencySynthesizerStatus { get; set; }

        [Description("中频输入功率")]
        public float IfInputPower { get; set; }

        [Description("中频输出功率")]
        public float IfOutputPower { get; set; }

        [Description("备份1")]
        public float Backup1 { get; set; }

        [Description("备份2")]
        public float Backup2 { get; set; }

        [Description("备份3")]
        public float Backup3 { get; set; }

        [Description("备份4")]
        public float Backup4 { get; set; }

        [Description("备份5")]
        public float Backup5 { get; set; }

        [Description("备份6")]
        public float Backup6 { get; set; }

        [Description("备份7")]
        public float Backup7 { get; set; }

        [Description("备份8")]
        public float Backup8 { get; set; }

        #endregion

        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}

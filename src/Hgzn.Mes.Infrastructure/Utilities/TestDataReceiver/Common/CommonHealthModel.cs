using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common
{
    /// <summary>
    /// 雷达转台
    /// </summary>
    public class SL_2_CommonHealthModel
    {
        [Description("状态类型")]
        public byte StatusType { get; set; }

        [Description("自检状态")]
        public byte SelfTestStatus { get; set; }

        [Description("运行状态")]
        public byte HealthOperationStatus { get; set; }
    }

    /// <summary>
    /// 固定电源
    /// </summary>
    public class SL_4_CommonHealthModel
    {
        [Description("状态类型")]
        public byte StatusType { get; set; }

        [Description("工作状态")]
        public byte OperationStatus { get; set; }
    }

    /// <summary>
    /// 红外源
    /// </summary>
    public class SL_7_CommonHealthModel
    {
        [Description("状态类型")]
        public byte StatusType { get; set; }

        [Description("露点温度状态")]
        public byte DewPointTemperatureStatus { get; set; }

        [Description("真空度状态")]
        public byte VacuumStatus { get; set; }

        [Description("冷水机流量状态")]
        public byte ChillerFlowStatus { get; set; }

        [Description("环境箱温度状态")]
        public byte EnvironmentalChamberTemperatureStatus { get; set; }

        [Description("衬底温度状态")]
        public byte SubstrateTemperatureStatus { get; set; }

        [Description("功率电源状态")]
        public byte PowerSupplyStatus { get; set; }

        [Description("控制电源状态")]
        public byte ControlPowerStatus { get; set; }
    }


    public enum XT_0_SL_6DeviceStatus
    {
        [Description("主控存储单元状态异常")]
        MainControlStorage = 1 << 0,

        [Description("光纤反射内存单元状态异常")]
        FiberOpticMemory = 1 << 1,

        [Description("频综单元状态异常")]
        FrequencySynthesizer = 1 << 2,

        [Description("下变频单元状态异常")]
        DownConverter = 1 << 3,

        [Description("基带信号处理单元DRFM1状态异常")]
        BasebandDRFM1 = 1 << 4,

        [Description("基带信号处理单元DRFM2状态异常")]
        BasebandDRFM2 = 1 << 5,

        [Description("上变频单元1状态异常")]
        UpConverter1 = 1 << 6,

        [Description("上变频单元2状态异常")]
        UpConverter2 = 1 << 7
    }
}

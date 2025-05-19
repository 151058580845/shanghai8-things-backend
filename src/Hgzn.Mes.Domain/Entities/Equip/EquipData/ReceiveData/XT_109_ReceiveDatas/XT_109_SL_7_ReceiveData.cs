using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_109_ReceiveDatas
{
    /// <summary>
    /// 109_红外源
    /// </summary>
    public class XT_109_SL_7_ReceiveData : UniversalEntity, IAudited
    {
        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }

        [Description("仿真试验系统识别编码")]
        public byte SimuTestSysld { get; set; }

        [Description("设备类型识别编码")]
        public byte DevTypeld { get; set; }

        [Description("本机识别编码")]
        public string? Compld { get; set; }

        #region 工作模式信息 2个

        [Description("开启/关闭")]
        public byte OpenOrClose { get; set; }

        [Description("预留")]
        public byte Reserved { get; set; }

        #endregion

        #region 健康状态信息

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

        #endregion

        #region 物理量

        [Description("物理量参数数量")]
        public uint PhysicalQuantityCount { get; set; }

        [Description("真空度")]
        public float Vacuum { get; set; }

        [Description("环境箱温度")]
        public float ChamberTemperature { get; set; }

        [Description("衬底温度")]
        public float SubstrateTemp { get; set; }

        [Description("环境温度")]
        public float AmbientTemp { get; set; }

        [Description("环境湿度")]
        public float Humidity { get; set; }

        [Description("露点温度")]
        public float DewPoint { get; set; }

        [Description("功率电源电压")]
        public float PowerVoltage { get; set; }

        [Description("功率电源电流")]
        public float PowerCurrent { get; set; }

        [Description("控制电源电压")]
        public float CtrlVoltage { get; set; }

        [Description("控制电源电流")]
        public float CtrlCurrent { get; set; }

        [Description("控制电源逻辑电压")]
        public float LogicVoltage { get; set; }

        [Description("控制电源逻辑电流")]
        public float LogicCurrent { get; set; }

        [Description("液体温度")]
        public float LiquidTemp { get; set; }

        [Description("冷却液流量")]
        public float CoolantFlow { get; set; }

        #endregion

        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}

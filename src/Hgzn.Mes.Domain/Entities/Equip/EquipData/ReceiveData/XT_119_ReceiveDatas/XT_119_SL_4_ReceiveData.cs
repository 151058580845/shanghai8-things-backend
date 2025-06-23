using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_119_ReceiveDatas
{
    /// <summary>
    /// _固定电源
    /// </summary>
    public class XT_119_SL_4_ReceiveData : UniversalEntity, IAudited
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

        [Description("电源数量")]
        public byte PowerSupplyCount { get; set; }

        [Description("电源类型1")]
        public byte PowerSupplyType1 { get; set; }

        [Description("电源类型2")]
        public byte PowerSupplyType2 { get; set; }

        [Description("是否上电")]
        public byte IsPoweredOn { get; set; }

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

        [Description("物理量参数数量")]
        public uint PhysicalParameterCount { get; set; }

        [Description("电源1电压设置值")]
        public float Power1VoltageSet { get; set; }

        [Description("电源1电流设置值")]
        public float Power1CurrentSet { get; set; }

        [Description("电源2电压设置值")]
        public float Power2VoltageSet { get; set; }

        [Description("电源2电流设置值")]
        public float Power2CurrentSet { get; set; }

        [Description("电源3电压设置值")]
        public float Power3VoltageSet { get; set; }

        [Description("电源3电流设置值")]
        public float Power3CurrentSet { get; set; }

        [Description("电源4电压设置值")]
        public float Power4VoltageSet { get; set; }

        [Description("电源4电流设置值")]
        public float Power4CurrentSet { get; set; }

        [Description("电源5电压设置值")]
        public float Power5VoltageSet { get; set; }

        [Description("电源5电流设置值")]
        public float Power5CurrentSet { get; set; }

        [Description("电源6电压设置值")]
        public float Power6VoltageSet { get; set; }

        [Description("电源6电流设置值")]
        public float Power6CurrentSet { get; set; }

        [Description("电源7电压设置值")]
        public float Power7VoltageSet { get; set; }

        [Description("电源7电流设置值")]
        public float Power7CurrentSet { get; set; }

        [Description("电源8电压设置值")]
        public float Power8VoltageSet { get; set; }

        [Description("电源8电流设置值")]
        public float Power8CurrentSet { get; set; }

        [Description("电源1电压采集值")]
        public float Power1VoltageRead { get; set; }

        [Description("电源1电流采集值")]
        public float Power1CurrentRead { get; set; }

        [Description("电源2电压采集值")]
        public float Power2VoltageRead { get; set; }

        [Description("电源2电流采集值")]
        public float Power2CurrentRead { get; set; }

        [Description("电源3电压采集值")]
        public float Power3VoltageRead { get; set; }

        [Description("电源3电流采集值")]
        public float Power3CurrentRead { get; set; }

        [Description("电源4电压采集值")]
        public float Power4VoltageRead { get; set; }

        [Description("电源4电流采集值")]
        public float Power4CurrentRead { get; set; }

        [Description("电源5电压采集值")]
        public float Power5VoltageRead { get; set; }

        [Description("电源5电流采集值")]
        public float Power5CurrentRead { get; set; }

        [Description("电源6电压采集值")]
        public float Power6VoltageRead { get; set; }

        [Description("电源6电流采集值")]
        public float Power6CurrentRead { get; set; }

        [Description("电源7电压采集值")]
        public float Power7VoltageRead { get; set; }

        [Description("电源7电流采集值")]
        public float Power7CurrentRead { get; set; }

        [Description("电源8电压采集值")]
        public float Power8VoltageRead { get; set; }

        [Description("电源8电流采集值")]
        public float Power8CurrentRead { get; set; }

        #endregion

        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int CreatorLevel { get; set; } = 0;
    }
}

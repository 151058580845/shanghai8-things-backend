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
    /// 移动设备_移动电源
    /// </summary>
    public class XT_0_SL_5_ReceiveData : UniversalEntity, IAudited
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

        [Description("电源类型")]
        public byte PowerSupplyType { get; set; }

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

        [Description("电源电压设置值")]
        public float PowerVoltageSet { get; set; }

        [Description("电源电流设置值")]
        public float PowerCurrentSet { get; set; }

        [Description("电源电压采集值")]
        public float PowerVoltageRead { get; set; }

        [Description("电源电流采集值")]
        public float PowerCurrentRead { get; set; }

        #endregion

        [Description("运行时间")]
        public uint? RunTime { get; set; }

        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int CreatorLevel { get; set; } = 0;
    }
}

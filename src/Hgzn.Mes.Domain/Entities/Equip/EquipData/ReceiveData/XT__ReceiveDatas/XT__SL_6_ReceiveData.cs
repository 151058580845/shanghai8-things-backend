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
    /// _雷达源
    /// </summary>
    public class XT__SL_6_ReceiveData : UniversalEntity, IAudited
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

        #region 健康状态信息



        #endregion

        #region 物理量



        #endregion

        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int CreatorLevel { get; set; } = 0;
    }
}

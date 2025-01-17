using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.ExperimentData
{
    public class ReceiveExperimentData : UniversalEntity, IAudited
    {
        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }

        [Description("仿真系统资源名称")]
        public string? SysName { get; set; }
        [Description("型号项⽬名称")]
        public string? ProjectName { get; set; }
        [Description("试验名称")]
        public string? TaskName { get; set; }
        [Description("当前型号研制阶段")]
        public string? DevPhase { get; set; }
        [Description("当前试验起始⽇期")]
        public string? TaskStartTime { get; set; }
        [Description("当前试验计划结束⽇期")]
        public string? TaskEndTime { get; set; }
        [Description("申请项⽬办")]
        public string? ReqDep { get; set; }
        [Description("申请调度人")]
        public string? ReqManager { get; set; }
        [Description("试验专业代表")]
        public string? SimuResp { get; set; }
        [Description("试验参与⼈员")]
        public string? simuStaff { get; set; }
        [Description("制导控制专业代表")]
        public string? GncResp { get; set; }
        [Description("UUT数据信息")]
        public UUT[] UUTs { get; set; }
        
    }

    public class UUT
    {
        [Description("产品名称")]
        public string? Name { get; set; }

        [Description("产品编号")]
        public string? Code { get; set; }

        [Description("产品技术状态")]
        public string? TechnicalStatus { get; set; }
    }

}

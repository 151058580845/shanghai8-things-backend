using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData;

[Description("试验计划")]
public class TestData : UniversalEntity
{
    [Description("试验计划ID")]
    public string? TestDataId { get; set; }
    [Description("仿真系统资源名称")]
    public string? SysName { get; set; }
    [Description("型号项目名称")]
    public string? ProjectName { get; set; }
    [Description("试验名称")]
    public string? TaskName { get; set; }
    [Description("当前型号研制阶段")]
    public string? DevPhase { get; set; }
    [Description("当前试验起始日期")]
    public string? TaskStartTime { get; set; }
    [Description("当前试验计划结束日期")]
    public string? TaskEndTime { get; set; }
    [Description("申请项目办")]
    public string? ReqDep { get; set; }
    [Description("申请调度,多个人名用 英文‘,’ 分隔")]
    public string? ReqManager { get; set; }
    [Description("申请调度员工编号")]
    public string? ReqManagerCode { get; set; }
    [Description("系统责任人,多个人名用 英文‘,’ 分隔")]
    public string? GncResp { get; set; }
    [Description("系统责任人员工编号")]
    public string? GncRespCode { get; set; }
    [Description("试验专业代表,多个人名用 英文‘,’ 分隔")]
    public string? SimuResp { get; set; }
    [Description("试验专业代表员工编号")]
    public string? simuRespCode { get; set; }
    [Description("试验参与人员,多个人名用 英文‘,’ 分隔")]
    public string? SimuStaff { get; set; }
    [Description("试验参与人员代码编号,多个人名用 英文‘,’ 分隔")]
    public string? simuStaffCodes { get; set; }
    [Description("制导控制专业代表，多个人名用 英文‘,’ 分隔")]
    public string? QncResp { get; set; }

    public List<TestDataProduct>? UUT { get; set; }

}
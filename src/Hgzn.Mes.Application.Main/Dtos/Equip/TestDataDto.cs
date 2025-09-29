using System.ComponentModel;
using System.Text.Json.Serialization;
using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;

namespace Hgzn.Mes.Application.Main.Dtos.Equip;

public class TestDataDto
{
    
}

public class TestDataReadDto : ReadDto
{
    [Description("试验计划ID")]
    public string TestDataId { get; set; }
    [Description("仿真系统资源名称")]
    public string SysName { get; set; }
    [Description("型号项目名称")]
    public string ProjectName { get; set; }
    [Description("试验名称")]
    public string TaskName { get; set; }
    [Description("当前型号研制阶段")]
    public string DevPhase { get; set; }
    [Description("当前试验起始日期")]
    public string TaskStartTime { get; set; }
    [Description("当前试验计划结束日期")]
    public string TaskEndTime { get; set; }
    [Description("申请项目办")]
    public string ReqDep { get; set; }
    [Description("申请调度,多个人名用 英文‘,’ 分隔")]
    public string ReqManager { get; set; }
    [Description("申请调度员工编号")]
    public string ReqManagerCode { get; set; }
    [Description("制导控制专业代表,多个人名用 英文‘,’ 分隔")]
    public string GncResp { get; set; }
    [Description("制导控制专业代表员工编号")]
    public string GncRespCode { get; set; }
    [Description("试验专业代表,多个人名用 英文‘,’ 分隔")]
    public string SimuResp { get; set; }
    [Description("试验专业代表员工编号")]
    public string SimuRespCode { get; set; }
    [Description("试验参与人员,多个人名用 英文‘,’ 分隔")]
    public string SimuStaff { get; set; }
    [Description("试验参与人员代码编号,多个人名用 英文‘,’ 分隔")]
    public string simuStaffCodes { get; set; }
    [Description("制导控制专业代表，多个人名用 英文‘,’ 分隔,没有这个字段")]
    public string QncResp { get; set; }
    [Description("产品信息")]
    public string ProductInfo { get; set; }
    public List<TestDataProductReadDto> UUT { get; set; }
    
    public List<TestDataUSTReadDto>? UST { get; set; }
    
    [Description("当前状态")]
    public string? Status { get; set; }

    [Description("试验结果")] public string? Result { get; set; } = "未知";
}

public class TestDataCreateDto : CreateDto
{
    [Description("试验计划ID")]
    [JsonPropertyName("ID")]
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
    public string TaskStartTime { get; set; }
    [Description("当前试验计划结束日期")]
    public string TaskEndTime { get; set; }
    [Description("申请项目办")]
    public string ReqDep { get; set; }
    [Description("申请调度,多个人名用 英文‘,’ 分隔")]
    public string ReqManager { get; set; }
    [Description("申请调度员工编号")]
    public string ReqManagerCode { get; set; }
    [Description("系统责任人,多个人名用 英文‘,’ 分隔")]
    public string GncResp { get; set; }
    [Description("系统责任人员工编号")]
    public string GncRespCode { get; set; }
    public string SimuResp { get; set; }
    [Description("试验专业代表员工编号")]
    public string simuRespCode { get; set; }
    [Description("试验参与人员,多个人名用 英文‘,’ 分隔")]
    public string SimuStaff { get; set; }
    [Description("试验参与人员代码编号,多个人名用 英文‘,’ 分隔")]
    public string simuStaffCodes { get; set; }
    [Description("制导控制专业代表，多个人名用 英文‘,’ 分隔")]
    public string QncResp { get; set; }
    public List<TestDataProductCreateDto>? UUT { get; set; }
    
    [JsonPropertyName("UST")]
    public List<TestDataUSTCreateDto>? UST { get; set; }
}

public class TestDataUpdateDto : UpdateDto
{
    [Description("试验计划ID")]
    public string TestDataId { get; set; }
    [Description("仿真系统资源名称")]
    public string SysName { get; set; }
    [Description("型号项目名称")]
    public string ProjectName { get; set; }
    [Description("试验名称")]
    public string TaskName { get; set; }
    [Description("当前型号研制阶段")]
    public string DevPhase { get; set; }
    [Description("当前试验起始日期")]
    public string TaskStartTime { get; set; }
    [Description("当前试验计划结束日期")]
    public string TaskEndTime { get; set; }
    [Description("申请项目办")]
    public string ReqDep { get; set; }
    [Description("申请调度,多个人名用 英文‘,’ 分隔")]
    public string ReqManager { get; set; }
    [Description("申请调度员工编号")]
    public string ReqManagerCode { get; set; }
    [Description("系统责任人,多个人名用 英文‘,’ 分隔")]
    public string GncResp { get; set; }
    [Description("系统责任人员工编号")]
    public string GncRespCode { get; set; }
    [Description("试验专业代表,多个人名用 英文‘,’ 分隔")]
    public string? SimuResp { get; set; }
    [Description("试验专业代表员工编号")]
    public string? simuRespCode { get; set; }
    [Description("试验参与人员,多个人名用 英文‘,’ 分隔")]
    public string SimuStaff { get; set; }
    [Description("试验参与人员代码编号,多个人名用 英文‘,’ 分隔")]
    public string? simuStaffCodes { get; set; }
    [Description("制导控制专业代表，多个人名用 英文‘,’ 分隔")]
    public string QncResp { get; set; }
    public List<TestDataProductUpdateDto> UUT { get; set; }
    
    public List<TestDataUSTUpdateDto>? UST { get; set; }
}

public class TestDataQueryDto : PaginatedQueryDto
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
}
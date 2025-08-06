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

#if DEBUG
    public static TestData[] Seeds { get; } = new TestData[]
    {
        new TestData()
        {
            Id = Guid.Parse("1279ef0b-0274-4bf0-bd88-ac72d653af37"),
            TestDataId = "368ed946-c938-40f1-af15-b9e4171ed6a4",
            SysName = "微波/毫米波复合半实物仿真系统",
            ProjectName = "型号项目名称1",
            TaskName = "种子试验名称1",
            DevPhase = "种子当前型号研制阶段1",
            TaskStartTime = $"{DateTime.MinValue}",
            TaskEndTime = $"{DateTime.MinValue}",
            ReqDep = "种子申请项目办1",
            ReqManager = "种子申请调度1",
            ReqManagerCode = "种子申请调度员工编号1",
            GncResp = "种子系统责任人1",
            GncRespCode = "种子系统责任人编号1",
            SimuResp = "种子试验专业代表1",
            simuRespCode = "种子试验专业代表编号1",
            SimuStaff = "种子试验参与人员1,种子试验参与人员2,种子试验参与人员3",
            simuStaffCodes = "种子试验参与人员代码编号1",
            QncResp = "种子制导控制专业代表1"
        },
        new TestData()
        {
            Id = Guid.Parse("fabbcc05-4987-4d67-9be7-6bacc8b3f6ac"),
            TestDataId = "dc29d70c-1d83-4458-871e-f21837392c16",
            SysName = "微波/毫米波复合半实物仿真系统",
            ProjectName = "型号项目名称2",
            TaskName = "种子试验名称2",
            DevPhase = "种子当前型号研制阶段2",
            TaskStartTime = $"{DateTime.MinValue}",
            TaskEndTime = $"{DateTime.MaxValue}",
            ReqDep = "种子申请项目办2",
            ReqManager = "种子申请调度2",
            ReqManagerCode = "种子申请调度员工编号2",
            GncResp = "种子系统责任人2",
            GncRespCode = "种子系统责任人编号2",
            SimuResp = "种子试验专业代表2",
            simuRespCode = "种子试验专业代表编号2",
            SimuStaff = "种子试验参与人员1,种子试验参与人员2,种子试验参与人员3",
            simuStaffCodes = "种子试验参与人员代码编号2",
            QncResp = "种子制导控制专业代表2"
        },
        new TestData()
        {
            Id = Guid.Parse("b6d79d9f-f519-41cb-b167-35382b5a65cc"),
            TestDataId = "3e226e28-f6c2-4706-a7ee-01156099709f",
            SysName = "微波/毫米波复合半实物仿真系统",
            ProjectName = "型号项目名称3",
            TaskName = "种子试验名称3",
            DevPhase = "种子当前型号研制阶段3",
            TaskStartTime = $"{DateTime.MaxValue}",
            TaskEndTime = $"{DateTime.MaxValue}",
            ReqDep = "种子申请项目办3",
            ReqManager = "种子申请调度3",
            ReqManagerCode = "种子申请调度员工编号3",
            GncResp = "种子系统责任人3",
            GncRespCode = "种子系统责任人编号3",
            SimuResp = "种子试验专业代表3",
            simuRespCode = "种子试验专业代表编号3",
            SimuStaff = "种子试验参与人员1,种子试验参与人员2,种子试验参与人员3",
            simuStaffCodes = "种子试验参与人员代码编号3",
            QncResp = "种子制导控制专业代表3"
        },
        new TestData()
        {
            Id = Guid.Parse("b5bf8812-f02d-4381-944d-f5f70b17ca3a"),
            TestDataId = "409451de-67cc-4073-a5cd-d9759371aa5e",
            SysName = "微波/毫米波复合半实物仿真系统",
            ProjectName = "型号项目名称4",
            TaskName = "种子试验名称4",
            DevPhase = "种子当前型号研制阶段4",
            TaskStartTime = $"{new DateTime(2025,6,4)}",
            TaskEndTime = $"{new DateTime(2025,7,4)}",
            ReqDep = "种子申请项目办4",
            ReqManager = "种子申请调度4",
            ReqManagerCode = "种子申请调度员工编号4",
            GncResp = "种子系统责任人4",
            GncRespCode = "种子系统责任人编号4",
            SimuResp = "种子试验专业代表4",
            simuRespCode = "种子试验专业代表编号4",
            SimuStaff = "种子试验参与人员1,种子试验参与人员2,种子试验参与人员3",
            simuStaffCodes = "种子试验参与人员代码编号4",
            QncResp = "种子制导控制专业代表4"
        },
        new TestData()
        {
            Id = Guid.Parse("3fbcfd6f-e603-4837-99aa-a00074ba9d87"),
            TestDataId = "9f504e3b-d084-4f0b-bec4-bee51724c52b",
            SysName = "微波/毫米波复合半实物仿真系统",
            ProjectName = "型号项目名称5",
            TaskName = "种子试验名称5",
            DevPhase = "种子当前型号研制阶段5",
            TaskStartTime = $"{new DateTime(2025,6,4)}",
            TaskEndTime = $"{new DateTime(2025,7,8)}",
            ReqDep = "种子申请项目办5",
            ReqManager = "种子申请调度5",
            ReqManagerCode = "种子申请调度员工编号5",
            GncResp = "种子系统责任人5",
            GncRespCode = "种子系统责任人编号5",
            SimuResp = "种子试验专业代表5",
            simuRespCode = "种子试验专业代表编号5",
            SimuStaff = "种子试验参与人员1,种子试验参与人员2,种子试验参与人员3",
            simuStaffCodes = "种子试验参与人员代码编号5",
            QncResp = "种子制导控制专业代表5"
        },
        new TestData()
        {
            Id = Guid.Parse("8aa9329d-6b85-46a7-adc4-c9f920ef412e"),
            TestDataId = "4c29cb28-ab4e-4ebf-a329-eb70e473907b",
            SysName = "微波/毫米波复合半实物仿真系统",
            ProjectName = "型号项目名称6",
            TaskName = "种子试验名称6",
            DevPhase = "种子当前型号研制阶段6",
            TaskStartTime = $"{new DateTime(2025,7,4)}",
            TaskEndTime = $"{new DateTime(2025,8,3)}",
            ReqDep = "种子申请项目办6",
            ReqManager = "种子申请调度6",
            ReqManagerCode = "种子申请调度员工编号6",
            GncResp = "种子系统责任人6",
            GncRespCode = "种子系统责任人编号6",
            SimuResp = "种子试验专业代表6",
            simuRespCode = "种子试验专业代表编号6",
            SimuStaff = "种子试验参与人员1,种子试验参与人员2,种子试验参与人员3",
            simuStaffCodes = "种子试验参与人员代码编号6",
            QncResp = "种子制导控制专业代表6"
        },
    };
#endif
}
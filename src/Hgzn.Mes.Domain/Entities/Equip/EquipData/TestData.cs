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

    public List<TestDataUST>? UST { get; set; }

    #region audit

    public Guid? CreatorId { get; set; }
    public DateTime? CreationTime { get; set; }

    #endregion audit

#if DEBUG
    public static TestData[] Seeds { get; } = new TestData[]
    {
        new TestData()
        {
            Id = Guid.Parse("1279ef0b-0274-4bf0-bd88-ac72d653af37"),
            TestDataId = "1234",
            SysName = "微波/毫米波复合半实物仿真系统",
            ProjectName = "型号项目名称1",
            TaskName = "种子试验名称1",
            DevPhase = "种子当前型号研制阶段1",
            TaskStartTime = $"{DateTime.Now.AddDays(-500)}",
            TaskEndTime = $"{DateTime.Now.AddDays(-495)}",
            ReqDep = "种子申请项目办1",
            ReqManager = "种子申请调度1",
            ReqManagerCode = "种子申请调度员工编号1",
            GncResp = "种子系统责任人1",
            GncRespCode = "种子系统责任人编号1",
            SimuResp = "种子试验专业代表1",
            simuRespCode = "种子试验专业代表编号1",
            SimuStaff = "种子试验参与人员1,种子试验参与人员2,种子试验参与人员3",
            simuStaffCodes = "种子试验参与人员代码编号1",
            QncResp = "种子制导控制专业代表1",
            UST = new List<TestDataUST>
            {
                new TestDataUST()
                {
                    Id = Guid.Parse("018060da-3cb6-4f80-b179-1d6319d9663a"),
                    TestDataId = "1234",
                    Code = "873112010314",
                    ModelSpecification = "GEN40-38",
                    Name = "直流稳压电源",
                    ValidityPeriod = "2027-05-29 00:00:00.0"
                }
            }
        },
        new TestData()
        {
            Id = Guid.Parse("fabbcc05-4987-4d67-9be7-6bacc8b3f6ac"),
            TestDataId = "2345",
            SysName = "微波/毫米波复合半实物仿真系统",
            ProjectName = "型号项目名称2",
            TaskName = "种子试验名称2",
            DevPhase = "种子当前型号研制阶段2",
            TaskStartTime = $"{DateTime.Now.AddDays(-3)}",
            TaskEndTime = $"{DateTime.Now.AddDays(-2)}",
            ReqDep = "种子申请项目办2",
            ReqManager = "种子申请调度2",
            ReqManagerCode = "种子申请调度员工编号2",
            GncResp = "种子系统责任人2",
            GncRespCode = "种子系统责任人编号2",
            SimuResp = "种子试验专业代表2",
            simuRespCode = "种子试验专业代表编号2",
            SimuStaff = "种子试验参与人员1,种子试验参与人员2,种子试验参与人员3",
            simuStaffCodes = "种子试验参与人员代码编号2",
            QncResp = "种子制导控制专业代表2",
            UST = new List<TestDataUST>
            {
                new TestDataUST()
                {
                    Id = Guid.Parse("931946e0-1d9e-4e6f-a1a4-a10453ad6a3c"),
                    TestDataId = "2345",
                    Code = "873112010315",
                    ModelSpecification = "GEN40-39",
                    Name = "交流稳压电源",
                    ValidityPeriod = "2027-06-15 00:00:00.0"
                }
            }
        },
        new TestData()
        {
            Id = Guid.Parse("b6d79d9f-f519-41cb-b167-35382b5a65cc"),
            TestDataId = "3456",
            SysName = "微波/毫米波复合半实物仿真系统",
            ProjectName = "型号项目名称3",
            TaskName = "种子试验名称3",
            DevPhase = "种子当前型号研制阶段3",
            TaskStartTime = $"{DateTime.Now}",
            TaskEndTime = $"{DateTime.Now.AddDays(1)}",
            ReqDep = "种子申请项目办3",
            ReqManager = "种子申请调度3",
            ReqManagerCode = "种子申请调度员工编号3",
            GncResp = "种子系统责任人3",
            GncRespCode = "种子系统责任人编号3",
            SimuResp = "种子试验专业代表3",
            simuRespCode = "种子试验专业代表编号3",
            SimuStaff = "种子试验参与人员1,种子试验参与人员2,种子试验参与人员3",
            simuStaffCodes = "种子试验参与人员代码编号3",
            QncResp = "种子制导控制专业代表3",
            UST = new List<TestDataUST>
            {
                new TestDataUST()
                {
                    Id = Guid.Parse("a8f9e7d6-c5b4-a3b2-c1d0-e9f8a7b6c5d4"),
                    TestDataId = "3456",
                    Code = "873112010317",
                    ModelSpecification = "GEN40-41",
                    Name = "频谱分析仪",
                    ValidityPeriod = "2027-08-10 00:00:00.0"
                }
            }
        },
        new TestData()
        {
            Id = Guid.Parse("b5bf8812-f02d-4381-944d-f5f70b17ca3a"),
            TestDataId = "4567",
            SysName = "微波/毫米波复合半实物仿真系统",
            ProjectName = "型号项目名称4",
            TaskName = "种子试验名称4",
            DevPhase = "种子当前型号研制阶段4",
            TaskStartTime = $"{DateTime.Now.AddDays(2)}",
            TaskEndTime = $"{DateTime.Now.AddDays(3)}",
            ReqDep = "种子申请项目办4",
            ReqManager = "种子申请调度4",
            ReqManagerCode = "种子申请调度员工编号4",
            GncResp = "种子系统责任人4",
            GncRespCode = "种子系统责任人编号4",
            SimuResp = "种子试验专业代表4",
            simuRespCode = "种子试验专业代表编号4",
            SimuStaff = "种子试验参与人员1,种子试验参与人员2,种子试验参与人员3",
            simuStaffCodes = "种子试验参与人员代码编号4",
            QncResp = "种子制导控制专业代表4",
            UST = new List<TestDataUST>
            {
                new TestDataUST()
                {
                    Id = Guid.Parse("d32cc889-bda8-4393-8ef4-887b2be5685d"),
                    TestDataId = "4567",
                    Code = "873112010316",
                    ModelSpecification = "GEN40-40",
                    Name = "数字示波器",
                    ValidityPeriod = "2027-07-20 00:00:00.0"
                }
            }
        }
    };
#endif
}
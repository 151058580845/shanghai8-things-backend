using System.ComponentModel;
using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos.App;

public class AppDto
{
}

/// <summary>
/// 试验系统数据列表
/// </summary>
public class EquipLedgerTestReadDto : ReadDto
{
    [Description("正常设备列表")] public int NormalCount { get; set; } = 0;

    [Description("空闲设备列表")] public int FreeCount { get; set; } = 0;

    [Description("离线设备列表")] public int LeaveCount { get; set; } = 0;

    [Description("健康设备列表")] public int HealthCount { get; set; } = 100;

    [Description("较好设备列表")] public int BetterCount { get; set; } = 0;
    [Description("故障设备列表")] public int ErrorCount { get; set; } = 0;
    [Description("开机率")] public int UpRate { get; set; } = 0;
    [Description("关机率")] public int DownRate { get; set; } = 0;
    public List<TestListReadDto> TestList { get; set; } = new();
    public List<TestErrorListReadDto> TestErrorList { get; set; } = new();
    public List<TestListLevelEquipReadDto> ListLevelEquipReadDtos { get; set; } = new();
}
public class TestListLevelEquipReadDto : ReadDto
{
    [Description("系统名称")] public string TestName { get; set; }
    [Description("设备主键")] public Guid EquipId { get; set; }
    [Description("设备名称")] public Guid EquipName { get; set; }
    [Description("设备编号")] public Guid EquipCode { get; set; }

    [Description("利用率")] public int Rate { get; set; } = 0;
    [Description("空闲设备列表")] public int FreeCount { get; set; } = 0;

    [Description("故障设备列表")] public int ErrorCount { get; set; } = 0;
}

public class TestListReadDto : ReadDto
{
    [Description("系统名称")] public string TestName { get; set; }
    [Description("设备列表")] public int EquipCount { get; set; } = 0;

    [Description("利用率")] public int Rate { get; set; } = 0;

    [Description("健康度")] public int HealthRate { get; set; } = 0;
}

public class TestErrorListReadDto : ReadDto
{
    [Description("系统名称")] public string TestName { get; set; }
    [Description("设备主键")] public Guid EquipId { get; set; }
    [Description("设备名称")] public Guid EquipName { get; set; }
    [Description("设备编号")] public Guid EquipCode { get; set; }
    [Description("故障时间")] public DateTime CreateTime { get; set; }
}

public class TestDataAppReadDto : ReadDto
{
    [Description("正常设备列表")] public int NormalCount { get; set; } = 0;

    [Description("空闲设备列表")] public int FreeCount { get; set; } = 0;

    [Description("离线设备列表")] public int LeaveCount { get; set; } = 0;

    [Description("健康设备列表")] public int HealthCount { get; set; } = 100;

    [Description("较好设备列表")] public int BetterCount { get; set; } = 0;
    [Description("故障设备列表")] public int ErrorCount { get; set; } = 0;
    public List<TestDataListReadDto> TestList { get; set; } = new List<TestDataListReadDto>();
}

public class TestDataListReadDto : ReadDto
{
    [Description("仿真系统资源名称")] public string SysName { get; set; }
    [Description("型号项目名称")] public string ProjectName { get; set; }
    [Description("试验名称")] public string TaskName { get; set; }
    [Description("当前型号研制阶段")] public string DevPhase { get; set; }
    [Description("当前试验起始日期")] public string TaskStartTime { get; set; }
    [Description("当前试验计划结束日期")] public string TaskEndTime { get; set; }
    [Description("申请项目办")] public string ReqDep { get; set; }
    [Description("申请调度,多个人名用 英文‘,’ 分隔")] public string ReqManager { get; set; }
    [Description("试验专业代表,多个人名用 英文‘,’ 分隔")] public string SimuResp { get; set; }
    [Description("试验参与人员,多个人名用 英文‘,’ 分隔")] public string SimuStaff { get; set; }

    [Description("制导控制专业代表，多个人名用 英文‘,’ 分隔")]
    public string QncResp { get; set; }

    [Description("当前状态")] public string? Status { get; set; }
    [Description("试验结果")] public string? Result { get; set; } = "未知";
}
using Hgzn.Mes.Application.Main.Dtos.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Dtos.App
{
    /// <summary>
    /// 试验设备在线物联系统主页数据响应模型
    /// </summary>
    public class ShowSystemHomeDataDto : ReadDto
    {
        /// <summary>
        /// 环境数据
        /// </summary>
        public EnvironmentData? EnvironmentData { get; set; } = new EnvironmentData();

        /// <summary>
        /// 试验系统列表数据
        /// </summary>
        public List<SystemDeviceData>? SystemDeviceList { get; set; } = new List<SystemDeviceData>();

        /// <summary>
        /// 在线设备状态统计（在线率）
        /// </summary>
        public OnlineRateData? OnlineRateData { get; set; } = new OnlineRateData();

        /// <summary>
        /// 在线设备状态统计（故障率）
        /// </summary>
        public FailureRateData? FailureRateData { get; set; } = new FailureRateData();

        /// <summary>
        /// 设备状态统计详细数据
        /// </summary>
        public EquipmentState? EquipmentState { get; set; } = new EquipmentState();

        /// <summary>
        /// 试验系统利用率数据
        /// </summary>
        public List<DeviceUtilizationData>? DeviceListData { get; set; } = new List<DeviceUtilizationData>();

        /// <summary>
        /// 实验任务数据
        /// </summary>
        public List<ExperimentData>? ExperimentData { get; set; } = new List<ExperimentData>();

        /// <summary>
        /// 异常信息列表
        /// </summary>
        public List<AbnormalDeviceData>? AbnormalDeviceList { get; set; } = new List<AbnormalDeviceData>();

        /// <summary>
        /// 关键设备利用率数据
        /// </summary>
        public List<KeyDeviceData>? KeyDeviceList { get; set; } = new List<KeyDeviceData>();

        /// <summary>
        /// 按系统统计的试验时间
        /// </summary>
        public SystemTestTimes SystemTestTimes { get; set; } = new SystemTestTimes();

        /// <summary>
        /// 按型号统计的试验时间
        /// </summary>
        public TypeTestTimes TypeTestTimes { get; set; } = new TypeTestTimes();

        /// <summary>
        /// 按系统统计的试验成本
        /// </summary>
        public SystemTestCost SystemTestCost { get; set; } = new SystemTestCost();

        /// <summary>
        /// 按型号统计的试验成本
        /// </summary>
        public TypeTestCost TypeTestCost { get; set; } = new TypeTestCost();

        /// <summary>
        /// 摄像头信息
        /// </summary>
        public List<CameraDto>? CameraData { get; set; }
    }

    /// <summary>
    /// 环境数据
    /// </summary>
    public class EnvironmentData : ReadDto
    {
        /// <summary>
        /// 湿度
        /// </summary>
        public int Humidity { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        public int Temperature { get; set; }
    }

    /// <summary>
    /// 试验系统设备数据
    /// </summary>
    public class SystemDeviceData : ReadDto
    {
        /// <summary>
        /// 系统名称
        /// </summary>
        public string? Name { get; set; } = string.Empty;

        /// <summary>
        /// 系统名简写
        /// </summary>
        public string? SystemAbbreviationName { get; set; }

        /// <summary>
        /// 设备数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        public int Temperature { get; set; }

        /// <summary>
        /// 湿度
        /// </summary>
        public int Humidity { get; set; }

        /// <summary>
        /// 状态（实验中、维修中、空闲中等）
        /// </summary>
        public string? Status { get; set; } = string.Empty;

        /// <summary>
        /// 型号名
        /// </summary>
        public string? TypeName { get; set; }

        /// <summary>
        /// 系统所在的房间ID,用于获取MQTT推上来的温湿度数据,不展示
        /// </summary>
        public string? RoomId { get; set; }

        /// <summary>
        /// 当前任务名称
        /// </summary>
        public string? CurrentTaskName { get; set; }

        /// <summary>
        /// 已经开展天数
        /// </summary>
        public int? FinishingDays { get; set; }
    }

    /// <summary>
    /// 在线设备状态统计（在线率）
    /// </summary>
    public class OnlineRateData : ReadDto
    {
        /// <summary>
        /// 在线设备数（工作中）
        /// </summary>
        public int WorkingRateData { get; set; }

        /// <summary>
        /// 空闲设备数
        /// </summary>
        public int FreeRateData { get; set; }

        /// <summary>
        /// 下线设备数
        /// </summary>
        public int OfflineRateData { get; set; }
    }

    /// <summary>
    /// 在线设备状态统计（故障率）
    /// </summary>
    public class FailureRateData : ReadDto
    {
        /// <summary>
        /// 健康设备数
        /// </summary>
        public int HealthData { get; set; }

        /// <summary>
        /// 较好设备数
        /// </summary>
        public int PreferablyData { get; set; }

        /// <summary>
        /// 故障设备数
        /// </summary>
        public int BreakdownData { get; set; }
    }

    /// <summary>
    /// 设备状态统计数据
    /// </summary>
    public class EquipmentData : ReadDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string? Code { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 设备所在位置
        /// </summary>
        public string? Location { get; set; }
        /// <summary>
        /// 设备在线状态（"工作中"、"空闲中"、"离线中"）
        /// </summary>
        public string? State { get; set; }
        /// <summary>
        /// 设备健康状态（"健康"、"较好"、"故障"）
        /// </summary>
        public string? Health { get; set; }
    }

    /// <summary>
    /// 设备状态统计详细数据
    /// </summary>
    public class EquipmentState : ReadDto
    {
        /// <summary>
        /// 表头，如 ["index", "序号"]
        /// </summary>
        public List<string[]>? Headers { get; set; }
        /// <summary>
        /// 设备数据列表
        /// </summary>
        public List<EquipmentData>? Data { get; set; }
    }

    /// <summary>
    /// 试验系统利用率数据
    /// </summary>
    public class DeviceUtilizationData : ReadDto
    {
        /// <summary>
        /// 系统名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 系统状态（在线、离线）
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 设备利用率
        /// </summary>
        public int Utilization { get; set; }

        /// <summary>
        /// 设备空闲率
        /// </summary>
        public int Idle { get; set; }
    }

    /// <summary>
    /// 实验任务数据
    /// </summary>
    public class ExperimentData : ReadDto
    {
        /// <summary>
        /// 标题（当前试验任务、后续试验任务、历史试验任务）
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 表头数据
        /// </summary>
        public List<TableHeader> Headers { get; set; } = new List<TableHeader>();

        /// <summary>
        /// 表格数据
        /// </summary>
        public List<Dictionary<string, object>> Data { get; set; } = new List<Dictionary<string, object>>();
    }

    /// <summary>
    /// 表头数据
    /// </summary>
    public class TableHeader : ReadDto
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// 显示名称
        /// </summary>
        public string Label { get; set; } = string.Empty;
    }

    /// <summary>
    /// 异常信息数据
    /// </summary>
    public class AbnormalDeviceData : ReadDto
    {
        /// <summary>
        /// 试验系统名
        /// </summary>
        public string System { get; set; } = string.Empty;

        /// <summary>
        /// 异常数量
        /// </summary>
        public int AbnormalCount { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string Time { get; set; } = string.Empty;
    }

    /// <summary>
    /// 关键设备利用率数据
    /// </summary>
    public class KeyDeviceData : ReadDto
    {
        /// <summary>
        /// 系统名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 设备使用数
        /// </summary>
        public int Utilization { get; set; }

        /// <summary>
        /// 设备空闲数
        /// </summary>
        public int Idle { get; set; }

        /// <summary>
        /// 设备故障数
        /// </summary>
        public int Breakdown { get; set; }
    }

    /// <summary>
    /// API响应包装类
    /// </summary>
    public class ShowSystemHomeDataResponse : ReadDto
    {
        /// <summary>
        /// 响应状态码
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 响应信息
        /// </summary>
        public string Info { get; set; } = string.Empty;

        /// <summary>
        /// 响应数据
        /// </summary>
        public ShowSystemHomeDataDto Data { get; set; } = new ShowSystemHomeDataDto();
    }

    public class DateTimeRange
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class SystemTestTimes : ReadDto
    {
        /// <summary>
        /// 时间范围 (key 是系统名, Value是时间段集合),按系统名分类汇总,后续可能需要根据具体的时间画图表
        /// </summary>
        public Dictionary<string, List<DateTimeRange>>? Times { get; set; }

        /// <summary>
        /// 每个系统每月的工作天数统计 (key: 系统名, value: 每月工作天数字典)
        /// </summary>
        public Dictionary<string, Dictionary<NaturalMonth, int>>? SystemMonthlyWorkDays { get; set; }

        /// <summary>
        /// 每月所有系统总工作天数 (key: 月份, value: 所有系统该月总工作天数)
        /// </summary>
        public Dictionary<NaturalMonth, int>? TimePerMonth { get; set; }

        /// <summary>
        /// 当前自然月所有系统总工作天数(可能用于显示于大屏主页的"月试验天数")
        /// </summary>
        public int? CurrentMonthTotalSystemTestDays { get; set; }

        /// <summary>
        /// 当前自然年所有系统总工作天数(可能用于显示于大屏主页的"年试验天数")
        /// </summary>
        public int? CurrentYearTotalSystemTestDays { get; set; }
    }

    public class TypeTestTimes : ReadDto
    {
        /// <summary>
        /// 时间范围 (key 是型号名, Value是时间段集合),按型号名分类汇总,后续可能需要根据具体的时间画图表
        /// </summary>
        public Dictionary<string, List<DateTimeRange>>? Times { get; set; }

        /// <summary>
        /// 每个型号每月的工作天数统计 (key: 型号名, value: 每月工作天数字典)
        /// </summary>
        public Dictionary<string, Dictionary<NaturalMonth, int>>? TypeMonthlyWorkDays { get; set; }

        /// <summary>
        /// 每月所有型号总工作天数 (key: 月份, value: 所有型号该月总工作天数)
        /// </summary>
        public Dictionary<NaturalMonth, int>? TimePerMonth { get; set; }

        /// <summary>
        /// 当前自然月所有型号总工作天数(可能用于显示于大屏主页的"月试验天数")
        /// </summary>
        public int? CurrentMonthTotalTypeTestDays { get; set; }

        /// <summary>
        /// 当前自然年所有型号总工作天数(可能用于显示于大屏主页的"年试验天数")
        /// </summary>
        public int? CurrentYearTotalTypeTestDays { get; set; }
    }



    public class CostBreakdown
    {
        /// <summary>
        /// 自然月
        /// </summary>
        public NaturalMonth NaturalMonth { get; set; }

        /// <summary>
        /// 厂房使用费
        /// </summary>
        public decimal? FactoryUsageFee { get; set; }

        /// <summary>
        /// 设备使用费
        /// </summary>
        public decimal? EquipmentUsageFee { get; set; }

        /// <summary>
        /// 人力成本
        /// </summary>
        public decimal? LaborCost { get; set; }

        /// <summary>
        /// 电费
        /// </summary>
        public decimal? ElectricityCost { get; set; }

        /// <summary>
        /// 燃料动力费
        /// </summary>
        public decimal? FuelPowerCost { get; set; }

        /// <summary>
        /// 设备保养费用
        /// </summary>
        public decimal? EquipmentMaintenanceCost { get; set; }

        /// <summary>
        /// 系统空置成本
        /// </summary>
        public decimal? SystemIdleCost { get; set; }
    }

    public class SystemTestCost : ReadDto
    {
        /// <summary>
        /// 成本明细(key是系统名称,value是该系统的成本明细)
        /// </summary>
        public Dictionary<string, List<CostBreakdown>>? Costs { get; set; }

        /// <summary>
        /// 每个系统每月的总成本统计 (key: 系统名, value: 每月总成本字典)
        /// </summary>
        public Dictionary<string, Dictionary<NaturalMonth, decimal>>? SystemMonthlyCosts { get; set; }

        /// <summary>
        /// 每月所有系统总成本 (key: 月份, value: 所有系统该月总成本)
        /// </summary>
        public Dictionary<NaturalMonth, decimal>? CostPerMonth { get; set; }

        /// <summary>
        /// 当前自然月所有系统总成本(可能用于显示于大屏主页的"月试验成本")
        /// </summary>
        public int? CurrentMonthTotalSystemTestCost { get; set; }

        /// <summary>
        /// 当前自然年所有系统总成本(可能用于显示于大屏主页的"年试验成本")
        /// </summary>
        public int? CurrentYearTotalSystemTestCost { get; set; }
    }

    public class TypeTestCost : ReadDto
    {
        /// <summary>
        /// 成本明细(key是型号名,value是该型号的成本明细)
        /// </summary>
        public Dictionary<string, List<CostBreakdown>>? Costs { get; set; }

        /// <summary>
        /// 每个型号每月的总成本统计 (key: 型号名, value: 每月总成本字典)
        /// </summary>
        public Dictionary<string, Dictionary<NaturalMonth, decimal>>? TypeMonthlyCosts { get; set; }

        /// <summary>
        /// 每月所有型号总成本 (key: 月份, value: 所有型号该月总成本)
        /// </summary>
        public Dictionary<NaturalMonth, decimal>? CostPerMonth { get; set; }

        /// <summary>
        /// 当前自然月所有型号总成本(可能用于显示于大屏主页的"月试验成本")
        /// </summary>
        public int? CurrentMonthTotalTypeTestCost { get; set; }

        /// <summary>
        /// 当前自然年所有型号总成本(可能用于显示于大屏主页的"年试验成本")
        /// </summary>
        public int? CurrentYearTotalTypeTestCost { get; set; }
    }

    /// <summary>
    /// 自然月枚举
    /// </summary>
    public enum NaturalMonth
    {
        /// <summary>
        /// 一月
        /// </summary>
        January = 1,

        /// <summary>
        /// 二月
        /// </summary>
        February = 2,

        /// <summary>
        /// 三月
        /// </summary>
        March = 3,

        /// <summary>
        /// 四月
        /// </summary>
        April = 4,

        /// <summary>
        /// 五月
        /// </summary>
        May = 5,

        /// <summary>
        /// 六月
        /// </summary>
        June = 6,

        /// <summary>
        /// 七月
        /// </summary>
        July = 7,

        /// <summary>
        /// 八月
        /// </summary>
        August = 8,

        /// <summary>
        /// 九月
        /// </summary>
        September = 9,

        /// <summary>
        /// 十月
        /// </summary>
        October = 10,

        /// <summary>
        /// 十一月
        /// </summary>
        November = 11,

        /// <summary>
        /// 十二月
        /// </summary>
        December = 12
    }
}

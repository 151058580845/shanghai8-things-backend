using Hgzn.Mes.Application.Main.Dtos.Base;
using System;
using System.Collections.Generic;
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
        /// 系统所在的房间ID,用于获取MQTT推上来的温湿度数据,不展示
        /// </summary>
        public string? RoomId { get; set; }
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
}

using Hgzn.Mes.Application.Main.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Dtos.App
{
    public class ShowSystemDetailQueryDto : QueryDto
    {
        public string? systemName { get; set; }
    }

    /// <summary>
    /// 分页详情数据总 DTO（用于一次性传递所有数据）
    /// </summary>
    public class ShowSystemDetailDto : ReadDto
    {
        /// <summary>
        /// 实验人员信息
        /// </summary>
        public List<ExperimenterDto> ExperimentersData { get; set; }

        /// <summary>
        /// 图表数据
        /// </summary>
        public List<ChartDataDto> ChartData { get; set; }

        /// <summary>
        /// 异常设备列表
        /// </summary>
        public List<AbnormalDeviceDto> AbnormalDeviceListData { get; set; }

        /// <summary>
        /// 当前实验任务
        /// </summary>
        public TaskDetailDto CurrentTask { get; set; }

        /// <summary>
        /// 后续实验任务
        /// </summary>
        public TaskDetailDto FollowTask { get; set; }

        /// <summary>
        /// 射频阵列运行状态
        /// </summary>
        public List<TableDto> Queue { get; set; }

        /// <summary>
        /// 产品清单
        /// </summary>
        public List<TableDto> ProductList { get; set; }

        /// <summary>
        /// 摄像头信息
        /// </summary>
        public List<CameraDto> CameraData { get; set; }
    }

    /// <summary>
    /// 实验人员信息 DTO
    /// </summary>
    public class ExperimenterDto : ReadDto
    {
        /// <summary>
        /// 系统名称
        /// </summary>
        public string System { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public string Person { get; set; }
    }

    /// <summary>
    /// 图表数据点 DTO
    /// </summary>
    public class ChartDataPointDto : ReadDto
    {
        /// <summary>
        /// 时间点
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 数值
        /// </summary>
        public double Value { get; set; }
    }

    /// <summary>
    /// 图表数据 DTO
    /// </summary>
    public class ChartDataDto : ReadDto
    {
        /// <summary>
        /// 图表名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据点集合
        /// </summary>
        public List<ChartDataPointDto> Data { get; set; }
    }

    /// <summary>
    /// 异常设备信息 DTO
    /// </summary>
    public class AbnormalDeviceDto : ReadDto
    {
        /// <summary>
        /// 系统名称
        /// </summary>
        public string System { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// 状态值（如"正常"、"异常"）
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 时间（字符串格式）
        /// </summary>
        public string Time { get; set; }
    }

    /// <summary>
    /// 任务详情 DTO
    /// </summary>
    public class TaskDetailDto : ReadDto
    {
        /// <summary>
        /// 任务标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 任务详情（二维数组，每项为[label, value]）
        /// </summary>
        public List<List<string>> Details { get; set; }
    }

    /// <summary>
    /// 通用表格 DTO（如产品清单、射频阵列等）
    /// </summary>
    public class TableDto : ReadDto
    {
        /// <summary>
        /// 表格标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 表头（每项为[key, label]）
        /// </summary>
        public List<List<string>> Header { get; set; }

        /// <summary>
        /// 数据行（每行为一个字典，key与header对应）
        /// </summary>
        public List<Dictionary<string, string>> Data { get; set; }
    }

    /// <summary>
    /// 摄像头信息 DTO
    /// </summary>
    public class CameraDto : ReadDto
    {
        /// <summary>
        /// 摄像头IP地址
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 摄像头端口
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}

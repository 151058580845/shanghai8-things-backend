using System.ComponentModel;

namespace Hgzn.Mes.Application.Main.Dtos.Equip
{
    /// <summary>
    /// 温湿度记录表Word导出请求DTO
    /// </summary>
    public class TemperatureHumidityRecordExportRequestDto
    {
        [Description("房间ID")]
        public Guid RoomId { get; set; }

        [Description("开始日期")]
        public DateTime StartDate { get; set; }

        [Description("结束日期")]
        public DateTime EndDate { get; set; }
    }

    /// <summary>
    /// 温湿度记录表数据DTO
    /// </summary>
    public class TemperatureHumidityRecordDataDto
    {
        [Description("测量日期")]
        public string MeasurementDate { get; set; } = string.Empty;

        [Description("温度值(℃)")]
        public string TemperatureValue { get; set; } = string.Empty;

        [Description("湿度值(%)")]
        public string HumidityValue { get; set; } = string.Empty;

        [Description("测量时间")]
        public string MeasurementTime { get; set; } = string.Empty;

        [Description("测量人员")]
        public string Measurer { get; set; } = "自动测量";
    }

    /// <summary>
    /// 温湿度记录表完整数据DTO
    /// </summary>
    public class TemperatureHumidityRecordExportDataDto
    {
        [Description("系统名称")]
        public string SystemName { get; set; } = string.Empty;

        [Description("房间号")]
        public string RoomNumber { get; set; } = string.Empty;

        [Description("记录数据")]
        public List<TemperatureHumidityRecordDataDto> RecordData { get; set; } = new();
    }
}

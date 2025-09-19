using System.ComponentModel;

namespace Hgzn.Mes.Application.Main.Dtos.Equip
{
    /// <summary>
    /// 温湿度数据导出请求DTO
    /// </summary>
    public class TemperatureHumidityExportRequestDto
    {
        [Description("设备编码数组")]
        public List<string> EquipCodes { get; set; } = new();

        [Description("开始日期")]
        public string? StartDate { get; set; }

        [Description("结束日期")]
        public string? EndDate { get; set; }
    }

    /// <summary>
    /// 温湿度数据导出响应DTO
    /// </summary>
    public class TemperatureHumidityExportDataDto
    {
        [Description("序号")]
        public int SequenceNumber { get; set; }

        [Description("设备名称")]
        public string? EquipName { get; set; }

        [Description("设备编码")]
        public string? EquipCode { get; set; }

        [Description("房间名称")]
        public string? RoomName { get; set; }

        [Description("温度(°C)")]
        public float? Temperature { get; set; }

        [Description("湿度(%)")]
        public float? Humidness { get; set; }

        [Description("IP地址")]
        public string? IpAddress { get; set; }

        [Description("记录时间")]
        public string? CreateTime { get; set; }
    }
}

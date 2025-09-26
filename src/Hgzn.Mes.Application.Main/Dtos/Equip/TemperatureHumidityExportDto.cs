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

        [Description("房间ID")]
        public Guid? RoomId { get; set; }

        [Description("开始日期")]
        public string? StartDate { get; set; }

        [Description("结束日期")]
        public string? EndDate { get; set; }
    }

}

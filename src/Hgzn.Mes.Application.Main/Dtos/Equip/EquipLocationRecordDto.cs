using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos.Equip;

public class EquipLocationRecordReadDto : ReadDto
{
    /// <summary>
    /// 设备名
    /// </summary>
    public string? EquipName { get; set; }
    /// <summary>
    /// 读卡器名称
    /// </summary>
    public string? RfidEquipName { get; set; }

    /// <summary>
    /// 定位时间
    /// </summary>
    public DateTime DateTime { get; set; }

    /// <summary>
    /// 标签ID
    /// </summary>
    public string? Tid { get; set; }

    /// <summary>
    /// 标签用户数据
    /// </summary>
    public string? Userdata { get; set; }

    /// <summary>
    /// Rfid连接ID
    /// </summary>
    public Guid ConnectId { get; set; }

    /// <summary>
    /// 定位房间名称
    /// </summary>
    public string? RoomName { get; set; }
}

public class EquipLocationRecordQueryDto : PaginatedTimeQueryDto
{
    /// <summary>
    /// 设备id
    /// </summary>
    public string? EquipId { get; set; }
    /// <summary>
    /// 定位起始时间
    /// </summary>
    public DateTime? StartDateTime { get; set; }
    /// <summary>
    /// 定位结束时间
    /// </summary>
    public DateTime? EndDateTime { get; set; }
    /// <summary>
    /// 房间id
    /// </summary>
    public string? RoomId { get; set; }
}

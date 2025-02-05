using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Application.Main.Dtos.Equip;


public class EquipConnectReadDto : ReadDto
{
    /// <summary>
    /// 状态
    /// </summary>
    public bool State { get; set; }

    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;

    /// <summary>
    /// 设备编号
    /// </summary>
    public string? EquipCode { get; set; }

    /// <summary>
    /// 设备主键
    /// </summary>
    public Guid EquipId { get; set; }

    /// <summary>
    /// 设备名称
    /// </summary>
    public string? EquipName { get; set; }

    /// <summary>
    /// 设备类型名称
    /// </summary>
    public string? TypeName { get; set; }

    /// <summary>
    /// 采集协议
    /// </summary>
    public Protocol? ProtocolEnum { get; set; }

    /// <summary>
    /// 连接参数
    /// </summary>
    public string? ConnectStr { get; set; }

    /// <summary>
    /// 连接状态
    /// </summary>
    public bool ConnectState { get; set; }

    /// <summary>
    /// 连接状态
    /// </summary>
    public string? ConnectStateStr { get; set; }

    /// <summary>
    /// 当前状态
    /// </summary>
    public string? CollectionModel { get; set; }

    /// <summary>
    /// 转发地址
    /// </summary>
    public List<Guid>? ForwardIds { get; set; } = null;

    /// <summary>
    /// 转发地址名称
    /// </summary>
    public string? ForwardNames { get; set; }

    /// <summary>
    /// 扩展字段：
    /// RfidReaderClient模式下 0采集数据，1绑定标签 2解绑标签
    /// </summary>
    public int? CollectionExtension { get; set; }

    /// <summary>
    /// 转发速率
    /// </summary>
    public int? ForwardRate { get; set; }
}

public class EquipConnectCreateDto : CreateDto
{
    public string? Name { get; set; }
    public string? Code { get; set; }

    /// <summary>
    /// 设备Id
    /// </summary>
    public Guid? EquipId { get; set; }

    /// <summary>
    /// 采集协议
    /// </summary>
    public Protocol? ProtocolEnum { get; set; }

    /// <summary>
    /// 连接参数
    /// </summary>
    public string? ConnectStr { get; set; }

    /// <summary>
    /// 启用状态
    /// </summary>
    public bool? State { get; set; }

    /// <summary>
    /// 转发地址
    /// </summary>
    public List<Guid>? ForwardIds { get; set; } = null;

    /// <summary>
    /// 转发速率
    /// </summary>
    public int? ForwardRate { get; set; }
}

public class EquipConnectUpdateDto : UpdateDto
{
    public string? Name { get; set; }
    public string? Code { get; set; }

    /// <summary>
    /// 设备Id
    /// </summary>
    public Guid? EquipId { get; set; }

    /// <summary>
    /// 采集协议
    /// </summary>
    public Protocol? ProtocolEnum { get; set; }

    /// <summary>
    /// 连接参数
    /// </summary>
    public string? ConnectStr { get; set; }

    /// <summary>
    /// 启用状态
    /// </summary>
    public bool? State { get; set; }

    /// <summary>
    /// 转发地址
    /// </summary>
    public List<Guid>? ForwardIds { get; set; } = null;

    /// <summary>
    /// 转发速率
    /// </summary>
    public int? ForwardRate { get; set; }
}

public class EquipConnectQueryDto : PaginatedQueryDto
{
    public bool? State { get; set; }

    /// <summary>
    /// 设备编号
    /// </summary>
    public string? EquipCode { get; set; }

    /// <summary>
    /// 设备名称
    /// </summary>
    public string? EquipName { get; set; }

    /// <summary>
    /// 采集协议
    /// </summary>
    public Protocol? ProtocolEnum { get; set; }
}
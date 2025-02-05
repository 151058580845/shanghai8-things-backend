using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos.Equip;

public class EquipTypeReadDto : ReadDto
{
    /// <summary>
    /// 设备类型编号
    /// </summary>
    public string TypeCode { get; set; } = null!;

    /// <summary>
    /// 设备类型名称
    /// </summary>
    public string TypeName { get; set; } = null!;

    /// <summary>
    /// 设备描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int OrderNum { get; set; } = 0;

    /// <summary>
    /// 上级Id
    /// </summary>
    public Guid? ParentId { get; set; }

    public bool State { get; set; }

    public List<EquipTypeReadDto>? Children { get; set; }

    /// <summary>
    /// 设备创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 设备最后修改时间
    /// </summary>
    public DateTime? LastModificationTime { get; set; }
}

public class EquipTypeCreateDto : CreateDto
{
    /// <summary>
    /// 设备类型编号
    /// </summary>
    public string? TypeCode { get; set; }

    /// <summary>
    /// 设备类型名称
    /// </summary>
    public string? TypeName { get; set; }

    /// <summary>
    /// 设备描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int OrderNum { get; set; }

    /// <summary>
    /// 上级Id
    /// </summary>
    public Guid ParentId { get; set; } = Guid.Empty;

    public bool State { get; set; } = true;
}

public class EquipTypeUpdateDto : UpdateDto
{
    /// <summary>
    /// 设备类型编号
    /// </summary>
    public string TypeCode { get; set; } = null!;

    /// <summary>
    /// 设备类型名称
    /// </summary>
    public string TypeName { get; set; } = null!;

    /// <summary>
    /// 设备描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int OrderNum { get; set; }

    /// <summary>
    /// 上级Id
    /// </summary>
    public Guid? ParentId { get; set; }

    public bool State { get; set; }
}

public class EquipTypeQueryDto : PaginatedQueryDto
{
    /// <summary>
    /// 设备类型编号
    /// </summary>
    public string? TypeCode { get; set; }

    /// <summary>
    /// 设备类型名称
    /// </summary>
    public string? TypeName { get; set; }

    /// <summary>
    /// 设备描述
    /// </summary>
    public string? Description { get; set; }

    public bool? State { get; set; }

    /// <summary>
    /// 上级Id
    /// </summary>
    public Guid? ParentId { get; set; } = Guid.Empty;
}
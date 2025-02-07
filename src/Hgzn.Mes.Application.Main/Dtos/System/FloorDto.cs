using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos.System;

public class FloorDto
{
    
}
public class FloorReadDto : ReadDto
{
    /// <summary>
    /// 建筑物ID
    /// </summary>
    public string? ParentId { get; set; }
    /// <summary>
    /// 楼层名称
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// 楼层编号
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 楼层面积（平方米）
    /// </summary>
    public string? Area { get; set; }

    /// <summary>
    /// 房间数量
    /// </summary>
    public string? NumberOfRooms { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? OrderNum { get; set; }
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

public class FloorCreateDto : CreateDto
{
    /// <summary>
    /// 建筑物ID
    /// </summary>
    public Guid ParentId { get; set; }
    /// <summary>
    /// 楼层名称
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// 楼层编号
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 楼层面积（平方米）
    /// </summary>
    public decimal? Area { get; set; }

    /// <summary>
    /// 房间数量
    /// </summary>
    public int? NumberOfRooms { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? OrderNum { get; set; }
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

public class FloorUpdateDto : UpdateDto
{
    /// <summary>
    /// 建筑物ID
    /// </summary>
    public Guid BuildingId { get; set; }
    /// <summary>
    /// 楼层名称
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// 楼层编号
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 楼层面积（平方米）
    /// </summary>
    public decimal Area { get; set; }

    /// <summary>
    /// 房间数量
    /// </summary>
    public int NumberOfRooms { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int OrderNum { get; set; }
}

public class FloorQueryDto : PaginatedQueryDto
{
    /// <summary>
    /// 建筑物ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 楼层编号
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// 楼层编号
    /// </summary>
    public string? Code { get; set; }
}
using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos.System;

public class BuildingDto
{
    
}
public class BuildingReadDto : ReadDto
{
    /// <summary>
    /// 建筑物名称
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// 建筑物名称
    /// </summary>
    public string? Code { get; set; }
    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 纬度
    /// </summary>
    public string? Latitude { get; set; }

    /// <summary>
    /// 经度
    /// </summary>
    public string? Longitude { get; set; }

    /// <summary>
    /// 建造日期
    /// </summary>
    public string? ConstructionDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? OrderNum { get; set; }
}

public class BuildingCreateDto : CreateDto
{
    /// <summary>
    /// 建筑物名称
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 建筑物编号
    /// </summary>
    public string Code { get; set; }= null!;
    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 纬度
    /// </summary>
    public decimal? Latitude { get; set; }

    /// <summary>
    /// 经度
    /// </summary>
    public decimal? Longitude { get; set; }

    /// <summary>
    /// 建造日期
    /// </summary>
    public DateTime? ConstructionDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? OrderNum { get; set; }
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

public class BuildingUpdateDto : UpdateDto
{
    /// <summary>
    /// 建筑物名称
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 建筑物编号
    /// </summary>
    public string Code { get; set; }= null!;

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 纬度
    /// </summary>
    public decimal? Latitude { get; set; }

    /// <summary>
    /// 经度
    /// </summary>
    public decimal? Longitude { get; set; }

    /// <summary>
    /// 建造日期
    /// </summary>
    public DateTime? ConstructionDate { get; set; }
}

public class BuildingQueryDto : PaginatedTimeQueryDto
{
    /// <summary>
    /// 建筑物名称
    /// </summary>
    public string? Name { get; set; }
}
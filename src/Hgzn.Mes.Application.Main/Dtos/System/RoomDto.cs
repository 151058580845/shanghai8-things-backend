using System.ComponentModel.DataAnnotations.Schema;
using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos.System;

public class RoomDto
{
    
}
public class RoomReadDto : ReadDto
{
    /// <summary>
    /// 楼层ID
    /// </summary>
    public Guid ParentId { get; set; }

    /// <summary>
    /// 房间名称
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// 房间编号
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 房间长度（米）
    /// </summary>
    public string? Length { get; set; }

    /// <summary>
    /// 房间宽度（米）
    /// </summary>
    public string? Width { get; set; }

    /// <summary>
    /// 房间高度（米）
    /// </summary>
    public string? Height { get; set; }

    /// <summary>
    /// 用途
    /// </summary>
    public string? Purpose { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? OrderNum { get; set; }
    /// <summary>
    /// 房间组
    /// </summary>
    public Guid GroupId { get; set; }
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
    
    /// <summary>
    /// 房间二维码
    /// </summary>
    public string? QrCode { get; set; }

    public override string ToString()
    {
        return Id + Code + Name;
    }
}

public class RoomQrCode
{
    /// <summary>
    /// 主键
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// 房间名称
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// 房间编号
    /// </summary>
    public string? Code { get; set; }
}

public class RoomCreateDto : CreateDto
{
    /// <summary>
    /// 楼层ID
    /// </summary>
    public Guid ParentId { get; set; }

    /// <summary>
    /// 房间名称
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// 房间编号
    /// </summary>
    public string? Code { get; set; }
    /// <summary>
    /// 房间长度（米）
    /// </summary>
    public decimal? Length { get; set; }

    /// <summary>
    /// 房间宽度（米）
    /// </summary>
    public decimal? Width { get; set; }

    /// <summary>
    /// 房间高度（米）
    /// </summary>
    public decimal? Height { get; set; }

    /// <summary>
    /// 用途
    /// </summary>
    public string? Purpose { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? OrderNum { get; set; }
    /// <summary>
    /// 房间组
    /// </summary>
    public Guid? GroupId { get; set; }
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

public class RoomUpdateDto : UpdateDto
{
    /// <summary>
    /// 楼层ID
    /// </summary>
    public Guid FloorId { get; set; }

    /// <summary>
    /// 房间名称
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// 房间编号
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 房间长度（米）
    /// </summary>
    public decimal? Length { get; set; }

    /// <summary>
    /// 房间宽度（米）
    /// </summary>
    public decimal? Width { get; set; }

    /// <summary>
    /// 房间高度（米）
    /// </summary>
    public decimal? Height { get; set; }

    /// <summary>
    /// 用途
    /// </summary>
    public string? Purpose { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? OrderNum { get; set; }
}

public class RoomQueryDto : PaginatedQueryDto
{
    /// <summary>
    /// 楼层ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 房间名称
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// 房间编号
    /// </summary>
    public string? Code { get; set; }
    /// <summary>
    /// 房间组
    /// </summary>
    public Guid? GroupId { get; set; }
}
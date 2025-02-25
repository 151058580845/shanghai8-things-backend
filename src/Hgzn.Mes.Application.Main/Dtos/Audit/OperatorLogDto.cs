using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Shared.Enums.Log;

namespace Hgzn.Mes.Application.Main.Dtos.Audit;

public class OperatorLogDto
{
    
}

public class OperatorLogReadDto : ReadDto
{
    /// <summary>
    /// 操作模块    
    ///</summary>
    public string? Title { get; set; }
    /// <summary>
    /// 操作类型 
    ///</summary>
    public string? OperType { get; set; }
    /// <summary>
    /// 请求方法 
    ///</summary>
    public string? RequestMethod { get; set; }
    /// <summary>
    /// 操作人员 
    ///</summary>
    public string? OperUser { get; set; }
    /// <summary>
    /// 操作Ip 
    ///</summary>
    public string? OperIp { get; set; }
    /// <summary>
    /// 操作地点 
    ///</summary>
    public string? OperLocation { get; set; }
    /// <summary>
    /// 操作方法 
    ///</summary>
    public string? Method { get; set; }
    /// <summary>
    /// 请求参数 
    ///</summary>
    public string? RequestParam { get; set; }
    /// <summary>
    /// 请求结果 
    ///</summary>
    public string? RequestResult { get; set; }

    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
}

public class OperatorLogQueryDto : PaginatedTimeQueryDto
{
    /// <summary>
    /// 操作模块    
    ///</summary>
    public string? Title { get; set; }
}
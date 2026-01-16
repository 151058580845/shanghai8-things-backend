namespace Hgzn.Mes.Application.Main.Dtos.Audit;

/// <summary>
/// 增量备份导出请求DTO
/// </summary>
public class IncrementalBackupRequestDto
{
    /// <summary>
    /// 客户端标识（用于记录该客户端的导出时间戳）
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
    
    /// <summary>
    /// 表名（可选，如果为空则导出所有表）
    /// </summary>
    public string? TableName { get; set; }
    
    /// <summary>
    /// 数据库名称（可选，如果为空则使用当前数据库）
    /// </summary>
    public string? DatabaseName { get; set; }
    
    /// <summary>
    /// 上次导出时间（如果为空，则从Redis获取；如果Redis也没有，则导出全部数据）
    /// </summary>
    public DateTime? LastExportTime { get; set; }
    
    /// <summary>
    /// 是否更新导出时间戳（默认true，导出成功后更新）
    /// </summary>
    public bool UpdateTimestamp { get; set; } = true;
    
    /// <summary>
    /// 是否压缩（默认false）
    /// </summary>
    public bool Compress { get; set; } = false;
}

/// <summary>
/// 增量备份状态查询DTO
/// </summary>
public class IncrementalBackupStatusDto
{
    /// <summary>
    /// 客户端ID
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
    
    /// <summary>
    /// 上次导出时间
    /// </summary>
    public DateTime? LastExportTime { get; set; }
    
    /// <summary>
    /// 是否有导出历史
    /// </summary>
    public bool HasExportHistory { get; set; }
    
    /// <summary>
    /// 导出记录数（如果有）
    /// </summary>
    public int? LastExportRecordCount { get; set; }
}

/// <summary>
/// 增量备份导出结果DTO
/// </summary>
public class IncrementalBackupResultDto
{
    /// <summary>
    /// 导出时间
    /// </summary>
    public DateTime ExportTime { get; set; }
    
    /// <summary>
    /// 上次导出时间
    /// </summary>
    public DateTime? LastExportTime { get; set; }
    
    /// <summary>
    /// 导出记录数
    /// </summary>
    public int RecordCount { get; set; }
    
    /// <summary>
    /// 总记录数
    /// </summary>
    public int TotalCount => RecordCount;
    
    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }
    
    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName { get; set; } = string.Empty;
}

/// <summary>
/// 增量备份导入请求DTO
/// </summary>
public class IncrementalBackupImportRequestDto
{
    /// <summary>
    /// 备份文件数据（JSON格式）
    /// </summary>
    public byte[] FileData { get; set; } = Array.Empty<byte>();
    
    /// <summary>
    /// 是否压缩
    /// </summary>
    public bool Compress { get; set; } = false;
    
    /// <summary>
    /// 客户端ID（可选，用于记录）
    /// </summary>
    public string? ClientId { get; set; }
}

/// <summary>
/// 增量备份导入结果DTO
/// </summary>
public class IncrementalBackupImportResultDto
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// 导入的记录数（新增/更新）
    /// </summary>
    public int InsertUpdateCount { get; set; }
    
    /// <summary>
    /// 总记录数
    /// </summary>
    public int TotalCount => InsertUpdateCount;
    
    /// <summary>
    /// 错误消息（如果有）
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// 导入时间
    /// </summary>
    public DateTime ImportTime { get; set; } = DateTime.Now;
}
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Hgzn.Mes.Domain.Entities.Base;
using SqlSugar;

namespace Hgzn.Mes.Domain.Entities.Audit;

/// <summary>
/// 增量备份导出时间戳
/// </summary>
[Description("增量备份导出时间戳")]
[SugarTable("incremental_backup_timestamp")]
public class IncrementalBackupTimestamp : AggregateRoot
{
    /// <summary>
    /// 客户端ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "客户端ID")]
    [Required]
    [StringLength(100)]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// 上次导出时间
    /// </summary>
    [SugarColumn(ColumnDescription = "上次导出时间")]
    [Required]
    public DateTime LastExportTime { get; set; }

    /// <summary>
    /// 上次导出记录数（可选）
    /// </summary>
    [SugarColumn(ColumnDescription = "上次导出记录数")]
    public int? LastExportRecordCount { get; set; }

    /// <summary>
    /// 创建时间（记录首次创建时间）
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreationTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 最后修改时间
    /// </summary>
    [SugarColumn(ColumnDescription = "最后修改时间")]
    public DateTime LastModificationTime { get; set; } = DateTime.Now;
}

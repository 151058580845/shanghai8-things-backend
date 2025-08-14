using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.System;

[Description("数据库备份版本管理")]
// public class DbConfig: UniversalEntity,ICreationAudited
public class DbConfig
{
    [Description("数据库标识")]
    public string? Tag { get; set; }
    [Description("数据库名称")]
    public string? DbName { get; set; }
    [Description("数据库连接字符串")]
    public string? ConnectionString { get; set; }
    [Description("数据库类型")]
    public string? DbType { get; set; }
    [Description("数据库版本")]
    public string? DbVersion { get; set; }
    [Description("数据库描述")]
    public string? Description { get; set; }
    [Description("备份数据库文件")]
    public string? BackFile { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
    public int CreatorLevel { get; set; }
}
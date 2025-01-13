using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlSugar;

namespace Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;

public sealed class SqlSugarContext
{
    public ISqlSugarClient DbContext { get; set; }
    private DbConnOptions DbOptions { get; set; }
    private ILoggerFactory Logger { get; set; }

    public SqlSugarContext(IOptions<DbConnOptions> dbConnOptions, ILoggerFactory logger)
    {
        DbOptions = dbConnOptions.Value;
        Logger = logger;
        DbContext = new SqlSugarClient(Build());
        OnSqlSugarClientConfig(DbContext);
        DbContext.Aop.OnLogExecuting = OnLogExecuting;
        DbContext.Aop.OnLogExecuted = OnLogExecuted;
    }
    private readonly Dictionary<string, string> _prefixDic = new()
    {
        { "Equip", "EQUIP_" },
        { "System", "BASE_" }
    };
    private const string DefaultConnectionStringName = "Default";

    /// <summary>
    /// 初始化连接字符串
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    private ConnectionConfig Build()
    {
        if (DbOptions.DbType is null)
        {
            throw new NullReferenceException("DbOptions.DbType is null");
        }

        var slavaConfig = new List<SlaveConnectionConfig>();
        var connConfig = new ConnectionConfig()
        {
            ConfigId = DefaultConnectionStringName,
            DbType = DbOptions.DbType ?? DbType.Sqlite,
            ConnectionString = DbOptions.Url,
            InitKeyType = InitKeyType.Attribute,
            IsAutoCloseConnection = true,
            SlaveConnectionConfigs = slavaConfig,
            ConfigureExternalServices = new ConfigureExternalServices()
            {
                EntityNameService = (t, e) =>
                {
                    string? tableName;
                    var table = t.GetCustomAttribute<TableAttribute>();
                    var name = t.FullName?.Split('.');
                    var tablePrefix = "";
                    if (name != null && name.Length > 3)
                    {
                        tablePrefix = name[4];
                    }
                    tableName = table != null ? table.Name : name?[^1];
                    var tableDesc = t.GetCustomAttribute<DescriptionAttribute>();

                    if (_prefixDic.TryGetValue(tablePrefix, out var prefix) && !string.IsNullOrEmpty(prefix))
                    {
                        e.DbTableName = prefix + tableName;
                        e.TableDescription = tableDesc?.Description;
                    }
                },
                EntityService = (p, c) =>
                {
                    //配置表外键
                    c.IfTable<EquipLedger>()
                        .OneToOne(t => t.Room, nameof(EquipLedger.RoomId));

                    var desc = p.GetCustomAttribute<DescriptionAttribute>();
                    c.ColumnDescription = desc?.Description;
                    if (new NullabilityInfoContext().Create(p).WriteState is NullabilityState.Nullable)
                    {
                        c.IsNullable = true;
                    }
                    if (p.Name.ToLower() == "id")
                    {
                        c.IsPrimarykey = true;
                    }
                }
            }
        };
        return connConfig;
    }

    /// <summary>
    /// 实体配置
    /// </summary>
    /// <param name="sqlSugarClient"></param>
    private void OnSqlSugarClientConfig(ISqlSugarClient sqlSugarClient)
    {
        //是否开启软删除查询
        if (DbOptions.IsSoftDelete)
            sqlSugarClient.QueryFilter.AddTableFilter<ISoftDelete>(t => t.SoftDeleted == false);
    }

    /// <summary>
    /// 初始化数据表
    /// </summary>
    public void InitTables()
    {
        if (DbOptions.EnabledCodeFirst)
        {
            var tables = Assembly.Load("Hgzn.Mes." + nameof(Domain))
                .GetTypes()
                .Where(t => t.GetCustomAttribute<TableAttribute>() != null)
                .ToArray();
            DbContext.CodeFirst.InitTables(tables);
        }
    }

    /// <summary>
    /// 软删除实现
    /// </summary>
    /// <param name="id"></param>
    /// <typeparam name="T"></typeparam>
    public async Task SoftDeleteAsync<T>(Guid id) where T : class, ISoftDelete, new()
    {
        var entity = await DbContext.Queryable<T>().In(id).SingleAsync();
        if (entity is { SoftDeleted: false })
        {
            entity.SoftDeleted = true;
            entity.DeleteTime = DateTime.Now;
            await DbContext.Updateable(entity).ExecuteCommandAsync();
        }
    }

    /// <summary>
    /// 硬删除实现
    /// </summary>
    /// <param name="id"></param>
    /// <typeparam name="T"></typeparam>
    public async Task HardDeleteAsync<T>(Guid id) where T : class, new()
    {
        await DbContext.Deleteable<T>().In(id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 日志
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="pars"></param>
    private void OnLogExecuting(string sql, SugarParameter[] pars)
    {
        if (DbOptions.EnabledSqlLog)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("==========SQL执行:==========");
            sb.AppendLine(UtilMethods.GetSqlString(DbType.SqlServer, sql, pars));
            sb.AppendLine("===============================");
            Logger.CreateLogger<SqlSugarContext>().LogDebug(sb.ToString());
        }
    }

    /// <summary>
    /// 日志
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="pars"></param>
    private void OnLogExecuted(string sql, SugarParameter[] pars)
    {
        if (DbOptions.EnabledSqlLog)
        {
            var sqllog = $"=========SQL耗时{DbContext.Ado.SqlExecutionTime.TotalMilliseconds}毫秒=====";
            Logger.CreateLogger<SqlSugarContext>().LogDebug(sqllog);
        }
    }

    /// <summary>
    /// 备份
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task BackUpDataBaseAsync()
    {
        string directory = Path.Combine(Directory.GetCurrentDirectory(), "database_backup");
        string fileName = DateTime.Now.ToString($"yyyyMMdd_HHmmss") + $"_{DbContext.Ado.Connection.Database}";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        switch (DbOptions.DbType)
        {
            case DbType.MySql:
                //MySql
                DbContext.DbMaintenance.BackupDataBase(DbContext.Ado.Connection.Database,
                    $"{Path.Combine(directory, fileName)}.sql"); //mysql 只支持.net core
                break;


            case DbType.Sqlite:
                //Sqlite
                DbContext.DbMaintenance.BackupDataBase(null, $"{fileName}.db"); //sqlite 只支持.net core
                break;


            case DbType.SqlServer:
                //SqlServer
                DbContext.DbMaintenance.BackupDataBase(DbContext.Ado.Connection.Database,
                    $"{Path.Combine(directory, fileName)}.bak" /*服务器路径*/); //第一个参数库名 
                break;
            default:
                throw new NotImplementedException("其他数据库备份未实现");
        }
        return Task.CompletedTask;
    }
}
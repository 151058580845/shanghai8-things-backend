using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.System.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Hgzn.Mes.Infrastructure.SqlSugarContext;

public sealed class SqlSugarContext
{
    public ISqlSugarClient DbContext { get; set; }
    private DbConnOptions DbOptions { get; set; }
    private ILoggerFactory Logger {get;set;}

    public SqlSugarContext(IConfiguration configuration, DbConnOptions dbConnOptions, ILoggerFactory logger)
    {
        this.DbOptions = dbConnOptions;
        Logger = logger;
        var connectionString = configuration.GetConnectionString("SqlConnection");
        DbContext = new SqlSugarClient(Build());
        OnSqlSugarClientConfig(DbContext);
        InitTables();
        DbContext.Aop.OnLogExecuting = OnLogExecuting;
        DbContext.Aop.OnLogExecuted = OnLogExecuted;

    }
    public Dictionary<string, string> PrefixDic = new()
    {
        { "EquipManager", "EQUIP_" },
        { "EquipControl", "EQUIP_" },
        { "SystemManager", "BASE_" },
        { "WarehouseManagement", "WH_" }
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
                EntityService = (p, c) =>
                {
                    //配置表外键
                    c.IfTable<EquipLedger>()
                        .OneToOne(t => t.Room, nameof(EquipLedger.RoomId));
                    
                    
                    if (new NullabilityInfoContext().Create(p).WriteState is NullabilityState.Nullable)
                    {
                        c.IsNullable = true;
                    }
                    if (p.Name.ToLower() == "id")
                    {
                        c.IsPrimarykey = true;
                    }
                },
                EntityNameService = (t, e) =>
                {
                    var tableName = "";
                    var table = t.GetCustomAttribute<TableAttribute>();
                    if (table != null)
                    {
                        tableName = table.Name;
                    }
                    else
                    {
                        var name = t.FullName?.Split('.');
                        if (name != null && name.Length>1)
                        {
                            tableName = name[0];
                        }
                    }
                    var tableDesc = t.GetCustomAttribute<DescriptionAttribute>();
                    
                    if (PrefixDic.TryGetValue(tableName, out var prefix) && !string.IsNullOrEmpty(prefix))
                    {
                        e.DbTableName = prefix + tableName;
                        e.TableDescription = tableDesc?.Description;
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
            sqlSugarClient.QueryFilter.AddTableFilter<ISoftDelete>(t=>t.SoftDeleted == false);
    }

    /// <summary>
    /// 初始化数据表
    /// </summary>
    private void InitTables()
    {
        var tables = Assembly.Load("HgznMes." + nameof(Domain))
            .GetTypes()
            .Where(t => t.GetCustomAttribute<TableAttribute>() != null)
            .ToArray();
        DbContext.CodeFirst.InitTables(tables);
    }
    
    /// <summary>
    /// 软删除实现
    /// </summary>
    /// <param name="id"></param>
    /// <typeparam name="T"></typeparam>
    public async Task SoftDeleteAsync<T>(Guid id) where T : class, ISoftDelete, new()
    {
        var entity =await DbContext.Queryable<T>().In(id).SingleAsync();
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
            Logger.CreateLogger<SqlSugarContext>().LogDebug(sqllog.ToString());
        }
    }
    
    /// <summary>
    /// 备份
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task BackUpDataBaseAsync()
    {
        string directory =Path.Combine(Directory.GetCurrentDirectory(),"database_backup");
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
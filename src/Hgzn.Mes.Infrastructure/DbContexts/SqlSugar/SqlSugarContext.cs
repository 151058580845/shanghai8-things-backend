using CaseExtensions;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.System.Notice;
using Hgzn.Mes.Domain.Shared.Extensions;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Hgzn.Mes.Domain.Entities.System.Account;

namespace Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;

public sealed class SqlSugarContext
{
    public ISqlSugarClient DbContext { get; set; } = null!;
    private readonly DbConnOptions _dbOptions;
    private readonly ILogger<SqlSugarContext> _logger;

    public SqlSugarContext(
        ILogger<SqlSugarContext> logger, 
        DbConnOptions dbOptions, 
        ISqlSugarClient client
        )
    {
        _dbOptions = dbOptions;
        _logger = logger;
        DbContext = client;
        OnSqlSugarClientConfig(DbContext);
        DbContext.Aop.OnLogExecuting = OnLogExecuting;
        DbContext.Aop.OnLogExecuted = OnLogExecuted;
    }

    private const string DefaultConnectionStringName = "Default";

    /// <summary>
    /// 初始化连接字符串
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public static ConnectionConfig Build(DbConnOptions dbOptions)
    {
        if (dbOptions.DbType is null)
        {
            throw new NullReferenceException("DbOptions.DbType is null");
        }

        var slavaConfig = new List<SlaveConnectionConfig>();
        var connConfig = new ConnectionConfig()
        {
            ConfigId = DefaultConnectionStringName,
            DbType = dbOptions.DbType ?? DbType.Sqlite,
            ConnectionString = dbOptions.Url,
            InitKeyType = InitKeyType.Attribute,
            IsAutoCloseConnection = true,
            SlaveConnectionConfigs = slavaConfig,
            ConfigureExternalServices = new ConfigureExternalServices()
            {
                EntityNameService = (t, e) =>
                {
                    string? tableName;
                    var table = t.GetCustomAttribute<TableAttribute>();
                    var name = t.FullName!.Split('.');
                    var tablePrefix = "";
                    if (name.Length > 3)
                    {
                        tablePrefix = name[4] + '_';
                    }
                    tableName = table != null ? table.Name : name?[^1].ToSnakeCase();
                    var tableDesc = t.GetCustomAttribute<DescriptionAttribute>();

                    e.DbTableName = tablePrefix + tableName;
                    e.TableDescription = tableDesc?.Description;
                },
                EntityService = (p, c) =>
                {
                    //配置表外键
                    c.IfTable<EquipLedger>()
                        .OneToOne(t => t.Room, nameof(EquipLedger.RoomId));
                    c.IfTable<NoticeInfo>()
                        .OneToMany(t => t.NoticeTargets, nameof(NoticeTarget.NoticeId), nameof(NoticeInfo.Id));
                    c.IfTable<User>()
                        .ManyToMany(t=>t.Roles,typeof(UserRole),nameof(UserRole.UserId),nameof(UserRole.RoleId));
                    c.IfTable<Role>()
                        .ManyToMany(t=>t.Users,typeof(UserRole),nameof(UserRole.RoleId),nameof(UserRole.UserId));
                    var desc = p.GetCustomAttribute<DescriptionAttribute>();
                    c.ColumnDescription = desc?.Description;
                    var name = p.Name.ToSnakeCase();
                    c.DbColumnName = name;
                    if (new NullabilityInfoContext().Create(p).WriteState is NullabilityState.Nullable)
                    {
                        c.IsNullable = true;
                    }
                    if (p.GetMethod!.IsStatic || !p.PropertyType.IsDatabaseType())
                    {
                        c.IsIgnore = true;
                    }
                    if (name == "id")
                    {
                        c.IsPrimarykey = true;
                        c.IsIdentity = p.GetType() == typeof(int) || p.GetType() == typeof(long);
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
        if (_dbOptions.IsSoftDelete)
            sqlSugarClient.QueryFilter.AddTableFilter<ISoftDelete>(t => t.SoftDeleted == false);
    }

    public void SeedData()
    {
        var entities = Assembly.Load("Hgzn.Mes." + nameof(Domain))
            .GetTypes()
            .Where(t => (typeof(AggregateRoot)).IsAssignableFrom(t) && !t.Namespace!.Contains("Base"))
            .ToArray();

        foreach (var entity in entities)
        {
            var methodGeneric = typeof(ISqlSugarClient).GetMethods()
                .Where(me => me.Name == "Insertable" &&
                me.GetParameters().Length == 1 &&
                me.GetParameters()[0].ParameterType.IsArray);
            //获取静态种子属性
            var seeds = entity.GetProperty("Seeds", BindingFlags.Public | BindingFlags.Static);
            var value = seeds?.GetValue(null);
            if (value != null)
            {
                try
                {
                    var data = methodGeneric.FirstOrDefault()?.MakeGenericMethod(entity)
                    .Invoke(DbContext, [value]);
                    var typeInsert = typeof(IInsertable<>).MakeGenericType(entity);
                    var typeTask = typeof(Task<>).MakeGenericType(typeof(int));
                    var excuteMethod = typeInsert.GetMethods()
                        .Where(me => me.Name == "ExecuteCommandAsync" &&
                            me.GetParameters().Length == 0 &&
                            me.ReturnType == typeTask)
                        .FirstOrDefault();
                    var task = (Task?)excuteMethod?.Invoke(data, null);
                    task?.Wait();
                }
                //只捕获主键重复异常
                catch (AggregateException)
                {                    
                    _logger.LogWarning("data seeds exist");
                }

            }
        }
    }

    /// <summary>
    /// 初始化数据表
    /// </summary>
    public void InitTables()
    {
        if (_dbOptions.EnabledCodeFirst)
        {
            var tables = Assembly.Load("Hgzn.Mes." + nameof(Domain))
                .GetTypes()
                .Where(t => (typeof(AggregateRoot)).IsAssignableFrom(t) && !t.Namespace!.Contains("Base"))
                .ToArray();
            DbContext.CodeFirst.InitTables(tables);
        }
        SeedData();
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
        if (_dbOptions.EnabledSqlLog)
        {
            _logger.LogDebug($"sql excuting: {UtilMethods.GetSqlString(DbType.SqlServer, sql, pars)}");
        }
    }

    /// <summary>
    /// 日志
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="pars"></param>
    private void OnLogExecuted(string sql, SugarParameter[] pars)
    {
        if (_dbOptions.EnabledSqlLog)
        {
            _logger.LogDebug($"excuted take {DbContext.Ado.SqlExecutionTime.TotalMilliseconds}ms");
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
        switch (_dbOptions.DbType)
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
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
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Entities.System.Authority;
using Hgzn.Mes.Domain.Entities.System.Code;
using Hgzn.Mes.Infrastructure.Utilities.CurrentUser;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.System.Location;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Entities.Equip;
using Hgzn.Mes.Domain.Entities.Basic;
using Hgzn.Mes.Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.Text;
using AutoMapper.Internal;
using Hgzn.Mes.Domain.Shared.Exceptions;

namespace Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;

public sealed class SqlSugarContext
{
    public ISqlSugarClient DbContext { get; set; } = null!;
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private readonly DbConnOptions _dbOptions;
    private readonly ILogger<SqlSugarContext> _logger;
    private Guid? _userId;
    private List<Guid> _rids = [];
    private int _level = 0;
    /// <summary>
    /// 所有软删除的数据表
    /// </summary>
    private static readonly Dictionary<string, EntityInfo> EntityInfos=new();

    public SqlSugarContext(
        ILogger<SqlSugarContext> logger,
        DbConnOptions dbOptions,
        ISqlSugarClient client,
        IHttpContextAccessor? httpContextAccessor
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _dbOptions = dbOptions;
        _logger = logger;
        DbContext = client;
        if(_httpContextAccessor?.HttpContext?.User.Claims is not null)
        {
            var plain = _httpContextAccessor?.HttpContext?.User.Claims
                .FirstOrDefault(c => ClaimType.UserId == c.Type);
            var str = _httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(c =>
                c.Type == ClaimType.RoleId);
            if (str is not null) _rids = str.Value.Split(',').Select(Guid.Parse).ToList();
            _userId = plain is null ? null : Guid.Parse(plain.Value);
            var levelPlain = _httpContextAccessor?.HttpContext?.User.Claims
                .FirstOrDefault(c => ClaimType.Level == c.Type);
            _level = levelPlain is null ? -1 : int.Parse(levelPlain!.Value);
        }
        // DbContext = new SqlSugarClient(Build(dbOptions));
        OnSqlSugarClientConfig(DbContext);
        DbContext.Aop.OnLogExecuting = OnLogExecuting;
        DbContext.Aop.OnLogExecuted = OnLogExecuted;
        DbContext.Aop.DataExecuting = DataExecuting;
        //DbContext.Aop.OnExecutingChangeSql = OnExecutingChangeSql;
    }

    /// <summary>
    /// 软删除，将delete变成update
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private KeyValuePair<string, SugarParameter[]> OnExecutingChangeSql(string sql, SugarParameter[] parameters)
    {
        var span = sql.AsSpan().TrimStart();
        if (!span.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase)) 
            return new(sql, parameters);
        // 定位 FROM 后的表名
        var fromIdx = span.IndexOf("FROM ", StringComparison.OrdinalIgnoreCase);
        if (fromIdx < 0) 
            return new(sql, parameters);
        var afterFrom = span[(fromIdx + 5)..];
        var endIdx     = afterFrom.IndexOfAny(new[] {' ', '\r', '\n', '\t'});
        if (endIdx < 0) 
            return new(sql, parameters);

        var tableName = afterFrom[..endIdx].ToString();
        tableName = tableName
            .Trim()                   // 去两头空格
            .Trim('"', '[', ']');     // 去双引号和方括号
        if (!EntityInfos.TryGetValue(tableName, out var ei) ||
            !typeof(ISoftDelete).IsAssignableFrom(ei.Type))
        {
            return new(sql, parameters);
        }
        
        // 从元数据里拿到对应 C# 属性映射到的列名
        var colSoftDeleted = ei.Columns
            .First(c => c.PropertyName == nameof(ISoftDelete.SoftDeleted))
            .DbColumnName;
        var colDeleteTime  = ei.Columns
            .First(c => c.PropertyName == nameof(ISoftDelete.DeleteTime))
            .DbColumnName;
        var pars = parameters ?? [];
        // 拼 WHERE 子句
        var whereIdx = span.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase);
        var remainder = whereIdx >= 0 
            ? span[whereIdx..].ToString() 
            : "";
        var newSql = $"UPDATE {tableName} SET {colSoftDeleted} = @__softDeleted, {colDeleteTime}  = @__deleteTime {remainder}";
        var newPars = pars
            .Append(new SugarParameter("@__softDeleted", true))
            .Append(new SugarParameter("@__deleteTime", DateTime.Now))
            .ToArray();
        return new(newSql, newPars);
    }


    private void DataExecuted(object oldValue, DataAfterModel entityInfo)
    {
        throw new NotImplementedException();
    }

    private void DataExecuting(object oldValue, DataFilterModel entityInfo)
    {
        switch (entityInfo.OperationType)
        {
            case DataFilterType.UpdateByObject:
                
                if (entityInfo.PropertyName.Equals(nameof(ICreationAudited.CreatorId)))
                {
                    var dataLevel = ((ICreationAudited) entityInfo.EntityValue).CreatorLevel;
                    if (dataLevel < _level) 
                        throw new ForbiddenException("not allowed to update this data");
                }
                if (entityInfo.PropertyName.Equals(nameof(IAudited.LastModificationTime)))
                {
                    entityInfo.SetValue(DateTime.Now);
                }

                if (entityInfo.PropertyName.Equals(nameof(IAudited.LastModifierId)))
                {
                    if (_userId != null)
                    {
                        entityInfo.SetValue(_userId);
                    }
                }

                break;
            case DataFilterType.InsertByObject:
                if (entityInfo.PropertyName.Equals(nameof(UniversalEntity.Id)))
                {
                    //主键为空或者为默认最小值
                    if (Guid.Empty.Equals(oldValue))
                    {
                        entityInfo.SetValue(Guid.NewGuid());
                    }
                }

                if (entityInfo.PropertyName.Equals(nameof(IAudited.CreationTime)))
                {
                    // if (!DateTime.MinValue.Equals(oldValue))
                    // {
                    entityInfo.SetValue(DateTime.Now);
                    // }
                }

                if (entityInfo.PropertyName.Equals(nameof(IAudited.CreatorId)))
                {
                    if (_userId != null)
                    {
                        entityInfo.SetValue(_userId);
                    }
                }
                
                if (entityInfo.PropertyName.Equals(nameof(IAudited.CreatorLevel)))
                {
                    entityInfo.SetValue(_level);
                }

                break;
            case DataFilterType.DeleteByObject:
                // entityInfo.SetValue(oldValue);
                // if (entityInfo.PropertyName.Equals(nameof(ICreationAudited.CreatorId)))
                // {
                //     var dataLevel = ((ICreationAudited)entityInfo.EntityValue).CreatorLevel;
                //     if (dataLevel < _level)
                //         throw new ForbiddenException("not allowed to update this data");
                // }
                // if (entityInfo.PropertyName.Equals(nameof(UniversalEntity.Id)))
                // {
                //     if (entityInfo.EntityValue is ISoftDelete softDelete)
                //     {
                //         if (entityInfo.PropertyName.Equals(nameof(ISoftDelete.SoftDeleted)))
                //         {
                //             entityInfo.SetValue(1);
                //         }
                //
                //         if (entityInfo.PropertyName.Equals(nameof(ISoftDelete.DeleteTime)))
                //         {
                //             entityInfo.SetValue(DateTime.Now);
                //         }
                //     }
                // }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private const string DefaultConnectionStringName = "Default";

    public async Task GetSeedsCodeFromDatabaseAsync()
    {
        var entities = Assembly.Load("Hgzn.Mes." + nameof(Domain))
            .GetTypes()
            .Where(t => (typeof(ISeedsGeneratable)).IsAssignableFrom(t) && !t.Namespace!.Contains("Base"))
            .ToArray();
        foreach (var entity in entities)
        {
            var methodinfo = typeof(SqlSugarContext).GetMethod($"GetSeedsFromDatabaseAsync");
            var task = (Task?)methodinfo?.MakeGenericMethod(entity).Invoke(this, null);
            await task!.WaitAsync(TimeSpan.FromSeconds(10));
        }
    }

    public async Task<string> GetSeedsFromDatabaseAsync<TEntity>() where TEntity : AggregateRoot
    {
        var entities = await DbContext.Queryable<TEntity>().ToArrayAsync();
        var builder = new StringBuilder();
        var type = typeof(TEntity);
        var index = 0;
        builder.Append("#region static\r\n\r\n");
        var seedsBuilder = new StringBuilder();
        seedsBuilder.AppendLine($"public static {type.Name} [] Seeds {{ get; }} = \r\n[");
        foreach (var entity in entities)
        {
            builder.AppendLine($"public static {type.Name} {type.Name}_{index} = new ()");
            seedsBuilder.Append($"\t{type.Name}_{index},\r\n");
            builder.AppendLine("{");
            foreach(var property in type.GetProperties())
            {
                var propertyPrefix = $"  {property.Name} = ";
                var value = property.GetValue(entity);
                var propertyType = property.PropertyType.IsNullableType() ?
                    property.PropertyType.GetGenericArguments().First() : property.PropertyType;
                //枚举类型typecode是对应的数值类型
                var propertySuffix = propertyType.IsEnum ?
                    (value is null ? "null," : $"({propertyType.Name}){(int)value},") :
                    Type.GetTypeCode(propertyType) switch
                {
                    TypeCode.DBNull or TypeCode.Empty => "null,",
                    TypeCode.DateTime => string.IsNullOrEmpty(Convert.ToString(value)) ? "null," : $"DateTime.Parse(\"{value}\"),",
                    TypeCode.String => value is null ? "null," : $"\"{((string)value).Replace("\\", "\\\\").Replace("\"", "\\\"")}\",",
                    TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32 or
                    TypeCode.Double or TypeCode.Single or TypeCode.Decimal or
                    TypeCode.UInt64 or TypeCode.Int64 => value is null ? "null," : $"{value},",
                    TypeCode.Boolean => $"{property.GetValue(entity)},".ToLower(),
                    _ => DealWithDefaultSuffix(propertyType, value)
                };
                if (propertySuffix is null) continue;
                builder.Append(propertyPrefix + propertySuffix + "\r\n");
            }
            builder.AppendLine("};\r\n");
            index++;
        }
        seedsBuilder.AppendLine("];");
        builder.AppendLine("#endregion static");

        builder.AppendLine(seedsBuilder.ToString());
        string? DealWithDefaultSuffix(Type propertyType, object? value)
        {
            if (propertyType.Name == "guid" || propertyType.Name == "Guid")
            {
                return string.IsNullOrEmpty(Convert.ToString(value)) ? "null," : $"new Guid(\"{value}\"),";
            }
            return null;
        }
        var code = builder.ToString();
        LoggerAdapter.LogTrace($"[已生成种子代码<{typeof(TEntity).Name}>]:\r\n {code}\r\n");
        return code;
    }
    public void InitDatabase()
    {
        if (_dbOptions.DbType == DbType.OpenGauss || _dbOptions.DbType == DbType.PostgreSQL)
        {


            var uri = _dbOptions.Url!.Split(";");
            var newDatabaseName = uri[1].Substring(uri[1].IndexOf('=') + 1);
            uri[1] = "DATABASE=postgres";
            string connectionString = string.Join(";", uri);

            var connConfig = new ConnectionConfig()
            {
                ConfigId = DefaultConnectionStringName,
                DbType = _dbOptions.DbType ?? DbType.Sqlite,
                ConnectionString = connectionString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            };
            var client = new SqlSugarClient(connConfig);

            string checkDbQuery = $"SELECT 1 FROM pg_database WHERE datname = '{newDatabaseName}'";
            var data = client.Ado.SqlQuery<string>(checkDbQuery);
            if (data.Count == 0)
            {
                LoggerAdapter.LogWarning($"Database '{newDatabaseName}' does not exist. Creating it...");
                // 如果数据库不存在，创建数据库
                string createDbQuery = $"CREATE DATABASE {newDatabaseName}";
                client.Ado.SqlQuery<string>(createDbQuery);
                LoggerAdapter.LogInformation($"Database '{newDatabaseName}' created.");
            }
            else
            {
                LoggerAdapter.LogInformation($"Database '{newDatabaseName}' already exists.");
            }
        }

        InitTables();
    }

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
                    if (typeof(ISoftDelete).IsAssignableFrom(t) && !EntityInfos.ContainsKey(e.DbTableName.ToLower()))
                    {
                        EntityInfos.Add(e.DbTableName.ToLower(), e);
                    }
                },
                EntityService = (p, c) =>
                {
                    //配置表外键
                    c.IfTable<EquipConnect>()
                        .OneToOne(t => t.EquipLedger, nameof(EquipConnect.EquipId));
                    c.IfTable<EquipLedger>()
                        .OneToOne(t => t.EquipType, nameof(EquipLedger.TypeId));
                    c.IfTable<EquipLedger>()
                        .OneToOne(t => t.Room, nameof(EquipLedger.RoomId));
                    c.IfTable<LocationLabel>()
                        .OneToOne(t => t.Room, nameof(LocationLabel.RoomId));
                    c.IfTable<LocationLabel>()
                        .OneToOne(t => t.EquipLedger, nameof(LocationLabel.EquipLedgerId));
                    c.IfTable<NoticeInfo>()
                        .OneToMany(t => t.NoticeTargets, nameof(NoticeTarget.NoticeId), nameof(NoticeInfo.Id));
                    c.IfTable<User>()
                        .ManyToMany(t => t.Roles, typeof(UserRole), nameof(UserRole.UserId), nameof(UserRole.RoleId));
                    c.IfTable<Role>()
                        .ManyToMany(t => t.Menus, typeof(RoleMenu), nameof(RoleMenu.RoleId), nameof(RoleMenu.MenuId));
                    c.IfTable<EquipLedger>()
                        .OneToMany(t => t.Labels, nameof(LocationLabel.EquipLedgerId), nameof(EquipLedger.Id));
                    c.IfTable<CodeRule>()
                     .OneToMany(t => t.CodeRuleRules, nameof(CodeRuleDefine.CodeRuleId), nameof(CodeRule.Id));

                    c.IfTable<Supplier>()
                       .OneToMany(t => t.Contacts, nameof(Contact.ParentId), nameof(Supplier.Id));
                    c.IfTable<Supplier>()
                        .OneToMany(t => t.AddressBs, nameof(AddressB.ParentId), nameof(Supplier.Id));

                    c.IfTable<Customer>()
                   .OneToMany(t => t.Contacts, nameof(Contact.ParentId), nameof(Customer.Id));
                    c.IfTable<Customer>()
                        .OneToMany(t => t.AddressBs, nameof(AddressB.ParentId), nameof(Customer.Id));

                    c.IfTable<Supplier>()
                       .OneToMany(t => t.Contacts, nameof(Contact.ParentId), nameof(Supplier.Id));
                    c.IfTable<Supplier>()
                        .OneToMany(t => t.AddressBs, nameof(AddressB.ParentId), nameof(Supplier.Id));

                    c.IfTable<Building>()
                        .OneToMany(t => t.Floors, nameof(Floor.ParentId), nameof(Building.Id));
                    c.IfTable<Floor>()
                        .OneToMany(t => t.Rooms, nameof(Room.ParentId), nameof(Floor.Id));
                    c.IfTable<TestData>()
                     .OneToMany(t => t.UUT, nameof(TestDataProduct.TestDataId), nameof(TestData.Id));
                    c.IfTable<EquipDataPoint>()
                     .OneToOne(t => t.EquipReceiveData, nameof(EquipDataPoint.EquipReceiveDataId))
                     .OneToOne(t => t.Connection, nameof(EquipDataPoint.ConnectionId));
                    
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

                    // if (c.EntityName == nameof(RoleMenu))
                    // {
                    //     LoggerAdapter.WriteLine("asdasd");
                    // }
                    if (name == "id")
                    {
                        c.IsPrimarykey = true;
                        c.IsIdentity = p.PropertyType == typeof(int) || p.PropertyType == typeof(long);
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
            sqlSugarClient.QueryFilter.AddTableFilter<ISoftDelete>(t => !t.SoftDeleted);
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
                    _logger.LogWarning($"{entity.Name} data seeds exist");
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
            DbContext.CopyNew().CodeFirst.InitTables(tables);
        }

        if (_dbOptions.EnabledDbSeed)
        {
            SeedData();
        }
    }

    // /// <summary>
    // /// 软删除实现
    // /// </summary>
    // /// <param name="id"></param>
    // /// <typeparam name="T"></typeparam>
    // public async Task SoftDeleteAsync<T>(Guid id) where T : class, ISoftDelete, new()
    // {
    //     var entity = await DbContext.Queryable<T>().In(id).SingleAsync();
    //     if (entity is { SoftDeleted: false })
    //     {
    //         entity.SoftDeleted = true;
    //         entity.DeleteTime = DateTime.Now;
    //         await DbContext.Updateable(entity).ExecuteCommandAsync();
    //     }
    // }
    //
    // /// <summary>
    // /// 硬删除实现
    // /// </summary>
    // /// <param name="id"></param>
    // /// <typeparam name="T"></typeparam>
    // public async Task HardDeleteAsync<T>(Guid id) where T : class, new()
    // {
    //     await DbContext.Deleteable<T>().In(id).ExecuteCommandAsync();
    // }

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
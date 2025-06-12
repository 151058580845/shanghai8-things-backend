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
using Org.BouncyCastle.Asn1.X509.Qualified;
using Microsoft.Extensions.Primitives;
using AutoMapper.Internal;

namespace Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;

public sealed class SqlSugarContext
{
    public ISqlSugarClient DbContext { get; set; } = null!;
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private readonly DbConnOptions _dbOptions;
    private readonly ILogger<SqlSugarContext> _logger;
    private Guid? _userId;

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
        var plain = _httpContextAccessor?.HttpContext?.User.Claims
            .FirstOrDefault(c => ClaimType.UserId == c.Type);
        _userId = plain is null ? null : Guid.Parse(plain.Value);
        // DbContext = new SqlSugarClient(Build(dbOptions));
        OnSqlSugarClientConfig(DbContext);
        DbContext.Aop.OnLogExecuting = OnLogExecuting;
        DbContext.Aop.OnLogExecuted = OnLogExecuted;
        DbContext.Aop.DataExecuting = DataExecuting;
        // DbContext.Aop.DataExecuted = DataExecuted;
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

                break;
            case DataFilterType.DeleteByObject:
                if (entityInfo.PropertyName.Equals(nameof(UniversalEntity.Id)))
                {
                    if (entityInfo.EntityValue is ISoftDelete softDelete)
                    {
                        if (entityInfo.PropertyName.Equals(nameof(ISoftDelete.SoftDeleted)))
                        {
                            entityInfo.SetValue(1);
                        }

                        if (entityInfo.PropertyName.Equals(nameof(ISoftDelete.DeleteTime)))
                        {
                            entityInfo.SetValue(DateTime.Now);
                        }
                    }
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private const string DefaultConnectionStringName = "Default";
    public async Task<string> GetSeedsFromDatabase<TEntity>() where TEntity : AggregateRoot
    {
        var entities = await DbContext.Queryable<TEntity>().ToArrayAsync();
        var builder = new StringBuilder();
        var type = typeof(TEntity);
        var index = 0;
        builder.Append("#region static\r\n\r\n");

        foreach (var entity in entities)
        {
            builder.AppendLine($"public static {type.Name} {type.Name}_{index} = new ()");
            builder.AppendLine("{");
            foreach(var property in type.GetProperties())
            {
                var propertyPrefix = $"  {property.Name} = ";
                var value = property.GetValue(entity);
                var propertyType = property.PropertyType.IsNullableType() ?
                    property.PropertyType.GetGenericArguments().First() : property.PropertyType;
                var propertySuffix = Type.GetTypeCode(propertyType) switch
                {
                    TypeCode.DBNull or TypeCode.Empty => "null,",
                    TypeCode.DateTime => string.IsNullOrEmpty(Convert.ToString(value)) ? "null," : $"DateTime.Parse(\"{value}\"),",
                    TypeCode.String => value is null ? "null," : $"\"{value}\",",
                    TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32 or
                    TypeCode.Double or TypeCode.Single or TypeCode.Decimal or
                    TypeCode.UInt64 or TypeCode.Int64 => value is null ? "null," : $"{value},",
                    TypeCode.Boolean => $"{property.GetValue(entity)},".ToLower(),
                    _ => $"{DealWithDefaultSuffix(propertyType, value)},"
                };
                builder.Append(propertyPrefix + propertySuffix + "\r\n");
            }
            builder.AppendLine("};\r\n");
            index++;
        }
        builder.Append("#endregion static");

        string DealWithDefaultSuffix(Type propertyType, object? value)
        {
            if (propertyType.IsEnum)
            {
                return value is null ? "null" : $"({propertyType.Name}){value}";
            }
            if (propertyType.Name == "guid" || propertyType.Name == "Guid")
            {
                return string.IsNullOrEmpty(Convert.ToString(value)) ? "null" : $"new Guid(\"{value}\")";
            }
            return $"throw new NotImplementedException()";
        }

        return builder.ToString();
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
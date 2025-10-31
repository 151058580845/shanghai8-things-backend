using CaseExtensions;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.System.Notice;
using Hgzn.Mes.Domain.Shared.Extensions;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
using System.Text.Json;
using AutoMapper.Internal;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_307_ReceiveDatas;
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
    private static readonly Dictionary<string, EntityInfo> EntityInfos = new();

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
        if (_httpContextAccessor?.HttpContext?.User.Claims is not null)
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
        var endIdx = afterFrom.IndexOfAny(new[] { ' ', '\r', '\n', '\t' });
        if (endIdx < 0)
            return new(sql, parameters);

        var tableName = afterFrom[..endIdx].ToString();
        tableName = tableName
            .Trim() // 去两头空格
            .Trim('"', '[', ']'); // 去双引号和方括号
        if (!EntityInfos.TryGetValue(tableName, out var ei) ||
            !typeof(ISoftDelete).IsAssignableFrom(ei.Type))
        {
            return new(sql, parameters);
        }

        // 从元数据里拿到对应 C# 属性映射到的列名
        var colSoftDeleted = ei.Columns
            .First(c => c.PropertyName == nameof(ISoftDelete.SoftDeleted))
            .DbColumnName;
        var colDeleteTime = ei.Columns
            .First(c => c.PropertyName == nameof(ISoftDelete.DeleteTime))
            .DbColumnName;
        var pars = parameters ?? [];
        // 拼 WHERE 子句
        var whereIdx = span.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase);
        var remainder = whereIdx >= 0
            ? span[whereIdx..].ToString()
            : "";
        var newSql =
            $"UPDATE {tableName} SET {colSoftDeleted} = @__softDeleted, {colDeleteTime}  = @__deleteTime {remainder}";
        var newPars = pars
            .Append(new SugarParameter("@__softDeleted", true))
            .Append(new SugarParameter("@__deleteTime", DateTime.Now.ToLocalTime()))
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
                    int? dataLevel = (entityInfo.EntityValue as ICreationAudited)?.CreatorLevel;
                    if (dataLevel != null && dataLevel < _level)
                        throw new ForbiddenException("not allowed to update this data");
                }

                if (entityInfo.PropertyName.Equals(nameof(IAudited.LastModificationTime)))
                {
                    entityInfo.SetValue(DateTime.Now.ToLocalTime());
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
                    entityInfo.SetValue(DateTime.Now.ToLocalTime());
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
                //             entityInfo.SetValue(DateTime.Now.ToLocalTime());
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
            foreach (var property in type.GetProperties())
            {
                var propertyPrefix = $"  {property.Name} = ";
                var value = property.GetValue(entity);
                var propertyType = property.PropertyType.IsNullableType()
                    ? property.PropertyType.GetGenericArguments().First()
                    : property.PropertyType;
                //枚举类型typecode是对应的数值类型
                var propertySuffix = propertyType.IsEnum
                    ? (value is null ? "null," : $"({propertyType.Name}){(int)value},")
                    : Type.GetTypeCode(propertyType) switch
                    {
                        TypeCode.DBNull or TypeCode.Empty => "null,",
                        TypeCode.DateTime => string.IsNullOrEmpty(Convert.ToString(value))
                            ? "null,"
                            : $"DateTime.Parse(\"{value}\"),",
                        TypeCode.String => value is null
                            ? "null,"
                            : $"\"{((string)value).Replace("\\", "\\\\").Replace("\"", "\\\"")}\",",
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
        _logger.LogTrace($"[已生成种子代码<{typeof(TEntity).Name}>]:\r\n {code}\r\n");
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
                _logger.LogWarning($"Database '{newDatabaseName}' does not exist. Creating it...");
                // 如果数据库不存在，创建数据库
                string createDbQuery = $"CREATE DATABASE {newDatabaseName}";
                client.Ado.SqlQuery<string>(createDbQuery);
                _logger.LogInformation($"Database '{newDatabaseName}' created.");
            }
            else
            {
                _logger.LogInformation($"Database '{newDatabaseName}' already exists.");
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
                    if (tableName == nameof(Receive))
                    {
                        return;
                    }

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
                        .OneToMany(t => t.UUT, nameof(TestDataProduct.TestDataId), nameof(TestData.TestDataId))
                        .OneToMany(t => t.UST, nameof(TestDataUST.TestDataId), nameof(TestData.TestDataId));
                    c.IfTable<EquipDataPoint>()
                        .OneToOne(t => t.EquipReceiveData, nameof(EquipDataPoint.EquipReceiveDataId))
                        .OneToOne(t => t.Connection, nameof(EquipDataPoint.ConnectionId));
                    c.IfTable<AssetData>().OneToMany(t => t.Projects, nameof(AssetDataProjectItem.SystemId), nameof(AssetData.Id));

                    var desc = p.GetCustomAttribute<DescriptionAttribute>();
                    c.ColumnDescription = desc?.Description;
                    var name = p.Name.ToSnakeCase();
                    c.DbColumnName = name;
                    if (new NullabilityInfoContext().Create(p).WriteState is NullabilityState.Nullable)
                    {
                        c.IsNullable = true;
                    }

                    if (p.Name.Contains("receive"))
                    {
                        Console.WriteLine();
                    }
                    // 如果有 Required 特性，则不忽略
                    bool hasRequired = p.GetCustomAttribute<RequiredAttribute>() != null;

                    if (!hasRequired && (p.GetMethod!.IsStatic || !p.PropertyType.IsDatabaseType()))
                    {
                        c.IsIgnore = true;
                    }

                    // if (c.EntityName == nameof(RoleMenu))
                    // {
                    //     _logger.WriteLine("asdasd");
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
            // foreach (var allReceiveDataTest in GetAllReceiveDataTests())
            // {
            //     Console.WriteLine(allReceiveDataTest.ToString());
            //     
            // }
            DbContext.Insertable(GetAllReceiveDataTests())
                .SplitTable()
                .ExecuteCommand();
        }


    }

    //测试自动分页，获取数据
    public List<Receive> GetAllReceiveDataTests()
    {
        List<Receive> list = [];
        List<string> uuids =
        [
            "0198BFBC-2382-762B-8C99-AD1E7FFC047B",
            "0198BFBC-2382-762B-8C99-B316283CF7B7",
            "0198BFBC-2382-762B-8C99-B641B482AF27",
            "0198BFBC-2382-762B-8C99-B8EE325C8EAC",
            "0198BFBC-2382-762B-8C99-BDF08284F050",
            "0198BFBC-2382-762B-8C99-C1757578755F",
            "0198BFBC-2382-762B-8C99-C45F7FD09501",
            "0198BFBC-2382-762B-8C99-C9A015AB7406",
            "0198BFBC-2382-762B-8C99-CDDC14CF275A",
            "0198BFBC-2382-762B-8C99-D26CC8F98D08",
            "0198BFBC-2382-762B-8C99-D6C0AB71FA2A",
            "0198BFBC-2382-762B-8C99-D986267A162D",
            "0198BFBC-2382-762B-8C99-DCD1C9E91CEA",
            "0198BFBC-2382-762B-8C99-E115176A5BF4",
            "0198BFBC-2382-762B-8C99-E5DC6A8CC042",
            "0198BFBC-2382-762B-8C99-E8C822AC5DB4",
            "0198BFBC-2382-762B-8C99-ED203932858B",
            "0198BFBC-2382-762B-8C99-F13916C88A89",
            "0198BFBC-2382-762B-8C99-F4EBC7BAAE9A",
            "0198BFBC-2382-762B-8C99-F893F27DF255",
            "0198BFBC-2382-762B-8C99-FC310E176296",
            "0198BFBC-2382-762B-8C9A-03499F013EDA",
            "0198BFBC-2382-762B-8C9A-068644E74446",
            "0198BFBC-2382-762B-8C9A-0AB327526FEE",
            "0198BFBC-2382-762B-8C9A-0F6910A2320A",
            "0198BFBC-2382-762B-8C9A-1104B2C3BC7A",
            "0198BFBC-2382-762B-8C9A-144A5B602297",
            "0198BFBC-2382-762B-8C9A-1826B6C55706",
            "0198BFBC-2382-762B-8C9A-1DAC067921FE",
            "0198BFBC-2382-762B-8C9A-233BB40FD271",
            "0198BFBC-2382-762B-8C9A-24EF36CE907D",
            "0198BFBC-2382-762B-8C9A-2886110DDDD7",
            "0198BFBC-2382-762B-8C9A-2D51A4CE4D07",
            "0198BFBC-2383-7058-8BB1-ACBFEFFBA2C1",
            "0198BFBC-2383-7058-8BB1-B356C1977905",
            "0198BFBC-2383-7058-8BB1-B72E9F540C7C",
            "0198BFBC-2383-7058-8BB1-BBF8C91A276D",
            "0198BFBC-2383-7058-8BB1-BC5761EEEADE",
            "0198BFBC-2383-7058-8BB1-C2060E57821E",
            "0198BFBC-2383-7058-8BB1-C61F1E7861F6",
            "0198BFBC-2383-7058-8BB1-C8929556434A",
            "0198BFBC-2383-7058-8BB1-CD7E5CEB0410",
            "0198BFBC-2383-7058-8BB1-D2348890D991",
            "0198BFBC-2383-7058-8BB1-D5DD5EEFB527",
            "0198BFBC-2383-7058-8BB1-DBBCA29CB2A7",
            "0198BFBC-2383-7058-8BB1-DC59CE2B4B12",
            "0198BFBC-2383-7058-8BB1-E0BA6859329A",
            "0198BFBC-2383-7058-8BB1-E6024D445BE5",
            "0198BFBC-2383-7058-8BB1-E9D24E67FBB9",
            "0198BFBC-2383-7058-8BB1-EDB572144A34",
            "0198BFBC-2383-7058-8BB1-F0180841F9F7",
            "0198BFBC-2383-7058-8BB1-F7B97BA7F964",
            "0198BFBC-2383-7058-8BB1-F9B70FE5EC18",
            "0198BFBC-2383-7058-8BB1-FEB236CBDC27",
            "0198BFBC-2383-7058-8BB2-03F34DF9E1C2",
            "0198BFBC-2383-7058-8BB2-049BDAB7E610",
            "0198BFBC-2383-7058-8BB2-0B0889C48E18",
            "0198BFBC-2383-7058-8BB2-0F11F6561DEB",
            "0198BFBC-2383-7058-8BB2-10A30D318950",
            "0198BFBC-2383-7058-8BB2-16BC5C41EF02",
            "0198BFBC-2383-7058-8BB2-1B471198CE5A",
            "0198BFBC-2383-7058-8BB2-1D983040011E",
            "0198BFBC-2383-7058-8BB2-2091633B0D7F",
            "0198BFBC-2383-7058-8BB2-244A9E60DC72",
            "0198BFBC-2383-7058-8BB2-29D33ECEF404",
            "0198BFBC-2383-7058-8BB2-2C0695BDEF82",
            "0198BFBC-2383-7058-8BB2-3148FA36F817",
            "0198BFBC-2383-7058-8BB2-342EFA3EDCFC",
            "0198BFBC-2383-7058-8BB2-38D7CE5DBE98",
            "0198BFBC-2383-7058-8BB2-3E36566002DE",
            "0198BFBC-2383-7058-8BB2-404334898304",
            "0198BFBC-2383-7058-8BB2-45953BA29AE3",
            "0198BFBC-2383-7058-8BB2-487ED59A36DC",
            "0198BFBC-2383-7058-8BB2-4EDD14987C10",
            "0198BFBC-2383-7058-8BB2-506163C37764",
            "0198BFBC-2383-7058-8BB2-56B3E9476FAA",
            "0198BFBC-2383-7058-8BB2-582FDCDBD908",
            "0198BFBC-2383-7058-8BB2-5D6ADED9B03F",
            "0198BFBC-2383-7058-8BB2-6023560DB433",
            "0198BFBC-2383-7058-8BB2-66D46A4FA9AE",
            "0198BFBC-2383-7058-8BB2-6BA0233B4DCD",
            "0198BFBC-2383-7058-8BB2-6E7DEABA62ED",
            "0198BFBC-2383-7058-8BB2-72FC04AAC627",
            "0198BFBC-2383-7058-8BB2-7535BFAC0DA9",
            "0198BFBC-2383-7058-8BB2-7AACE3F85E9B",
            "0198BFBC-2383-7058-8BB2-7FF25A4CFC45",
            "0198BFBC-2383-7058-8BB2-80992183A6F9",
            "0198BFBC-2383-7058-8BB2-86DBF455A0D0",
            "0198BFBC-2383-7058-8BB2-8832E34FBEDB",
            "0198BFBC-2383-7058-8BB2-8ECA07BDDAFF",
            "0198BFBC-2383-7058-8BB2-93FDA1B3B54F",
            "0198BFBC-2383-7058-8BB2-9515777931DE",
            "0198BFBC-2383-7058-8BB2-984A550B5FE8",
            "0198BFBC-2383-7058-8BB2-9DA01030AC32",
            "0198BFBC-2383-7058-8BB2-A158EC114609",
            "0198BFBC-2383-7058-8BB2-A42AC6ED09C3",
            "0198BFBC-2383-7058-8BB2-A968296A4794",
            "0198BFBC-2383-7058-8BB2-ACF62F2AE834",
            "0198BFBC-2383-7058-8BB2-B0C0AA02EFBC",
            "0198BFBC-2383-7058-8BB2-B5C0B018CE86"
        ];
        List<string> equipUuids =
        [
            "0198BBB27F42727EB6F4A1AAA67A828B",
            "0198BBB27F42727EB6F4A68D9702EFE6",
            "0198BBB27F42727EB6F4AB3FBC007942",
            "0198BBB27F42727EB6F4ACE6DF594ABC",
            "0198BBB27F42727EB6F4B382B5DAB7E5",
            "0198BBB27F42727EB6F4B5169125885C",
            "0198BBB27F42727EB6F4BB0D2FB237AE",
            "0198BBB27F42727EB6F4BC1D86A9055D",
            "0198BBB27F42727EB6F4C3CF4DB5D446",
            "0198BBB27F42727EB6F4C7C00A3A738E"
        ];
        var rand = new Random();
        var now = DateTime.Now;
        int a = 0;
        List<Attribute> attributes = [];
        Type? type = null;
        Dictionary<PropertyInfo, int> properties = [];
        foreach (var uuid in uuids)
        {
            var b = Activator.CreateInstance<XT_307_SL_1_ReceiveData>();
            type ??= b.GetType();
            if (!properties.Any())
            {
                var tempProperties = type.GetProperties();
                foreach (var property in tempProperties)
                {
                    var attrs = property.GetCustomAttributes();
                    foreach (var attr in attrs)
                    {
                        if (attr is DescriptionAttribute descattr)
                        {
                            if (descattr.Description.Contains("-5V"))
                            {
                                properties.Add(property, -5);
                                break;
                            }
                            else if (descattr.Description.Contains("-12V"))
                            {
                                properties.Add(property, -12);
                                break;
                            }
                            else if (descattr.Description.Contains("5V"))
                            {
                                properties.Add(property, 5);
                                break;
                            }
                            else if (descattr.Description.Contains("12V"))
                            {
                                properties.Add(property, 12);
                                break;
                            }
                        }
                    }
                }
            }

            b.Id = Guid.Parse(uuid);
            b.SimuTestSysld = 2;
            b.DevTypeld = 1;
            b.Compld = equipUuids[rand.Next(equipUuids.Count - 1)];
            b.MicroWare = 1;
            b.Channel = 1;
            b.ModelValid = 1;
            b.ArrayEndPolarizationMode = 1;
            b.ArrayEndPowerMode = 1;
            b.ArrayChannelMultiplexing = 1;
            b.ChannelPolarizationMode1 = 1;
            b.ChannelPolarizationMode2 = 1;
            b.ChannelPowerMode = 1;
            b.Reserved = 1;
            b.StateType = 1;
            b.SelfTest = 1;
            b.SupplyVoltageState = 1;
            b.PhysicalQuantityCount = 462;
            b.RunTime = (uint?)new DateTimeOffset(now.AddSeconds(a)).ToUnixTimeSeconds();
            b.CreationTime = now.AddSeconds(a);
            foreach (var dic in properties)
            {
                dic.Key.SetValue(b, dic.Value);
            }

            list.Add(new Receive()
            {
                SimuTestSysld = b.SimuTestSysld,
                DevTypeld = b.DevTypeld,
                Compld = b.Compld,
                CreateTime = new DateTime(2023, 11, 1)
                    .AddDays(new Random().Next((new DateTime(2028, 12, 31) - new DateTime(2023, 11, 1)).Days + 1))
                    .AddHours(new Random().Next(0, 24))
                    .AddMinutes(new Random().Next(0, 60))
                    .AddSeconds(new Random().Next(0, 60)),
                Content = b,
            });
            a++;
        }

        return [.. list];
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
    //         entity.DeleteTime = DateTime.Now.ToLocalTime();
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
            _logger.LogTrace($"sql excuting: {UtilMethods.GetSqlString(DbType.SqlServer, sql, pars)}");
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
            _logger.LogTrace($"excuted take {DbContext.Ado.SqlExecutionTime.TotalMilliseconds}ms");
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
        string fileName = DateTime.Now.ToLocalTime().ToString($"yyyyMMdd_HHmmss") +
                          $"_{DbContext.Ado.Connection.Database}";
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
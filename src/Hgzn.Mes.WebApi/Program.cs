using Autofac;
using Autofac.Extensions.DependencyInjection;

using Hgzn.Mes.Application.Main.Auth;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.System.Location;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Utilities;
using Hgzn.Mes.Domain.Utilities;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Hgzn.Mes.Infrastructure.Hub;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Utilities.CurrentUser;
using Hgzn.Mes.Infrastructure.Utilities.Filter;
using Hgzn.Mes.WebApi;
using Hgzn.Mes.WebApi.Utilities;
using Hgzn.Mes.WebApi.Worker;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Serilog;

using SqlSugar;

using StackExchange.Redis;

using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using Hangfire;
using Hangfire.SQLite;
using Hgzn.Mes.Infrastructure.Utilities;
using Hgzn.Mes.Application.Main.Jobs.RecurringTask;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hgzn.Mes.WebApi.Utilities.Json;

var builder = WebApplication.CreateBuilder(args);

#region util Initialize

RequireScopeUtil.Initialize();
SettingUtil.Initialize(builder.Configuration);
CryptoUtil.Initialize(SettingUtil.Jwt.KeyFolder);
#endregion util Initialize

builder.Services.AddHttpContextAccessor();
// Change container to autoFac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(config =>
    config.RegisterAssemblyModules(Assembly.GetExecutingAssembly(), Assembly.Load("Hgzn.Mes." + nameof(Hgzn.Mes.Application) + ".Main")));

// Add services to the container.
builder.Host.UseSerilog((context, logger) =>
{
    logger.ReadFrom.Configuration(context.Configuration);
    logger.Enrich.FromLogContext();
});

builder.Services.AddLogging();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers().AddJsonOptions(config =>
{
    config.JsonSerializerOptions.DefaultIgnoreCondition = Options.CustomJsonSerializerOptions.DefaultIgnoreCondition;
    config.JsonSerializerOptions.PropertyNameCaseInsensitive = Options.CustomJsonSerializerOptions.PropertyNameCaseInsensitive;
    config.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    config.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
    config.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
});
builder.Services.AddControllers(options =>
{
    options.Filters.Add<OperLogFilterAttribute>();
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = CryptoUtil.PublicECDsaSecurityKey, // 公钥
        ValidAlgorithms = [SecurityAlgorithms.EcdsaSha256], // 必须与生成时算法一致
        ValidateIssuer = true,
        ValidIssuer = SettingUtil.Jwt.Issuer,
        ValidateAudience = true,
        ValidAudience = SettingUtil.Jwt.Audience,
        RequireExpirationTime = true,
        ValidateLifetime = true,

        SaveSigninToken = true, //记录原始令牌
        LogValidationExceptions = true //开启详细日志
    };
    // 监听 Token 验证成功事件
    option.Events = new JwtBearerEvents()
    {
        OnMessageReceived = async context =>
        {
            var accessToken = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var path = context.HttpContext.Request.Path;

            // 处理 WebSocket 的 Token 传递（从 QueryString 获取）
            if (path.StartsWithSegments("/hub") && string.IsNullOrEmpty(accessToken))
            {
                accessToken = context.Request.Query["access_token"];
                context.Token = accessToken;
            }
            if (accessToken != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validatedToken = tokenHandler.ReadJwtToken(accessToken);
                var userNameClaim = validatedToken?.Claims.FirstOrDefault(c => c.Type == ClaimType.Name)?.Value;
                var userRoleClaim = validatedToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                // 输出日志，调试用
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation($"用户: {userNameClaim}, 角色: {userRoleClaim}");
            }
            // var accessToken = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            // //WebSocket不支持自定义报文头，所以把JWT通过url中的QueryString传递
            // var path = context.HttpContext.Request.Path;
            // Console.WriteLine("accessToken:" + accessToken);
            // //如果是MyHub的请求，就在服务器端的OnMessageReceived中把QueryString中的JWT读出来赋值给context.Token
            // if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/hub")))
            // {
            //     context.Token = accessToken;
            // }
            //
            // // 获取 JWT Token 解析器
            // var tokenHandler = new JwtSecurityTokenHandler();
            //
            // // 解析 token 并验证
            // var key = new ECDsaSecurityKey(CryptoUtil.PublicECDsa); // 使用与生成 token 时相同的公钥
            // var validationParameters = option.TokenValidationParameters;
            //
            // try
            // {
            //     if (accessToken != null)
            //     {
            //         // 验证 token 并解析，获取 ClaimsPrincipal
            //         var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out var validatedToken);
            //
            //         // 从 ClaimsPrincipal 中提取 user.Username
            //         var userNameClaim = principal.FindFirst(ClaimType.UserId);
            //         // 如果用户名存在，返回用户名
            //         if (userNameClaim != null)
            //         {
            //             var userid = Guid.Parse(userNameClaim.Value);
            //             var myService = context.HttpContext.RequestServices.GetRequiredService<IConnectionMultiplexer>();
            //
            //             var database = myService.GetDatabase();
            //             var userKey = string.Format(CacheKeyFormatter.Token, userid);
            //             var raw = (await database.StringGetAsync(userKey));
            //             // var udService = builder.Services.BuildServiceProvider().GetService<UserDomainService>();
            //            // var tokenData = await myService.VerifyTokenAsync(userid, accessToken);
            //             if (!raw.HasValue)
            //             {
            //                 context.Fail("Token is no longer valid or has expired.");
            //                 return; // 直接返回，不再继续处理
            //             }
            //         }
            //     }
            // }
            // catch (SecurityTokenException ex)
            // {
            //     // Token 验证失败
            //     context.Fail($"Token validation failed: {ex.Message}");
            //     return;
            // }

            await Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddLocalization();

builder.Services.AddAuthorization(options =>
    options.AddPolicyExt(RequireScopeUtil.Scopes)
);

//注册SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

//注册hangfire
builder.Services.AddHangfire(configuration =>
{
    configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSQLiteStorage("Data Source=hangfire.db;")
        .UseSerilogLogProvider();
});
builder.Services.AddHangfireServer(options =>
{
    options.Queues = ["default"];
    options.WorkerCount = 1;
});
// 在 builder.Services.AddHangfireServer() 之后添加：
builder.Services.AddScoped<HangfireHelper>();
builder.Services.AddScoped<DeleteRFIDStaleDataEveryDayJob>();
builder.Services.AddScoped<SaveEquipRuntimeToDbJob>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Description = SettingUtil.OpenApi.Description,
        Title = SettingUtil.OpenApi.Title,
        Contact = new OpenApiContact
        {
            Name = SettingUtil.OpenApi.Name,
            Email = SettingUtil.OpenApi.Email,
            Url = new Uri(SettingUtil.OpenApi.Url)
        }
    });
    option.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "",
        Name = "Authentication",
        Scheme = JwtBearerDefaults.AuthenticationScheme,
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        },
    });
});

// Add dbContext pool
// builder.Services.AddDbContextPool<ApiDbContext>(options =>
// {
//     options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")).EnableDetailedErrors();
//     //options.UseMySQL(builder.Configuration.GetConnectionString("MySql")!).EnableDetailedErrors();
//     //options.UseOpenGauss(builder.Configuration.GetConnectionString("Postgres")!).EnableDetailedErrors();
//     options.UseSnakeCaseNamingConvention();
// });



// Add mapper profiles
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<BaseProfile>();
    config.AddMaps(Assembly.Load("Hgzn.Mes." + nameof(Hgzn.Mes.Application) + ".Main"));
});

// Add mediatR
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssemblies(Assembly.Load("Hgzn.Mes." + nameof(Hgzn.Mes.Application) + ".Main")));

builder.Services.AddHttpClient(); // 注册 IHttpClientFactory

// 注册后台服务
// 周期导入试验计划
// builder.Services.AddHostedService<TestDataImport>();
// 自动连接采集适配器
#if DEBUG
// builder.Services.AddHostedService<ConnCollectWorker>();
#else
// builder.Services.AddHostedService<ConnCollectWorker>();
#endif

var app = builder.Build();

// global Logger
var loggerAdapter = app.Services.GetService<ILogger<LoggerAdapter>>()!;
LoggerAdapter.Initialize(loggerAdapter);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//ORM源码：
if (StaticConfig.AppContext_ConvertInfinityDateTime == false)
{
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
}
app.MapControllers();
app.MapHub<OnlineHub>("/hub/online");
// app.Services.GetService<InitialDatabase>()?.Initialize();
var sqlSugarContext = app.Services.GetService<SqlSugarContext>();
sqlSugarContext?.InitDatabase();
if (app.Environment.IsDevelopment())
{
    // await sqlSugarContext!.GetSeedsCodeFromDatabaseAsync();
}
app.Services.GetService<IMqttExplorer>()?.StartAsync();
app.UseExceptionHandler(builder =>
    builder.Run(async context =>
        await ExceptionLocalizerExtension.LocalizeException(context, app.Services.GetService<IStringLocalizer<Exception>>()!)));
//启动hangfire展示界面
app.UseHangfireDashboard();

// 开启循环任务前,请务必先结束其他循环任务 - AG
HangfireHelper hangfireHelper = app.Services.GetService<HangfireHelper>()!;
List<RecurringJobInfo> jobs = hangfireHelper.GetAllRecurringJobs();
foreach (var job in jobs)
{
    hangfireHelper.RemoveRecurringJob(job.Id);
}
// 每天凌晨0点30分执行设备运行时长数据持久化任务（先保存数据）
hangfireHelper.AddDailyJob<SaveEquipRuntimeToDbJob>("SaveEquipRuntimeToDbJob", x => x.ExecuteAsync(), hour: 0, minute: 30);

// 每天凌晨1点执行数据清理任务（在数据持久化后再清理）
hangfireHelper.AddDailyJob<DeleteRFIDStaleDataEveryDayJob>("DeleteRFIDStaleDataEveryDayJob", x => x.ExecuteAsync(), hour: 1, minute: 0);

app.Run();
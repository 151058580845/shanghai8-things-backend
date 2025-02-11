using System.IdentityModel.Tokens.Jwt;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hgzn.Mes.Application.Main.Auth;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Utilities;
using Hgzn.Mes.Domain.Utilities;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text.Json;
using Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Hgzn.Mes.Infrastructure.Hub;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Utilities;
using Hgzn.Mes.Infrastructure.Utilities.Filter;
using IPTools.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using SqlSugar;

var builder = WebApplication.CreateBuilder(args);

#region util Initialize

RequireScopeUtil.Initialize();
SettingUtil.Initialize(builder.Configuration);
CryptoUtil.Initialize(SettingUtil.Jwt.KeyFolder);
#endregion util Initialize

// Change container to autoFac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(config =>
    config.RegisterAssemblyModules(Assembly.GetExecutingAssembly(), Assembly.Load("Hgzn.Mes." + nameof(Hgzn.Mes.Application)+".Main")));

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
});
builder.Services.AddControllers(options =>
{
    options.Filters.Add<OperLogFilterAttribute>();
});
builder.Services.AddHttpContextAccessor();
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
        IssuerSigningKey = new ECDsaSecurityKey(CryptoUtil.PublicECDsa),    // Use ECDsa
        ValidAlgorithms = [SecurityAlgorithms.EcdsaSha256],
        ValidateIssuer = true,
        ValidIssuer = SettingUtil.Jwt.Issuer,
        ValidateAudience = true,
        ValidAudience = SettingUtil.Jwt.Audience,
        RequireExpirationTime = true,
        ValidateLifetime = true,
    };
// 监听 Token 验证成功事件
    option.Events = new JwtBearerEvents()
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            //WebSocket不支持自定义报文头，所以把JWT通过url中的QueryString传递
            var path = context.HttpContext.Request.Path;
            //如果是MyHub的请求，就在服务器端的OnMessageReceived中把QueryString中的JWT读出来赋值给context.Token
            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/hub")))
            {
                context.Token = accessToken;
            }
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
// builder.Services.AddHangfireServer();

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
    config.RegisterServicesFromAssemblies(Assembly.Load("Hgzn.Mes." + nameof(Hgzn.Mes.Application)+".Main")));

var app = builder.Build();

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
app.Services.GetService<SqlSugarContext>()?.InitDatabase(app.Services.GetService<IConfiguration>().GetSection(nameof(DbConnOptions))
    .Get<DbConnOptions>());
app.Services.GetService<IMqttExplorer>()?.StartAsync();
app.UseExceptionHandler(builder =>
    builder.Run(async context =>
        await ExceptionLocalizerExtension.LocalizeException(context, app.Services.GetService<IStringLocalizer<Exception>>()!)));
//启动hangfire展示界面
// app.UseHangfireDashboard();
app.Run();
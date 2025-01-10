using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hgzn.Mes.Application.Auth;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Utilities;
using Hgzn.Mes.Domain.Utilities;
using Hgzn.Mes.Infrastructure.DbContexts;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using Hgzn.Mes.Infrastructure.SqlSugarContext;

var builder = WebApplication.CreateBuilder(args);

#region util Initialize

RequireScopeUtil.Initialize();
SettingUtil.Initialize(builder.Configuration);
CryptoUtil.Initialize(SettingUtil.Jwt.KeyFolder);
builder.Services.Configure<DbConnOptions>(builder.Configuration.GetSection(nameof(DbConnOptions)));
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
        ValidateLifetime = true
    };
});

builder.Services.AddLocalization();

builder.Services.AddAuthorization(options =>
    options.AddPolicyExt(RequireScopeUtil.Scopes)
);

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
builder.Services.AddSingleton<SqlSugarContext>();

// Add mapper profiles
builder.Services.AddAutoMapper(config => config.AddMaps(Assembly.Load("Hgzn.Mes." + nameof(Hgzn.Mes.Application)+".Main")));

// Add mediatR
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssemblies(Assembly.Load("Hgzn.Mes." + nameof(Hgzn.Mes.Application)+".Main")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// app.Services.GetService<InitialDatabase>()?.Initialize();
app.Services.GetService<SqlSugarContext>()?.InitTables();

app.UseExceptionHandler(builder =>
    builder.Run(async context =>
        await ExceptionLocalizerExtension.LocalizeException(context, app.Services.GetService<IStringLocalizer<Exception>>()!)));

app.Run();
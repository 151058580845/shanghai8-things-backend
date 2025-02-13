// See https://aka.ms/new-console-template for more information
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Iot.EquipManager;
using Hgzn.Mes.Iot.Mqtt;
using Hgzn.Mes.Iot.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using SqlSugar;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

var logger = (new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration))
    .CreateLogger();
builder.Configuration.AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.json"));
//builder.Logging.AddSerilog(logger);
builder.Services.AddSingleton<MqttMessageHandler>();
builder.Services.AddSingleton<ConnManager>();
builder.Services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(_ =>
        ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));
builder.Services.AddScoped<ISqlSugarClient, SqlSugarClient>(context =>
{
    var setting = builder.Configuration.GetSection(nameof(DbConnOptions))
        .Get<DbConnOptions>() ?? throw new Exception("sqlsugar config not found!");
    return new SqlSugarClient(SqlSugarContext.Build(setting));
});
builder.Services.AddSingleton<MqttMessageHandler>();
builder.Services.AddSingleton<IMqttExplorer, IotMqttExplorer>();
builder.Services.AddSingleton<ConnManager>();

builder.Services.AddSerilog(logger);

builder.Services.AddHostedService<MqttWorker>();

// Add dbContext pool
//builder.Services.AddPooledDbContextFactory<PatientDbContext>(options =>
//{
//    options.UseNpgsql(new NpgsqlDataSourceBuilder(
//        builder.Configuration.GetConnectionString("HisPg")).Build()
//    ).EnableDetailedErrors();
//    options.UseSnakeCaseNamingConvention();
//});

var host = builder.Build();

var loggerAdapter = host.Services.GetService<ILogger<LoggerAdapter>>()!;
LoggerAdapter.Initialize(loggerAdapter);

host.Run();
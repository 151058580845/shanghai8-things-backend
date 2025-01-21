// See https://aka.ms/new-console-template for more information
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Iot.Mqtt;
using Hgzn.Mes.Iot.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using StackExchange.Redis;

Console.WriteLine("Hello, World!");
var builder = Host.CreateApplicationBuilder(args);

var logger = (new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration))
    .CreateLogger();

//builder.Logging.AddSerilog(logger);

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

builder.Services.AddSingleton<IMqttExplorer, IotMqttExplorer>();
builder.Services.AddSingleton<MqttMessageHandler>();
builder.Services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(instance =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));
var host = builder.Build();

var loggerAdapter = host.Services.GetService<ILogger<LoggerAdapter>>()!;
LoggerAdapter.Initialize(loggerAdapter);

host.Run();
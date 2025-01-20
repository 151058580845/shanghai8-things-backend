using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Hgzn.Mes.Infrastructure.Mqtt;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using Serilog;
using SqlSugar;
using TcpServerClient.mqtt;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");
//Configure Serilog for logging
 builder.Logging.AddSerilog(new LoggerConfiguration()
     .ReadFrom.Configuration(builder.Configuration)
     .Enrich.FromLogContext()
     .CreateLogger());

// Add logging service
builder.Services.AddLogging();

// Bind configuration settings
builder.Services.Configure<DbConnOptions>(builder.Configuration.GetSection(nameof(DbConnOptions)));
builder.Services.Configure<MqttSettings>(builder.Configuration.GetSection(nameof(MqttSettings)));

// Register SqlSugar and related services
builder.Services.AddSingleton<ISqlSugarClient>(provider =>
{
    var options = provider.GetRequiredService<IOptions<DbConnOptions>>().Value;
    return new SqlSugarClient(SqlSugarContext.Build(options));
});

builder.Services.AddSingleton<SqlSugarContext>(provider =>
{
    var options = provider.GetRequiredService<IOptions<DbConnOptions>>().Value;
    var logger = provider.GetRequiredService<ILogger<SqlSugarContext>>();
    var client = provider.GetRequiredService<ISqlSugarClient>();
    return new SqlSugarContext(logger, options, client);
});

// Register MQTT related services
builder.Services.AddSingleton<IManagedMqttClient>(provider =>
{
    var factory = new MqttFactory();
    return factory.CreateManagedMqttClient();
});

builder.Services.AddSingleton<IMqttExplorer>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<ApiMqttPub>>();
    var client = provider.GetRequiredService<IManagedMqttClient>();
    var options = provider.GetRequiredService<IOptions<MqttSettings>>().Value;
    return new ApiMqttPub(logger, client, options);
});

builder.Services.AddSingleton<MqttHelp>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<MqttHelp>>();
    var client = provider.GetRequiredService<IMqttExplorer>();
    return new MqttHelp(logger, client);
});

var app = builder.Build();

// Initialize services
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Initialize DB tables
    var dbContext = services.GetRequiredService<SqlSugarContext>();
    dbContext.InitTables();

    // Start MQTT services
    var mqttExplorer = services.GetRequiredService<IMqttExplorer>();
    var mqttHelp = services.GetRequiredService<MqttHelp>();
    await mqttExplorer.StartAsync();
    await mqttHelp.StartAsync();
}

// Run the application
app.Run();

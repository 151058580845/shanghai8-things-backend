using Hgzn.Mes.Application.Main.Services.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using StackExchange.Redis;

namespace Hgzn.Mes.WebApi.Worker
{
    public class ConnCollectWorker : BackgroundService
    {
        private readonly ISqlSugarClient _sqlClient;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDatabase _redis;
        
        // 检测间隔(秒), 默认5秒
        private const int DEFAULT_CHECK_INTERVAL = 5;
        
        public ConnCollectWorker(
            ISqlSugarClient sqlClient,
            IConfiguration configuration,
            IConnectionMultiplexer connectionMultiplexer,
            IServiceProvider serviceProvider)
        {
            _sqlClient = sqlClient;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _redis = connectionMultiplexer.GetDatabase();
            
            LoggerAdapter.LogInformation("AG - ConnCollectWorker构造函数已调用");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // 延迟启动等待数据库初始化和其他服务启动
                await Task.Delay(5000, stoppingToken);
                
                // 从配置读取检测间隔,如果没有配置则使用默认值5秒
                var checkInterval = _configuration.GetValue<int?>("EquipConnCheckInterval") ?? DEFAULT_CHECK_INTERVAL;
                
                LoggerAdapter.LogInformation($"AG - 设备连接守护进程启动,检测间隔: {checkInterval}秒");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await CheckAndReconnectEquipmentsAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        LoggerAdapter.LogError($"AG - 设备连接检测发生异常: {ex.Message}\n{ex.StackTrace}");
                    }
                    
                    // 等待下一次检测
                    await Task.Delay(checkInterval * 1000, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                LoggerAdapter.LogInformation("AG - 设备连接守护进程已停止");
            }
            catch (Exception e)
            {
                LoggerAdapter.LogError($"AG - 设备连接守护进程发生致命错误: {e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// 检查并重连需要保持连接的设备
        /// </summary>
        private async Task CheckAndReconnectEquipmentsAsync(CancellationToken stoppingToken)
        {
            // 创建作用域来获取IEquipConnService
            using var scope = _serviceProvider.CreateScope();
            var equipConnectService = scope.ServiceProvider.GetRequiredService<IEquipConnService>();
            
            // 查询所有应该保持连接的设备连接配置
            // 1. State为true(启用状态)
            // 2. ConnectStr不为空(有连接配置)
            // 3. 协议类型为TcpServer(8)或UdpServer(9) - 采集适配器使用这两种协议
            // 4. 未被软删除
            EquipConnect[] connections = await _sqlClient.Queryable<EquipConnect>()
                .Where(ec => ec.State && ec.ConnectStr != null)
                .Where(ec => ec.ProtocolEnum == ConnType.TcpServer || ec.ProtocolEnum == ConnType.UdpServer)
                .Where(ec => !ec.SoftDeleted)
                .ToArrayAsync();

            if (!connections.Any())
            {
                LoggerAdapter.LogInformation("AG - 没有需要保持连接的设备");
                return;
            }

            LoggerAdapter.LogInformation($"AG - 检测到 {connections.Length} 个需要保持连接的设备");
            
            int reconnectCount = 0;
            
            foreach (EquipConnect connection in connections)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;
                    
                try
                {
                    // 从Redis读取连接状态
                    // EquipState格式: "equip:{equipId}:{connectId}:state"
                    // 值: 3 = Run(运行中), 1 = On(已连接), 0 = Off(断开), 2 = Stop(停止)
                    string redisKey = string.Format(CacheKeyFormatter.EquipState, connection.EquipId, connection.Id);
                    var redisValue = await _redis.StringGetAsync(redisKey);
                    
                    bool isConnected = false;
                    int stateValue = -1;
                    
                    if (redisValue.HasValue && redisValue.TryParse(out stateValue))
                    {
                        // 状态为1(On)或3(Run)表示连接正常
                        isConnected = (stateValue == (int)ConnStateType.On || stateValue == (int)ConnStateType.Run);
                    }
                    
                    if (!isConnected)
                    {
                        LoggerAdapter.LogDebug($"AG - 设备连接已断开,准备重连: {connection.Name} (Id: {connection.Id}, Protocol: {connection.ProtocolEnum}, RedisState: {stateValue})");
                        
                        // 调用服务启动连接
                        await equipConnectService.PutStartConnect(connection.Id);
                        reconnectCount++;
                        
                        // 每次重连后延迟500ms,避免同时发起太多连接请求
                        await Task.Delay(500, stoppingToken);
                    }
                    else
                    {
                        LoggerAdapter.LogInformation($"AG - 设备连接正常: {connection.Name} (Id: {connection.Id}, State: {stateValue})");
                    }
                }
                catch (Exception ex)
                {
                    LoggerAdapter.LogError($"AG - 检测设备连接状态失败: {connection.Name} (Id: {connection.Id}), 错误: {ex.Message}");
                }
            }
            
            if (reconnectCount > 0)
            {
                LoggerAdapter.LogInformation($"AG - 本次检测完成,尝试重连 {reconnectCount} 个设备");
            }
            else
            {
                LoggerAdapter.LogInformation($"AG - 本次检测完成,所有设备连接正常");
            }
        }
    }
}


using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message.Base;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Hgzn.Mes.Iot.EquipManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.X509;
using SqlSugar;

namespace Hgzn.Mes.Iot.Worker
{
    public class ConnWorker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISqlSugarClient _sqlClient;
        private readonly ConnManager _manager;
        private readonly IConfiguration _configuration;

        public ConnWorker(
            ISqlSugarClient sqlClient ,
            ILogger<ConnWorker> logger,
            ConnManager manager,
            IConfiguration configuration)
        {
            _logger = logger;
            _sqlClient = sqlClient;
            _manager = manager;
            _configuration = configuration;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //延迟启动等待数据库初始化
            await Task.Delay(1000, stoppingToken);
            var interval = _configuration.GetValue<int>("ReConnInterval");
            while (!stoppingToken.IsCancellationRequested)
            {
                var connections = await _sqlClient.Queryable<EquipConnect>()
                    .Includes(ec => ec.EquipLedger, el => el!.EquipType)
                    .Where(ec => ec.State && ec.ConnectStr != null)
                    .Where(ec => ec.EquipLedger!.EquipType!.Id == EquipType.RfidReaderType.Id)
                    .ToArrayAsync();

                //关闭多余的连接
                var targetIds = connections.Select(c => c.Id);
                try
                {
                    var closeTasks = _manager.Connections
                    .Where(c => !targetIds.Contains(c.Key))
                    .Select(c => _manager.GetEquip(c.Key)!.CloseConnectionAsync());
                    await Task.WhenAll(closeTasks);
                }
                catch
                {
                    _logger.LogWarning("close excessive connections failed!");
                }

                foreach (var connection in connections)
                {
                    try
                    {
                        var connInfo = new ConnInfo
                        {
                            ConnType = connection.ProtocolEnum,
                            ConnString = connection.ConnectStr,
                            Type = CmdType.Conn,
                            StateType = ConnStateType.On,
                        };
                        var equip = _manager.GetEquip(connection.Id) ??
                            _manager.AddEquip(connection.Id, EquipConnType.RfidReader,
                            connection.ConnectStr!, connInfo);
                        if (!equip.ConnState)
                        {
                            await equip!.CloseConnectionAsync();
                            await equip!.ConnectAsync(connInfo);
                            await equip!.StartAsync(connection.Id);
                            _logger.LogInformation($"start connection[{connection!.Name}](connId:{connection.Id}) succeed!");
                        }
                        else
                        {
                            _logger.LogInformation($"connection[{connection!.Name}](connId:{connection.Id}) is exist");
                        }

                        await Task.Delay(500, stoppingToken);
                    }
                    catch
                    {
                        _logger.LogError($"start connection[{connection!.Name}](connId:{connection.Id}) failed!");
                    }

                }

                await Task.Delay(1000 * interval, stoppingToken);
            }
        }
    }
}

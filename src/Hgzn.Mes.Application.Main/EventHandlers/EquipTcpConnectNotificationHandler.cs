using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Events;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using MediatR;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.EventHandlers
{
    public class EquipTcpConnectNotificationHandler : INotificationHandler<EquipTcpConnectNotification>
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public EquipTcpConnectNotificationHandler(ISqlSugarClient sqlSugarClient, IEquipLedgerService equipLedger)
        {
            this._sqlSugarClient = sqlSugarClient;
        }

        public async Task Handle(EquipTcpConnectNotification notification, CancellationToken cancellationToken)
        {
            if (notification?.IpAddress == null) return;
            EquipLedger equipLedger = await _sqlSugarClient.Queryable<EquipLedger>().FirstAsync(x => x.IpAddress == notification.IpAddress.ToString());
            await _sqlSugarClient.Updateable(equipLedger).ExecuteCommandAsync();
        }
    }
}

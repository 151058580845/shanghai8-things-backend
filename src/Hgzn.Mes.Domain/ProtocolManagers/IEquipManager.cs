using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.ProtocolManagers
{
    public interface IEquipManager
    {
        Task UpdateConnectionParameter(string parameter);
        Task ConnectionAsync();
        Task DisConnectionAsync();
        Task StartAsync();
        Task StartAsync(Guid dataPointId);
        Task StopAsync();
        Task StopAsync(Guid dataPointId);
        Task<bool> TestConnectionAsync();
        Task SendDataAsync(byte[] buffer);
        // Task UpdateEquipConnectForward(List<Guid> targetIds);

        Task<bool> IsConnectedAsync();

        Task<bool> AddDataPointAsync(EquipDataPoint dataPoint);
        Task<bool> RemoveDataPointAsync(EquipDataPoint dataPoint);
    }
}

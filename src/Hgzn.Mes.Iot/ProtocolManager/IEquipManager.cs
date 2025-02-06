namespace Hgzn.Mes.Iot.ProtocolManager;

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
    Task UpdateEquipConnectForward(List<Guid> targetIds);
    Task<bool> IsConnectedAsync();
    
    // Task<bool> AddDataPointAsync(EquipDataPointAggregateRoot dataPoint);
    // Task<bool> RemoveDataPointAsync(EquipDataPointAggregateRoot dataPoint);
}
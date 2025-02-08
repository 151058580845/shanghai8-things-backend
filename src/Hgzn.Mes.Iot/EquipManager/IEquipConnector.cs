using Hgzn.Mes.Domain.ValueObjects.Message.Commads;
namespace Hgzn.Mes.Iot.EquipManager;

public interface IEquipConnector
{
    Task<bool> ConnectAsync(ConnInfoBase connInfo);
    Task CloseConnectionAsync();
    Task StartAsync();
    //Task StartAsync(Guid dataPointId);
    Task StopAsync();
    //Task StopAsync(Guid dataPointId);
    Task SendDataAsync(byte[] buffer);
    //Task UpdateEquipConnectForward(List<Guid> targetIds);
}
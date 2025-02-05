using Hgzn.Mes.Iot.ProtocolManager.Protocols;

namespace Hgzn.Mes.Iot.ProtocolManager.RfidReader;

public class RfidReaderManages : BaseProtocolManager, IEquipManager
{
    
    /// <summary>
    /// 连接参数
    /// </summary>
    private ProtocolRfidClient? _parameters;
    // private GClient? _client;
    public override Task UpdateConnectionParameter(string parameter)
    {
        throw new NotImplementedException();
    }

    protected override Task BaseConnectionAsync()
    {
        throw new NotImplementedException();
    }

    protected override Task BaseDisConnectionAsync()
    {
        throw new NotImplementedException();
    }

    public override Task StartAsync(Guid dataPointId)
    {
        throw new NotImplementedException();
    }

    public override Task UpdateEquipConnectForward(List<Guid> targetIds)
    {
        throw new NotImplementedException();
    }

    public override Task<bool> IsConnectedAsync()
    {
        throw new NotImplementedException();
    }
}
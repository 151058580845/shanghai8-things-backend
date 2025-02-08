using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using System.Collections.Concurrent;

namespace Hgzn.Mes.Iot.EquipManager
{
    public class ConnManager
    {
        private readonly IMqttExplorer _mqtt;

        public ConnManager(IMqttExplorer mqtt)
        {
            _mqtt = mqtt;
        }
        public static ConcurrentDictionary<Guid, IEquipConnector> Connections { get; private set; } = new();

        public IEquipConnector AddEquip(Guid id, Protocol type)
        {
            if (Connections.TryGetValue(id, out var value))
            {
                return value;
            }
            IEquipConnector? connector = null;
            switch (type)
            {
                case Protocol.ModbusTcp:
                    connector = new RfidReaderConnector(_mqtt);
                    if (Connections.TryAdd(id, connector))
                        throw new Exception("equip exist");
                    break;
            }
            return connector ?? throw new ArgumentNullException();
        }

        public IEquipConnector? GetEquip(Guid id)
        {
            if(Connections.TryGetValue(id, out var connector))
            {
                return connector;
            }
            return null;
        }
    }
}

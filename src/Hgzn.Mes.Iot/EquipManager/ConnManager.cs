using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using System.Collections.Concurrent;

namespace Hgzn.Mes.Iot.EquipManager
{
    public class ConnManager
    {
        private IMqttExplorer _mqtt = null!;

        public static ConcurrentDictionary<Guid, IEquipConnector> Connections { get; private set; } = new();

        public void Initialize(IMqttExplorer mqttExplorer)
        {
            _mqtt = mqttExplorer;
        }

        public IEquipConnector AddEquip(Guid id, string equipType)
        {
            if (Connections.TryGetValue(id, out var value))
            {
                return value;
            }
            IEquipConnector? connector = null;
            switch (equipType)
            {
                case "rfid-reader":
                    connector = new RfidReaderConnector(_mqtt, id.ToString(), equipType);
                    if (!Connections.TryAdd(id, connector))
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

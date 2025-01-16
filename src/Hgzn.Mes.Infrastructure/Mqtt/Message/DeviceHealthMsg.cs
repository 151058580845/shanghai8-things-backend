using System.Text.Json.Nodes;
using Hgzn.Mes.Infrastructure.Mqtt.Message.Base;

namespace Hgzn.Mes.Infrastructure.Mqtt.Message
{
    public class DeviceHealthMsg : IJsonMessage
    {
        public DeviceHealthMsg(string raw)
        {
            Node = JsonNode.Parse(raw) ??
                throw new ArgumentException("can't parse json node");
        }

        public JsonNode? Node { get; set; }
    }
}
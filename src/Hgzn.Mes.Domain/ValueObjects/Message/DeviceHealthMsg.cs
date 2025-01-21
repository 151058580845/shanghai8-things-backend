using System.Text.Json.Nodes;
using Hgzn.Mes.Domain.ValueObjects.Message.Base;

namespace Hgzn.Mes.Domain.ValueObjects.Message
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
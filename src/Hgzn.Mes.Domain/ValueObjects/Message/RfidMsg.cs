
using Hgzn.Mes.Domain.ValueObjects.Message.Base;
using System.Text.Json.Nodes;

namespace Hgzn.Mes.Domain.ValueObjects.Message
{
    public class RfidMsg : IJsonMessage
    {
        public string? Tid { get; set; }
        public string? Userdata { get; set; }
        public JsonNode? Node { get; set; }
    }
}

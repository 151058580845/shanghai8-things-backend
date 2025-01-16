using System.Text.Json.Nodes;

namespace Hgzn.Mes.Infrastructure.Mqtt.Message.Base
{
    public interface IJsonMessage : IIotMessage
    {
        JsonNode? Node { get; set; }
    }
}
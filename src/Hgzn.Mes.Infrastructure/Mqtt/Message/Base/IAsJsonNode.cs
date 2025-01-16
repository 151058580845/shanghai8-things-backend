using System.Text.Json.Nodes;

namespace Hgzn.Mes.Infrastructure.Mqtt.Message.Base
{
    public interface IAsJsonNode
    {
        JsonNode? AsJson();

        JsonNode? Node { get; set; }
    }
}
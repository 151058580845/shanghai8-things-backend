using System.Text.Json.Nodes;

namespace Hgzn.Mes.Domain.ValueObjects.Message.Base
{
    public interface IAsJsonNode
    {
        JsonNode? AsJson();

        JsonNode? Node { get; set; }
    }
}
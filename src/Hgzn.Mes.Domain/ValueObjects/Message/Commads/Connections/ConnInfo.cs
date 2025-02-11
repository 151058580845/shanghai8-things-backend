using Hgzn.Mes.Domain.ValueObjects.Message.Base;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections
{
    public class ConnInfo<TContent> : ConnInfoBase
        where TContent : IConnInfo
    {
        public TContent? Content { get => ConnString is null ? default : JsonSerializer.Deserialize<TContent>(ConnString); }
        [JsonIgnore]
        public string? ConnString { get; set; }
    }
}

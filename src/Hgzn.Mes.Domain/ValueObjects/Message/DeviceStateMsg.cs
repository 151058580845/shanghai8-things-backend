using System.Text.Json.Nodes;
using Hgzn.Mes.Domain.ValueObjects.Message.Base;

namespace Hgzn.Mes.Domain.ValueObjects.Message
{
    /// <summary>
    /// 设备状态消息内容
    /// </summary>
    public class DeviceStateMsg : IJsonMessage
    {
        public DeviceStateMsg(string raw)
        {
            Node = JsonNode.Parse(raw) ??
                throw new ArgumentException("can't parse json node");
        }

        public JsonNode? Node { get; set; }
    }
}
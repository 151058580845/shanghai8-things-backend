using Hgzn.Mes.Domain.ValueObjects.Message.Base;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;

public class SocketConnInfo : IConnInfo
{
    /// <summary>
    /// IP地址
    /// </summary>
    [Description("Ip地址")]
    public string Address { get; set; } = null!;

    /// <summary>
    /// 端口号（默认44818）
    /// </summary>
    [Description("Port")]
    [JsonConverter(typeof(PortConverter))]
    public int Port { get; set; } = 44818;

    /// <summary>
    /// 会话超时时间（Session Timeout）
    /// </summary>
    [Description("SessionTimeout")]
    public int SessionTimeout { get; set; }

    /// <summary>
    /// 注册会话（Register Session）
    /// </summary>
    [Description("RegisterSession")]
    public bool RegisterSession { get; set; } = true;

}


public class PortConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            // 如果是字符串，尝试解析为整数
            if (int.TryParse(reader.GetString(), out int port))
            {
                return port;
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            // 如果是数字，直接返回
            return reader.GetInt32();
        }

        throw new JsonException("Invalid value for port."); // 抛出异常
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value); // 直接写入值
    }
}
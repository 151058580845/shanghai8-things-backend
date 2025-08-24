using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class LocalDateTimeConverter : JsonConverter<DateTime>
{
    private const string Format = "yyyy-MM-dd HH:mm:ss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 反序列化时按本地时间解析
        return DateTime.Parse(reader.GetString() ?? string.Empty);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // 序列化时只输出本地时间字符串
        writer.WriteStringValue(value.ToLocalTime().ToString(Format));
    }
}
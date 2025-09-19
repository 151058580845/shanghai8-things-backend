using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hgzn.Mes.WebApi.Utilities.Json
{
    public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new FormatException("Invalid DateOnly string");
            }

            // Try exact date-only first (e.g., 2025-09-17)
            if (DateOnly.TryParseExact(str, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dOnly))
            {
                return dOnly;
            }

            // Try ISO 8601 date-time strings (e.g., 2025-08-31T16:00:00.000Z)
            if (DateTimeOffset.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dto))
            {
                return DateOnly.FromDateTime(dto.UtcDateTime);
            }

            if (DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt))
            {
                return DateOnly.FromDateTime(dt);
            }

            throw new FormatException($"String '{str}' was not recognized as a valid DateOnly.");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
    }
}


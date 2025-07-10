using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Dtos.Equip
{
    public class TestAnalyseReadDto : ReadDto
    {
        [Description("试验数据")]
        public ApiResponse? Response { get; set; }
    }

    public class TestAnalyseQueryDto : PaginatedQueryDto
    {
        [Description("仿真系统资源名称")]
        public string? SysName { get; set; }
        [Description("型号项目名称")]
        public string? ProjectName { get; set; }
        [Description("试验名称")]
        public string? TestName { get; set; }
        [Description("表格类型")]
        public string? FormTypes { get; set; }
        [Description("历史时间")]
        [JsonConverter(typeof(DateTimeISO8601NoT))]
        public DateTime History { get; set; }
        [Description("物理量表格名称")]
        public List<string>? PhysicalQuantityTableName { get; set; }

    }

    /// <summary>
    /// 识别日期格式为：yyyy-MM-dd HH:mm:ss的json字符串
    /// </summary>
    public class DateTimeISO8601NoT : JsonConverter<DateTime>
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (string.IsNullOrEmpty(reader.GetString()))
                return DateTime.Now.ToLocalTime();
            if (reader.TokenType == JsonTokenType.String && DateTime.TryParseExact(reader.GetString(), DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime;
            }
            throw new JsonException($"Unable to convert '{reader.GetString()}' to {typeof(DateTime)}.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateTimeFormat));
        }
    }
}

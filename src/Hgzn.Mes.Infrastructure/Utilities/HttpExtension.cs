using Microsoft.AspNetCore.Http;

namespace Hgzn.Mes.Infrastructure.Utilities;

public static class HttpExtension
{
    public static async Task<string?> GetRequestValue(this HttpContext context, string requestMethod)
    {
        string? requestParams = string.Empty;

        if (requestMethod == HttpMethods.Get)
        {
            // 对于 GET 请求，从查询字符串中获取参数
            requestParams = context.Request.QueryString.Value;
        }
        else if (requestMethod == HttpMethods.Post)
        {
            // 对于 POST 请求，检查请求体的类型（JSON 或 表单数据）
            if (context.Request.ContentType != null && context.Request.ContentType.Contains("application/json"))
            {
                // 如果是 JSON 请求，读取请求体
                using (var reader = new StreamReader(context.Request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    requestParams = body; // 可以进一步解析 JSON
                }
            }
            else if (context.Request.ContentType != null && context.Request.ContentType.Contains("application/x-www-form-urlencoded"))
            {
                // 如果是表单数据，获取表单参数
                var formData = context.Request.Form;
                requestParams = formData.ToString(); // 可以根据需要调整格式
            }
        }
        else if (requestMethod == HttpMethods.Put || requestMethod == HttpMethods.Delete)
        {
            // 对于 PUT 和 DELETE 请求，假设请求体是 JSON 格式
            if (context.Request.ContentType != null && context.Request.ContentType.Contains("application/json"))
            {
                // 如果是 JSON 请求，读取请求体
                using (var reader = new StreamReader(context.Request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    requestParams = body; // 这里返回的是原始 JSON 字符串，可以根据需要解析成对象
                }
            }
        }

        return requestParams;
    }
}
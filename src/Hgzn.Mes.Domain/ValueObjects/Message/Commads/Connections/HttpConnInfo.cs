using Hgzn.Mes.Domain.ValueObjects.Message.Base;

namespace Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;

public class HttpConnInfo : IConnInfo
{
    /// <summary>
    /// 请求的URL地址
    /// </summary>
    public string Url { get; set; } = null!;

    /// <summary>
    /// 请求方法（GET, POST等）
    /// </summary>
    public string Method { get; set; } = null!; 

    /// <summary>
    /// 请求头参数（Headers）
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = null!;

    /// <summary>
    /// 请求超时时间（单位：毫秒）
    /// </summary>
    public int Timeout { get; set; }

    /// <summary>
    /// 是否使用SSL/TLS加密
    /// </summary>
    public bool UseSsl { get; set; }

}
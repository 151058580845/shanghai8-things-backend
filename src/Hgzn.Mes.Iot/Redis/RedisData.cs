namespace Hgzn.Mes.Iot.Redis;

/// <summary>
/// 存储采集到的数据到缓存中
/// </summary>
public class RedisData
{
    /// <summary>
    /// 编码（根据编码查找）
    /// </summary>
    public string Code { get; set; }
    /// <summary>
    /// 数据
    /// </summary>
    public object Date { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }=DateTime.Now;
}
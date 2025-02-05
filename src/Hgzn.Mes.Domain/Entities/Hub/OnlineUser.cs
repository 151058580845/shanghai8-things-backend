using Hgzn.Mes.Domain.Entities.Base;

namespace Hgzn.Mes.Domain.Entities.Hub;

public class OnlineUser:UniversalEntity
{
    /// <summary>
    /// 客户端连接Id
    /// </summary>
    public string? ConnnectionId { get; set; }
    /// <summary>
    /// 用户id
    /// </summary>
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime LoginTime { get; set; }
    public string? Ipaddr { get; set; }
    public string? LoginLocation { get; set; }

    public string? Os { get; set; }
    public string? Browser { get; set; }
}
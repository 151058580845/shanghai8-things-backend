using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos.System
{
    public class OnlineUserQueryDto : PaginatedQueryDto
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

    }
}

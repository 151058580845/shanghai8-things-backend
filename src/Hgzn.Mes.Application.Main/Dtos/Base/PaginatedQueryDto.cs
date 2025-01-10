
namespace Hgzn.Mes.Application.Main.Dtos.Base
{
    public class PaginatedQueryDto
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        
        /// <summary>
        /// 查询开始时间条件
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 查询结束时间条件
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}

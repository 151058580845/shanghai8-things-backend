using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Dtos.Base
{
    public class PaginatedTimeQueryDto : QueryTimeDto
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}

﻿namespace Hgzn.Mes.Application.Main.Dtos.Base
{
    public class PaginatedQueryDto : QueryDto
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
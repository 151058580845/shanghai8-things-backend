using Hgzn.Mes.Application.Main.Dtos.Base;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Dtos.Basic
{
    public class UnitDto
    {
    }

    public class UnitReadDto : ReadDto
    {
        [Description("单位编码")] public string Code { get; set; } 
        [Description("单位名称")] public string Name { get; set; } 
        [Description("是否是主单位")] public bool? IsMain { get; set; }  
        [Description("主单位Id")] public string? ParentId { get; set; }
        [Description("换算比例")] public decimal? Ratio { get; set; }



        [Description("备注")] public string? Remark { get; set; }
    }
    public class UnitCreateDto : CreateDto
    {
        [Description("单位编码")] public string Code { get; set; } 
        [Description("单位名称")] public string Name { get; set; } 
        [Description("是否是主单位")] public bool? IsMain { get; set; }
        [Description("主单位Id")] public Guid? ParentId { get; set; }

        public string? PId { get; set; }

        [Description("换算比例")] public decimal? Ratio { get; set; }
        [Description("备注")] public string? Remark { get; set; }
    }

    public class UnitUpdateDto : UpdateDto
    {
        [Description("单位编码")] public string Code { get; set; } 
        [Description("单位名称")] public string Name { get; set; } 
        [Description("是否是主单位")] public bool? IsMain { get; set; }  
        [Description("主单位Id")] public Guid? ParentId { get; set; }

        public string? PId { get; set; }

        [Description("换算比例")] public decimal? Ratio { get; set; }
        [Description("备注")] public string? Remark { get; set; }
    }

    public class UnitQueryDto : PaginatedQueryDto {
        [Description("单位编码")] public string Code { get; set; } 
        [Description("单位名称")] public string Name { get; set; } 
    }
}

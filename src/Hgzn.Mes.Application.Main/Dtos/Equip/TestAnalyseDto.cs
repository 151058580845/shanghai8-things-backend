using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
        [Description("物理量表格名称")]
        public List<string>? PhysicalQuantityTableName { get; set; }

    }
}

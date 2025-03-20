using Hgzn.Mes.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData
{
    [Description("实验数据解析")]
    public class TestAnalyse : UniversalEntity
    {
        [Description("仿真系统资源名称")]
        public string? SysName { get; set; }
        [Description("型号项目名称")]
        public string? ProjectName { get; set; }
        [Description("试验名称")]
        public string? TestName { get; set; }
        [Description("物理量表格名称")]
        public List<string>? PhysicalQuantityTableName { get; set; }
        [Description("试验数据")]
        public string? TestDataJson { get; set; }
    }
}

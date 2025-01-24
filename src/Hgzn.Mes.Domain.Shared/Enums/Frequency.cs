using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Shared.Enums
{
    /// <summary>
    /// 维护计划频率
    /// </summary>
    public enum Frequency
    {
        [Description("小时")]
        Hour,
        [Description("天")]
        Day,
        [Description("周")]
        Week,
        [Description("月")]
        Month,
        [Description("季度")]
        Quarter,
        [Description("年")]
        Year,
    }
}

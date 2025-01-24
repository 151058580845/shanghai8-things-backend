using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Shared.Enums
{
    public enum DataPointStatus
    {
        [Description("初始化")]
        Initialization,
        [Description("待执行")]
        Pending,
        [Description("执行中")]
        Progress,
        [Description("暂停")]
        Paused,
        [Description("恢复")]
        Resumed,
        [Description("完成")]
        Completed,
        [Description("失败")]
        Failed,
        [Description("未添加任务")]
        None,
    }
}

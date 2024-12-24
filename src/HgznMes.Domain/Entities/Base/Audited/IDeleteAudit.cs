using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HgznMes.Domain.Entities.Base.Audited
{
    /// <summary>
    /// 删除模型
    /// </summary>
    internal interface IDeleteAudit
    {
        Guid? DeleteUserId { get; set; }
        DateTime DeleteTime { get; set; }
    }
}

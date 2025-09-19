using Hgzn.Mes.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData
{
    public class AssetDataProjectItem : UniversalEntity
    {
        /// <summary>
        /// 系统ID(外键)
        /// </summary>
        public Guid SystemId { get; set; }
        public string ProjectType { get; set; } = null!;
        public decimal? Amount { get; set; }
        public DateOnly? StartDate { get; set; }
        public string? Remark { get; set; }
        public string? Responsible { get; set; }
    }
}

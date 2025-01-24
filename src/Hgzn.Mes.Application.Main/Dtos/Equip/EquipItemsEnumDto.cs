using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Dtos.Equip
{
    public class EquipItemsEnumReadDto : ReadDto
    {
        /// <summary>
        /// 设备项目类型
        /// </summary>
        public List<EnumFieldInfo> EquipMaintenanceType { get; set; }
    }
}

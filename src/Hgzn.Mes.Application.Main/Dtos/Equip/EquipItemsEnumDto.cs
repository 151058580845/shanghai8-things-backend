using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Application.Main.Dtos.Equip
{
    public class EquipItemsEnumReadDto : ReadDto
    {
        /// <summary>
        /// 设备项目类型
        /// </summary>
        public List<EnumFieldInfo>? EquipMaintenanceType { get; set; }
    }
}

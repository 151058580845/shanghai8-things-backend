using Hgzn.Mes.Application.Main.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Dtos.Equip
{
    public class EquipMeasurementReadDto : ReadDto
    {
        /// <summary>
        /// 是否计量设备
        /// </summary>
        public bool? IsMeasurementDevice { get; set; }

        /// <summary>
        /// 责任人
        /// </summary>
        public string? ResponsiblePerson { get; set; }

        /// <summary>
        /// 本地化资产编号
        /// </summary>
        public string? LocalAssetNumber { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string? Model { get; set; }

        /// <summary>
        /// 资产名称
        /// </summary>
        public string? AssetName { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime? ExpiryDate { get; set; }
    }

    public class EquipMeasurementQueryDto : PaginatedQueryDto
    {
        /// <summary>
        /// 责任人
        /// </summary>
        public string? ResponsiblePerson { get; set; }

        /// <summary>
        /// 本地化资产编号
        /// </summary>
        public string? LocalAssetNumber { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string? Model { get; set; }

        /// <summary>
        /// 资产名称
        /// </summary>
        public string? AssetName { get; set; }
    }

    public class EquipMeasurementCreateDto : CreateDto
    {
        /// <summary>
        /// 是否计量设备
        /// </summary>
        public bool? IsMeasurementDevice { get; set; }

        /// <summary>
        /// 责任人
        /// </summary>
        public string? ResponsiblePerson { get; set; }

        /// <summary>
        /// 本地化资产编号
        /// </summary>
        public string? LocalAssetNumber { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string? Model { get; set; }

        /// <summary>
        /// 资产名称
        /// </summary>
        public string? AssetName { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime? ExpiryDate { get; set; }
    }

    public class EquipMeasurementUpdateDto : UpdateDto
    {
        /// <summary>
        /// 是否计量设备
        /// </summary>
        public bool? IsMeasurementDevice { get; set; }

        /// <summary>
        /// 责任人
        /// </summary>
        public string? ResponsiblePerson { get; set; }

        /// <summary>
        /// 本地化资产编号
        /// </summary>
        public string? LocalAssetNumber { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string? Model { get; set; }

        /// <summary>
        /// 资产名称
        /// </summary>
        public string? AssetName { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime? ExpiryDate { get; set; }
    }
}

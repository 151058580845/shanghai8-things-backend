using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipMeasurementManager
{
    public class EquipMeasurement : UniversalEntity, IAudited
    {
        public Guid? CreatorId { get; set; }

        public DateTime CreationTime { get; set; }
        public int CreatorLevel { get; set; } = 0;

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

        /// <summary>
        /// 是否即将到期（计算属性）
        /// </summary>
        public bool? IsExpiringSoon => (ExpiryDate - DateTime.Now.ToLocalTime())?.TotalDays <= 30;

        /// <summary>
        /// 输出格式
        /// </summary>
        /// <returns></returns>
        public override string? ToString()
        {
            return $"{ResponsiblePerson} {LocalAssetNumber} {Model} {AssetName} {ExpiryDate:yyyy-MM-dd} 即将到期，请及时计量";
        }

        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}

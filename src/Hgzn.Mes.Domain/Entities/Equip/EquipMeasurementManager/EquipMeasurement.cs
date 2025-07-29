using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;

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

        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }

        public static EquipMeasurement[] Seeds { get; } = new EquipMeasurement[]
        {
            new EquipMeasurement()
            {
                Id = Guid.Parse("a45cbce0-598c-4592-9b1f-04c6d21ff5db"),
                IsMeasurementDevice = true,
                LocalAssetNumber = "2",
                AssetName = "虚拟设备2",
                ExpiryDate = DateTime.Parse("2025-08-11 08:16:22.234"),
            },
            new EquipMeasurement()
            {
                Id = Guid.Parse("6f0b2951-316a-487e-8069-13875ec33b9b"),
                IsMeasurementDevice = true,
                LocalAssetNumber = "virtual_device",
                AssetName = "虚拟设备",
                ExpiryDate = DateTime.Parse("2025-07-28 08:16:22.234"),
            },
            new EquipMeasurement()
            {
                Id = Guid.Parse("a92eb918-d509-4044-af33-b1fd5c614c8c"),
                IsMeasurementDevice = true,
                LocalAssetNumber = "issuer-001",
                AssetName = "rfid-issuer",
                ExpiryDate = DateTime.Parse("2025-09-28 08:16:22.234"),
            },
        };
    }
}

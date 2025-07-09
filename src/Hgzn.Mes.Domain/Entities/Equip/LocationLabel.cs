using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Entities.System.Location;
using Hgzn.Mes.Domain.Shared.Enum;

namespace Hgzn.Mes.Domain.Entities.Equip
{
    public class LocationLabel : UniversalEntity, IAudited
    {
        public string TagId { get; set; } = null!;

        public LabelType Type { get; set; }

        public Guid? EquipLedgerId { get; set; }

        public EquipLedger? EquipLedger { get; set; }

        public Guid? RoomId { get; set; }

        public Room? Room { get; set; }
        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }
        public int CreatorLevel { get; set; } = 0;
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }

        public static LocationLabel[] Seeds { get; } = new LocationLabel[]
        {
            new LocationLabel()
            {
                Id = Guid.Parse("1e7c868a-b6c4-4d44-a998-8b03f7293bb4"),
                TagId = "680993e6-05a8-4af7-b4fd-d7aa74023429",
                Type = LabelType.Equip,
                EquipLedgerId = Guid.Parse("0e8fe514-1171-4eac-ba91-7482d15996eb"),
                EquipLedger = new EquipLedger
                {
                    Id = Guid.Parse("0e8fe514-1171-4eac-ba91-7482d15996eb"),
                    EquipCode = "202506060442************20537TEX",
                    EquipName = "测试读卡器",
                    TypeId = Guid.Parse("ece1d093-80b5-4343-8dbf-a9c330387c5e"),
                    Model = "",
                    PurchaseDate = DateTime.Parse("2025-06-06 03:04:40.523"),
                    SupplierId = null,
                    AssetNumber = "RFID_ZICHAN",
                    DepartmentId = null,
                    RoomId = Guid.Parse("065fb4e9-05c0-b88c-78ff-5539a928f71c"),
                    IsMoving = false,
                    IsMovable = false,
                    LastMoveTime = null,
                    DeviceStatus = (Hgzn.Mes.Domain.Shared.Enums.DeviceStatus)0,
                    ValidityDate = DateTime.Parse("2025-06-06 03:04:40.523"),
                    ResourceId = null,
                    Remark = "",
                    Mac = null,
                    IpAddress = "",
                    EquipLevel = (Hgzn.Mes.Domain.Shared.Enums.EquipLevelEnum)3,
                    State = true,
                    OrderNum = 0,
                    SoftDeleted = false,
                    DeleteTime = null,
                    CreatorId = User.DevUser.Id,
                    CreationTime = DateTime.Parse("2025-06-06 11:06:08.498451"),
                    LastModifierId = null,
                    LastModificationTime = null,
                },
                RoomId = null,
                Room = null,
                CreatorId = null,
                CreationTime = DateTime.MinValue,
                CreatorLevel = 0,
            }
        };
    }
}

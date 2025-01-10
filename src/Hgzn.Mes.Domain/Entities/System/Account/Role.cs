using System.ComponentModel.DataAnnotations.Schema;
using Hgzn.Mes.Domain.Entities.System.Authority;
using Hgzn.Mes.Domain.Entities.System.Base;
using Hgzn.Mes.Domain.Entities.System.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.System.Account
{
    [Table("Role")]
    public class Role : UniversalEntity, ISoftDelete, IAudited, IState, IOrder
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Code { get; set; } = null!;

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderNum { get; set; } = 0;

        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; } = true;

        #region navigation

        public virtual ICollection<User>? Users { get; set; }

        public ICollection<Menu>? Menus { get; set; }

        #endregion navigation

        #region delete filter

        public bool SoftDeleted { get; set; } = false;
        public DateTime? DeleteTime { get; set; } = null;

        #endregion delete filter

        #region audit

        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }

        #endregion

        #region static roles

        public static readonly Role DevRole = new()
        {
            Id = new Guid("e8df3280-8ab1-4b45-8d6a-6c3e669317ac"),
            Name = "developer",
            Code = "dev",
            Description = "developer with all cathable resources even it was obselete"
        };

        public static readonly Role SuperRole = new()
        {
            Id = new Guid("4fe6ebb8-5001-40b4-a59e-d193ad9186f8"),
            Name = "super",
            Code = "super",
            Description = "super user with all catchable resources"
        };

        public static readonly Role AdminRole = new()
        {
            Id = new Guid("e1f23f37-919c-453b-aff1-1214415e54b8"),
            Name = "administrator",
            Code = "admin",
            Description = "admin to manage user resourcs"
        };

        public static readonly Role MemberRole = new()
        {
            Id = new Guid("4a15f57a-0cb7-4cc9-95c0-91ba672a341c"),
            Name = "member",
            Code = "member",
            Description = "normal user with some basic resources"
        };

        #endregion static roles

        public static Role[] Seeds { get; } =
        [
            DevRole,
            SuperRole,
            AdminRole,
            MemberRole,
        ];
    }
}
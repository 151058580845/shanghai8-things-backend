using HgznMes.Domain.Entities.Base;
using HgznMes.Domain.Entities.Authority;

namespace HgznMes.Domain.Entities.Login
{
    public class Role : UniversalEntity, ISoftDelete
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        #region navigation

        public virtual ICollection<Scope>? Scopes { get; set; }
        public virtual ICollection<User>? Users { get; set; }

        public ICollection<Menu>? Menus { get; set; }

        #endregion navigation

        #region delete filter

        public bool SoftDeleted { get; set; } = false;
        public DateTime? DeleteTime { get; set; } = null;

        #endregion delete filter

        #region static roles

        public static readonly Role DevRole = new()
        {
            Id = new Guid("e8df3280-8ab1-4b45-8d6a-6c3e669317ac"),
            Name = "developer",
            Description = "developer with all cathable resources even it was obselete"
        };

        public static readonly Role SuperRole = new()
        {
            Id = new Guid("4fe6ebb8-5001-40b4-a59e-d193ad9186f8"),
            Name = "super",
            Description = "super user with all catchable resources"
        };

        public static readonly Role AdminRole = new()
        {
            Id = new Guid("e1f23f37-919c-453b-aff1-1214415e54b8"),
            Name = "admin",
            Description = "admin to manage user resourcs"
        };

        public static readonly Role VipRole = new()
        {
            Id = new Guid("cbc91154-913e-40ba-aa9b-4ebb551bac99"),
            Name = "vip",
            Description = "import user with some special resources"
        };

        public static readonly Role MemberRole = new()
        {
            Id = new Guid("4a15f57a-0cb7-4cc9-95c0-91ba672a341c"),
            Name = "member",
            Description = "normal user with some basic resources"
        };

        public static readonly Role VisitorRole = new()
        {
            Id = new Guid("ffce17eb-a74c-4b44-aaac-2e2e78e04f9e"),
            Name = "visitor",
            Description = "a visitor with some read resources"
        };

        #endregion static roles

        public static Role[] Seeds { get; } =
        [
            DevRole,
            SuperRole,
            AdminRole,
            VipRole,
            MemberRole,
            VisitorRole
        ];
    }
}
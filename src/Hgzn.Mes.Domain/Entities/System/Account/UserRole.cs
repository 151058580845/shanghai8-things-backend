using Hgzn.Mes.Domain.Entities.Base;

namespace Hgzn.Mes.Domain.Entities.System.Account
{
    /// <summary>
    ///     关联表，如需使用可用Set获取
    /// </summary>
    public class UserRole : IncrementEntity
    {
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }

        public static UserRole[] Seeds { get; set; } =
        [
            new UserRole { Id = 1, UserId = User.DevUser.Id, RoleId = Role.DevRole.Id},
            new UserRole { Id = 2, UserId = User.SuperUser.Id, RoleId = Role.SuperRole.Id},
            new UserRole { Id = 2, UserId = User.AdminUser.Id, RoleId = Role.AdminRole.Id},
        ];
    }
}
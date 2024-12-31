using HgznMes.Domain.Entities.Authority;
using HgznMes.Domain.Entities.Base;

namespace HgznMes.Domain.Entities.Account
{
    /// <summary>
    ///     关联表，如需使用可用Set获取
    /// </summary>
    public class UserRole : IncrementEntity
    {
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }

        public static IEnumerable<UserRole> Seeds { get; set; } =
        [
            new UserRole { Id = 1, UserId = User.DevUser.Id, RoleId = Role.DevRole.Id},
            new UserRole { Id = 2, UserId = User.AdminUser.Id, RoleId = Role.AdminRole.Id}
        ];

    }
}

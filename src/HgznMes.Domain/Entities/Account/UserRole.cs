using HgznMes.Domain.Entities.Base;

namespace HgznMes.Domain.Entities.Account
{
    /// <summary>
    ///     关联表，如需使用可用Set获取
    /// </summary>
    public class UserRole : IncrementEntity
    {
        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        public Guid RoleId { get; set; }

        public Role Role { get; set; } = null!;
    }
}

using HgznMes.Domain.Entities.Base;
using HgznMes.Domain.Entities.Account;

namespace HgznMes.Domain.Entities.Authority
{
    /// <summary>
    ///     关联表，如需使用可用Set获取
    /// </summary>
    public class RoleMenu : IncrementEntity
    {
        public Guid MenuId { get; set; }

        public Menu Menu { get; set; } = null!;

        public Guid RoleId { get; set; }

        public Role Role { get; set; } = null!;

        public static IEnumerable<RoleMenu> Seeds { get; set; } =
        [
            new RoleMenu{ },
            new RoleMenu{ },
            new RoleMenu{ },
            new RoleMenu{ },
        ];

    }

}

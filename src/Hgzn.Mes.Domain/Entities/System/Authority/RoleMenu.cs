using System.ComponentModel.DataAnnotations.Schema;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Entities.Base;

namespace Hgzn.Mes.Domain.Entities.System.Authority
{
    /// <summary>
    ///     关联表，如需使用可用Set获取
    /// </summary>
    public class RoleMenu : IncrementEntity
    {
        public Guid MenuId { get; set; }

        public Guid RoleId { get; set; }

        public static IEnumerable<RoleMenu> Seeds { get; set; } =
        [
            new RoleMenu { Id = 1, MenuId = Menu.Root.Id, RoleId = Role.DevRole.Id},
            new RoleMenu { Id = 2, MenuId = Menu.Root.Id, RoleId = Role.SuperRole.Id}
        ];
    }
}

using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.System.Account;

namespace Hgzn.Mes.Domain.Entities.System.Authority
{
    /// <summary>
    ///     关联表，如需使用可用Set获取
    /// </summary>
    public class RoleMenu : IncrementEntity, ISeedsGeneratable
    {
        public Guid MenuId { get; set; }

        public Guid RoleId { get; set; }

        #region audit

        public Guid? CreatorId { get; set; }
        public DateTime? CreationTime { get; set; }

        #endregion audit

        public static RoleMenu[] Seeds { get; set; } =
        [
            new RoleMenu { Id = 1, MenuId = Menu.Root.Id, RoleId = Role.DevRole.Id},
            new RoleMenu { Id = 2, MenuId = Menu.Root.Id, RoleId = Role.SuperRole.Id},
            new RoleMenu { Id = 2, MenuId = Menu.Root.Id, RoleId = Role.AdminRole.Id},
            new RoleMenu { Id = 2, MenuId = Menu.APIQuery.Id, RoleId = Role.AdminRole.Id},
            new RoleMenu { Id = 2, MenuId = Menu.APIAdd.Id, RoleId = Role.AdminRole.Id},
            new RoleMenu { Id = 2, MenuId = Menu.APIList.Id, RoleId = Role.AdminRole.Id},
            new RoleMenu { Id = 2, MenuId = Menu.Root.Id, RoleId = Role.MemberRole.Id},
            new RoleMenu { Id = 2, MenuId = Menu.APIList.Id, RoleId = Role.MemberRole.Id},
            new RoleMenu { Id = 2, MenuId = Menu.APIQuery.Id, RoleId = Role.MemberRole.Id},
        ];
    }
}
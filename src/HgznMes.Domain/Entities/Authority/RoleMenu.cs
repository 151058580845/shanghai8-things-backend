using HgznMes.Domain.Entities.Base;
using HgznMes.Domain.Entities.Account;

namespace HgznMes.Domain.Entities.Authority
{
    public class RoleMenu : IncrementEntity
    {
        public Guid MenuId { get; set; }

        public Menu Menu { get; set; } = null!;

        public Guid RoleId { get; set; }

        public Role Role { get; set; } = null!;
    }
}

using HgznMes.Domain.Entities.Base;
using HgznMes.Domain.ValueObjects.UserValue;

namespace HgznMes.Domain.Entities.Login
{
    public class User : UniversalEntity, ISoftDelete
    {
        public string Username { get; set; } = null!;
        public string Passphrase { get; set; } = null!;
        public string Salt { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string TelephoneNumber { get; set; } = null!;
        public DateTime RegisterTime { get; set; }
        public Setting? Settings { get; set; }
        public Detail? Detail { get; set; }

        #region navigation

        public Guid RoleId { get; set; }
        public virtual Role Role { get; set; } = null!;

        #endregion navigation

        #region delete filter

        public bool SoftDeleted { get; set; } = false;
        public DateTime? DeleteTime { get; set; } = null;

        #endregion delete filter

        public static readonly User DevUser = new()
        {
            Username = "developer",
            Passphrase = "Uh+8E9ft9jptdMzAVRKo0UYQtqn5epsbJUZQGbL/Xhk=",
            Salt = "5+fPPv0FShtKo3ed746TiuNojEZsxuPkhbU+YvF5DuQ=",
            DisplayName = "initial-developer",
            Email = "unknow",
            TelephoneNumber = "unknow",
            RegisterTime = DateTime.UnixEpoch,
            RoleId = Role.DevRole.Id,
        };

        public static readonly User SuperUser = new()
        {
            Username = "super",
            Passphrase = "WSAcdSAvzQFUq3iXLWXLmcuPmWHIjE8ffSBTVjJVBPQ=",
            Salt = "aY68cuKZh+LNfYczaGclgtTOYy34yvl1O/H9IX3bBtU=",
            DisplayName = "initial-super",
            Email = "unknow",
            TelephoneNumber = "unknow",
            RegisterTime = DateTime.UnixEpoch,
            RoleId = Role.SuperRole.Id,
        };

        public static readonly User AdminUser = new()
        {
            Username = "admin",
            Passphrase = "Lc8DL5jIpDxDfsDp6gYk2HjVIEzXZ30MJc5eW6OU6ko=",
            Salt = "JO3wh7gOTUQ5cBydCoQqnazvw5dgRoVQkNpdrIAvVgI=",
            DisplayName = "initial-admin",
            Email = "unknow",
            TelephoneNumber = "unknow",
            RegisterTime = DateTime.UnixEpoch,
            RoleId = Role.AdminRole.Id,
        };

        public static User[] Seeds { get; } = new User[]
        {
            DevUser, SuperUser, AdminUser
        };
    }
}
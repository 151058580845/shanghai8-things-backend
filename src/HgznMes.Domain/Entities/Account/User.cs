using HgznMes.Domain.Entities.Base;
using HgznMes.Domain.ValueObjects.UserValue;

namespace HgznMes.Domain.Entities.Account
{
    public class User : UniversalEntity, ISoftDelete, IState
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = null!;
        public string Passphrase { get; set; } = null!;
        
        /// <summary>
        /// 姓名
        /// </summary>s
        public string? Name { get; set; }
        public string Salt { get; set; } = null!;
        public string? Nick { get; set; } = null!;
        public string? Icon { get; set; }
        public string? Email { get; set; } = null!;
        public string? Phone { get; set; } = null!;
        public DateTime RegisterTime { get; set; }
        public Setting? Settings { get; set; }
        public Detail? Detail { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public DateTime? BirthDate { get; set; }

        public bool State { get; set; }

        #region navigation

        public virtual ICollection<Role> Roles { get; set; } = null!;

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

        public static readonly User DevUser = new()
        {
            Username = "developer",
            Passphrase = "Uh+8E9ft9jptdMzAVRKo0UYQtqn5epsbJUZQGbL/Xhk=",
            Salt = "5+fPPv0FShtKo3ed746TiuNojEZsxuPkhbU+YvF5DuQ=",
            Nick = "initial-developer",
            Email = "unknow",
            Phone = "unknow",
            RegisterTime = DateTime.UnixEpoch,
        };

        public static readonly User SuperUser = new()
        {
            Username = "super",
            Passphrase = "WSAcdSAvzQFUq3iXLWXLmcuPmWHIjE8ffSBTVjJVBPQ=",
            Salt = "aY68cuKZh+LNfYczaGclgtTOYy34yvl1O/H9IX3bBtU=",
            Nick = "initial-super",
            Email = "unknow",
            Phone = "unknow",
            RegisterTime = DateTime.UnixEpoch,
        };

        public static readonly User AdminUser = new()
        {
            Username = "admin",
            Passphrase = "Lc8DL5jIpDxDfsDp6gYk2HjVIEzXZ30MJc5eW6OU6ko=",
            Salt = "JO3wh7gOTUQ5cBydCoQqnazvw5dgRoVQkNpdrIAvVgI=",
            Nick = "initial-admin",
            Email = "unknow",
            Phone = "unknow",
            RegisterTime = DateTime.UnixEpoch,
        };

        public static User[] Seeds { get; } =
        [
            DevUser, SuperUser, AdminUser
        ];
    }
}
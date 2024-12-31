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
        public string? Nick { get; set; }
        public string? Icon { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
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
            Passphrase = "ZLdMAg0N2xN8NbXr5wsoevc/bBay/lJT4sLFbUClwTI=",
            Salt = "ue9OQmiW1aH5gzkFKXEB84ToTcHjuroMdzDxymov0CA=",
            Nick = "initial-developer",
            Email = "unknow",
            Phone = "unknow",
            RegisterTime = DateTime.UnixEpoch,
        };

        public static readonly User AdminUser = new()
        {
            Username = "admin",
            Passphrase = "qLGu+48XZDn5UC5TmgIgwb+29lIXYVA1i1vjPAjSY1A=",
            Salt = "hxF4RZh/IdmJmTuzjBChb1d5vdotQmESgTkxJ1Yede0=",
            Nick = "initial-admin",
            Email = "unknow",
            Phone = "unknow",
            RegisterTime = DateTime.UnixEpoch,
        };

        public static User[] Seeds { get; } =
        [
            DevUser, AdminUser
        ];
    }
}
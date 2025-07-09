using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mime;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.ValueObjects.UserValue;

namespace Hgzn.Mes.Domain.Entities.System.Account
{
    public class User : UniversalEntity, /*IAudited*/ILastModificationAudited, ISoftDelete, IState
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
        public string? JobNumber { get; set; }
        public string? Icon { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime RegisterTime { get; set; }
        public Setting? Settings { get; set; }

        public Gender Gender { get; set; } = Gender.Unknow;

        [Description("部门Id")]
        public Guid? DeptId { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public DateTime? BirthDate { get; set; }

        public bool State { get; set; }

        #region navigation

        public virtual List<Role> Roles { get; set; } = null!;

        #endregion navigation

        #region delete filter

        public bool SoftDeleted { get; set; } = false;
        public DateTime? DeleteTime { get; set; } = null;

        #endregion delete filter

        #region audit

        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }
        public int CreatorLevel { get; set; } = 5;
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }

        #endregion audit

        public static readonly User DevUser = new()
        {
            Id = new Guid("08e8bafc-1a6d-4ce8-a921-e95fae5ac56b"),
            Username = "000",
            Passphrase = "ZLdMAg0N2xN8NbXr5wsoevc/bBay/lJT4sLFbUClwTI=",
            Salt = "ue9OQmiW1aH5gzkFKXEB84ToTcHjuroMdzDxymov0CA=",
            Name = "initial-developer",
            Email = "unknow",
            Phone = "unknow",
            RegisterTime = DateTime.UnixEpoch,
            DeptId = Dept.Banzu1.Id,
            State = true,
            CreatorLevel = -1,
            Gender = Gender.Male
        };

        public static readonly User SuperUser = new()
        {
            Id = new Guid("950ead5a-d93a-45e3-8268-505967bb753a"),
            Username = "001",
            Passphrase = "JH0yoMLEexMKJINJEk3hW9tYy5QMdXc0hkB8v2KydFo=",
            Salt = "jrqWKWWcLih7MjrONBiEFCPP3/AcmyhmhAY1seCvl70=",
            Name = "initial-super",
            Email = "unknow",
            Phone = "unknow",
            RegisterTime = DateTime.UnixEpoch,
            DeptId = Dept.Banzu1.Id,
            State = true,
            CreatorLevel = -1,
            Gender = Gender.Male
        };

        public static readonly User AdminUser = new()
        {
            Id = new Guid("d1c3f0fb-f716-4059-bd23-99a1bbfa503d"),
            Username = "002",
            Passphrase = "qLGu+48XZDn5UC5TmgIgwb+29lIXYVA1i1vjPAjSY1A=",
            Salt = "hxF4RZh/IdmJmTuzjBChb1d5vdotQmESgTkxJ1Yede0=",
            Name = "initial-admin",
            Email = "unknow",
            Phone = "unknow",
            RegisterTime = DateTime.UnixEpoch,
            DeptId = Dept.Banzu1.Id,
            State = true,
            CreatorLevel = 2,
            Gender = Gender.Male
        };

        public static User[] Seeds { get; } =
        [
            DevUser, SuperUser, AdminUser
        ];
    }
}
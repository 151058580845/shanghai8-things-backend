using HgznMes.Domain.Entities.Base;
using HgznMes.Domain.Entities.Base.Audited;

namespace HgznMes.Domain.Entities.Authority
{
    public class Menu : UniversalEntity, ISoftDelete , IAudited, IOrder, IState
    {
        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string? Description { get; set; }

        public Guid? ParentId { get; set; }

        public int Type {  get; set; }

        public int OrderNum { get; set; } = -1;

        public int Level { get; set; }

        public string Path { get; set; } = null!;

        /// <summary>
        ///    菜单图标 
        /// </summary>
        public string? IconUrl { get; set; }

        /// <summary>
        /// 是否为外部链接 
        ///</summary>
        public bool IsLink { get; set; }

        /// <summary>
        ///     路由名称
        /// </summary>
        public string? Route { get; set; }

        public string? ScopeCode { get; set; }

        public bool Visible { get; set; } = true;

        public bool Favorite { get; set; } = false;

        public bool State { get; set; }

        #region audit

        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }

        #endregion

        #region delete filter

        public bool SoftDeleted { get; set; } = false;
        public DateTime? DeleteTime { get; set; } = null;

        #endregion delete filter

        #region navigation

        public Menu? Parent {  get; set; }

        #endregion navigation

        #region static

        public static Menu Root = new()
        {
            Id = new Guid("e9df3280-8ab1-4b45-8d6a-6c3e669317ac"),
            Name = "Root",
            Code = "root",
            Description = "root menu",
            Type = 1,
            Path = "root"
        };

        public static IEnumerable<Menu> Seeds { get; } = [Root];

        #endregion
    }
}

using HgznMes.Domain.Entities.Base;

namespace HgznMes.Domain.Entities.Authority
{
    public class Menu : UniversalEntity, ISoftDelete
    {
        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string? Description { get; set; }

        public Guid? ParentId { get; set; }

        public int Type {  get; set; }

        public int Order { get; set; } = -1;

        public int Level { get; set; }

        public string Path { get; set; } = null!;

        public string? IconUrl { get; set; }

        public string? Route { get; set; }

        public bool Visible { get; set; } = true;

        public bool Favorite { get; set; } = false;

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

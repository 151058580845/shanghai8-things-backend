using Hgzn.Mes.Application.Dtos.Base;

namespace Hgzn.Mes.Application.Dtos
{
    public class MenuReadDto : ReadDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string? Description { get; set; }

        public Guid? ParentId { get; set; }

        public int Type { get; set; }

        public int Order { get; set; } = -1;

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

        public IEnumerable<MenuReadDto>? Children { get; set; }
    }

    public class MenuCreateDto : CreateDto
    {
        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string? Description { get; set; }

        public Guid? ParentId { get; set; }

        public int Type { get; set; }

        public int Order { get; set; } = -1;

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

    }

    public class MenuUpdateDto : UpdateDto
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public Guid? ParentId { get; set; }

        public int Type { get; set; }

        public int Order { get; set; } = -1;

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
    }

    public class MenuQueryDto : PaginatedQueryDto
    {
        public bool? State { get; set; }
        public string? Filter { get; set; }
    }
}

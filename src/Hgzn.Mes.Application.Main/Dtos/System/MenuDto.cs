using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos.System
{
    public class MenuReadDto : ReadDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Code { get; set; }

        public string? Description { get; set; }

        public Guid? ParentId { get; set; }

        public int Type { get; set; }

        public int OrderNum { get; set; } = -1;

        public int Level { get; set; }
        /// <summary>
        ///    菜单图标
        /// </summary>
        public string? IconUrl { get; set; }

        /// <summary>
        /// 是否为外部链接
        ///</summary>
        public bool IsLink { get; set; }
        public string? Component { get; set; }

        /// <summary>
        ///     路由名称
        /// </summary>
        public string? Path { get; set; }
        public string? RouteName { get; set; }
        public string? ScopeCode { get; set; }

        public bool Visible { get; set; } = true;
        public bool IsCache { get; set; } = false;

        public bool Favorite { get; set; } = false;
        public bool State { get; set; }
        public MetaDto Meta { get; set; } = new MetaDto();
        public IEnumerable<MenuReadDto>? Children { get; set; }
    }

    public class MenuReaderRouterDto : ReadDto
    {
        public Guid? ParentId { get; set; }
        public string? Path { get; set; }
        public string? Name { get; set; }
        public MetaDto Meta { get; set; } = new MetaDto();
    
        public string? component { get; set; }
        public List<MenuReaderRouterDto>? Children { get; set; }
    }
    
    public class MetaDto : ReadDto
    {
        public string? Icon { get; set; }
        public string? Title { get; set; }

        public List<string>? Roles { get; set; }

        public List<string>? Auths { get; set; }

        public string? FrameSrc { get; set; }

        public string? FrameLoading { get; set; }

        public bool? KeepAlive { get; set; }

        public bool? showLink { get; set; }
    }

    public class MenuCreateDto : CreateDto
    {
        public string Name { get; set; } = null!;

        public string? Code { get; set; }

        public string? Description { get; set; }

        public Guid? ParentId { get; set; }

        public int Type { get; set; }

        public int Order { get; set; } = -1;
        
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
        public string? RouteName { get; set; }

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
        public string? RouteName { get; set; }

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
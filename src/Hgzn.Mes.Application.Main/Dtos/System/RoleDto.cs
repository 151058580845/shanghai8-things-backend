using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.System;

namespace Hgzn.Mes.Application.Main.Dtos
{
    public class RoleCreateDto : CreateDto
    {
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public IEnumerable<string>? MenuIds { get; set; }
    }

    public class RoleReadDto : ReadDto
    {
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool State { get; set; }
        public IEnumerable<MenuReadDto> Menus { get; set; } = null!;
      
        
    }

    public class RoleQueryDto : PaginatedQueryDto
    {
        public string? Name { get; set; }
        public string? Code { get; set; }

        public bool? State { get; set; }
    }

    public class RoleUpdateDto : UpdateDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public IEnumerable<Guid>? MenuIds { get; set; } = null!;

        public IEnumerable<Guid>? UserIds { get; set; } = null!;
    }
}
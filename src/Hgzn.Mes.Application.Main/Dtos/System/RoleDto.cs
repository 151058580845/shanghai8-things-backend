using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos
{
    public class RoleCreateDto : CreateDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public IEnumerable<string> MenuIds { get; set; } = null!;
    }

    public class RoleReadDto : ReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public IEnumerable<MenuReadDto> Menus { get; set; } = null!;
    }

    public class RoleQueryDto : PaginatedQueryDto
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
    }

    public class RoleUpdateDto : UpdateDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public IEnumerable<string> MenuIds { get; set; } = null!;
    }
}
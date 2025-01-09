using HgznMes.Application.Dtos.Base;

namespace HgznMes.Application.Dtos
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
}
using HgznMes.Application.Dtos.Base;

namespace HgznMes.Application.Dtos
{
    public class MenuReadDto : ReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string? Description { get; set; }

        public Guid? ParentId { get; set; }

        public int Type { get; set; }

        public int Order { get; set; }

        public int Level { get; set; }

        public string Path { get; set; } = null!;

        public string IconUrl { get; set; } = null!;

        public string Route { get; set; } = null!;

        public bool Visible { get; set; }

        public bool Favorite { get; set; }

        public IEnumerable<MenuReadDto>? Children { get; set; }
    }

    public class MenuCreateDto : CreateDto
    {
        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string? Description { get; set; }

        public Guid? ParentId { get; set; }

        public int Type { get; set; }

        public int Order { get; set; }

        public string Path { get; set; } = null!;

        public string IconUrl { get; set; } = null!;

        public string Route { get; set; } = null!;

        public bool Visible { get; set; }

        public bool Favorite { get; set; }
    }
}

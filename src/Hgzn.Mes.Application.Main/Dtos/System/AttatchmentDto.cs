
using Hgzn.Mes.Application.Main.Dtos.Base;
using Microsoft.AspNetCore.Http;

namespace Hgzn.Mes.Application.Main.Dtos.System
{
    public class AttachmentReadDto : ReadDto
    {
        public string FileName { get; set; } = null!;
        public string FileExtension { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public string Description { get; set; } = null!;

        public string Version { get; set; } = null!;
        public string VersionDescription { get; set; } = null!;
        public DateTime CreationTime { get; set; }
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public bool State { get; set; } = true;
    }

    public class AttachmentQueryDto : PaginatedQueryDto
    {
        public string? FileType { get; set; } = null!;
        public string? FileName { get; set; } = null!;
        public bool State { get; set; } = true;
    }

    public class AttachmentCreateDto : CreateDto
    {
        public bool? Replace { get; set; }
        public string FileName { get; set; } = null!;
        public string FileExtension { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public string? Version { get; set; } = null!;
        public string? VersionDescription { get; set; } = null!;

        public IFormFile? File { get; set; }
    }
    public class AttachmentUpdateDto : UpdateDto
    {
        public string FileName { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Version { get; set; } = null!;
        public string VersionDescription { get; set; } = null!;
    }
    
}

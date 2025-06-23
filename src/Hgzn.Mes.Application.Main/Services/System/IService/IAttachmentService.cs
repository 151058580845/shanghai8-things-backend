using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Resource;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.Application.Main.Services.System.IService
{
    public interface IAttachmentService : ICrudAppService<
        Attachment, Guid,
        AttachmentReadDto, AttachmentQueryDto,
        AttachmentCreateDto, AttachmentUpdateDto>
    {
        Task<Guid> UploadFileAsync(AttachmentCreateDto dto, IFormFile file);

        Task<byte[]> GetFileAsync(string filePath); 
        Task<string> GetFileBase64Async(Guid id);
    }
}

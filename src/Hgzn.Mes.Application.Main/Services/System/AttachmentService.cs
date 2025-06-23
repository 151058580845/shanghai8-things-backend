using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Resource;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Exceptions;
using Hgzn.Mes.Infrastructure.Utilities;
using Microsoft.AspNetCore.Http;

namespace Hgzn.Mes.Application.Main.Services.System
{
    public class AttachmentService : SugarCrudAppService<
        Attachment, Guid,
        AttachmentReadDto, AttachmentQueryDto,
        AttachmentCreateDto, AttachmentUpdateDto>,
        IAttachmentService
    {
        public override async Task<IEnumerable<AttachmentReadDto>> GetListAsync(AttachmentQueryDto? queryDto = null)
        {
            var entities = await Queryable
                .WhereIF(!string.IsNullOrEmpty(queryDto?.FileName), x => x.FileName.Contains(queryDto!.FileName!))
                .WhereIF(!string.IsNullOrEmpty(queryDto?.FileType), x => x.FileName.Contains(queryDto!.FileType!))
                .OrderBy(x => x.CreationTime)
                .ToListAsync();
            return Mapper.Map<IEnumerable<Attachment>, IEnumerable<AttachmentReadDto>>(entities);
        }

        public override async Task<PaginatedList<AttachmentReadDto>> GetPaginatedListAsync(AttachmentQueryDto queryDto)
        {
            var entities = await Queryable
                .WhereIF(!string.IsNullOrEmpty(queryDto.FileName), x => x.FileName.Contains(queryDto!.FileName!))
                .WhereIF(!string.IsNullOrEmpty(queryDto?.FileType), x => x.FileName.Contains(queryDto!.FileType!))
                .OrderBy(x=>x.CreationTime)
                .ToPaginatedListAsync(queryDto!.PageIndex, queryDto.PageSize);
            return Mapper.Map<PaginatedList<Attachment>, PaginatedList<AttachmentReadDto>>(entities);
        }

        public async Task<Guid> UploadFileAsync(AttachmentCreateDto dto, IFormFile file)
        {
            var directoryPath = Path.Combine(Environment.CurrentDirectory, "attachs");
            var trueName = dto.FileName + '.' + dto.FileExtension;
            if (Attachment.ForbidExtension.Contains(Path.GetExtension(file.FileName)) ||
                trueName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                throw new BadRequestException("file type or name not support");
            var fullPath = Path.Combine(directoryPath, trueName);
            dto.FileName = trueName;
            var attachment = new Attachment();
            await DbContext.Ado.BeginTranAsync();
            try
            {
                var attach = await Queryable.FirstAsync(a => a.FileName == trueName);
                if (attach is null)
                {
                    var entity = Mapper.Map<AttachmentCreateDto, Attachment>(dto);
                    attachment = await DbContext.Insertable(entity).ExecuteReturnEntityAsync();
                }
                else if (dto.Replace != true && File.Exists(fullPath))
                {
                    throw new ForbiddenException("file exist!");
                }

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                using var reader = file.OpenReadStream();
                using var writer = new FileStream(fullPath, FileMode.Create);
                await reader.CopyToAsync(writer);
                writer.Close();
                reader.Close();
                await DbContext.Ado.CommitTranAsync();
            }
            catch
            {
                await DbContext.Ado.RollbackTranAsync();
                return Guid.Empty;
            }

            return attachment.Id;
        }

        public async Task<byte[]> GetFileAsync(string fileName)
        {
            var attach = await Queryable.FirstAsync(a => a.FileName == fileName) ??
                throw new NotFoundException("file record not exist!");
            attach.Count++;
            var fullPath = Path.Combine(Environment.CurrentDirectory, "attachs", fileName);
            if(!File.Exists(fullPath))
                throw new NotFoundException("file not exist!");
            using var reader = new FileStream(fullPath, FileMode.Open);
            var buff = new byte[reader.Length];
            var conut = await reader.ReadAsync(buff);
            await DbContext.Updateable(attach).ExecuteCommandAsync();
            return buff;
        }

        public async Task<string> GetFileBase64Async(Guid id)
        {
            var attach = await DbContext.Queryable<Attachment>().FirstAsync(t => t.Id == id);
            if (attach == null)
                return "";

            var fullPath = Path.Combine(Environment.CurrentDirectory, "attachs", attach.FileName);
            if (!File.Exists(fullPath))
                return "";
            await using var reader = new FileStream(fullPath, FileMode.Open);
            var buffer = new byte[reader.Length];
            await reader.ReadExactlyAsync(buffer, 0, buffer.Length);
            
            // 获取文件扩展名并转换为小写
            var fileExtension = Path.GetExtension(attach.FileName).TrimStart('.').ToLowerInvariant();
            // 根据文件扩展名确定 MIME 类型
            var mimeType = GetMimeType(fileExtension);
            // 构建包含 MIME 类型的 Base64 数据 URI
            var base64String = $"data:{mimeType};base64,{Convert.ToBase64String(buffer)}";
            return base64String;
        }

        private string GetMimeType(string fileExtension)
        {
            // 根据文件扩展名返回对应的 MIME 类型
            switch (fileExtension)
            {
                case "jpg":
                case "jpeg":
                    return "image/jpeg";
                case "png":
                    return "image/png";
                case "gif":
                    return "image/gif";
                case "bmp":
                    return "image/bmp";
                case "svg":
                    return "image/svg+xml";
                default:
                    return "application/octet-stream";
            }
        }

    }
}

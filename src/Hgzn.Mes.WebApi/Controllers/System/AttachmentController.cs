using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Hgzn.Mes.Infrastructure.Utilities.CurrentUser;

namespace Hgzn.Mes.WebApi.Controllers.System
{
    /// <summary>
    ///     附件资源
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentController : Controller
    {
        public AttachmentController(
            IAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }
        private readonly IAttachmentService _attachmentService;
        /// <summary>
        ///     获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<AttachmentReadDto>> GetAsync(Guid id) =>
            (await _attachmentService.GetAsync(id)).Wrap();

        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<int>> DeleteAsync(Guid id) =>
            (await _attachmentService.DeleteAsync(id)).Wrap();

        /// <summary>
        ///     更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<AttachmentReadDto>> UpdateAsync(Guid id, AttachmentUpdateDto input) =>

            (await _attachmentService.UpdateAsync(id, input)).Wrap();

        /// <summary>
        ///     分页查询
        ///     auth: anonymous
        /// </summary>
        /// <param name="input">用于验证用户身份</param>
        /// <returns>更换密码状态</returns>
        [HttpPost]
        [Route("page")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<PaginatedList<AttachmentReadDto>>> GetPaginatedListAsync(AttachmentQueryDto input) =>
            (await _attachmentService.GetPaginatedListAsync(input)).Wrap();

        /// <summary>
        ///     上传文件
        ///     auth: anonymous
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("file")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes(contentType:"multipart/form-data")]
        public async Task<ResponseWrapper<Guid>> UploadFileAsync(AttachmentCreateDto dto) =>
            (await _attachmentService.UploadFileAsync(dto, dto?.File!)).Wrap();

        [HttpPost]
        [Route("file-icon")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<ResponseWrapper<Guid>> UploadIconAsync(IFormFile? file)
        {
            if (file == null) throw new ArgumentNullException("File");
            var user = HttpContext.User.Claims;
            var userId = user.FirstOrDefault(c => c.Type == ClaimType.UserId)!.Value;
            var fileCreate = new AttachmentCreateDto();
            fileCreate.FileName = userId + "-" + DateTimeOffset.Now.ToUnixTimeMilliseconds();
            fileCreate.File = file;
            fileCreate.FileExtension = "png";
            fileCreate.FileType = "image/png";
            
            return (await _attachmentService.UploadFileAsync(fileCreate, file)).Wrap();
        }
           
        
        /// <summary>
        ///      获取文件
        ///      auth: anomy
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("file/{filename}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionReadDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAttachmentFile(string filename)
        {
            var result = await _attachmentService.GetFileAsync(filename);
            // 设置响应头
            // Response.Headers.Add("Content-Disposition", "attachment; filename=yourfilename.pdf");
            Response.ContentType = "application/pdf";

            return File(result, "application/pdf");
            var name = Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(filename))) + Path.GetExtension(filename);
            return File(result, "application/octet-stream", name, true);
        }
        
        
    }
}

using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.System
{
    /// <summary>
    ///     通知资源
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NoticeController : ControllerBase
    {
        public NoticeController(
            INoticeService noticeService
            )
        {
            _noticeService = noticeService;
        }

        private readonly INoticeService _noticeService;

        #region notice

        /// <summary>
        ///     获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:notice:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<NoticeReadDto>> GetAsync(Guid id) =>
            (await _noticeService.GetAsync(id)).Wrap();

        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:notice:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<int>> DeleteAsync(Guid id) =>
            (await _noticeService.DeleteAsync(id)).Wrap();

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
        [Authorize(Policy = $"system:notice:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<NoticeReadDto>> UpdateAsync(Guid id, NoticeUpdateDto input) =>
            (await _noticeService.UpdateAsync(id, input)).Wrap();

        /// <summary>
        ///     分页查询
        /// </summary>
        /// <param name="input">用于验证用户身份</param>
        /// <returns>更换密码状态</returns>
        [HttpPost]
        [Route("page")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:notice:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<PaginatedList<NoticeReadDto>>> GetPaginatedListAsync(NoticeQueryDto input) =>
            (await _noticeService.GetPaginatedListAsync(input)).Wrap();

        /// <summary>
        ///     分页查询
        /// </summary>
        /// <param name="input">用于验证用户身份</param>
        /// <returns>更换密码状态</returns>
        [HttpPost]
        [Route("list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:notice:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<IEnumerable<NoticeReadDto>>> GetListAsync(NoticeQueryDto? input) =>
            (await _noticeService.GetListAsync(input)).Wrap();
        #endregion
    }
}

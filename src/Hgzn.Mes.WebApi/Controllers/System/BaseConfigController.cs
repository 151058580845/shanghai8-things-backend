using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.System
{
    /// <summary>
    ///     代码资源
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BaseConfigController : ControllerBase
    {
        public BaseConfigController(IBaseConfigService baseConfig)
        {
            _baseConfig = baseConfig;
        }

        private readonly IBaseConfigService _baseConfig;

        #region

        /// <summary>
        ///     获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:baseConfig:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<BaseConfigReadDto>> GetAsync(Guid id) =>
            (await _baseConfig.GetAsync(id)).Wrap();

        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:baseConfig:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<int>> DeleteAsync(Guid id) =>
            (await _baseConfig.DeleteAsync(id)).Wrap();

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
        [Authorize(Policy = $"system:baseConfig:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<BaseConfigReadDto>> UpdateAsync(Guid id, BaseConfigUpdateDto input) =>
            (await _baseConfig.UpdateAsync(id, input)).Wrap();

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
        [Authorize(Policy = $"system:baseConfig:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<PaginatedList<BaseConfigReadDto>>> GetPaginatedListAsync(BaseConfigQueryDto input) =>
            (await _baseConfig.GetPaginatedListAsync(input)).Wrap();

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:baseConfig:{ScopeMethodType.Add}")]
        public async Task<ResponseWrapper<BaseConfigReadDto>> PostCreateAsync(BaseConfigCreateDto dto) =>
        (await _baseConfig.CreateAsync(dto)).Wrap();

        #endregion


    }
}

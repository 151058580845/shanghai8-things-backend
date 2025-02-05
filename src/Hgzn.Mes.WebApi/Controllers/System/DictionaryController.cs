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
    ///     字典资源
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        public DictionaryController(
            IDictionaryInfoService infoService,
            IDictionaryTypeService typeService
            )
        {
            _infoService = infoService;
            _typeService = typeService;
        }

        private readonly IDictionaryInfoService _infoService;
        private readonly IDictionaryTypeService _typeService;

        #region _info

        /// <summary>
        ///     获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        
        [Authorize(Policy = $"system:dictionary:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<DictionaryInfoReadDto?>> GetAsync(Guid id) =>
            (await _infoService.GetAsync(id)).Wrap();

        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        
        [Authorize(Policy = $"system:dictionary:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<int>> DeleteAsync(Guid id) =>
            (await _infoService.DeleteAsync(id)).Wrap();

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
        
        [Authorize(Policy = $"system:dictionary:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<DictionaryInfoReadDto?>> UpdateAsync(Guid id, DictionaryInfoUpdateDto input) =>
            (await _infoService.UpdateAsync(id, input)).Wrap();

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
        [Authorize(Policy = $"system:dictionary:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<PaginatedList<DictionaryInfoReadDto>?>> GetPaginatedListAsync(DictionaryInfoQueryDto input) =>
            (await _infoService.GetPaginatedListAsync(input)).Wrap();
        /// <summary>
        ///     分页查询
        ///     auth: anonymous
        /// </summary>
        /// <param name="input">用于验证用户身份</param>
        /// <returns>更换密码状态</returns>
        [HttpPost]
        [Route("list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:dictionary:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<IEnumerable<DictionaryInfoReadDto>>> GetListAsync(DictionaryInfoQueryDto input) =>
            (await _infoService.GetListAsync(input)).Wrap();
        #endregion

        #region _inforule

        /// <summary>
        ///     获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("type/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        
        [Authorize(Policy = $"system:dictionary:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<DictionaryTypeReadDto?>> GetDefineAsync(Guid id) =>
            (await _typeService.GetAsync(id)).Wrap();

        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("type/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        
        [Authorize(Policy = $"system:dictionary:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<int>> DeleteDefineAsync(Guid id) =>
            (await _typeService.DeleteAsync(id)).Wrap();

        /// <summary>
        ///     新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("type")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        
        [Authorize(Policy = $"system:dictionary:{ScopeMethodType.Add}")]
        public async Task<ResponseWrapper<DictionaryTypeReadDto?>> CreateDefineAsync(DictionaryTypeCreateDto input) =>
            (await _typeService.CreateAsync(input)).Wrap();
        /// <summary>
        ///     更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("type/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        
        [Authorize(Policy = $"system:dictionary:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<DictionaryTypeReadDto?>> UpdateDefineAsync(Guid id, DictionaryTypeUpdateDto input) =>
            (await _typeService.UpdateAsync(id, input)).Wrap();
        /// <summary>
        ///     分页查询
        ///     auth: anonymous
        /// </summary>
        /// <param name="input">用于验证用户身份</param>
        /// <returns>更换密码状态</returns>
        [HttpPost]
        [Route("type/page")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        
        [Authorize(Policy = $"system:dictionary:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<PaginatedList<DictionaryTypeReadDto>?>> GetPaginatedDefineListAsync(DictionaryTypeQueryDto input) =>
            (await _typeService.GetPaginatedListAsync(input)).Wrap();

        #endregion
    }
}

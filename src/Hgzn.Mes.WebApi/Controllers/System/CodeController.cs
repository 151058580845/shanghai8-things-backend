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
    ///     代码资源
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CodeController : ControllerBase
    {
        public CodeController(
            ICodeRuleService codeService,
            ICodeRuleDefineService defineService
            )
        {
            _codeService = codeService;
            _defineService = defineService;
        }

        private readonly ICodeRuleService _codeService;
        private readonly ICodeRuleDefineService _defineService;


        #region code

        /// <summary>
        ///     获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:code:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<CodeRuleReadDto?>> GetAsync(Guid id) =>
            (await _codeService.GetAsync(id)).Wrap();

        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:code:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<int>> DeleteAsync(Guid id) =>
            (await _codeService.DeleteAsync(id)).Wrap();

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
        [Authorize(Policy = $"system:code:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<CodeRuleReadDto?>> UpdateAsync(Guid id, CodeRuleUpdateDto input) =>
            (await _codeService.UpdateAsync(id, input)).Wrap();

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
        [Authorize(Policy = $"system:code:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<PaginatedList<CodeRuleReadDto>>> GetPaginatedListAsync(CodeRuleQueryDto input) =>
            (await _codeService.GetPaginatedListAsync(input)).Wrap();

        /// <summary>
        /// 代码生成
        /// </summary>
        /// <param name="codeNumber"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("num/{codeNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:code:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<string?>> GenerateCodeByCodeAsync(string codeNumber) =>
            (await _codeService.GenerateCodeByCodeAsync(codeNumber)).Wrap();

        #endregion

        #region _coderule

        /// <summary>
        ///     获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("define/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:code:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<CodeRuleDefineReadDto?>> GetDefineAsync(Guid id) =>
            (await _defineService.GetAsync(id)).Wrap();

        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("define/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:code:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<int>> DeleteDefineAsync(Guid id) =>
            (await _defineService.DeleteAsync(id)).Wrap();

        /// <summary>
        ///     更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("define/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:code:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<CodeRuleDefineReadDto?>> UpdateDefineAsync(Guid id, CodeRuleDefineUpdateDto input) =>
            (await _defineService.UpdateAsync(id, input)).Wrap();

        /// <summary>
        ///     分页查询
        ///     auth: anonymous
        /// </summary>
        /// <param name="input">用于验证用户身份</param>
        /// <returns>更换密码状态</returns>
        [HttpPost]
        [Route("define/page")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:code:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<PaginatedList<CodeRuleDefineReadDto>>> GetPaginatedDefineListAsync(CodeRuleDefineQueryDto input) =>
            (await _defineService.GetPaginatedListAsync(input)).Wrap();

        #endregion
    }
}

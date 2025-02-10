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
    /// 编码规则控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CodeRuleController : ControllerBase
    {

        public CodeRuleController(
           ICodeRuleService codeRuleService
           )
        {
            _codeRuleService = codeRuleService;
        }

        private readonly ICodeRuleService _codeRuleService;

        #region notice

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:notice:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<CodeRuleReadDto>> GetAsync(Guid id) =>
            (await _codeRuleService.GetAsync(id)).Wrap();

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
            (await _codeRuleService.DeleteAsync(id)).Wrap();

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
        public async Task<ResponseWrapper<CodeRuleReadDto>> UpdateAsync(Guid id, CodeRuleUpdateDto input) =>
            (await _codeRuleService.UpdateAsync(id, input)).Wrap();

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
        [Authorize(Policy = $"system:notice:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<PaginatedList<CodeRuleReadDto>>> GetPaginatedListAsync(CodeRuleQueryDto input) =>
            (await _codeRuleService.GetPaginatedListAsync(input)).Wrap();

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[AllowAnonymous]
        public async Task<ResponseWrapper<CodeRuleReadDto>> PostCreateAsync(CodeRuleCreateDto dto)=>
        (await _codeRuleService.CreateAsync(dto)).Wrap();

        #endregion

        /// <summary>
        /// 返回对应的编码
        /// </summary>
        /// <param name="codeNumber"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/generate-code/{codeNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:notice:{ScopeMethodType.Query}")]
        public async Task<string> GetGenerateCodeByCodeAsync(string codeNumber)
        {
            return await _codeRuleService.GenerateCodeByCodeAsync(codeNumber);
        }

        /// <summary>
        /// 修改编码规则状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:guid}/state")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:notice:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<CodeRuleReadDto>> GetGenerateCodeByCodeAsync(Guid id, bool? state) =>
             (await _codeRuleService.GetGenerateCodeByCodeAsync(id, state)).Wrap();

    }
}

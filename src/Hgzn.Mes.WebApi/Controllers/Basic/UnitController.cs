using Hgzn.Mes.Application.Main.Dtos.Basic;
using Hgzn.Mes.Application.Main.Services.Basic;
using Hgzn.Mes.Application.Main.Services.Basic.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Basic
{

    /// <summary>
    /// 单位控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        public UnitController(
          IUnitService unitService
          )
        {
            _unitService = unitService;
        }

        private readonly IUnitService  _unitService;

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"basic:unit:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<UnitReadDto>> GetAsync(Guid id) =>
            (await _unitService.GetAsync(id)).Wrap();

        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"basic:unit:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<int>> DeleteAsync(Guid id) =>
            (await _unitService.DeleteAsync(id)).Wrap();

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
        [Authorize(Policy = $"basic:unit:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<UnitReadDto>> UpdateAsync(Guid id, UnitUpdateDto input) {
            //if (!string.IsNullOrWhiteSpace(input.PId))
            //{
            //    if (Guid.TryParse(input.PId, out Guid parentId))
            //    {
            //        input.ParentId = parentId;
            //    }
            //}

            return (await _unitService.UpdateAsync(id, input)).Wrap();
        }
           

        /// <summary>
        ///     分页查询
        /// </summary>
        /// <param name="input">用于验证用户身份</param>
        /// <returns>更换密码状态</returns>
        [HttpPost]
        [Route("page")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"basic:unit:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<PaginatedList<UnitReadDto>>> GetPaginatedListAsync(UnitQueryDto input) =>
            (await _unitService.GetPaginatedListAsync(input)).Wrap();

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"basic:unit:{ScopeMethodType.Add}")]
        public async Task<ResponseWrapper<UnitReadDto>> PostCreateAsync(UnitCreateDto dto) {
            //if (!string.IsNullOrWhiteSpace(dto.PId)) {
            //    if (Guid.TryParse(dto.PId, out Guid parentId))
            //    {
            //        dto.ParentId = parentId;
            //    }
            //}

            return (await _unitService.CreateAsync(dto)).Wrap();
        }
       

    }
}

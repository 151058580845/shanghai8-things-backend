using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Equip
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipDataController : ControllerBase
    {
        private readonly IEquipDataService _equipDataService;

        public EquipDataController(IEquipDataService equipDataService)
        {
            _equipDataService = equipDataService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("page")]
        public async Task<ResponseWrapper<PaginatedList<EquipDataReadDto>?>> GetPaginatedListAsync(EquipDataQueryDto queryDto)
        => (await _equipDataService.GetPaginatedListAsync(queryDto)).Wrap();

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("list")]
        public async Task<ResponseWrapper<IEnumerable<EquipDataReadDto>?>> GetListAsync(EquipDataQueryDto queryDto) =>
            (await _equipDataService.GetListAsync(queryDto)).Wrap()!;

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:code:{ScopeMethodType.Add}")]
        public async Task<ResponseWrapper<EquipDataReadDto?>> CreateAsync(EquipDataCreateDto input) =>
            (await _equipDataService.CreateAsync(input)).Wrap();

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:code:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<int>> DeleteAsync(Guid id) =>
            (await _equipDataService.DeleteAsync(id)).Wrap();

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:code:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<EquipDataReadDto?>> UpdateAsync(Guid id, EquipDataUpdateDto input) =>
            (await _equipDataService.UpdateAsync(id, input)).Wrap();

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:code:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<EquipDataReadDto?>> GetAsync(Guid id) =>
            (await _equipDataService.GetAsync(id)).Wrap();
    }
}

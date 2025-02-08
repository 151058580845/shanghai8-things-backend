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
    public class EquipConnectController : ControllerBase
    {
        private readonly IEquipConnService _equipConnectService;
        public EquipConnectController(IEquipConnService equipConnectService)
        {
            _equipConnectService = equipConnectService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("page")]
        public async Task<ResponseWrapper<PaginatedList<EquipConnectReadDto>?>> GetPaginatedListAsync(EquipConnectQueryDto queryDto)
        => (await _equipConnectService.GetPaginatedListAsync(queryDto)).Wrap()!;

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("list")]
        public async Task<ResponseWrapper<IEnumerable<EquipConnectReadDto>?>> GetListAsync(EquipConnectQueryDto queryDto)
            => (await _equipConnectService.GetListAsync(queryDto)).Wrap()!;

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
        public async Task<ResponseWrapper<EquipConnectReadDto?>> CreateAsync(EquipConnectCreateDto input) =>
            (await _equipConnectService.CreateAsync(input)).Wrap()!;

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
            (await _equipConnectService.DeleteAsync(id)).Wrap();

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
        public async Task<ResponseWrapper<EquipConnectReadDto?>> UpdateAsync(Guid id, EquipConnectUpdateDto input) =>
            (await _equipConnectService.UpdateAsync(id, input)).Wrap()!;

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
        public async Task<ResponseWrapper<EquipConnectReadDto?>> GetAsync(Guid id) =>
            (await _equipConnectService.GetAsync(id)).Wrap()!;
    }
}

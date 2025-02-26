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
    public class EquipDataPointController : ControllerBase
    {
        private readonly IEquipDataPointService _equipDataPointService;

        public EquipDataPointController(IEquipDataPointService equipDataPointService)
        {
            _equipDataPointService = equipDataPointService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("page")]
        [Authorize(Policy = $"equip:equipdatapoint:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<PaginatedList<EquipDataPointReadDto>>> GetPaginatedListAsync(EquipDataPointQueryDto queryDto)
        => (await _equipDataPointService.GetPaginatedListAsync(queryDto)).Wrap();

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("list")]
        [Authorize(Policy = $"equip:equipdatapoint:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<IEnumerable<EquipDataPointReadDto>?>> GetListAsync(EquipDataPointQueryDto queryDto)
            => (await _equipDataPointService.GetListAsync(queryDto)).Wrap()!;

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipdatapoint:{ScopeMethodType.Add}")]
        public async Task<ResponseWrapper<EquipDataPointReadDto>> CreateAsync(EquipDataPointCreateDto input) =>
            (await _equipDataPointService.CreateAsync(input)).Wrap();

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipdatapoint:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<int>> DeleteAsync(Guid id) =>
            (await _equipDataPointService.DeleteAsync(id)).Wrap();

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
        [Authorize(Policy = $"equip:equipdatapoint:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<EquipDataPointReadDto>> UpdateAsync(Guid id, EquipDataPointUpdateDto input) =>
            (await _equipDataPointService.UpdateAsync(id, input)).Wrap();

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipdatapoint:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<EquipDataPointReadDto>> GetAsync(Guid id) =>
            (await _equipDataPointService.GetAsync(id)).Wrap();

        /// <summary>
        /// 开始连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:guid}/startconnect")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipdatapoint:{ScopeMethodType.Query}")]
        public async Task StartConnectAsync(Guid id) =>
            await _equipDataPointService.PutStartConnect(id);

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:guid}/stopconnect")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipdatapoint:{ScopeMethodType.Query}")]
        public async Task StopConnectAsync(Guid id) =>
            await _equipDataPointService.PutStopConnect(id);
    }
}

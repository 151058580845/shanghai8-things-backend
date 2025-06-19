using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.Equip;
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
        [Authorize(Policy = $"equip:equipconnect:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<PaginatedList<EquipConnectReadDto>?>> GetPaginatedListAsync(EquipConnectQueryDto queryDto)
        => (await _equipConnectService.GetPaginatedListAsync(queryDto)).Wrap()!;

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("list")]
        [Authorize(Policy = $"equip:equipconnect:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<IEnumerable<EquipConnectReadDto>?>> GetListAsync(EquipConnectQueryDto queryDto)
            => (await _equipConnectService.GetListAsync(queryDto)).Wrap()!;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("name-value-list")]
        [Authorize(Policy = $"equip:equipconnect:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<IEnumerable<NameValueDto>>> GetNameValueListAsync()
            => (await _equipConnectService.GetNameValueListAsync()).Wrap();
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipconnect:{ScopeMethodType.Add}")]
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
        [Authorize(Policy = $"equip:equipconnect:{ScopeMethodType.Remove}")]
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
        [Authorize(Policy = $"equip:equipconnect:{ScopeMethodType.Edit}")]
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
        [Authorize(Policy = $"equip:equipconnect:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<EquipConnectReadDto?>> GetAsync(Guid id) =>
            (await _equipConnectService.GetAsync(id)).Wrap()!;

        
        /// <summary>
        /// 开始连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:guid}/startconnect")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipconnect:{ScopeMethodType.Query}")]
        public async Task StartConnectAsync(Guid id) =>
            await _equipConnectService.PutStartConnect(id);

        /// <summary>
        /// 停止连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:guid}/stopconnect")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipconnect:{ScopeMethodType.Query}")]
        public async Task StopConnectAsync(Guid id) =>
            await _equipConnectService.StopConnectAsync(id);

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <param name="protocolEnum"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("testconnection")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:equipconnect:{ScopeMethodType.Query}")]
        public async Task TestConnection(Domain.Shared.Enums.ConnType protocolEnum, string connectionString) =>
            await _equipConnectService.TestConnection(protocolEnum, connectionString);
        
        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:guid}/{state:bool}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipconnect:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<EquipConnectReadDto>> UpdateStateAsync(Guid id, bool state) =>
            (await _equipConnectService.UpdateStateAsync(id, state)).Wrap();

        /// <summary>
        /// rfid设备
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("issuer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipconnect:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<IEnumerable<EquipConnectReadDto>>> GetRfidIssuerConnectionsAsync() =>
            (await _equipConnectService.GetRfidIssuerConnectionsAsync()).Wrap();
    }
}

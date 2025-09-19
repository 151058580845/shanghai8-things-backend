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
    [AllowAnonymous]
    public class AssetDataController : ControllerBase
    {
        private readonly IAssetDataServer _assetDataServer;

        public AssetDataController(IAssetDataServer assetDataServer)
        {
            _assetDataServer = assetDataServer;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("list")]
        public async Task<ResponseWrapper<IEnumerable<AssetDataReadDto>>> GetListAsync([FromBody] AssetDataQueryDto queryDto)
        => (await _assetDataServer.GetListAsync(queryDto)).Wrap();

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("page")]
        public async Task<ResponseWrapper<PaginatedList<AssetDataReadDto>>> GetPaginatedListAsync([FromBody] AssetDataQueryDto queryDto)
            => (await _assetDataServer.GetPaginatedListAsync(queryDto)).Wrap();

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<AssetDataReadDto>> CreateAsync([FromBody] AssetDataCreateDto input) =>
            (await _assetDataServer.CreateAsync(input)).Wrap();

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<int>> DeleteAsync(Guid id) =>
            (await _assetDataServer.DeleteAsync(id)).Wrap();

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
        public async Task<ResponseWrapper<AssetDataReadDto>> UpdateAsync(Guid id, [FromBody] AssetDataUpdateDto input) =>
            (await _assetDataServer.UpdateAsync(id, input)).Wrap();

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<AssetDataReadDto>> GetAsync(Guid id) =>
            (await _assetDataServer.GetAsync(id)).Wrap();
    }
}

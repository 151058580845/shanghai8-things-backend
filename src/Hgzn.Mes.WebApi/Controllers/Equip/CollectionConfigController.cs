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
    public class CollectionConfigController : ControllerBase
    {
        private readonly ICollectionConfigService _collectionConfigService;

        public CollectionConfigController(ICollectionConfigService collectionConfigService)
        {
            _collectionConfigService = collectionConfigService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("page")]
        [Authorize(Policy = $"equip:collectionconfig:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<PaginatedList<CollectionConfigReadDto>>> GetPaginatedListAsync(CollectionConfigQueryDto queryDto) =>
         (await _collectionConfigService.GetPaginatedListAsync(queryDto)).Wrap();

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("list")]
        [Authorize(Policy = $"equip:collectionconfig:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<IEnumerable<CollectionConfigReadDto>?>> GetListAsync(CollectionConfigQueryDto queryDto) =>
             (await _collectionConfigService.GetListAsync(queryDto)).Wrap()!;

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:collectionconfig:{ScopeMethodType.Add}")]
        public async Task<ResponseWrapper<CollectionConfigReadDto>> CreateAsync(CollectionConfigCreateDto input) =>
            (await _collectionConfigService.CreateAsync(input)).Wrap();

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:collectionconfig:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<int>> DeleteAsync(Guid id) =>
            (await _collectionConfigService.DeleteAsync(id)).Wrap();

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
        [Authorize(Policy = $"equip:collectionconfig:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<CollectionConfigReadDto>> UpdateAsync(Guid id, CollectionConfigUpdateDto input) =>
            (await _collectionConfigService.UpdateAsync(id, input)).Wrap();

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:collectionconfig:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<CollectionConfigReadDto>> GetAsync(Guid id) =>
            (await _collectionConfigService.GetAsync(id)).Wrap();
    }
}

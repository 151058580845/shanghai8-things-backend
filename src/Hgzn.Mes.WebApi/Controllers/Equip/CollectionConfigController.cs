using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.WebApi.Utilities;
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("list")]
        public async Task<ResponseWrapper<PaginatedList<CollectionConfigReadDto>>> GetPaginatedListAsync(CollectionConfigQueryDto queryDto)
        => (await _collectionConfigService.GetPaginatedListAsync(queryDto)).Wrap();
    }
}

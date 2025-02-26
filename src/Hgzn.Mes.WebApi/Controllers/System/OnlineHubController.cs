using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.Hub;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.System
{

    /// <summary>
    ///     主页
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OnlineHubController : ControllerBase
    {
        public OnlineHubController(
              IOnlineHubService  onlineHubService
          )
        {
            _onlineHubService = onlineHubService;
        }

        private readonly IOnlineHubService  _onlineHubService;

        /// <summary>
        ///    查询所有在线用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:role:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<PaginatedList<OnlineUser>>>  GetRolesList(OnlineUserQueryDto queryDto) =>
            ( await _onlineHubService.GetList(queryDto)).Wrap();


        /// <summary>
        ///   用户登出
        /// </summary>
        [HttpDelete]
        [Route("logout/{userid:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<bool> Logout(Guid userid)
            => await _onlineHubService.PutOutUser(userid);
    }
}

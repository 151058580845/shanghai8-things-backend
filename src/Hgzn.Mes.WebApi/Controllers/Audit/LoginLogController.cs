using Hgzn.Mes.Application.Main.Dtos.Audit;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.Audit.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Audit
{

    [Route("api/[controller]")]
    [ApiController]
    public class LoginLogController : ControllerBase
    {
        private readonly ILoginLogService _loginLogService;

        public LoginLogController(ILoginLogService loginLogService)
        {
            _loginLogService = loginLogService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("page")]
        public async Task<ResponseWrapper<PaginatedList<LoginLogReadDto>>> GetPaginatedListAsync(LoginLogQueryDto queryDto) =>
            (await _loginLogService.GetPaginatedListAsync(queryDto)).Wrap();

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("list")]
        public async Task<ResponseWrapper<IEnumerable<LoginLogReadDto>?>> GetListAsync(LoginLogQueryDto queryDto) =>
            (await _loginLogService.GetListAsync(queryDto)).Wrap()!;


        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("deleteAll")]
        public async Task<ResponseWrapper<int>> DeleteAsync(List<Guid>? guids)
        {
            var dCount = 0;
            if (guids != null && guids.Count() > 0)
            {
                foreach (var logId in guids)
                {
                    dCount += await _loginLogService.DeleteAsync(logId);
                }
            }

            return dCount.Wrap();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<LoginLogReadDto>> PostCreateAsync(LoginLogCreateDto dto) =>
        (await _loginLogService.CreateAsync(dto)).Wrap();
    }
}

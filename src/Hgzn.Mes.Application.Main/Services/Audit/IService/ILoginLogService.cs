using Hgzn.Mes.Application.Main.Dtos.Audit;
using Hgzn.Mes.Domain.Entities.Audit;

using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.Audit.IService
{
    public interface ILoginLogService : ICrudAppService<
    LoginLog, Guid,
    LoginLogReadDto, LoginLogQueryDto, LoginLogCreateDto>
    {

        LoginLog GetInfoByHttpContext(HttpContext? httpContext);

        /// <summary>
        /// 删除全部日志
        /// </summary>
        /// <returns></returns>
        Task<int> DeleteAllLoginfo();
    }
}

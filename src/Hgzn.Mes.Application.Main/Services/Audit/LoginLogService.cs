using Hgzn.Mes.Application.Main.Dtos.Audit;
using Hgzn.Mes.Application.Main.Services.Audit.IService;
using Hgzn.Mes.Domain.Entities.Audit;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Exceptions;
using Hgzn.Mes.Infrastructure.Utilities;
using IPTools.Core;
using Microsoft.AspNetCore.Http;

using UAParser;

namespace Hgzn.Mes.Application.Main.Services.Audit
{
    public class LoginLogService : SugarCrudAppService<
        LoginLog, Guid,
        LoginLogReadDto, LoginLogQueryDto,
        LoginLogCreateDto>,
    ILoginLogService
    {
        public async override Task<IEnumerable<LoginLogReadDto>> GetListAsync(LoginLogQueryDto? queryDto = null)
        {
            var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(queryDto?.LoginUser), x => x.LoginUser.Contains(queryDto.LoginUser))
            .ToListAsync();
            return Mapper.Map<IEnumerable<LoginLogReadDto>>(entities);
        }

        public async override Task<PaginatedList<LoginLogReadDto>> GetPaginatedListAsync(LoginLogQueryDto queryDto)
        {
            var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(queryDto.LoginUser), x => x.LoginUser.Contains(queryDto.LoginUser))
            .WhereIF(queryDto.StartTime.HasValue, x=>x.CreationTime >= queryDto.StartTime)
            .WhereIF(queryDto.EndTime.HasValue, x => x.CreationTime <= queryDto.EndTime)
            .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
            return Mapper.Map<PaginatedList<LoginLogReadDto>>(entities);
        }

        public LoginLog GetInfoByHttpContext(HttpContext? httpContext)
        {
            ClientInfo GetClientInfo(HttpContext? context)
            {
                var str = GetUserAgent(context);
                var uaParser = Parser.GetDefault();
                ClientInfo c;
                try
                {
                    c = uaParser.Parse(str);
                }
                catch
                {
                    c = new ClientInfo("null", new OS("null", "null", "null", "null", "null"),
                        new Device("null", "null", "null"), new UserAgent("null", "null", "null", "null"));
                }

                return c;
            }

            var ipAddr = HttpContextExtensions.GetClientIp(httpContext);
            var location = ipAddr == "127.0.0.1" ? new IpInfo() { Province = "本地", City = "本机" } : IpTool.Search(ipAddr);
            var clientInfo = GetClientInfo(httpContext);
            LoginLog entity = new()
            {
                Browser = clientInfo.Device.Family,
                Os = clientInfo.OS.ToString(),
                LoginIp = ipAddr,
                LoginLocation = location.Province + "-" + location.City
            };
            return entity;
        }



        /// <summary>
        /// 获取浏览器标识
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private  string GetUserAgent( HttpContext? context)
        {
            return context.Request.Headers["User-Agent"]!;
        }

        /// <summary>
        /// 删除全部日志
        /// </summary>
        /// <returns></returns>
        public async Task<int> DeleteAllLoginfo() {

            var entities = await Queryable.Select(a => a.Id).ToListAsync();
            var delcount = 0;
            if (entities.Any())
            {
                delcount =  await DbContext.Deleteable<LoginLog>().Where(s => entities.Contains(s.Id)).ExecuteCommandAsync();
            }

            return delcount;
        }
 
    }
}

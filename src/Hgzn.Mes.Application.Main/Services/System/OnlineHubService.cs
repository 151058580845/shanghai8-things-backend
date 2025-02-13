using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.Hub;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Services;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Hub;

using SqlSugar;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.System
{
    /// <summary>
    /// 在线用户服务 
    /// </summary>
    public class OnlineHubService :BaseService, IOnlineHubService
    {
        private readonly IUserDomainService _userDomainService;


        public OnlineHubService(IUserDomainService  userDomainService)
        {
            _userDomainService = userDomainService;
        }

        public async Task<PaginatedList<OnlineUser>>   GetList(OnlineUserQueryDto dto)
        {
            var retData = OnlineHub.OnlineUserInfos
                .WhereIF(!string.IsNullOrWhiteSpace(dto.UserName), a => a.UserName.Contains(dto.UserName))
                .WhereIF(!string.IsNullOrWhiteSpace(dto.Ipaddr), a => a.Ipaddr.Contains(dto.Ipaddr))
                .Skip((dto.PageIndex - 1) * dto.PageSize)
                .Take(dto.PageSize).ToList();

            return new PaginatedList<OnlineUser>(retData, retData.Count(), dto.PageIndex, dto.PageSize);
        }

        public async Task<bool> PutOutUser(Guid? userId)
        {
            if (await _userDomainService.DeleteTokenAsync(userId.Value)) {
                OnlineHub.RemoveByUserId(userId.Value);
                return true;
            }
          return false;
        }
    }
}

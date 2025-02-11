using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.Hub;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Hub;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.System.IService
{
    /// <summary>
    ///  再先用户服务
    /// </summary>
  public  interface IOnlineHubService: IBaseService
    {
        /// <summary>
        /// 获取登录用户列表
        /// </summary>
        /// <returns></returns>
        Task<PaginatedList<OnlineUser>> GetList(OnlineUserQueryDto dto);

        /// <summary>
        /// 登出用户
        /// </summary>
        /// <returns></returns>
        Task<bool> PutOutUser(Guid? userId);
     

    }
}

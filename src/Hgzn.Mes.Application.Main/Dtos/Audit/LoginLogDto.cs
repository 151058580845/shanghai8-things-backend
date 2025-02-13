using Hgzn.Mes.Application.Main.Dtos.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Dtos.Audit
{
    public class LoginLogDto
    {
    }

    public class LoginLogReadDto : ReadDto
    {
        /// <summary>
        /// 登录用户 
        ///</summary>
        public string? LoginUser { get; set; }
        /// <summary>
        /// 登录地点 
        ///</summary>
        public string? LoginLocation { get; set; }
        /// <summary>
        /// 登录Ip 
        ///</summary>
        public string? LoginIp { get; set; }
        /// <summary>
        /// 浏览器 
        ///</summary>
        public string? Browser { get; set; }
        /// <summary>
        /// 操作系统 
        ///</summary>
        public string? Os { get; set; }
        /// <summary>
        /// 登录信息 
        ///</summary>
        public string? LogMsg { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
    public class LoginLogQueryDto : PaginatedQueryDto
    {
        /// <summary>
        /// 操作模块    
        ///</summary>
        public string? LoginUser { get; set; }
    }

    public class LoginLogCreateDto : CreateDto
    {
        /// <summary>
        /// 登录用户 
        ///</summary>
        public string? LoginUser { get; set; }
        /// <summary>
        /// 登录地点 
        ///</summary>
        public string? LoginLocation { get; set; }
        /// <summary>
        /// 登录Ip 
        ///</summary>
        public string? LoginIp { get; set; }
        /// <summary>
        /// 浏览器 
        ///</summary>
        public string? Browser { get; set; }
        /// <summary>
        /// 操作系统 
        ///</summary>
        public string? Os { get; set; }
        /// <summary>
        /// 登录信息 
        ///</summary>
        public string? LogMsg { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }

    public class LoginLogDeleteDto 
    {
        public IEnumerable<Guid>?  LoginIds { get; set; }
    }
}

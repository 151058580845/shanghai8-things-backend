using Hgzn.Mes.Application.Main.Dtos.Base;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Dtos.System
{
    public class BaseConfigDto
    {
       
    }

    public class BaseConfigReadDto : ReadDto
    {
        /// <summary>
        /// 配置名称 
        ///</summary>
        [Description("配置名称")]
        public string ConfigName { get; set; } = string.Empty;
        /// <summary>
        /// 配置键 
        ///</summary>
        [Description("配置键")]
        public string ConfigKey { get; set; } = string.Empty;
        /// <summary>
        /// 配置值 
        ///</summary>
        [Description("配置值")]
        public string ConfigValue { get; set; } = string.Empty;
        /// <summary>
        /// 配置类别 
        ///</summary>
        [Description("配置类别")]
        public string? ConfigType { get; set; }
        /// <summary>
        /// 描述 
        ///</summary>
        [Description("描述")]
        public string? Remark { get; set; }

        [Description("状态")]
        public bool State { get; set; }

        [Description("排序")]
        public int OrderNum { get; set; }
    }

    public class BaseConfigCreateDto : CreateDto
    {
        /// <summary>
        /// 配置名称 
        ///</summary>
        [Description("配置名称")]
        public string ConfigName { get; set; } = string.Empty;
        /// <summary>
        /// 配置键 
        ///</summary>
        [Description("配置键")]
        public string ConfigKey { get; set; } = string.Empty;
        /// <summary>
        /// 配置值 
        ///</summary>
        [Description("配置值")]
        public string ConfigValue { get; set; } = string.Empty;
        /// <summary>
        /// 配置类别 
        ///</summary>
        [Description("配置类别")]
        public string? ConfigType { get; set; }
        /// <summary>
        /// 描述 
        ///</summary>
        [Description("描述")]
        public string? Remark { get; set; }


        [Description("状态")]
        public bool State { get; set; }

        [Description("排序")]
        public int OrderNum { get; set; }
    }

    public class BaseConfigUpdateDto : UpdateDto
    {
        /// <summary>
        /// 配置名称 
        ///</summary>
        [Description("配置名称")]
        public string ConfigName { get; set; } = string.Empty;
        /// <summary>
        /// 配置键 
        ///</summary>
        [Description("配置键")]
        public string ConfigKey { get; set; } = string.Empty;
        /// <summary>
        /// 配置值 
        ///</summary>
        [Description("配置值")]
        public string ConfigValue { get; set; } = string.Empty;
        /// <summary>
        /// 配置类别 
        ///</summary>
        [Description("配置类别")]
        public string? ConfigType { get; set; }

        /// <summary>
        /// 描述 
        ///</summary>
        [Description("描述")]
        public string? Remark { get; set; }


        [Description("状态")]
        public bool State { get; set; }

        [Description("排序")]
        public int OrderNum { get; set; }
    }

    public class BaseConfigQueryDto : PaginatedQueryDto
    {
        /// <summary>
        /// 配置名称 
        ///</summary>
        [Description("配置名称")]
        public string ConfigName { get; set; } = string.Empty;

        /// <summary>
        /// 配置类别 
        ///</summary>
        [Description("配置类别")]
        public string? ConfigType { get; set; }

        /// <summary>
        /// 配置键 
        ///</summary>
        [Description("配置键")]
        public string ConfigKey { get; set; } = string.Empty;

    }
}

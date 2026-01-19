using Hgzn.Mes.Domain.Entities.Base;
using System.ComponentModel;

namespace Hgzn.Mes.Domain.Entities.System.Config
{
    public class BaseConfig : UniversalEntity, ISoftDelete, IState, IOrder, ISeedsGeneratable
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
        public string? ConfigValue { get; set; } = string.Empty;
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

        [Description("软删除标志")]
        public bool SoftDeleted { get; set; } = false;

        [Description("删除时间")]
        public DateTime? DeleteTime { get; set; } = null;

        [Description("状态")]
        public bool State { get; set; }

        [Description("排序")]
        public int OrderNum { get; set; }

        #region audit

        public Guid? CreatorId { get; set; }
        public DateTime? CreationTime { get; set; }

        #endregion audit

        #region static

        public static BaseConfig ImportUrl = new BaseConfig()
        {
            Id = Guid.Parse("CF89CCBA-5A39-8C7F-CEB5-3855519BE067"),
            ConfigName = "导入地址",
            ConfigKey = "import_plan_url",
            ConfigValue = "http://10.125.157.101:8090",
            Remark = "导入试验计划请求地址"
        };

        public static BaseConfig DefaultPassword = new BaseConfig()
        {
            Id = Guid.Parse("d977CCBA-5A39-8C7F-CEB5-3855519BE067"),
            ConfigName = "初始密码",
            ConfigKey = "defaultPassword",
            ConfigValue = "12345678",
            Remark = ""
        };

        public static BaseConfig CameraIp = new BaseConfig()
        {
            Id = Guid.Parse("f34153e0-2a1b-41f9-8686-16ce11392fff"),
            ConfigName = "摄像头地址",
            ConfigKey = "camera_ip",
            ConfigValue = "192.168.1.52:8000,",
            Remark = "英文逗号分割"
        };

        public static BaseConfig CameraUser = new BaseConfig()
        {
            Id = Guid.Parse("63529562-431a-4005-af04-47fc70c2b846"),
            ConfigName = "摄像头用户名",
            ConfigKey = "camera_user",
            ConfigValue = "admin",
            Remark = ""
        };

        public static BaseConfig CameraPassword = new BaseConfig()
        {
            Id = Guid.Parse("dccb1a8f-d084-44fc-9541-74baf06fe3a1"),
            ConfigName = "摄像头密码",
            ConfigKey = "camera_password",
            ConfigValue = "Juyee4321",
            Remark = ""
        };

        #endregion

        public static BaseConfig[] Seeds { get; } = [ImportUrl, DefaultPassword, CameraIp, CameraUser, CameraPassword];
    }
}

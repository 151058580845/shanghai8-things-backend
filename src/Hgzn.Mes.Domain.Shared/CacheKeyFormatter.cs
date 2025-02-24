namespace Hgzn.Mes.Domain.Shared
{
    public static class CacheKeyFormatter
    {
        public const string Token = "token:{0}";

        public const string Captcha = "captcha:{0}";

        // 用于指示设备参数设置的 连接状态 3是开启,其他是停止
        public const string EquipState = "equip:{0}:{1}:state";

        public const string EquipHealthStatus = "simuTestSysId:{0}:devTypeId:{1}:compId:{2}:equipHealthStatus";

        // 用于展示设备操作状态,停止,暂停,运行,中止
        public const string EquipOperationStatus = "equipOperationStatus:{0}:status";

        // 用于指示数据采集状态 开启/停止 , 1是开始,0是停止
        public const string EquipDataPointOperationStatus = "equipDataPointOperationStatus:{0}:status";

        // 用于指示数据采集最后一次的数据
        public const string EquipDataPointStatus = "equipDataPointStatus:{0}:{1}:{2}:status";
    }
}
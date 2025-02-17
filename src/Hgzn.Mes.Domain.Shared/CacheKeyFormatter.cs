namespace Hgzn.Mes.Domain.Shared
{
    public static class CacheKeyFormatter
    {
        public const string Token = "token:{0}";

        public const string Captcha = "captcha:{0}";

        public const string EquipState = "equip:{0}:{1}:state";

        public const string EquipHealthStatus = "simuTestSysId:{0}:devTypeId:{1}:compId:{2}:equipHealthStatus";

        public const string EquipOperationStatus = "equipOperationStatus:{0}:status";
    }
}
namespace Hgzn.Mes.Domain.Shared
{
    public static class CacheKeyFormatter
    {
        public const string Token = "token:{0}";

        public const string Captcha = "captcha:{0}";

        // 用于指示设备参数设置的 连接状态 3是开启,其他是停止
        public const string EquipState = "equip:{0}:{1}:state";

        // 用于指示设备的健康状态,是一个Set,我会将所有异常添加进去 "equipHealthStatus:simuTestSysId:devTypeId:equipId"; 注意最后的equipId是存在数据库里的设备台账的ID,而不是本地资产编码,那个可以根据台账的equipCode找到
        public const string EquipHealthStatus = "equipHealthStatus:{0}:{1}:{2}";

        // 设备运行时长"equipHealthStatus:simuTestSysId:devTypeId:equipId"; 注意最后equipId是存在数据库里的设备台账的ID,而不是本地资产编码,那个可以根据台账的equipCode找到
        public const string EquipRunTime = "equipRunTime:{0}:{1}:{2}";

        // 设备心跳,触发一次心跳可以活30秒 "EquipLive:simuTestSysId:devTypeId:equipId" 注意最后equipId是存在数据库里的设备台账的ID,而不是本地资产编码,那个可以根据台账的equipCode找到
        public const string EquipLive = "equipLive:{0}:{1}:{2}";

        // 用于展示设备操作状态,停止,暂停,运行,中止
        public const string EquipOperationStatus = "equipOperationStatus:{0}:status";

        public const string EquipPosAlarm = "equip:{0}:{1}:alarm:pos";

        // 用于指示数据采集状态 开启/停止 , 1是开始,0是停止
        public const string EquipDataPointOperationStatus = "equipDataPointOperationStatus:{0}:status";

        // 用于指示数据采集最后一次的数据
        public const string EquipDataPointStatus = "equipDataPointStatus:{0}:{1}:{2}:status";
        
        //设备当前所在位置
        public const string EquipRoom = "equipRoom:{0}";
        
        //设备当前报警任务主键
        public const string EquipAlarmJob = "equipAlarmJob:{0}";
    }
}
using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_307_ReceiveDatas;

/// <summary>
/// 307_雷达转台
/// </summary>
public class XT_307_SL_2_ReceiveData : UniversalEntity, IAudited
{
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }

    [Description("仿真试验系统识别编码")]
    public byte SimuTestSysld { get; set; }

    [Description("设备类型识别编码")]
    public byte DevTypeld { get; set; }

    [Description("本机识别编码")]
    public string? Compld { get; set; }

    #region 工作模式信息

    [Description("运行状态")]
    public byte OperationStatus { get; set; }

    [Description("是否接入弹道状态")]
    public byte IsTrajectoryConnected { get; set; }

    [Description("预留")]
    public byte Reserved { get; set; }

    #endregion

    #region 健康状态信息

    [Description("状态类型")]
    public byte StatusType { get; set; }

    [Description("自检状态")]
    public byte SelfTestStatus { get; set; }

    [Description("运行状态")]
    public byte HealthOperationStatus { get; set; }

    #endregion

    #region 物理量

    [Description("物理量参数数量")]
    public uint PhysicalQuantityCount { get; set; }

    [Description("内框位置")]
    public float InnerFramePosition { get; set; }

    [Description("中框位置")]
    public float MiddleFramePosition { get; set; }

    [Description("外框位置")]
    public float OuterFramePosition { get; set; }

    [Description("内框速度")]
    public float InnerFrameVelocity { get; set; }

    [Description("中框速度")]
    public float MiddleFrameVelocity { get; set; }

    [Description("外框速度")]
    public float OuterFrameVelocity { get; set; }

    [Description("内框加速度")]
    public float InnerFrameAcceleration { get; set; }

    [Description("中框加速度")]
    public float MiddleFrameAcceleration { get; set; }

    [Description("外框加速度")]
    public float OuterFrameAcceleration { get; set; }

    #endregion

    [Description("运行时间")]
    public uint? RunTime { get; set; }

    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public int CreatorLevel { get; set; } = 0;

#if DEBUG
    public static XT_307_SL_2_ReceiveData[] Seeds
    {
        get
        {
            List<string> uuids = [
                "0198BC7A-470E-737A-8967-7EBF4AC8DE95",
                "0198BC7A-470E-737A-8967-811A0B94F2A3",
                "0198BC7A-470E-737A-8967-86F0120067E4",
                "0198BC7A-470E-737A-8967-888FB62B7B4C",
                "0198BC7A-470E-737A-8967-8F48413984F1",
                "0198BC7A-470E-737A-8967-92A4EBB65E92",
                "0198BC7A-470E-737A-8967-95E9363549C5",
                "0198BC7A-470E-737A-8967-9BD98CA956CB",
                "0198BC7A-470E-737A-8967-9C5E18E2AF26",
                "0198BC7A-470E-737A-8967-A2F27AB72A8B",
                "0198BC7A-470E-737A-8967-A6E4B1294266",
                "0198BC7A-470E-737A-8967-AADE49C944A2",
                "0198BC7A-470E-737A-8967-AD273EA99C7C",
                "0198BC7A-470E-737A-8967-B23E775050F1",
                "0198BC7A-470E-737A-8967-B6C464E5C41A",
                "0198BC7A-470E-737A-8967-B91FF6E53512",
                "0198BC7A-470E-737A-8967-BE83224ABC4A",
                "0198BC7A-470E-737A-8967-C0F547E37B6E",
                "0198BC7A-470E-737A-8967-C5D8DA07DD63",
                "0198BC7A-470E-737A-8967-C8F278246D75",
                "0198BC7A-470E-737A-8967-CCAEDB6BB0C1",
                "0198BC7A-470E-737A-8967-D181145F99A2",
                "0198BC7A-470E-737A-8967-D6A4C8AE1D7F",
                "0198BC7A-470E-737A-8967-D942EA61917B",
                "0198BC7A-470E-737A-8967-DE06185284D6",
                "0198BC7A-470E-737A-8967-E0E0A6634E42",
                "0198BC7A-470E-737A-8967-E533A0CBBCF7",
                "0198BC7A-470E-737A-8967-E92DF5F5B048",
                "0198BC7A-470E-737A-8967-EE3FC3256DBD",
                "0198BC7A-470E-737A-8967-F0FB278010A5",
                "0198BC7A-470E-737A-8967-F55568651669",
                "0198BC7A-470E-737A-8967-FBB975A11A14",
                "0198BC7A-470E-737A-8967-FF2CD352C565",
                "0198BC7A-470E-737A-8968-0134368F8D50",
                "0198BC7A-470E-737A-8968-07A33801BB94",
                "0198BC7A-470E-737A-8968-08C0774B7FDF",
                "0198BC7A-470E-737A-8968-0FE8C64F624F",
                "0198BC7A-470E-737A-8968-11A732FDA355",
                "0198BC7A-470E-737A-8968-174B67C398EC",
                "0198BC7A-470E-737A-8968-18C792CAEE6A",
                "0198BC7A-470E-737A-8968-1E09C521A455",
                "0198BC7A-470E-737A-8968-21DD821D0AE3",
                "0198BC7A-470E-737A-8968-24C4EA84107A",
                "0198BC7A-470E-737A-8968-28FE127B862B",
                "0198BC7A-470E-737A-8968-2F96CC25F851",
                "0198BC7A-470E-737A-8968-301C9BB6DA8C",
                "0198BC7A-470E-737A-8968-36E75349A2DF",
                "0198BC7A-470E-737A-8968-3A6477606EC8",
                "0198BC7A-470E-737A-8968-3D9FEE0B4B11",
                "0198BC7A-470E-737A-8968-40DD4380A0B5",
                "0198BC7A-470E-737A-8968-47D43DBEE0F7",
                "0198BC7A-470E-737A-8968-49BC5A151C80",
                "0198BC7A-470E-737A-8968-4DBCBC9C5095",
                "0198BC7A-470E-737A-8968-527162AF19E5",
                "0198BC7A-470E-737A-8968-5505E55982F7",
                "0198BC7A-470E-737A-8968-589F8E7E9AE6",
                "0198BC7A-470E-737A-8968-5EA61C2183A9",
                "0198BC7A-470E-737A-8968-60A1975946C9",
                "0198BC7A-470E-737A-8968-64F739246B59",
                "0198BC7A-470E-737A-8968-6BF6A26FE5F0",
                "0198BC7A-470E-737A-8968-6CA669325AD5",
                "0198BC7A-470E-737A-8968-70E4741D3F5A",
                "0198BC7A-470E-737A-8968-77592F1EBEAF",
                "0198BC7A-470E-737A-8968-791DCD95F34F",
                "0198BC7A-470E-737A-8968-7C13B635E13D",
                "0198BC7A-470E-737A-8968-8033F94837B8",
                "0198BC7A-470E-737A-8968-86EC8E8602A5",
                "0198BC7A-470F-7032-A4F3-4A52A4B7B36E",
                "0198BC7A-470F-7032-A4F3-4CF5BDDE77D7",
                "0198BC7A-470F-7032-A4F3-51DF659E00F0",
                "0198BC7A-470F-7032-A4F3-57E668B06082",
                "0198BC7A-470F-7032-A4F3-59722E956095",
                "0198BC7A-470F-7032-A4F3-5CD9294A7560",
                "0198BC7A-470F-7032-A4F3-61C72530B166",
                "0198BC7A-470F-7032-A4F3-6450B79F889C",
                "0198BC7A-470F-7032-A4F3-6877D83D4F63",
                "0198BC7A-470F-7032-A4F3-6F8A27A986A9",
                "0198BC7A-470F-7032-A4F3-702395479B5C",
                "0198BC7A-470F-7032-A4F3-76794F8B22E2",
                "0198BC7A-470F-7032-A4F3-7955C56B40C6",
                "0198BC7A-470F-7032-A4F3-7D536B9FA8B4",
                "0198BC7A-470F-7032-A4F3-82C0C7095855",
                "0198BC7A-470F-7032-A4F3-84E25669F06A",
                "0198BC7A-470F-7032-A4F3-8B486D4C28EE",
                "0198BC7A-470F-7032-A4F3-8FD4E5D4DA69",
                "0198BC7A-470F-7032-A4F3-923046AA4583",
                "0198BC7A-470F-7032-A4F3-9646C26A090D",
                "0198BC7A-470F-7032-A4F3-9B99BBBB73E5",
                "0198BC7A-470F-7032-A4F3-9EA7BA61DAC3",
                "0198BC7A-470F-7032-A4F3-A0D5325B4BAC",
                "0198BC7A-470F-7032-A4F3-A4A6DE2C0094",
                "0198BC7A-470F-7032-A4F3-AABA122E3D14",
                "0198BC7A-470F-7032-A4F3-ADC53418BFFF",
                "0198BC7A-470F-7032-A4F3-B11333E7F253",
                "0198BC7A-470F-7032-A4F3-B682676865CC",
                "0198BC7A-470F-7032-A4F3-BA88729D964C",
                "0198BC7A-470F-7032-A4F3-BEF380A77BB6",
                "0198BC7A-470F-7032-A4F3-C0087AB31384",
                "0198BC7A-470F-7032-A4F3-C7745FBE346A",
                "0198BC7A-470F-7032-A4F3-C980AAFCC10D"
            ];
            List<XT_307_SL_2_ReceiveData> list = [];
            List<string> equipUuids = [
                "0198BBB27F42727EB6F4CA73BF2C2E67",
                "0198BBB27F42727EB6F4CF3960E0347A",
                "0198BBB27F42727EB6F4D39596158FB4",
                "0198BBB27F42727EB6F4D67DA0E89319",
                "0198BBB27F42727EB6F4D805950EC7B9",
                "0198BBB27F42727EB6F4DCA4D3D7E937",
                "0198BBB27F42727EB6F4E0D22136C705",
                "0198BBB27F42727EB6F4E67DC4FE3A30",
                "0198BBB27F42727EB6F4EBC1339F1CF0",
                "0198BBB27F42727EB6F4EC9D3742AA3F"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                var innerFrameVelocity = rand.NextDouble() * 40 + 20;
                var innerFrameAcceleration = rand.NextDouble() * 19.9 + 0.1;
                list.Add(new XT_307_SL_2_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 2,
                    DevTypeld = 2,
                    Compld = equipUuids[rand.Next(equipUuids.Count - 1)],
                    OperationStatus = 1,
                    IsTrajectoryConnected = 1,
                    Reserved = 1,
                    StatusType = 1,
                    SelfTestStatus = 1,
                    HealthOperationStatus = 1,
                    PhysicalQuantityCount = 9,
                    InnerFramePosition = rand.Next(-180, 180),
                    MiddleFramePosition = rand.Next(-120, 120),
                    OuterFramePosition = rand.Next(-180, 180),
                    InnerFrameVelocity = (float)innerFrameVelocity,
                    MiddleFrameVelocity = (float)(rand.NextDouble() * ((innerFrameVelocity < 30 ? innerFrameVelocity : 30) - 0.1) + 0.1),
                    OuterFrameVelocity = (float)(rand.NextDouble() * 19.9 + 0.1),
                    InnerFrameAcceleration = (float)innerFrameAcceleration,
                    MiddleFrameAcceleration = (float)(rand.NextDouble() * ((innerFrameAcceleration < 10 ? innerFrameVelocity : 10) - 0.05) + 0.05),
                    OuterFrameAcceleration = (float)(rand.NextDouble() * 4.95 + 0.05),
                    RunTime = (uint?)new DateTimeOffset(now.AddSeconds(a)).ToUnixTimeSeconds(),
                    CreationTime = now.AddSeconds(a)
                });
                a++;
            }
            return [.. list];
        }
    }
#endif
}

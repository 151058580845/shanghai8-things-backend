using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_314_ReceiveDatas;

/// <summary>
/// 314_雷达转台
/// </summary>
public class XT_314_SL_2_ReceiveData : UniversalEntity, IAudited
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
    public static XT_314_SL_2_ReceiveData[] Seeds
    {
        get
        {
            List<string> uuids = [
                "0198BC7F-2B09-70DF-B033-6CA46C0A3996",
                "0198BC7F-2B09-70DF-B033-70B3540BFC26",
                "0198BC7F-2B09-70DF-B033-76752B37654F",
                "0198BC7F-2B09-70DF-B033-783FEC1922D2",
                "0198BC7F-2B09-70DF-B033-7CA649B1AB3D",
                "0198BC7F-2B09-70DF-B033-8208F32E8FB3",
                "0198BC7F-2B09-70DF-B033-85B271FDDA7A",
                "0198BC7F-2B09-70DF-B033-8944272220B0",
                "0198BC7F-2B09-70DF-B033-8E965A3F89C7",
                "0198BC7F-2B09-70DF-B033-916D9F40A3A8",
                "0198BC7F-2B09-70DF-B033-968E0C2BFFF3",
                "0198BC7F-2B09-70DF-B033-98BD251A6140",
                "0198BC7F-2B09-70DF-B033-9F2EE3B17E85",
                "0198BC7F-2B09-70DF-B033-A245710DE0B2",
                "0198BC7F-2B09-70DF-B033-A640B9078844",
                "0198BC7F-2B09-70DF-B033-A833220132BF",
                "0198BC7F-2B09-70DF-B033-ACD8FE705C9E",
                "0198BC7F-2B09-70DF-B033-B00FD060C214",
                "0198BC7F-2B09-70DF-B033-B60FDFBD87E8",
                "0198BC7F-2B09-70DF-B033-BBF4CF0FCA50",
                "0198BC7F-2B09-70DF-B033-BD67164877AB",
                "0198BC7F-2B09-70DF-B033-C2E243C54590",
                "0198BC7F-2B09-70DF-B033-C5499C3F227E",
                "0198BC7F-2B09-70DF-B033-CB3624289E68",
                "0198BC7F-2B09-70DF-B033-CDAECB31C60D",
                "0198BC7F-2B09-70DF-B033-D0F58BB98712",
                "0198BC7F-2B09-70DF-B033-D732B11BA31B",
                "0198BC7F-2B09-70DF-B033-DB30D565B4D2",
                "0198BC7F-2B09-70DF-B033-DC4C16888B24",
                "0198BC7F-2B09-70DF-B033-E263CA70656E",
                "0198BC7F-2B09-70DF-B033-E5A947BB404E",
                "0198BC7F-2B09-70DF-B033-EB98E598C609",
                "0198BC7F-2B09-70DF-B033-ED255FB698E8",
                "0198BC7F-2B09-70DF-B033-F01712257FDD",
                "0198BC7F-2B09-70DF-B033-F5052A9D8A5D",
                "0198BC7F-2B09-70DF-B033-F957A149F01A",
                "0198BC7F-2B09-70DF-B033-FE094F0B682B",
                "0198BC7F-2B09-70DF-B034-02F1ADBA9D18",
                "0198BC7F-2B09-70DF-B034-0404B052A3B4",
                "0198BC7F-2B09-70DF-B034-089C436CE758",
                "0198BC7F-2B09-70DF-B034-0FD1B1FF9DE2",
                "0198BC7F-2B09-70DF-B034-127146B237C9",
                "0198BC7F-2B09-70DF-B034-17C841557EA5",
                "0198BC7F-2B09-70DF-B034-1A9D40FD6457",
                "0198BC7F-2B09-70DF-B034-1E67489F220A",
                "0198BC7F-2B09-70DF-B034-20D5425644C1",
                "0198BC7F-2B09-70DF-B034-252F0E811BCF",
                "0198BC7F-2B09-70DF-B034-2AF356B526F1",
                "0198BC7F-2B09-70DF-B034-2D8498E4F87F",
                "0198BC7F-2B09-70DF-B034-31141BDE08C3",
                "0198BC7F-2B09-70DF-B034-346BE8692ECC",
                "0198BC7F-2B09-70DF-B034-39B598662E88",
                "0198BC7F-2B09-70DF-B034-3EAE6F3F4CD6",
                "0198BC7F-2B09-70DF-B034-405DD8F2BCCA",
                "0198BC7F-2B09-70DF-B034-46A184AF985E",
                "0198BC7F-2B09-70DF-B034-4954E0C4124D",
                "0198BC7F-2B09-70DF-B034-4FD642ABFACF",
                "0198BC7F-2B09-70DF-B034-5117A2D9F902",
                "0198BC7F-2B09-70DF-B034-5519F73618F9",
                "0198BC7F-2B09-70DF-B034-5819130EB4A6",
                "0198BC7F-2B09-70DF-B034-5FC5CD9DBD91",
                "0198BC7F-2B09-70DF-B034-632F28111E8F",
                "0198BC7F-2B09-70DF-B034-642CD9B2CC26",
                "0198BC7F-2B09-70DF-B034-6BA1BB6C6AF1",
                "0198BC7F-2B09-70DF-B034-6FA83D75F5E3",
                "0198BC7F-2B09-70DF-B034-701E908F8CF3",
                "0198BC7F-2B09-70DF-B034-77EDC3CB58AD",
                "0198BC7F-2B09-70DF-B034-7915533A49F8",
                "0198BC7F-2B09-70DF-B034-7D790814AA9C",
                "0198BC7F-2B09-70DF-B034-80AC9C2394C3",
                "0198BC7F-2B09-70DF-B034-875A4D22733A",
                "0198BC7F-2B09-70DF-B034-88B1022939C6",
                "0198BC7F-2B09-70DF-B034-8E19CB5E66C5",
                "0198BC7F-2B09-70DF-B034-9350769B4598",
                "0198BC7F-2B09-70DF-B034-96020F818391",
                "0198BC7F-2B09-70DF-B034-983EA2A0A078",
                "0198BC7F-2B09-70DF-B034-9DEBD0188B1F",
                "0198BC7F-2B09-70DF-B034-A32D62BE34E0",
                "0198BC7F-2B09-70DF-B034-A7362C1E397C",
                "0198BC7F-2B09-70DF-B034-AAEDB7FE8430",
                "0198BC7F-2B09-70DF-B034-AE0009F3CA84",
                "0198BC7F-2B09-70DF-B034-B0C1BEA9F691",
                "0198BC7F-2B09-70DF-B034-B470DC1A5095",
                "0198BC7F-2B09-70DF-B034-BBD055645404",
                "0198BC7F-2B09-70DF-B034-BCCC8153E501",
                "0198BC7F-2B09-70DF-B034-C3077F55188C",
                "0198BC7F-2B09-70DF-B034-C44443D7CF33",
                "0198BC7F-2B09-70DF-B034-C9FFFDD71E4B",
                "0198BC7F-2B09-70DF-B034-CDC252E06A8E",
                "0198BC7F-2B09-70DF-B034-D0D2F1A0AB80",
                "0198BC7F-2B09-70DF-B034-D695A82BA7C4",
                "0198BC7F-2B09-70DF-B034-D8F97107CD23",
                "0198BC7F-2B09-70DF-B034-DD214526F20F",
                "0198BC7F-2B09-70DF-B034-E03242259679",
                "0198BC7F-2B09-70DF-B034-E4837917BD56",
                "0198BC7F-2B09-70DF-B034-E8DCF77A6AF7",
                "0198BC7F-2B09-70DF-B034-EE5D446158CE",
                "0198BC7F-2B09-70DF-B034-F0A1E930185B",
                "0198BC7F-2B09-70DF-B034-F40481ECB917",
                "0198BC7F-2B09-70DF-B034-F99DCA3D715D"
            ];
            List<XT_314_SL_2_ReceiveData> list = [];
            List<string> equipUuids = [
                "0198BBB27F42727EB6F54125464DB218",
                "0198BBB27F42727EB6F54782B83D3997",
                "0198BBB27F42727EB6F549178A649FAC",
                "0198BBB27F42727EB6F54EBBB74BDDB3",
                "0198BBB27F42727EB6F55162661A3560",
                "0198BBB27F42727EB6F55619E0EDB0BD",
                "0198BBB27F42727EB6F55A604A1BF332",
                "0198BBB27F42727EB6F55ED8F1193DE4",
                "0198BBB27F42727EB6F562109B8AAA44",
                "0198BBB27F42727EB6F565F338DB8A9C"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                var innerFrameVelocity = rand.NextDouble() * 40 + 20;
                var innerFrameAcceleration = rand.NextDouble() * 19.9 + 0.1;
                list.Add(new XT_314_SL_2_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 3,
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

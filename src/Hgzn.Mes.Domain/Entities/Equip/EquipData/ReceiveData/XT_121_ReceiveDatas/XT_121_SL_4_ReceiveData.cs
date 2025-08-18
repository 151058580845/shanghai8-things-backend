using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_121_ReceiveDatas;

/// <summary>
/// _固定电源
/// </summary>
public class XT_121_SL_4_ReceiveData : UniversalEntity, IAudited, IPowerSupplyData
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

    [Description("本地还是远程")]
    public byte LocalOrRemote { get; set; }

    [Description("电源数量")]
    public byte PowerSupplyCount { get; set; }

    [Description("电源类型1")]
    public byte PowerSupplyType1 { get; set; }

    [Description("电源类型2")]
    public byte PowerSupplyType2 { get; set; }

    [Description("是否上电")]
    public byte IsPoweredOn { get; set; }

    [Description("预留")]
    public byte Reserved { get; set; }

    #endregion

    #region 健康状态信息

    [Description("状态类型")]
    public byte StatusType { get; set; }

    [Description("工作状态")]
    public byte OperationStatus { get; set; }

    #endregion

    #region 物理量

    [Description("物理量参数数量")]
    public uint PhysicalParameterCount { get; set; }

    [Description("电源1电压设置值")]
    public float Power1VoltageSet { get; set; }

    [Description("电源1电流设置值")]
    public float Power1CurrentSet { get; set; }

    [Description("电源2电压设置值")]
    public float Power2VoltageSet { get; set; }

    [Description("电源2电流设置值")]
    public float Power2CurrentSet { get; set; }

    [Description("电源3电压设置值")]
    public float Power3VoltageSet { get; set; }

    [Description("电源3电流设置值")]
    public float Power3CurrentSet { get; set; }

    [Description("电源4电压设置值")]
    public float Power4VoltageSet { get; set; }

    [Description("电源4电流设置值")]
    public float Power4CurrentSet { get; set; }

    [Description("电源5电压设置值")]
    public float Power5VoltageSet { get; set; }

    [Description("电源5电流设置值")]
    public float Power5CurrentSet { get; set; }

    [Description("电源6电压设置值")]
    public float Power6VoltageSet { get; set; }

    [Description("电源6电流设置值")]
    public float Power6CurrentSet { get; set; }

    [Description("电源7电压设置值")]
    public float Power7VoltageSet { get; set; }

    [Description("电源7电流设置值")]
    public float Power7CurrentSet { get; set; }

    [Description("电源8电压设置值")]
    public float Power8VoltageSet { get; set; }

    [Description("电源8电流设置值")]
    public float Power8CurrentSet { get; set; }

    [Description("电源1电压采集值")]
    public float Power1VoltageRead { get; set; }

    [Description("电源1电流采集值")]
    public float Power1CurrentRead { get; set; }

    [Description("电源2电压采集值")]
    public float Power2VoltageRead { get; set; }

    [Description("电源2电流采集值")]
    public float Power2CurrentRead { get; set; }

    [Description("电源3电压采集值")]
    public float Power3VoltageRead { get; set; }

    [Description("电源3电流采集值")]
    public float Power3CurrentRead { get; set; }

    [Description("电源4电压采集值")]
    public float Power4VoltageRead { get; set; }

    [Description("电源4电流采集值")]
    public float Power4CurrentRead { get; set; }

    [Description("电源5电压采集值")]
    public float Power5VoltageRead { get; set; }

    [Description("电源5电流采集值")]
    public float Power5CurrentRead { get; set; }

    [Description("电源6电压采集值")]
    public float Power6VoltageRead { get; set; }

    [Description("电源6电流采集值")]
    public float Power6CurrentRead { get; set; }

    [Description("电源7电压采集值")]
    public float Power7VoltageRead { get; set; }

    [Description("电源7电流采集值")]
    public float Power7CurrentRead { get; set; }

    [Description("电源8电压采集值")]
    public float Power8VoltageRead { get; set; }

    [Description("电源8电流采集值")]
    public float Power8CurrentRead { get; set; }

    #endregion

    [Description("运行时间")]
    public uint? RunTime { get; set; }

    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public int CreatorLevel { get; set; } = 0;

#if DEBUG
    public static XT_121_SL_4_ReceiveData[] Seeds
    {
        get
        {
            List<XT_121_SL_4_ReceiveData> list = [];
            List<string> uuids = [
                "0198BCA4-43FB-719A-9D2F-C09196A34917",
                "0198BCA4-43FB-719A-9D2F-C7CFC9265F36",
                "0198BCA4-43FB-719A-9D2F-C90A45A5DB34",
                "0198BCA4-43FB-719A-9D2F-CFBA3F789BBA",
                "0198BCA4-43FB-719A-9D2F-D1CEFE16E151",
                "0198BCA4-43FB-719A-9D2F-D596BD13D8FD",
                "0198BCA4-43FB-719A-9D2F-D98253913F77",
                "0198BCA4-43FB-719A-9D2F-DC51ACDA24B4",
                "0198BCA4-43FB-719A-9D2F-E04D705B00D4",
                "0198BCA4-43FB-719A-9D2F-E4282C6773AB",
                "0198BCA4-43FB-719A-9D2F-E8403371713D",
                "0198BCA4-43FB-719A-9D2F-EC253C9A41C5",
                "0198BCA4-43FB-719A-9D2F-F0C6B9B42B47",
                "0198BCA4-43FB-719A-9D2F-F51D88508B67",
                "0198BCA4-43FB-719A-9D2F-FB6F030E7EEB",
                "0198BCA4-43FB-719A-9D2F-FC1D8CE56C20",
                "0198BCA4-43FB-719A-9D30-01F5BC942AF8",
                "0198BCA4-43FB-719A-9D30-07EE5FF36C31",
                "0198BCA4-43FB-719A-9D30-0B55AF48D253",
                "0198BCA4-43FB-719A-9D30-0F313B436582",
                "0198BCA4-43FB-719A-9D30-1269CBA77A16",
                "0198BCA4-43FB-719A-9D30-176BC82DE379",
                "0198BCA4-43FB-719A-9D30-1A7ADF6AA61F",
                "0198BCA4-43FB-719A-9D30-1E272DF50596",
                "0198BCA4-43FB-719A-9D30-2365D68FF3D5",
                "0198BCA4-43FB-719A-9D30-249CA6052A7E",
                "0198BCA4-43FB-719A-9D30-296C8FF18525",
                "0198BCA4-43FB-719A-9D30-2ECFDE2094D3",
                "0198BCA4-43FB-719A-9D30-32D2FA9899D7",
                "0198BCA4-43FB-719A-9D30-368B9B1F9BCC",
                "0198BCA4-43FB-719A-9D30-3B9F1E52731C",
                "0198BCA4-43FB-719A-9D30-3F13B99D4119",
                "0198BCA4-43FB-719A-9D30-43E9474FF75C",
                "0198BCA4-43FB-719A-9D30-4799ED42FAB9",
                "0198BCA4-43FB-719A-9D30-4BEA49347762",
                "0198BCA4-43FB-719A-9D30-4D96A1A9034A",
                "0198BCA4-43FB-719A-9D30-500DE0D6C10A",
                "0198BCA4-43FB-719A-9D30-5464D68A5F13",
                "0198BCA4-43FB-719A-9D30-5847BB5318C8",
                "0198BCA4-43FB-719A-9D30-5D2694ECAD44",
                "0198BCA4-43FB-719A-9D30-60872191CFEC",
                "0198BCA4-43FB-719A-9D30-66936E5DF512",
                "0198BCA4-43FB-719A-9D30-68C9B5EA6B1C",
                "0198BCA4-43FB-719A-9D30-6DE7A0C7E0E0",
                "0198BCA4-43FB-719A-9D30-725B8677F914",
                "0198BCA4-43FB-719A-9D30-75611974AEB7",
                "0198BCA4-43FB-719A-9D30-791913DE652E",
                "0198BCA4-43FB-719A-9D30-7FE07808E19E",
                "0198BCA4-43FB-719A-9D30-82102BF236EB",
                "0198BCA4-43FB-719A-9D30-85C87EEA962A",
                "0198BCA4-43FB-719A-9D30-8923C246DB10",
                "0198BCA4-43FB-719A-9D30-8D826D945458",
                "0198BCA4-43FB-719A-9D30-93C506770D40",
                "0198BCA4-43FB-719A-9D30-9789F901C741",
                "0198BCA4-43FB-719A-9D30-999544113BFC",
                "0198BCA4-43FB-719A-9D30-9F2EF887EB13",
                "0198BCA4-43FB-719A-9D30-A3AA0D23FAE4",
                "0198BCA4-43FB-719A-9D30-A708FC800E42",
                "0198BCA4-43FB-719A-9D30-A98C60BCFA28",
                "0198BCA4-43FB-719A-9D30-AFD61CC81DDF",
                "0198BCA4-43FB-719A-9D30-B09F5BDEAD19",
                "0198BCA4-43FB-719A-9D30-B4F772C9BEAC",
                "0198BCA4-43FB-719A-9D30-B968B2CC0BE1",
                "0198BCA4-43FB-719A-9D30-BC48789B8908",
                "0198BCA4-43FB-719A-9D30-C2564A298806",
                "0198BCA4-43FB-719A-9D30-C57CDCF50759",
                "0198BCA4-43FB-719A-9D30-C9FCBE9DA57D",
                "0198BCA4-43FB-719A-9D30-CFD076EDF079",
                "0198BCA4-43FB-719A-9D30-D381666CE97B",
                "0198BCA4-43FB-719A-9D30-D7E9336E1877",
                "0198BCA4-43FB-719A-9D30-D8FBD521A839",
                "0198BCA4-43FB-719A-9D30-DF57F3454174",
                "0198BCA4-43FB-719A-9D30-E0EA7EC1C3FE",
                "0198BCA4-43FB-719A-9D30-E72943D18408",
                "0198BCA4-43FB-719A-9D30-EBF5186DCC74",
                "0198BCA4-43FB-719A-9D30-EEBE13D7B18B",
                "0198BCA4-43FB-719A-9D30-F3D3581760FE",
                "0198BCA4-43FB-719A-9D30-F583FA77B59D",
                "0198BCA4-43FB-719A-9D30-FAC9BD99F1BB",
                "0198BCA4-43FB-719A-9D30-FD7081A01059",
                "0198BCA4-43FB-719A-9D31-00C23CB3B78D",
                "0198BCA4-43FB-719A-9D31-067C7F60B58D",
                "0198BCA4-43FB-719A-9D31-09D6F756AE12",
                "0198BCA4-43FB-719A-9D31-0FD1E0345B04",
                "0198BCA4-43FB-719A-9D31-119570879D07",
                "0198BCA4-43FB-719A-9D31-14D81B2B3E64",
                "0198BCA4-43FB-719A-9D31-182A8509C0FB",
                "0198BCA4-43FB-719A-9D31-1DA65B80A9F0",
                "0198BCA4-43FB-719A-9D31-203A82E567AC",
                "0198BCA4-43FB-719A-9D31-25DE9CF4A9FC",
                "0198BCA4-43FB-719A-9D31-2A3382FFFA06",
                "0198BCA4-43FB-719A-9D31-2EB6DAFA1A7B",
                "0198BCA4-43FB-719A-9D31-32729DB19DA6",
                "0198BCA4-43FB-719A-9D31-363C05FB8DAF",
                "0198BCA4-43FB-719A-9D31-3B51932E42C3",
                "0198BCA4-43FB-719A-9D31-3F23AF209C0D",
                "0198BCA4-43FB-719A-9D31-418C9C621F18",
                "0198BCA4-43FB-719A-9D31-4696F71502B9",
                "0198BCA4-43FB-719A-9D31-4A9788C978CA",
                "0198BCA4-43FB-719A-9D31-4EBAC0BA3018"
            ];
            List<string> equipUuids = [
                "0198BBB31BD8703DB5F2333F118B53AF",
                "0198BBB31BD8703DB5F23573EB877E31",
                "0198BBB31BD8703DB5F238957691B95E",
                "0198BBB31BD8703DB5F23EF5DB55E133",
                "0198BBB31BD8703DB5F240FD05ECDA44",
                "0198BBB31BD8703DB5F247845464371E",
                "0198BBB31BD8703DB5F2491013B5CE56",
                "0198BBB31BD8703DB5F24F0297061DDB",
                "0198BBB31BD8703DB5F250EF779D32C5",
                "0198BBB31BD8703DB5F2546BBEF11B11"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                list.Add(new XT_121_SL_4_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 6,
                    DevTypeld = 4,
                    Compld = equipUuids[rand.Next(equipUuids.Count - 1)],
                    LocalOrRemote = 1,
                    PowerSupplyCount = 8,
                    PowerSupplyType1 = 1,
                    PowerSupplyType2 = 1,
                    IsPoweredOn = 1,
                    Reserved = 1,
                    StatusType = 1,
                    OperationStatus = 1,
                    PhysicalParameterCount = 32,
                    Power1VoltageSet = (float)(rand.NextDouble() * 28 + 0.9),
                    Power1CurrentSet = (float)(rand.NextDouble() + 2),
                    Power2VoltageSet = (float)(rand.NextDouble() * 10 + 65),
                    Power2CurrentSet = (float)(rand.NextDouble() + 9),
                    Power3VoltageSet = (float)(rand.NextDouble() * 28 + 0.9),
                    Power3CurrentSet = (float)(rand.NextDouble() + 2),
                    Power4VoltageSet = (float)(rand.NextDouble() * 10 + 65),
                    Power4CurrentSet = (float)(rand.NextDouble() + 9),
                    Power5VoltageSet = (float)(rand.NextDouble() * 28 + 0.9),
                    Power5CurrentSet = (float)(rand.NextDouble() + 2),
                    Power6VoltageSet = (float)(rand.NextDouble() * 10 + 65),
                    Power6CurrentSet = (float)(rand.NextDouble() + 9),
                    Power7VoltageSet = (float)(rand.NextDouble() * 28 + 0.9),
                    Power7CurrentSet = (float)(rand.NextDouble() + 2),
                    Power8VoltageSet = (float)(rand.NextDouble() * 10 + 65),
                    Power8CurrentSet = (float)(rand.NextDouble() + 9),
                    Power1VoltageRead = (float)(rand.NextDouble() * 28 + 0.9),
                    Power1CurrentRead = (float)(rand.NextDouble() + 2),
                    Power2VoltageRead = (float)(rand.NextDouble() * 10 + 65),
                    Power2CurrentRead = (float)(rand.NextDouble() + 9),
                    Power3VoltageRead = (float)(rand.NextDouble() * 28 + 0.9),
                    Power3CurrentRead = (float)(rand.NextDouble() + 2),
                    Power4VoltageRead = (float)(rand.NextDouble() * 10 + 65),
                    Power4CurrentRead = (float)(rand.NextDouble() + 9),
                    Power5VoltageRead = (float)(rand.NextDouble() * 28 + 0.9),
                    Power5CurrentRead = (float)(rand.NextDouble() + 2),
                    Power6VoltageRead = (float)(rand.NextDouble() * 10 + 65),
                    Power6CurrentRead = (float)(rand.NextDouble() + 9),
                    Power7VoltageRead = (float)(rand.NextDouble() * 28 + 0.9),
                    Power7CurrentRead = (float)(rand.NextDouble() + 2),
                    Power8VoltageRead = (float)(rand.NextDouble() * 10 + 65),
                    Power8CurrentRead = (float)(rand.NextDouble() + 9),
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

using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_108_ReceiveDatas;

/// <summary>
/// _固定电源
/// </summary>
public class XT_108_SL_4_ReceiveData : UniversalEntity, IAudited, IPowerSupplyData
{
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }

    [Description("仿真试验系统识别编码")]
    public byte SimuTestSysld { get; set; }

    [Description("设备类型识别编码")]
    public byte DevTypeld { get; set; }

    [Description("本机识别编码")]
    public string? Compld { get; set; }

    #region 工作模式信息 6个

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

    #region 健康状态信息 2个

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
    public static XT_108_SL_4_ReceiveData[] Seeds
    {
        get
        {
            List<XT_108_SL_4_ReceiveData> list = [];
            List<string> uuids = [
                "0198BC98-41D5-7599-A115-BAD93F683E8E",
                "0198BC98-41D5-7599-A115-BEBE991CDAEF",
                "0198BC98-41D5-7599-A115-C22B0EBD2A54",
                "0198BC98-41D5-7599-A115-C57FF9DC152D",
                "0198BC98-41D5-7599-A115-C9E02FD49CB0",
                "0198BC98-41D5-7599-A115-CFD4E75007E2",
                "0198BC98-41D5-7599-A115-D01CB9AA5C5E",
                "0198BC98-41D5-7599-A115-D4783BA39E17",
                "0198BC98-41D5-7599-A115-DA8EB94DD15A",
                "0198BC98-41D5-7599-A115-DD1B706F478A",
                "0198BC98-41D5-7599-A115-E1635BFA4A8F",
                "0198BC98-41D5-7599-A115-E7FF0475E9C3",
                "0198BC98-41D5-7599-A115-EB332987B348",
                "0198BC98-41D5-7599-A115-EC3A4F7B2FB5",
                "0198BC98-41D5-7599-A115-F0A54BAC7782",
                "0198BC98-41D5-7599-A115-F6E74296047F",
                "0198BC98-41D5-7599-A115-F8A44E06674F",
                "0198BC98-41D5-7599-A115-FCA8464F4B82",
                "0198BC98-41D5-7599-A116-02F828E2945B",
                "0198BC98-41D5-7599-A116-05391C8A4BA7",
                "0198BC98-41D5-7599-A116-0A63DDCFE000",
                "0198BC98-41D5-7599-A116-0FB6416AA05D",
                "0198BC98-41D5-7599-A116-125F3B13AEBA",
                "0198BC98-41D5-7599-A116-141EF0DB3871",
                "0198BC98-41D5-7599-A116-19FAEF38B545",
                "0198BC98-41D5-7599-A116-1CCE4C2FA0AE",
                "0198BC98-41D5-7599-A116-231F80AF4181",
                "0198BC98-41D5-7599-A116-26521061EF54",
                "0198BC98-41D5-7599-A116-2BA8F5170432",
                "0198BC98-41D5-7599-A116-2CFB973115F0",
                "0198BC98-41D5-7599-A116-32F77F946EC5",
                "0198BC98-41D5-7599-A116-36BC63A9B26F",
                "0198BC98-41D5-7599-A116-3BD387A57874",
                "0198BC98-41D5-7599-A116-3C76D97413A0",
                "0198BC98-41D5-7599-A116-4101BF50DF9F",
                "0198BC98-41D5-7599-A116-448C3DB75327",
                "0198BC98-41D5-7599-A116-48BBFC08A627",
                "0198BC98-41D5-7599-A116-4ECCB4A58D2C",
                "0198BC98-41D5-7599-A116-52A9C1C4D1C1",
                "0198BC98-41D5-7599-A116-5670E4E2E0B0",
                "0198BC98-41D5-7599-A116-5BCB9F3148FD",
                "0198BC98-41D5-7599-A116-5D4A9A17724B",
                "0198BC98-41D5-7599-A116-615365323664",
                "0198BC98-41D5-7599-A116-67C2302BD9DE",
                "0198BC98-41D5-7599-A116-6918B499B137",
                "0198BC98-41D5-7599-A116-6D4FA60949C7",
                "0198BC98-41D5-7599-A116-72EFE2F8AD1B",
                "0198BC98-41D5-7599-A116-77C5D5C02AC5",
                "0198BC98-41D5-7599-A116-793BB6C5A992",
                "0198BC98-41D5-7599-A116-7D94475AE315",
                "0198BC98-41D5-7599-A116-8050874F90E5",
                "0198BC98-41D5-7599-A116-8456FD8C0FD9",
                "0198BC98-41D5-7599-A116-8A811FED65B0",
                "0198BC98-41D5-7599-A116-8D85D4EA30C8",
                "0198BC98-41D5-7599-A116-92E891960D0C",
                "0198BC98-41D5-7599-A116-9722DD07D34D",
                "0198BC98-41D5-7599-A116-9837A67C704D",
                "0198BC98-41D5-7599-A116-9E00C8F397A9",
                "0198BC98-41D5-7599-A116-A3EFEC4E19BB",
                "0198BC98-41D5-7599-A116-A5C610DFFA09",
                "0198BC98-41D5-7599-A116-AA7B4C59DAF1",
                "0198BC98-41D5-7599-A116-AD372E9FF91A",
                "0198BC98-41D5-7599-A116-B34F45EF0858",
                "0198BC98-41D5-7599-A116-B5BF48AE4494",
                "0198BC98-41D5-7599-A116-B9A3275E1737",
                "0198BC98-41D5-7599-A116-BC7DB3D4A416",
                "0198BC98-41D5-7599-A116-C069B3FD42DC",
                "0198BC98-41D5-7599-A116-C4C20698C290",
                "0198BC98-41D6-71DB-9021-9FD91B695198",
                "0198BC98-41D6-71DB-9021-A1C79574498A",
                "0198BC98-41D6-71DB-9021-A74573192337",
                "0198BC98-41D6-71DB-9021-A9042193DDD6",
                "0198BC98-41D6-71DB-9021-AD04359DEF35",
                "0198BC98-41D6-71DB-9021-B1BD2B68990E",
                "0198BC98-41D6-71DB-9021-B71036927C39",
                "0198BC98-41D6-71DB-9021-BAAC50259086",
                "0198BC98-41D6-71DB-9021-BD1D0AC6B136",
                "0198BC98-41D6-71DB-9021-C1E3B11CF656",
                "0198BC98-41D6-71DB-9021-C4D6231341B5",
                "0198BC98-41D6-71DB-9021-CB8481DB0103",
                "0198BC98-41D6-71DB-9021-CD900E14F14F",
                "0198BC98-41D6-71DB-9021-D2C21146F658",
                "0198BC98-41D6-71DB-9021-D49FF72B7AD5",
                "0198BC98-41D6-71DB-9021-DB65B8E76C6C",
                "0198BC98-41D6-71DB-9021-DDCD3091FA13",
                "0198BC98-41D6-71DB-9021-E2D11BD6E100",
                "0198BC98-41D6-71DB-9021-E748064AE3A1",
                "0198BC98-41D6-71DB-9021-EA52F28D5428",
                "0198BC98-41D6-71DB-9021-EF9B5AB8F4DF",
                "0198BC98-41D6-71DB-9021-F0E5B52FBF9E",
                "0198BC98-41D6-71DB-9021-F6F560FCC899",
                "0198BC98-41D6-71DB-9021-F86F6D69A846",
                "0198BC98-41D6-71DB-9021-FE2238190C9B",
                "0198BC98-41D6-71DB-9022-03BB692CAC07",
                "0198BC98-41D6-71DB-9022-073B1686A5E4",
                "0198BC98-41D6-71DB-9022-080D04E26A8E",
                "0198BC98-41D6-71DB-9022-0E19F4659509",
                "0198BC98-41D6-71DB-9022-11C9257C1B94",
                "0198BC98-41D6-71DB-9022-1424C61DE0C3",
                "0198BC98-41D6-71DB-9022-1B9F704FA122"
            ];
            List<string> equipUuids = [
                "0198BBB31BD8703DB5F1BAD5F8718271",
                "0198BBB31BD8703DB5F1BC1ACD1530D4",
                "0198BBB31BD8703DB5F1C09E4BCA0723",
                "0198BBB31BD8703DB5F1C40405EC3F8B",
                "0198BBB31BD8703DB5F1C9783CF855AB",
                "0198BBB31BD8703DB5F1CC7CBE752AC7",
                "0198BBB31BD8703DB5F1D184928F07A9",
                "0198BBB31BD8703DB5F1D49D9613D275",
                "0198BBB31BD8703DB5F1D8B086079C62",
                "0198BBB31BD8703DB5F1DF1271AFFE9F"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                list.Add(new XT_108_SL_4_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 5,
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

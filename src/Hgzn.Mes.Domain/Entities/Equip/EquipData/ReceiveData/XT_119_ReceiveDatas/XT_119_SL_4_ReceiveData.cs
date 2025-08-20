using System.ComponentModel;
using Hgzn.Mes.Domain.Attributes;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_119_ReceiveDatas;

/// <summary>
/// _固定电源
/// </summary>
public class XT_119_SL_4_ReceiveData : UniversalEntity, IAudited, IPowerSupplyData
{
    [TableNotShow]
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }

    [Description("仿真试验系统识别编码")]
    [TableNotShow]
    public byte SimuTestSysld { get; set; }

    [Description("设备类型识别编码")]
    [TableNotShow]
    public byte DevTypeld { get; set; }

    [Description("本机识别编码")]
    public string? Compld { get; set; }

    #region 工作模式信息

    [Description("本地还是远程")]
    [TableNotShow]
    public byte LocalOrRemote { get; set; }

    [Description("电源数量")]
    [TableNotShow]
    public byte PowerSupplyCount { get; set; }

    [Description("电源类型1")]
    [TableNotShow]
    public byte PowerSupplyType1 { get; set; }

    [Description("电源类型2")]
    [TableNotShow]
    public byte PowerSupplyType2 { get; set; }

    [Description("是否上电")]
    [TableNotShow]
    public byte IsPoweredOn { get; set; }

    [Description("预留")]
    [TableNotShow]
    public byte Reserved { get; set; }

    #endregion

    #region 健康状态信息

    [Description("状态类型")]
    [TableNotShow]
    public byte StatusType { get; set; }

    [Description("工作状态")]
    [TableNotShow]
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
    [TableNotShow]
    public uint? RunTime { get; set; }

    [TableNotShow]
    public Guid? LastModifierId { get; set; }
    [TableNotShow]
    public DateTime? LastModificationTime { get; set; }
    [TableNotShow]
    public int CreatorLevel { get; set; } = 0;

#if DEBUG
    public static XT_119_SL_4_ReceiveData[] Seeds
    {
        get
        {
            List<XT_119_SL_4_ReceiveData> list = [];
            List<string> uuids = [
                "0198BCA0-FB7C-77CA-BD30-D997042AF9C8",
                "0198BCA0-FB7C-77CA-BD30-DF3F03DA03C3",
                "0198BCA0-FB7C-77CA-BD30-E0C52B60D838",
                "0198BCA0-FB7C-77CA-BD30-E686332412C1",
                "0198BCA0-FB7C-77CA-BD30-E9C68F77A667",
                "0198BCA0-FB7C-77CA-BD30-ED8ACF8844BA",
                "0198BCA0-FB7C-77CA-BD30-F1B59B78BDDA",
                "0198BCA0-FB7C-77CA-BD30-F77C72F80202",
                "0198BCA0-FB7C-77CA-BD30-F85BD35B01CE",
                "0198BCA0-FB7C-77CA-BD30-FCEC545FF59B",
                "0198BCA0-FB7C-77CA-BD31-02E4E8A5870E",
                "0198BCA0-FB7C-77CA-BD31-0597836DA9C6",
                "0198BCA0-FB7C-77CA-BD31-0A8C8569CB82",
                "0198BCA0-FB7C-77CA-BD31-0EC5F2D810CE",
                "0198BCA0-FB7C-77CA-BD31-10026958F0AA",
                "0198BCA0-FB7C-77CA-BD31-17FA9779B7EB",
                "0198BCA0-FB7C-77CA-BD31-1B0FE63DD027",
                "0198BCA0-FB7C-77CA-BD31-1E7CC8E965FC",
                "0198BCA0-FB7C-77CA-BD31-211EFA94AD62",
                "0198BCA0-FB7C-77CA-BD31-27C777F17179",
                "0198BCA0-FB7C-77CA-BD31-2A45799160B7",
                "0198BCA0-FB7C-77CA-BD31-2D4BF304C3EC",
                "0198BCA0-FB7C-77CA-BD31-33F809DB3312",
                "0198BCA0-FB7C-77CA-BD31-36F14BC350FF",
                "0198BCA0-FB7C-77CA-BD31-382D1ACE1234",
                "0198BCA0-FB7C-77CA-BD31-3F5BD69AB737",
                "0198BCA0-FB7C-77CA-BD31-41EFDF835453",
                "0198BCA0-FB7C-77CA-BD31-4470F6859B16",
                "0198BCA0-FB7C-77CA-BD31-4BC327017437",
                "0198BCA0-FB7C-77CA-BD31-4F464A60AAEB",
                "0198BCA0-FB7C-77CA-BD31-5308AEB63371",
                "0198BCA0-FB7C-77CA-BD31-566DDEE3B6CE",
                "0198BCA0-FB7C-77CA-BD31-595B29FC593D",
                "0198BCA0-FB7C-77CA-BD31-5F3AE0F89DD3",
                "0198BCA0-FB7C-77CA-BD31-606F2134F0BC",
                "0198BCA0-FB7C-77CA-BD31-641F73C87D6F",
                "0198BCA0-FB7C-77CA-BD31-6A903A6034F7",
                "0198BCA0-FB7C-77CA-BD31-6D081BC417FE",
                "0198BCA0-FB7C-77CA-BD31-7161D926CAEB",
                "0198BCA0-FB7C-77CA-BD31-75FCD6D07CEC",
                "0198BCA0-FB7C-77CA-BD31-7B12F0666356",
                "0198BCA0-FB7C-77CA-BD31-7F7EA5514DAF",
                "0198BCA0-FB7C-77CA-BD31-8111CD3AB004",
                "0198BCA0-FB7C-77CA-BD31-86FFFDCD9936",
                "0198BCA0-FB7C-77CA-BD31-8B560B0EE803",
                "0198BCA0-FB7C-77CA-BD31-8E6BBDFC1D64",
                "0198BCA0-FB7C-77CA-BD31-9025FEAC8DE6",
                "0198BCA0-FB7C-77CA-BD31-95FBEA0D8896",
                "0198BCA0-FB7C-77CA-BD31-9A4563F3ABE5",
                "0198BCA0-FB7C-77CA-BD31-9F4E3C0A0BAC",
                "0198BCA0-FB7C-77CA-BD31-A0489F7C9B7A",
                "0198BCA0-FB7C-77CA-BD31-A68F9F12E0DC",
                "0198BCA0-FB7C-77CA-BD31-A8E50BE13E38",
                "0198BCA0-FB7C-77CA-BD31-ACB752878AFD",
                "0198BCA0-FB7C-77CA-BD31-B269000139CA",
                "0198BCA0-FB7C-77CA-BD31-B5E3651DC7E8",
                "0198BCA0-FB7C-77CA-BD31-B8892A4DC1A4",
                "0198BCA0-FB7C-77CA-BD31-BE40CCD14641",
                "0198BCA0-FB7C-77CA-BD31-C23514998287",
                "0198BCA0-FB7C-77CA-BD31-C58DCF73DE28",
                "0198BCA0-FB7C-77CA-BD31-CB42BBD89EB4",
                "0198BCA0-FB7C-77CA-BD31-CE4D31851ED6",
                "0198BCA0-FB7C-77CA-BD31-D0B585F432B8",
                "0198BCA0-FB7C-77CA-BD31-D7B03E81FFF8",
                "0198BCA0-FB7C-77CA-BD31-DB66367AED59",
                "0198BCA0-FB7C-77CA-BD31-DF5DF878F7B5",
                "0198BCA0-FB7C-77CA-BD31-E17BCE6CEBC7",
                "0198BCA0-FB7C-77CA-BD31-E773F879E6EA",
                "0198BCA0-FB7C-77CA-BD31-EB47CAD3F82B",
                "0198BCA0-FB7C-77CA-BD31-ED8D8DC75692",
                "0198BCA0-FB7C-77CA-BD31-F06F06F8F6DD",
                "0198BCA0-FB7C-77CA-BD31-F77FDCCCD444",
                "0198BCA0-FB7C-77CA-BD31-FAD89FA253BB",
                "0198BCA0-FB7C-77CA-BD31-FE225C0C075A",
                "0198BCA0-FB7C-77CA-BD32-034F692F095E",
                "0198BCA0-FB7C-77CA-BD32-040E55373B8E",
                "0198BCA0-FB7C-77CA-BD32-095F574FFC8D",
                "0198BCA0-FB7C-77CA-BD32-0C714D0BBB5E",
                "0198BCA0-FB7C-77CA-BD32-132787AB467C",
                "0198BCA0-FB7C-77CA-BD32-155EFB8C6DF0",
                "0198BCA0-FB7C-77CA-BD32-1A439A840473",
                "0198BCA0-FB7C-77CA-BD32-1D8812B2D2C7",
                "0198BCA0-FB7C-77CA-BD32-213DC15B3A80",
                "0198BCA0-FB7C-77CA-BD32-2720A1BFF302",
                "0198BCA0-FB7C-77CA-BD32-2B46561F0282",
                "0198BCA0-FB7C-77CA-BD32-2CACD7FDEBF7",
                "0198BCA0-FB7C-77CA-BD32-313D943F826D",
                "0198BCA0-FB7C-77CA-BD32-373A8959022B",
                "0198BCA0-FB7C-77CA-BD32-395CA4595086",
                "0198BCA0-FB7C-77CA-BD32-3D9B4FA206CE",
                "0198BCA0-FB7C-77CA-BD32-41828CC31B29",
                "0198BCA0-FB7C-77CA-BD32-4455E222858F",
                "0198BCA0-FB7C-77CA-BD32-4914EC8E6B7D",
                "0198BCA0-FB7C-77CA-BD32-4EA1B93B506A",
                "0198BCA0-FB7C-77CA-BD32-50C83BBED8F4",
                "0198BCA0-FB7C-77CA-BD32-565CC56737AA",
                "0198BCA0-FB7C-77CA-BD32-5BB507BC5D1D",
                "0198BCA0-FB7C-77CA-BD32-5CADB2B11474",
                "0198BCA0-FB7C-77CA-BD32-63D869C941BA",
                "0198BCA0-FB7C-77CA-BD32-675EA577BD57"
            ];
            List<string> equipUuids = [
                "0198BBB3A649726DBD9F898DF400EEF0",
                "0198BBB3A649726DBD9F8F151B534718",
                "0198BBB3A649726DBD9F91F5EB81DDDC",
                "0198BBB3A649726DBD9F95F3CB04DFE3",
                "0198BBB3A649726DBD9F990DA119567F",
                "0198BBB3A649726DBD9F9C1C954EE25C",
                "0198BBB3A649726DBD9FA0881F89E448",
                "0198BBB3A649726DBD9FA457CFBA6886",
                "0198BBB3A649726DBD9FAA865260A3CD",
                "0198BBB3A649726DBD9FAE83D82E90A6"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                list.Add(new XT_119_SL_4_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 10,
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

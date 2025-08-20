using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using Hgzn.Mes.Domain.Attributes;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_112_ReceiveDatas;

/// <summary>
/// _固定电源
/// </summary>
public class XT_112_SL_4_ReceiveData : UniversalEntity, IAudited, IPowerSupplyData
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

    #region 工作模式信息 6个

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

    #region 健康状态信息 2个

    [Description("状态类型")]
    [TableNotShow]
    public byte StatusType { get; set; }

    [Description("工作状态")]
    [TableNotShow]
    public byte OperationStatus { get; set; }

    #endregion

    #region 物理量

    [Description("物理量参数数量")]
    [TableNotShow]
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
    public static XT_112_SL_4_ReceiveData[] Seeds
    {
        get
        {
            List<XT_112_SL_4_ReceiveData> list = [];
            List<string> uuids = [
                "0198BC9E-890E-74E0-8CEA-B89A16DA4C81",
                "0198BC9E-890E-74E0-8CEA-BE480A70D1DE",
                "0198BC9E-890E-74E0-8CEA-C1E927FDF032",
                "0198BC9E-890E-74E0-8CEA-C5B0B81458E8",
                "0198BC9E-890E-74E0-8CEA-CAFC7E190720",
                "0198BC9E-890E-74E0-8CEA-CE37E6230493",
                "0198BC9E-890E-74E0-8CEA-D02860951B55",
                "0198BC9E-890E-74E0-8CEA-D612699F06C3",
                "0198BC9E-890E-74E0-8CEA-DA964A7DB036",
                "0198BC9E-890E-74E0-8CEA-DE778E998090",
                "0198BC9E-890E-74E0-8CEA-E196A6377867",
                "0198BC9E-890E-74E0-8CEA-E7AD2A599BED",
                "0198BC9E-890E-74E0-8CEA-E8DF5E0D97C5",
                "0198BC9E-890E-74E0-8CEA-EEB5D870D4CD",
                "0198BC9E-890E-74E0-8CEA-F1E15E2DC1C0",
                "0198BC9E-890E-74E0-8CEA-F7C541242A17",
                "0198BC9E-890E-74E0-8CEA-FA292825A654",
                "0198BC9E-890E-74E0-8CEA-FEDFB333277D",
                "0198BC9E-890E-74E0-8CEB-013271196A9F",
                "0198BC9E-890E-74E0-8CEB-0586368BE91E",
                "0198BC9E-890E-74E0-8CEB-0AB4481186E3",
                "0198BC9E-890E-74E0-8CEB-0DC2D1BE8D48",
                "0198BC9E-890E-74E0-8CEB-115C4AB3C4B3",
                "0198BC9E-890E-74E0-8CEB-15C980A893C3",
                "0198BC9E-890E-74E0-8CEB-18029D17A302",
                "0198BC9E-890E-74E0-8CEB-1C97A45E6379",
                "0198BC9E-890E-74E0-8CEB-23B9AB1E4758",
                "0198BC9E-890E-74E0-8CEB-254BAB9A49D3",
                "0198BC9E-890E-74E0-8CEB-2A68DFE69893",
                "0198BC9E-890E-74E0-8CEB-2C1CABA66818",
                "0198BC9E-890E-74E0-8CEB-33F7D0D885B1",
                "0198BC9E-890E-74E0-8CEB-35565C4F04EE",
                "0198BC9E-890E-74E0-8CEB-38286A960658",
                "0198BC9E-890E-74E0-8CEB-3C3235F8357E",
                "0198BC9E-890E-74E0-8CEB-40B0CA5EFBB9",
                "0198BC9E-890E-74E0-8CEB-47E38C9AB85E",
                "0198BC9E-890E-74E0-8CEB-4A04893D8442",
                "0198BC9E-890E-74E0-8CEB-4E85ABB99CDA",
                "0198BC9E-890E-74E0-8CEB-51B24839F5C0",
                "0198BC9E-890E-74E0-8CEB-5658C06148E3",
                "0198BC9E-890E-74E0-8CEB-5AE12D0DCC3E",
                "0198BC9E-890E-74E0-8CEB-5ECE054C0E3D",
                "0198BC9E-890E-74E0-8CEB-632C5A03E3B8",
                "0198BC9E-890E-74E0-8CEB-67B2C1BBAC9A",
                "0198BC9E-890E-74E0-8CEB-69E312C4B2DE",
                "0198BC9E-890E-74E0-8CEB-6DBAE6B86D70",
                "0198BC9E-890E-74E0-8CEB-72AFD47DECF7",
                "0198BC9E-890E-74E0-8CEB-77DDEAB75982",
                "0198BC9E-890E-74E0-8CEB-78DC458B444C",
                "0198BC9E-890E-74E0-8CEB-7F33962CA9E8",
                "0198BC9E-890E-74E0-8CEB-817743BFA2D6",
                "0198BC9E-890E-74E0-8CEB-85DC8522FB25",
                "0198BC9E-890E-74E0-8CEB-8819AC21141D",
                "0198BC9E-890E-74E0-8CEB-8DA1133BD03D",
                "0198BC9E-890E-74E0-8CEB-9274D825D5D4",
                "0198BC9E-890E-74E0-8CEB-973F4ED07448",
                "0198BC9E-890E-74E0-8CEB-99025B47376D",
                "0198BC9E-890E-74E0-8CEB-9E4E279E8C81",
                "0198BC9E-890E-74E0-8CEB-A1B9701B861B",
                "0198BC9E-890E-74E0-8CEB-A44E00BFCE5E",
                "0198BC9E-890E-74E0-8CEB-A991E61FF733",
                "0198BC9E-890E-74E0-8CEB-AF1048B47258",
                "0198BC9E-890E-74E0-8CEB-B28C4631636A",
                "0198BC9E-890E-74E0-8CEB-B7E4BED2F380",
                "0198BC9E-890E-74E0-8CEB-B923D84ADF52",
                "0198BC9E-890E-74E0-8CEB-BE4C49B5C816",
                "0198BC9E-890E-74E0-8CEB-C1E022FDC3D7",
                "0198BC9E-890E-74E0-8CEB-C61D37D14AC3",
                "0198BC9E-890E-74E0-8CEB-CAEA67BDCA90",
                "0198BC9E-890E-74E0-8CEB-CF48745CB312",
                "0198BC9E-890E-74E0-8CEB-D254B6A316DC",
                "0198BC9E-890E-74E0-8CEB-D57943667247",
                "0198BC9E-890E-74E0-8CEB-D992FE91CB56",
                "0198BC9E-890E-74E0-8CEB-DCB228CB7089",
                "0198BC9E-890E-74E0-8CEB-E2759E334B38",
                "0198BC9E-890E-74E0-8CEB-E72F908E0EB5",
                "0198BC9E-890E-74E0-8CEB-EBD3EC194ECB",
                "0198BC9E-890E-74E0-8CEB-EE3AE52519F6",
                "0198BC9E-890E-74E0-8CEB-F149C03589D1",
                "0198BC9E-890E-74E0-8CEB-F742CB47EC37",
                "0198BC9E-890E-74E0-8CEB-FB02894E7093",
                "0198BC9E-890E-74E0-8CEB-FDEC01156622",
                "0198BC9E-890E-74E0-8CEC-01113A869E35",
                "0198BC9E-890E-74E0-8CEC-05F1BD1AA696",
                "0198BC9E-890E-74E0-8CEC-0B4E9D1D62AA",
                "0198BC9E-890E-74E0-8CEC-0DBBFD281FC3",
                "0198BC9E-890E-74E0-8CEC-108BE40AE480",
                "0198BC9E-890E-74E0-8CEC-179BB3D2F77B",
                "0198BC9E-890E-74E0-8CEC-1BA0595651B4",
                "0198BC9E-890E-74E0-8CEC-1C4693A3FF17",
                "0198BC9E-890E-74E0-8CEC-236EAFC05CED",
                "0198BC9E-890E-74E0-8CEC-2682E15FC62B",
                "0198BC9E-890E-74E0-8CEC-2A8FD1F6C19E",
                "0198BC9E-890E-74E0-8CEC-2F0CFD1968E6",
                "0198BC9E-890E-74E0-8CEC-30639376B179",
                "0198BC9E-890E-74E0-8CEC-356589395574",
                "0198BC9E-890E-74E0-8CEC-386DC198C4CE",
                "0198BC9E-890E-74E0-8CEC-3CB031371374",
                "0198BC9E-890E-74E0-8CEC-40CEBE0D6B08",
                "0198BC9E-890E-74E0-8CEC-4409F8B0D58B"
            ];
            List<string> equipUuids = [
                "0198BBB3A649726DBD9F39C24621EE28",
                "0198BBB3A649726DBD9F3F86347A4F3B",
                "0198BBB3A649726DBD9F429A5B516C54",
                "0198BBB3A649726DBD9F44BD1D35845E",
                "0198BBB3A649726DBD9F489D62FD4807",
                "0198BBB3A649726DBD9F4CAA65048A8B",
                "0198BBB3A649726DBD9F5296D75F43CB",
                "0198BBB3A649726DBD9F5630FE1291DC",
                "0198BBB3A649726DBD9F5AEA5B8CD4E0",
                "0198BBB3A649726DBD9F5FCEDCBF04BD"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                list.Add(new XT_112_SL_4_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 9,
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

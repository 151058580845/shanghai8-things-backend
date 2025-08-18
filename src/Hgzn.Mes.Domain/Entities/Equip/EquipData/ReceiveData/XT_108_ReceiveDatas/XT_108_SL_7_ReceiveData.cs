using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_108_ReceiveDatas;

/// <summary>
/// _红外源
/// </summary>
public class XT_108_SL_7_ReceiveData : UniversalEntity, IAudited
{
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }

    [Description("仿真试验系统识别编码")]
    public byte SimuTestSysld { get; set; }

    [Description("设备类型识别编码")]
    public byte DevTypeld { get; set; }

    [Description("本机识别编码")]
    public string? Compld { get; set; }

    #region 工作模式信息 2个

    [Description("开启/关闭")]
    public byte OpenOrClose { get; set; }

    [Description("预留")]
    public byte Reserved { get; set; }

    #endregion

    #region 健康状态信息 8个

    [Description("状态类型")]
    public byte StatusType { get; set; }

    [Description("露点温度状态")]
    public byte DewPointTemperatureStatus { get; set; }

    [Description("真空度状态")]
    public byte VacuumStatus { get; set; }

    [Description("冷水机流量状态")]
    public byte ChillerFlowStatus { get; set; }

    [Description("环境箱温度状态")]
    public byte EnvironmentalChamberTemperatureStatus { get; set; }

    [Description("衬底温度状态")]
    public byte SubstrateTemperatureStatus { get; set; }

    [Description("功率电源状态")]
    public byte PowerSupplyStatus { get; set; }

    [Description("控制电源状态")]
    public byte ControlPowerStatus { get; set; }

    #endregion

    #region 物理量

    [Description("物理量参数数量")]
    public uint PhysicalQuantityCount { get; set; }

    [Description("真空度")]
    public float Vacuum { get; set; }

    [Description("环境箱温度")]
    public float ChamberTemperature { get; set; }

    [Description("衬底温度")]
    public float SubstrateTemp { get; set; }

    [Description("环境温度")]
    public float AmbientTemp { get; set; }

    [Description("环境湿度")]
    public float Humidity { get; set; }

    [Description("露点温度")]
    public float DewPoint { get; set; }

    [Description("功率电源电压")]
    public float PowerVoltage { get; set; }

    [Description("功率电源电流")]
    public float PowerCurrent { get; set; }

    [Description("控制电源电压")]
    public float CtrlVoltage { get; set; }

    [Description("控制电源电流")]
    public float CtrlCurrent { get; set; }

    [Description("控制电源逻辑电压")]
    public float LogicVoltage { get; set; }

    [Description("控制电源逻辑电流")]
    public float LogicCurrent { get; set; }

    [Description("液体温度")]
    public float LiquidTemp { get; set; }

    [Description("冷却液流量")]
    public float CoolantFlow { get; set; }

    #endregion

    [Description("运行时间")]
    public uint? RunTime { get; set; }

    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public int CreatorLevel { get; set; } = 0;

#if DEBUG
    public static XT_108_SL_7_ReceiveData[] Seeds
    {
        get
        {
            List<XT_108_SL_7_ReceiveData> list = [];
            List<string> uuids = [
                "0198BD01-2FD4-72B9-94B3-A7A671633545",
                "0198BD01-2FD5-74DF-B30E-83F52144E9FF",
                "0198BD01-2FD5-74DF-B30E-867E528845E9",
                "0198BD01-2FD5-74DF-B30E-8AE2808211FE",
                "0198BD01-2FD5-74DF-B30E-8CC4931D937C",
                "0198BD01-2FD5-74DF-B30E-91DE095FEC9F",
                "0198BD01-2FD5-74DF-B30E-9407C7E372AB",
                "0198BD01-2FD5-74DF-B30E-987E52651841",
                "0198BD01-2FD5-74DF-B30E-9E7B97094F5C",
                "0198BD01-2FD5-74DF-B30E-A3F40BD336B5",
                "0198BD01-2FD5-74DF-B30E-A4E6952924E8",
                "0198BD01-2FD5-74DF-B30E-A8DBEEA8E05F",
                "0198BD01-2FD5-74DF-B30E-AEE7E3F386A2",
                "0198BD01-2FD5-74DF-B30E-B1191DB4C880",
                "0198BD01-2FD5-74DF-B30E-B71AA679AFD8",
                "0198BD01-2FD5-74DF-B30E-BACA1BF79A63",
                "0198BD01-2FD5-74DF-B30E-BD445CC1D7B7",
                "0198BD01-2FD5-74DF-B30E-C3D4110A9863",
                "0198BD01-2FD5-74DF-B30E-C76AEE742168",
                "0198BD01-2FD5-74DF-B30E-CA1E40F2558A",
                "0198BD01-2FD5-74DF-B30E-CE690D6C5B56",
                "0198BD01-2FD5-74DF-B30E-D2911ABE3B70",
                "0198BD01-2FD5-74DF-B30E-D7DF1F60A947",
                "0198BD01-2FD5-74DF-B30E-D81B35E0EF4F",
                "0198BD01-2FD5-74DF-B30E-DDAB56B0EAF6",
                "0198BD01-2FD5-74DF-B30E-E3875E9E481D",
                "0198BD01-2FD5-74DF-B30E-E627BB878CAA",
                "0198BD01-2FD5-74DF-B30E-E9AF2BC26566",
                "0198BD01-2FD5-74DF-B30E-EF1EB3AA9EEC",
                "0198BD01-2FD5-74DF-B30E-F3E16E574FB3",
                "0198BD01-2FD5-74DF-B30E-F674C112E2C5",
                "0198BD01-2FD5-74DF-B30E-FB59EBB6065A",
                "0198BD01-2FD5-74DF-B30E-FCAB69BC2ED6",
                "0198BD01-2FD5-74DF-B30F-0323292FCC03",
                "0198BD01-2FD5-74DF-B30F-0652FCC5D832",
                "0198BD01-2FD5-74DF-B30F-0B4D98F9DC71",
                "0198BD01-2FD5-74DF-B30F-0EDF44498ED3",
                "0198BD01-2FD5-74DF-B30F-12713C2BF66B",
                "0198BD01-2FD5-74DF-B30F-179EAA51A346",
                "0198BD01-2FD5-74DF-B30F-18DC06CA6255",
                "0198BD01-2FD5-74DF-B30F-1D329353616A",
                "0198BD01-2FD5-74DF-B30F-23E074465888",
                "0198BD01-2FD5-74DF-B30F-26AC0C409F06",
                "0198BD01-2FD5-74DF-B30F-2816E644A8E9",
                "0198BD01-2FD5-74DF-B30F-2C4ADF0D31A9",
                "0198BD01-2FD5-74DF-B30F-318C023B0E29",
                "0198BD01-2FD5-74DF-B30F-3669C7E3BCCD",
                "0198BD01-2FD5-74DF-B30F-3A09DFB71079",
                "0198BD01-2FD5-74DF-B30F-3CA801E6054E",
                "0198BD01-2FD5-74DF-B30F-4123D35B859C",
                "0198BD01-2FD5-74DF-B30F-45F9367A7ADF",
                "0198BD01-2FD5-74DF-B30F-4A8E4E2779DF",
                "0198BD01-2FD5-74DF-B30F-4D3A9B4C9964",
                "0198BD01-2FD5-74DF-B30F-504570255184",
                "0198BD01-2FD5-74DF-B30F-56400C216B6C",
                "0198BD01-2FD5-74DF-B30F-5BB0F8897484",
                "0198BD01-2FD5-74DF-B30F-5D6FB24608BB",
                "0198BD01-2FD5-74DF-B30F-61296565F271",
                "0198BD01-2FD5-74DF-B30F-6751F9DBE2A2",
                "0198BD01-2FD5-74DF-B30F-6B4E2FB51979",
                "0198BD01-2FD5-74DF-B30F-6F81165BB6EE",
                "0198BD01-2FD5-74DF-B30F-71F42E4433C6",
                "0198BD01-2FD5-74DF-B30F-75D1E3577C75",
                "0198BD01-2FD5-74DF-B30F-7AE869113EA0",
                "0198BD01-2FD5-74DF-B30F-7CC0AEA9801B",
                "0198BD01-2FD5-74DF-B30F-81A75BC65126",
                "0198BD01-2FD5-74DF-B30F-85CBB1841ED7",
                "0198BD01-2FD5-74DF-B30F-8A89A2AC5EE5",
                "0198BD01-2FD5-74DF-B30F-8CE497338F8E",
                "0198BD01-2FD5-74DF-B30F-93EFE59EAE18",
                "0198BD01-2FD5-74DF-B30F-960C951C711C",
                "0198BD01-2FD5-74DF-B30F-98DA2ABE3095",
                "0198BD01-2FD5-74DF-B30F-9F2447260C32",
                "0198BD01-2FD5-74DF-B30F-A028358E072D",
                "0198BD01-2FD5-74DF-B30F-A443A01CD49E",
                "0198BD01-2FD5-74DF-B30F-A8E9365A6107",
                "0198BD01-2FD5-74DF-B30F-ACB9DB21F229",
                "0198BD01-2FD5-74DF-B30F-B1BF2C0A977C",
                "0198BD01-2FD5-74DF-B30F-B5CAB4D51A88",
                "0198BD01-2FD5-74DF-B30F-B878D47F9D87",
                "0198BD01-2FD5-74DF-B30F-BC67CCD16951",
                "0198BD01-2FD5-74DF-B30F-C305537A1717",
                "0198BD01-2FD5-74DF-B30F-C6E340E50B15",
                "0198BD01-2FD5-74DF-B30F-C9B52F4D32C5",
                "0198BD01-2FD5-74DF-B30F-CC338C02A15D",
                "0198BD01-2FD5-74DF-B30F-D0D5E04CAC56",
                "0198BD01-2FD5-74DF-B30F-D640D68D34E6",
                "0198BD01-2FD5-74DF-B30F-DA0EFB0506EC",
                "0198BD01-2FD5-74DF-B30F-DF300AA592F5",
                "0198BD01-2FD5-74DF-B30F-E0FBD142A47A",
                "0198BD01-2FD5-74DF-B30F-E6D6B7E4177E",
                "0198BD01-2FD5-74DF-B30F-EB4BB6017BA2",
                "0198BD01-2FD5-74DF-B30F-EEBE9A823228",
                "0198BD01-2FD5-74DF-B30F-F328C7CC5017",
                "0198BD01-2FD5-74DF-B30F-F7D1A1290E25",
                "0198BD01-2FD5-74DF-B30F-FBAE9534C250",
                "0198BD01-2FD5-74DF-B30F-FDC3761530D7",
                "0198BD01-2FD5-74DF-B310-033D694CAF70",
                "0198BD01-2FD5-74DF-B310-066B6C4E6466",
                "0198BD01-2FD5-74DF-B310-0BAAD5DFCE5C"
            ];
            List<string> equipUuids = [
                "0198BBB31BD8703DB5F1E0C020732F65",
                "0198BBB31BD8703DB5F1E4E8A88DB407",
                "0198BBB31BD8703DB5F1E897C90C0239",
                "0198BBB31BD8703DB5F1EE5D070B679C",
                "0198BBB31BD8703DB5F1F3779080DB15",
                "0198BBB31BD8703DB5F1F77B878A4DF8",
                "0198BBB31BD8703DB5F1FACCBF289672",
                "0198BBB31BD8703DB5F1FFA10AF7257B",
                "0198BBB31BD8703DB5F20174F915DEC4",
                "0198BBB31BD8703DB5F20527AF9CCFC5"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                list.Add(new XT_108_SL_7_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 5,
                    DevTypeld = 7,
                    Compld = equipUuids[rand.Next(equipUuids.Count - 1)],
                    OpenOrClose = 1,
                    Reserved = 1,
                    StatusType = 1,
                    DewPointTemperatureStatus = 1,
                    VacuumStatus = 1,
                    ChillerFlowStatus = 1,
                    EnvironmentalChamberTemperatureStatus = 1,
                    SubstrateTemperatureStatus = 1,
                    PowerSupplyStatus = 1,
                    ControlPowerStatus = 1,
                    PhysicalQuantityCount = 14,
                    Vacuum = (float)(1 * Math.Pow(10, -7)),
                    ChamberTemperature = (float)(rand.NextDouble() * 4 + 23),
                    SubstrateTemp = (float)(rand.NextDouble() * 40 + 160),
                    AmbientTemp = (float)(rand.NextDouble() * 20 + 15),
                    Humidity = (float)(rand.NextDouble() * 20 + 40),
                    DewPoint = (float)((rand.NextDouble() - 0.5) * 20),
                    PowerVoltage = 12f,
                    PowerCurrent = 4.5f,
                    CtrlVoltage = 12,
                    CtrlCurrent = (float)(rand.NextDouble() * 1.9 + 0.1),
                    LogicVoltage = 1.2f,
                    LogicCurrent = (float)rand.NextDouble(),
                    LiquidTemp = (float)(rand.NextDouble() * 10 + 15),
                    CoolantFlow = (float)(rand.NextDouble() * 2 + 1),
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

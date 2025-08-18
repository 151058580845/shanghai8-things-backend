using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_109_ReceiveDatas;

/// <summary>
/// 109_红外源
/// </summary>
public class XT_109_SL_7_ReceiveData : UniversalEntity, IAudited
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

    #region 健康状态信息

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
    public static XT_109_SL_7_ReceiveData[] Seeds
    {
        get
        {
            List<XT_109_SL_7_ReceiveData> list = [];
            List<string> uuids = [
                "0198BD03-A7ED-72FA-B07A-7365EDD74A58",
                "0198BD03-A7ED-72FA-B07A-766486790168",
                "0198BD03-A7ED-72FA-B07A-7B8FC55D8FC3",
                "0198BD03-A7ED-72FA-B07A-7C4B6C1D1542",
                "0198BD03-A7ED-72FA-B07A-81CF58D2701A",
                "0198BD03-A7ED-72FA-B07A-87936833E373",
                "0198BD03-A7ED-72FA-B07A-88F176AE429A",
                "0198BD03-A7ED-72FA-B07A-8C3CDA3DDCA2",
                "0198BD03-A7ED-72FA-B07A-93966397BAA5",
                "0198BD03-A7ED-72FA-B07A-97FAFF4C5E36",
                "0198BD03-A7ED-72FA-B07A-984656FA8E18",
                "0198BD03-A7ED-72FA-B07A-9CFD3A31DE75",
                "0198BD03-A7ED-72FA-B07A-A31AAE845858",
                "0198BD03-A7ED-72FA-B07A-A6C05F006A24",
                "0198BD03-A7ED-72FA-B07A-A9DF4E762E28",
                "0198BD03-A7ED-72FA-B07A-AD8046BC5568",
                "0198BD03-A7ED-72FA-B07A-B3F95182EEE8",
                "0198BD03-A7ED-72FA-B07A-B4E4F342B061",
                "0198BD03-A7ED-72FA-B07A-B9633C10FECB",
                "0198BD03-A7ED-72FA-B07A-BEF09755BC7D",
                "0198BD03-A7ED-72FA-B07A-C0757C055A18",
                "0198BD03-A7ED-72FA-B07A-C506DA07E94F",
                "0198BD03-A7ED-72FA-B07A-CA15B255F7CB",
                "0198BD03-A7ED-72FA-B07A-CD49289CB2EE",
                "0198BD03-A7ED-72FA-B07A-D067008429A1",
                "0198BD03-A7ED-72FA-B07A-D6C1AAC40A5F",
                "0198BD03-A7ED-72FA-B07A-D997C2BA3FBD",
                "0198BD03-A7ED-72FA-B07A-DCA1FAEC5920",
                "0198BD03-A7ED-72FA-B07A-E2993CCA19D2",
                "0198BD03-A7ED-72FA-B07A-E5DC7C52EADC",
                "0198BD03-A7ED-72FA-B07A-E97C2FFBF458",
                "0198BD03-A7ED-72FA-B07A-EFA932631350",
                "0198BD03-A7ED-72FA-B07A-F32CB9F98EF0",
                "0198BD03-A7ED-72FA-B07A-F6D363DEADAF",
                "0198BD03-A7ED-72FA-B07A-FA9CF20D21A4",
                "0198BD03-A7ED-72FA-B07A-FE2124BA9B59",
                "0198BD03-A7ED-72FA-B07B-02DB0BF411BA",
                "0198BD03-A7ED-72FA-B07B-045DCEC95024",
                "0198BD03-A7ED-72FA-B07B-0AB11D803044",
                "0198BD03-A7ED-72FA-B07B-0D1BF86ACFA4",
                "0198BD03-A7ED-72FA-B07B-12FFDDF2DB0C",
                "0198BD03-A7ED-72FA-B07B-142D5BD92F08",
                "0198BD03-A7ED-72FA-B07B-1B6B67ADD4D7",
                "0198BD03-A7ED-72FA-B07B-1C66606D660A",
                "0198BD03-A7ED-72FA-B07B-23782E5F253E",
                "0198BD03-A7ED-72FA-B07B-243DCC48E3EF",
                "0198BD03-A7ED-72FA-B07B-2918DBDCFFD8",
                "0198BD03-A7ED-72FA-B07B-2C4507764598",
                "0198BD03-A7ED-72FA-B07B-314D13C327A2",
                "0198BD03-A7ED-72FA-B07B-36E6A3F4004E",
                "0198BD03-A7ED-72FA-B07B-3A8078328290",
                "0198BD03-A7ED-72FA-B07B-3E5CA1DC24D5",
                "0198BD03-A7ED-72FA-B07B-43F8CB514CE4",
                "0198BD03-A7ED-72FA-B07B-447EF8D4C0FE",
                "0198BD03-A7ED-72FA-B07B-4ADD71387C5B",
                "0198BD03-A7ED-72FA-B07B-4C97AFEB0A5A",
                "0198BD03-A7ED-72FA-B07B-51F28C2F6BCB",
                "0198BD03-A7ED-72FA-B07B-54F268F45F9A",
                "0198BD03-A7ED-72FA-B07B-5A37DC45F05A",
                "0198BD03-A7ED-72FA-B07B-5FD68C113D93",
                "0198BD03-A7ED-72FA-B07B-63BD8004D61B",
                "0198BD03-A7ED-72FA-B07B-661650B9BF6F",
                "0198BD03-A7ED-72FA-B07B-6BFF8BF5FA3E",
                "0198BD03-A7ED-72FA-B07B-6DF47A027434",
                "0198BD03-A7ED-72FA-B07B-73DFE1FACF93",
                "0198BD03-A7ED-72FA-B07B-76AB45CE96D1",
                "0198BD03-A7ED-72FA-B07B-7BCF75F5BEC8",
                "0198BD03-A7ED-72FA-B07B-7FC790B94094",
                "0198BD03-A7ED-72FA-B07B-83C413FF1468",
                "0198BD03-A7ED-72FA-B07B-85FE4A587505",
                "0198BD03-A7ED-72FA-B07B-8A1658354F51",
                "0198BD03-A7ED-72FA-B07B-8F122B38CDE1",
                "0198BD03-A7ED-72FA-B07B-91E2B011E214",
                "0198BD03-A7ED-72FA-B07B-96539E6D2AB0",
                "0198BD03-A7ED-72FA-B07B-9A9A929937C1",
                "0198BD03-A7ED-72FA-B07B-9CC03D7A3DC7",
                "0198BD03-A7ED-72FA-B07B-A346C41CD812",
                "0198BD03-A7ED-72FA-B07B-A727B050DEB2",
                "0198BD03-A7ED-72FA-B07B-AAD1C365053B",
                "0198BD03-A7ED-72FA-B07B-AE2A279429D7",
                "0198BD03-A7ED-72FA-B07B-B3E22A2544EB",
                "0198BD03-A7ED-72FA-B07B-B5CEEE4E83C1",
                "0198BD03-A7ED-72FA-B07B-B9A7DFD4AD58",
                "0198BD03-A7ED-72FA-B07B-BC65F2A00071",
                "0198BD03-A7ED-72FA-B07B-C247487B18C8",
                "0198BD03-A7ED-72FA-B07B-C69E5F2192C5",
                "0198BD03-A7ED-72FA-B07B-C9C1FE15233E",
                "0198BD03-A7ED-72FA-B07B-CE9805CCC157",
                "0198BD03-A7ED-72FA-B07B-D203AA42941A",
                "0198BD03-A7ED-72FA-B07B-D67B642BF9E9",
                "0198BD03-A7ED-72FA-B07B-D9337CAA7A93",
                "0198BD03-A7ED-72FA-B07B-DD97E1838FBE",
                "0198BD03-A7ED-72FA-B07B-E22177DD3180",
                "0198BD03-A7ED-72FA-B07B-E6B75D9DD61F",
                "0198BD03-A7ED-72FA-B07B-E9858FA6514E",
                "0198BD03-A7ED-72FA-B07B-ED00C00B4800",
                "0198BD03-A7ED-72FA-B07B-F03BC932753E",
                "0198BD03-A7ED-72FA-B07B-F46D8C9C43B9",
                "0198BD03-A7ED-72FA-B07B-F9A3D53B7252",
                "0198BD03-A7ED-72FA-B07B-FC6945105B16"
            ];
            List<string> equipUuids = [
                "0198BBB31BD8703DB5F16BC27BC34F7B",
                "0198BBB31BD8703DB5F16F17F28622F8",
                "0198BBB31BD8703DB5F170B4E664A66A",
                "0198BBB31BD8703DB5F1744DCD0CFBA0",
                "0198BBB31BD8703DB5F17869B54162DD",
                "0198BBB31BD8703DB5F17E4CBEEB5ED6",
                "0198BBB31BD8703DB5F181493C6C77E8",
                "0198BBB31BD8703DB5F1842E18327954",
                "0198BBB31BD8703DB5F18A253733A839",
                "0198BBB31BD8703DB5F18EB450AF8400"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                list.Add(new XT_109_SL_7_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 4,
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

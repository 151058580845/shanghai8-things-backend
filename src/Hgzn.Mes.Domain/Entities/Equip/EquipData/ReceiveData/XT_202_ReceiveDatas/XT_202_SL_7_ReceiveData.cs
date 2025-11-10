using System.ComponentModel;
using Hgzn.Mes.Domain.Attributes;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_202_ReceiveDatas;

/// <summary>
/// _红外源
/// </summary>
public class XT_202_SL_7_ReceiveData : UniversalEntity, IAudited
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

    [Description("开启/关闭")]
    [TableNotShow]
    public byte OpenOrClose { get; set; }

    [Description("预留")]
    [TableNotShow]
    public byte Reserved { get; set; }

    #endregion

    #region 健康状态信息

    [Description("状态类型")]
    [TableNotShow]
    public byte StatusType { get; set; }

    [Description("露点温度状态")]
    [TableNotShow]
    public byte DewPointTemperatureStatus { get; set; }

    [Description("真空度状态")]
    [TableNotShow]
    public byte VacuumStatus { get; set; }

    [Description("冷水机流量状态")]
    [TableNotShow]
    public byte ChillerFlowStatus { get; set; }

    [Description("环境箱温度状态")]
    [TableNotShow]
    public byte EnvironmentalChamberTemperatureStatus { get; set; }

    [Description("衬底温度状态")]
    [TableNotShow]
    public byte SubstrateTemperatureStatus { get; set; }

    [Description("功率电源状态")]
    [TableNotShow]
    public byte PowerSupplyStatus { get; set; }

    [Description("控制电源状态")]
    [TableNotShow]
    public byte ControlPowerStatus { get; set; }

    [Description("逻辑电源状态")]
    [TableNotShow]
    public byte LogicPowerStatus { get; set; }

    #endregion

    #region 物理量

    [Description("物理量参数数量")]
    [TableNotShow]
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
    [TableNotShow]
    public uint? RunTime { get; set; }

    [TableNotShow]
    public Guid? LastModifierId { get; set; }
    [TableNotShow]
    public DateTime? LastModificationTime { get; set; }
    [TableNotShow]
    public int CreatorLevel { get; set; } = 0;

#if DEBUG
    public static XT_202_SL_7_ReceiveData[] Seeds
    {
        get
        {
            List<XT_202_SL_7_ReceiveData> list = [];
            List<string> uuids = [
                "0198BD08-5804-73CE-ABD7-665BE8946C73",
                "0198BD08-5804-73CE-ABD7-6BE8690E4BD4",
                "0198BD08-5804-73CE-ABD7-6C22A202ECA6",
                "0198BD08-5804-73CE-ABD7-720A6527F00A",
                "0198BD08-5804-73CE-ABD7-774695D2EF22",
                "0198BD08-5804-73CE-ABD7-78AC9C5B68EA",
                "0198BD08-5804-73CE-ABD7-7C0404D8ABDE",
                "0198BD08-5804-73CE-ABD7-83A9F94F9894",
                "0198BD08-5804-73CE-ABD7-85D90D9056B7",
                "0198BD08-5804-73CE-ABD7-8B3CA746B4C4",
                "0198BD08-5804-73CE-ABD7-8D3769591275",
                "0198BD08-5804-73CE-ABD7-935E2BFE847A",
                "0198BD08-5804-73CE-ABD7-9473D6BD496D",
                "0198BD08-5804-73CE-ABD7-9A81AFAAFAAD",
                "0198BD08-5804-73CE-ABD7-9CD10B4A8745",
                "0198BD08-5804-73CE-ABD7-A3B727C43314",
                "0198BD08-5804-73CE-ABD7-A6EE9EF321FC",
                "0198BD08-5804-73CE-ABD7-AAA03340E9A9",
                "0198BD08-5804-73CE-ABD7-AC944560C9B5",
                "0198BD08-5804-73CE-ABD7-B049295165F7",
                "0198BD08-5804-73CE-ABD7-B7A1D3014F88",
                "0198BD08-5804-73CE-ABD7-B8F88A4730E5",
                "0198BD08-5804-73CE-ABD7-BDB53BA3AEF8",
                "0198BD08-5804-73CE-ABD7-C3A908CC17D7",
                "0198BD08-5804-73CE-ABD7-C6A89E42B95C",
                "0198BD08-5804-73CE-ABD7-CB4C62C77F27",
                "0198BD08-5804-73CE-ABD7-CD266BEAD5B3",
                "0198BD08-5804-73CE-ABD7-D31608EF2A8D",
                "0198BD08-5804-73CE-ABD7-D4CB2B823B90",
                "0198BD08-5804-73CE-ABD7-D91E10AD25C6",
                "0198BD08-5804-73CE-ABD7-DDD02D1727FE",
                "0198BD08-5804-73CE-ABD7-E1643C25CE78",
                "0198BD08-5804-73CE-ABD7-E5AEC6E4F9EA",
                "0198BD08-5804-73CE-ABD7-E9DF2B02D2E7",
                "0198BD08-5804-73CE-ABD7-EC1BA480A87C",
                "0198BD08-5804-73CE-ABD7-F23093056E9D",
                "0198BD08-5804-73CE-ABD7-F434A6D8EA64",
                "0198BD08-5804-73CE-ABD7-F8CBE54D395E",
                "0198BD08-5804-73CE-ABD7-FCDBA5A9F596",
                "0198BD08-5804-73CE-ABD8-037A8963AB80",
                "0198BD08-5804-73CE-ABD8-0764F4D3ACA3",
                "0198BD08-5804-73CE-ABD8-0AE5CED14E83",
                "0198BD08-5804-73CE-ABD8-0DA06653A242",
                "0198BD08-5804-73CE-ABD8-10526AB62FFC",
                "0198BD08-5804-73CE-ABD8-16E339C82E48",
                "0198BD08-5804-73CE-ABD8-18AEA5EAE10C",
                "0198BD08-5804-73CE-ABD8-1F6A40BAB8B6",
                "0198BD08-5804-73CE-ABD8-2053C0C6338A",
                "0198BD08-5804-73CE-ABD8-27EE10F6615D",
                "0198BD08-5804-73CE-ABD8-28723F8CA52F",
                "0198BD08-5804-73CE-ABD8-2CAAEFEFEE60",
                "0198BD08-5804-73CE-ABD8-324D800F266C",
                "0198BD08-5804-73CE-ABD8-34ADA5E4C6CF",
                "0198BD08-5804-73CE-ABD8-393FBCE6CDF3",
                "0198BD08-5804-73CE-ABD8-3ECE69B0E803",
                "0198BD08-5804-73CE-ABD8-4073667B1427",
                "0198BD08-5804-73CE-ABD8-45FAB1B68774",
                "0198BD08-5804-73CE-ABD8-4A279E7FB80B",
                "0198BD08-5804-73CE-ABD8-4E21E930A323",
                "0198BD08-5804-73CE-ABD8-50DFE832F462",
                "0198BD08-5804-73CE-ABD8-5709F1B96C8E",
                "0198BD08-5804-73CE-ABD8-5B93DFB2B900",
                "0198BD08-5804-73CE-ABD8-5E43AD4FA1A1",
                "0198BD08-5804-73CE-ABD8-629A1559F785",
                "0198BD08-5804-73CE-ABD8-6407EF04CFA2",
                "0198BD08-5804-73CE-ABD8-69D7BF73308B",
                "0198BD08-5804-73CE-ABD8-6F5C73DF61D7",
                "0198BD08-5804-73CE-ABD8-72D8D5E136B3",
                "0198BD08-5805-703F-A4C4-C5E1BF0FEC96",
                "0198BD08-5805-703F-A4C4-C8662F6D01AD",
                "0198BD08-5805-703F-A4C4-CEA1CBBB6C5F",
                "0198BD08-5805-703F-A4C4-D2C60948A177",
                "0198BD08-5805-703F-A4C4-D78179486A76",
                "0198BD08-5805-703F-A4C4-D8861617DC99",
                "0198BD08-5805-703F-A4C4-DC85BFD23A69",
                "0198BD08-5805-703F-A4C4-E2684D409505",
                "0198BD08-5805-703F-A4C4-E7AF13B79578",
                "0198BD08-5805-703F-A4C4-EBAD68268703",
                "0198BD08-5805-703F-A4C4-ED1AA4F3D314",
                "0198BD08-5805-703F-A4C4-F38E73D7CE27",
                "0198BD08-5805-703F-A4C4-F5A285EEFC9E",
                "0198BD08-5805-703F-A4C4-F8935248CD65",
                "0198BD08-5805-703F-A4C4-FED66B54E331",
                "0198BD08-5805-703F-A4C5-00E8E239CDE5",
                "0198BD08-5805-703F-A4C5-04A8359E79E4",
                "0198BD08-5805-703F-A4C5-0B9C3B5451E3",
                "0198BD08-5805-703F-A4C5-0C1E2575D1CD",
                "0198BD08-5805-703F-A4C5-10100B5F78A4",
                "0198BD08-5805-703F-A4C5-16CDA34539C0",
                "0198BD08-5805-703F-A4C5-1929536E96EE",
                "0198BD08-5805-703F-A4C5-1E88382AF74B",
                "0198BD08-5805-703F-A4C5-20513B986D69",
                "0198BD08-5805-703F-A4C5-257DBDFFF4ED",
                "0198BD08-5805-703F-A4C5-2B87909667FE",
                "0198BD08-5805-703F-A4C5-2FCF30A0EAEE",
                "0198BD08-5805-703F-A4C5-30DC91ADE9E0",
                "0198BD08-5805-703F-A4C5-34B726483CBF",
                "0198BD08-5805-703F-A4C5-39766A65C187",
                "0198BD08-5805-703F-A4C5-3DA5707CC774",
                "0198BD08-5805-703F-A4C5-4036A5028BCB"
            ];
            List<string> equipUuids = [
                "0198BBB3A649726DBD9E9AB79DBB7294",
                "0198BBB3A649726DBD9E9DA5B6DDFBAC",
                "0198BBB3A649726DBD9EA1470CD1C96F",
                "0198BBB3A649726DBD9EA60121B0F04A",
                "0198BBB3A649726DBD9EA940D04D20D0",
                "0198BBB3A649726DBD9EAEB5DE45695A",
                "0198BBB3A649726DBD9EB3B738EA3876",
                "0198BBB3A649726DBD9EB7A50C5307F5",
                "0198BBB3A649726DBD9EBB8CEC658372",
                "0198BBB3A649726DBD9EBFCC731E07F0"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                list.Add(new XT_202_SL_7_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 7,
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

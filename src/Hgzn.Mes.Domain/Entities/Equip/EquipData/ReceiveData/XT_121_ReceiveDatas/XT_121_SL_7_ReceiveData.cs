using System.ComponentModel;
using Hgzn.Mes.Domain.Attributes;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_121_ReceiveDatas;

/// <summary>
/// _红外源
/// </summary>
public class XT_121_SL_7_ReceiveData : UniversalEntity, IAudited
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

    //[Description("控制电源逻辑电压")]
    //public float LogicVoltage { get; set; }

    //[Description("控制电源逻辑电流")]
    //public float LogicCurrent { get; set; }

    //[Description("液体温度")]
    //public float LiquidTemp { get; set; }

    //[Description("冷却液流量")]
    //public float CoolantFlow { get; set; }

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
    public static XT_121_SL_7_ReceiveData[] Seeds
    {
        get
        {
            List<XT_121_SL_7_ReceiveData> list = [];
            List<string> uuids = [
                "0198BD06-188D-747C-8DDA-4E20C8565536",
                "0198BD06-188D-747C-8DDA-51B9C3A22C5B",
                "0198BD06-188D-747C-8DDA-5694CBB93806",
                "0198BD06-188D-747C-8DDA-5BE774ACCF42",
                "0198BD06-188D-747C-8DDA-5D25572A1707",
                "0198BD06-188D-747C-8DDA-601F738AACB0",
                "0198BD06-188D-747C-8DDA-652663E1F094",
                "0198BD06-188D-747C-8DDA-69D18A32A0C7",
                "0198BD06-188D-747C-8DDA-6D3BF67767F4",
                "0198BD06-188D-747C-8DDA-72CD7A04C929",
                "0198BD06-188D-747C-8DDA-749D524EB578",
                "0198BD06-188D-747C-8DDA-79F21098AE00",
                "0198BD06-188D-747C-8DDA-7DF0739478B6",
                "0198BD06-188D-747C-8DDA-80A09649F42F",
                "0198BD06-188D-747C-8DDA-84837B7E9000",
                "0198BD06-188D-747C-8DDA-8A7C1B42C8E5",
                "0198BD06-188D-747C-8DDA-8C4130CAA898",
                "0198BD06-188D-747C-8DDA-939903F3C54E",
                "0198BD06-188D-747C-8DDA-97D8AF0DA4DA",
                "0198BD06-188D-747C-8DDA-99B1B7B2C24E",
                "0198BD06-188D-747C-8DDA-9E5FC2DC64C5",
                "0198BD06-188D-747C-8DDA-A318DD37584B",
                "0198BD06-188D-747C-8DDA-A640A4BE5E74",
                "0198BD06-188D-747C-8DDA-A90F890E1460",
                "0198BD06-188D-747C-8DDA-AD175EE2526D",
                "0198BD06-188D-747C-8DDA-B13B7ECEBC7A",
                "0198BD06-188D-747C-8DDA-B53A7D16872D",
                "0198BD06-188D-747C-8DDA-BBCBC01E2A20",
                "0198BD06-188D-747C-8DDA-BED50AE58275",
                "0198BD06-188D-747C-8DDA-C085C4F14B7A",
                "0198BD06-188D-747C-8DDA-C5374C2D12BA",
                "0198BD06-188D-747C-8DDA-CB35E4B6E3FF",
                "0198BD06-188D-747C-8DDA-CD126D15679C",
                "0198BD06-188D-747C-8DDA-D32B80479733",
                "0198BD06-188D-747C-8DDA-D6E531150AE3",
                "0198BD06-188D-747C-8DDA-DA61C674D3BF",
                "0198BD06-188D-747C-8DDA-DC98D46CD1C4",
                "0198BD06-188D-747C-8DDA-E34779433CD1",
                "0198BD06-188D-747C-8DDA-E5233862BD7B",
                "0198BD06-188D-747C-8DDA-E8C08343223B",
                "0198BD06-188D-747C-8DDA-EC191DB45D25",
                "0198BD06-188D-747C-8DDA-F0483A694C56",
                "0198BD06-188D-747C-8DDA-F458D39C0AA7",
                "0198BD06-188D-747C-8DDA-F8884E15A50E",
                "0198BD06-188D-747C-8DDA-FEE84B0CE6E4",
                "0198BD06-188D-747C-8DDB-0111A9D2D711",
                "0198BD06-188D-747C-8DDB-0683327919B5",
                "0198BD06-188D-747C-8DDB-080B86302B09",
                "0198BD06-188D-747C-8DDB-0DEA915AA5B6",
                "0198BD06-188D-747C-8DDB-12524BBBAE4C",
                "0198BD06-188D-747C-8DDB-17A492EDC8A7",
                "0198BD06-188D-747C-8DDB-192A72AC225D",
                "0198BD06-188D-747C-8DDB-1F1D164A2D19",
                "0198BD06-188D-747C-8DDB-22CCCF55CAA4",
                "0198BD06-188D-747C-8DDB-246B89592C7F",
                "0198BD06-188D-747C-8DDB-2A709641F650",
                "0198BD06-188D-747C-8DDB-2C960B873B63",
                "0198BD06-188D-747C-8DDB-31D63616EECA",
                "0198BD06-188D-747C-8DDB-34D7A959D859",
                "0198BD06-188D-747C-8DDB-39069A7B15E2",
                "0198BD06-188D-747C-8DDB-3E62388168B3",
                "0198BD06-188D-747C-8DDB-41A6F7395CD3",
                "0198BD06-188D-747C-8DDB-4718BD04657D",
                "0198BD06-188D-747C-8DDB-494A94FEDD1F",
                "0198BD06-188D-747C-8DDB-4E4E357C11A1",
                "0198BD06-188D-747C-8DDB-5396D6F88E22",
                "0198BD06-188D-747C-8DDB-5658A2827838",
                "0198BD06-188D-747C-8DDB-584EC226FD86",
                "0198BD06-188D-747C-8DDB-5C19D9879AC1",
                "0198BD06-188D-747C-8DDB-624A513C0091",
                "0198BD06-188D-747C-8DDB-67BD3E7F5201",
                "0198BD06-188D-747C-8DDB-6B0ED53C1CB2",
                "0198BD06-188D-747C-8DDB-6EE0250D4496",
                "0198BD06-188D-747C-8DDB-7078EDF21E62",
                "0198BD06-188D-747C-8DDB-7429071DE0F4",
                "0198BD06-188D-747C-8DDB-78FB2C10B7E1",
                "0198BD06-188D-747C-8DDB-7E4CA7ADD709",
                "0198BD06-188D-747C-8DDB-814B0C77D3AC",
                "0198BD06-188D-747C-8DDB-85D8D30BAB57",
                "0198BD06-188D-747C-8DDB-88B8BBD11B5E",
                "0198BD06-188D-747C-8DDB-8D027CF5F3CE",
                "0198BD06-188D-747C-8DDB-93D51C3FBB90",
                "0198BD06-188D-747C-8DDB-96E10BCED96B",
                "0198BD06-188D-747C-8DDB-9B0941EF0C07",
                "0198BD06-188D-747C-8DDB-9DD48E87B5B5",
                "0198BD06-188D-747C-8DDB-A2DAF2BBBD4B",
                "0198BD06-188D-747C-8DDB-A411BADD2024",
                "0198BD06-188D-747C-8DDB-AAFCECDAB6F8",
                "0198BD06-188D-747C-8DDB-AE5530E333A4",
                "0198BD06-188D-747C-8DDB-B1ED10BBF81D",
                "0198BD06-188D-747C-8DDB-B54558EC1C6A",
                "0198BD06-188D-747C-8DDB-BB18A59EAD9D",
                "0198BD06-188D-747C-8DDB-BEC90590500D",
                "0198BD06-188D-747C-8DDB-C057D5DD9AC3",
                "0198BD06-188D-747C-8DDB-C6DE9D9FC647",
                "0198BD06-188D-747C-8DDB-CBC543F30AE3",
                "0198BD06-188D-747C-8DDB-CC8EB5359316",
                "0198BD06-188D-747C-8DDB-D0D8119CE58A",
                "0198BD06-188D-747C-8DDB-D72CFB98B5CE",
                "0198BD06-188D-747C-8DDB-DBC8C2791B8C"
            ];
            List<string> equipUuids = [
                "0198BBB31BD8703DB5F2585A8BC6C76B",
                "0198BBB31BD8703DB5F25C9E7983AD28",
                "0198BBB31BD8703DB5F2610B4B44844D",
                "0198BBB31BD8703DB5F265F222C63C8D",
                "0198BBB31BD8703DB5F26AC16DF3759A",
                "0198BBB31BD8703DB5F26F592312F5A9",
                "0198BBB31BD8703DB5F271897C7E85E6",
                "0198BBB31BD8703DB5F2765607DAA598",
                "0198BBB31BD8703DB5F279EBAB440713",
                "0198BBB31BD8703DB5F27D3BFF4EAD1A"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            foreach (var uuid in uuids)
            {
                list.Add(new XT_121_SL_7_ReceiveData
                {
                    Id = Guid.Parse(uuid),
                    SimuTestSysld = 6,
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
                    //LogicVoltage = 1.2f,
                    //LogicCurrent = (float)rand.NextDouble(),
                    //LiquidTemp = (float)(rand.NextDouble() * 10 + 15),
                    //CoolantFlow = (float)(rand.NextDouble() * 2 + 1),
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

using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData;

//唯一索引 (true表示唯一索引 或者叫 唯一约束)
// [SugarIndex("TestEquipData_TestEquip",nameof(TestEquipData.TestEquip),OrderByType.Desc,true)]
[Description("试验系统设备自动增加")]
public class TestEquipData : UniversalEntity
{
    [Description("仿真试验系统识别编码,设备类型识别编码,本机识别编码")]
    public byte[] TestEquip { get; set; } = null!;

    [Description("仿真试验系统识别编码")]
    public byte SimuTestSysld { get; set; }

    [Description("设备类型识别编码")]
    public byte DevTypeld { get; set; }

    [Description("本机识别编码")]
    public string Compld { get; set; }

    #region audit

    public Guid? CreatorId { get; set; }
    public DateTime? CreationTime { get; set; }

    #endregion audit

    /// <summary>
    /// 根据仿真试验系统识别编码获取设备类型识别编码
    /// </summary>
    /// <param name="system"></param>
    /// <returns></returns>
    public static int[] EquipTypeCount(int system) => system switch
    {
        1 or 8 or 9 or 10 => [2, 4],
        2 => [1, 2, 4],
        3 => [1, 2, 3, 4],
        4 or 5 or 6 or 7 => [3, 4, 7],
        _ => [],
    };

    /// <summary>
    /// 根据仿真试验系统识别编码获取系统名
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string GetSystemName(int id) => id switch
    {
        1 => "微波/毫米波复合半实物仿真系统",
        2 => "微波寻的半实物仿真系统",
        3 => "射频/光学制导半实物仿真系统",
        4 => "紧缩场射频光学半实物仿真系统",
        5 => "光学复合半实物仿真系统",
        6 => "三通道控制红外制导半实物仿真系统",
        7 => "低温环境红外制导控制半实物仿真系统",
        8 => "机械式制导控制半实物仿真系统",
        9 => "独立回路半实物仿真系统",
        10 => "独立回路/可见光制导半实物仿真系统",
        _ => ""
    };

    /// <summary>
    /// 根据仿真试验系统名获取协议上的房间
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static Guid GetSystemRoom(string sysName) => sysName switch
    {
        "微波/毫米波复合半实物仿真系统" => new Guid("4c246470-da66-4408-b287-09fd82ffa3d4"),
        "微波寻的半实物仿真系统" => new Guid("83168845-ef46-4aed-9187-de2024488230"),
        "射频/光学制导半实物仿真系统" => new Guid("7412dda2-5413-43ab-9976-255df60c3e14"),
        "紧缩场射频光学半实物仿真系统" => new Guid("09b374e8-4e7f-4146-9fe0-375edc7b9d7a"),
        "光学复合半实物仿真系统" => new Guid("0ac9885d-da23-4c0f-a66c-2ba467b8086c"),
        "三通道控制红外制导半实物仿真系统" => new Guid("24be4856-d95f-4ba0-b2aa-7049fedc3e39"),
        "低温环境红外制导控制半实物仿真系统" => new Guid("916edd0e-df70-4137-806c-41817587e438"),
        "机械式制导控制半实物仿真系统" => new Guid("7d6b322d-bf48-4963-bfe6-579560e84530"),
        "独立回路半实物仿真系统" => new Guid("ddd64e08-5f2a-4578-84cb-2f90caa898e9"),
        "独立回路/可见光制导半实物仿真系统" => new Guid("a6ce46f1-d51f-45c8-a22e-2db3126da6cf"),
        _ => Guid.Empty
    };

    /// <summary>
    /// 根据仿真试验系统识别编码获取房间id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string GetRoomId(int id) => id switch
    {
        1 => "4c246470-da66-4408-b287-09fd82ffa3d4",
        2 => "83168845-ef46-4aed-9187-de2024488230",
        3 => "7412dda2-5413-43ab-9976-255df60c3e14",
        4 => "09b374e8-4e7f-4146-9fe0-375edc7b9d7a",
        5 => "0ac9885d-da23-4c0f-a66c-2ba467b8086c",
        6 => "24be4856-d95f-4ba0-b2aa-7049fedc3e39",
        7 => "00860af7-1e65-4085-ab2f-7af2396fd13d",
        8 => "7d6b322d-bf48-4963-bfe6-579560e84530",
        9 => "ddd64e08-5f2a-4578-84cb-2f90caa898e9",
        10 => "a6ce46f1-d51f-45c8-a22e-2db3126da6cf",
        _ => ""
    };

    /// <summary>
    /// 等价房间,因为8院有些系统跨了两个房间,所以某些房间是等价的关系,通用计量设备等信息 TODO - AG
    /// </summary>
    public static Dictionary<string, List<string>> EquivalentRoom { get; set; } = new Dictionary<string, List<string>>()
    {
        { "4c246470-da66-4408-b287-09fd82ffa3d4",new List<string>{ "4c246470-da66-4408-b287-09fd82ffa3d4", 
            "7bf3052d-2747-4939-bf98-421868ede2f5", "576901c4-ffe3-426c-92f2-99667f958ddc", "d235887e-ac0d-4c9f-ae8e-0d749f751bd8" } }, // 310 - 303,203,306
        { "83168845-ef46-4aed-9187-de2024488230",new List<string>{ "83168845-ef46-4aed-9187-de2024488230", 
            "db697f54-0de4-42d0-9c8a-34db59343e4a", "6c26b99f-d112-465b-919f-758a8b366d59" } }, // 307 - 209,206
        { "7412dda2-5413-43ab-9976-255df60c3e14",new List<string>{ "7412dda2-5413-43ab-9976-255df60c3e14",
            "198f64a9-c9e5-4c71-9b2c-cf7e6f7fdd77", "e184cde4-fe85-4ddb-9bf4-fdc7f33636e1" } }, // 314 - 213,215
        { "09b374e8-4e7f-4146-9fe0-375edc7b9d7a",new List<string>{ "09b374e8-4e7f-4146-9fe0-375edc7b9d7a" } }, // 109
        { "0ac9885d-da23-4c0f-a66c-2ba467b8086c",new List<string>{ "0ac9885d-da23-4c0f-a66c-2ba467b8086c" } }, // 108
        { "24be4856-d95f-4ba0-b2aa-7049fedc3e39",new List<string>{ "24be4856-d95f-4ba0-b2aa-7049fedc3e39", 
          "d902c47b-b178-49ca-8cb2-904a4c08dda6" } }, // 121 - 122
        { "00860af7-1e65-4085-ab2f-7af2396fd13d",new List<string>{ "00860af7-1e65-4085-ab2f-7af2396fd13d", 
          "916edd0e-df70-4137-806c-41817587e438", "a8bd6cc6-29b0-4e96-8eb1-e444df27de2d" } }, // 202-1 - 202-2,111
        { "916edd0e-df70-4137-806c-41817587e438",new List<string>{ "916edd0e-df70-4137-806c-41817587e438" ,
          "00860af7-1e65-4085-ab2f-7af2396fd13d", "a8bd6cc6-29b0-4e96-8eb1-e444df27de2d" } }, // 202-2(因为我也不知道协议的202是202-1还是202-2) - 202-1,111
        { "7d6b322d-bf48-4963-bfe6-579560e84530",new List<string>{ "7d6b322d-bf48-4963-bfe6-579560e84530" } }, // 103
        { "ddd64e08-5f2a-4578-84cb-2f90caa898e9",new List<string>{ "ddd64e08-5f2a-4578-84cb-2f90caa898e9" } }, // 112
        { "a6ce46f1-d51f-45c8-a22e-2db3126da6cf",new List<string>{ "a6ce46f1-d51f-45c8-a22e-2db3126da6cf" } }, // 119
    };

    /// <summary>
    /// 协议上的房间并不一定是温湿度计实际安装的房间,比如310 微波/毫米波复合半实物仿真系统 , 温湿度计就放在306房间
    /// </summary>
    public static Dictionary<Guid, Guid> HygrothermographRoom { get; set; } = new Dictionary<Guid, Guid>()
    {
        { new Guid("4c246470-da66-4408-b287-09fd82ffa3d4"),new Guid("d235887e-ac0d-4c9f-ae8e-0d749f751bd8") }, // 协议310的温湿度计在306
        { new Guid("83168845-ef46-4aed-9187-de2024488230"),new Guid("6c26b99f-d112-465b-919f-758a8b366d59") }, // 协议307的温湿度计在206
        { new Guid("7412dda2-5413-43ab-9976-255df60c3e14"),new Guid("198f64a9-c9e5-4c71-9b2c-cf7e6f7fdd77") }, // 协议314的温湿度计在213
        { new Guid("09b374e8-4e7f-4146-9fe0-375edc7b9d7a"),new Guid("09b374e8-4e7f-4146-9fe0-375edc7b9d7a") }, // 协议109的温湿度计在109
        { new Guid("0ac9885d-da23-4c0f-a66c-2ba467b8086c"),new Guid("0ac9885d-da23-4c0f-a66c-2ba467b8086c") }, // 协议108的温湿度计在108
        { new Guid("24be4856-d95f-4ba0-b2aa-7049fedc3e39"),new Guid("d902c47b-b178-49ca-8cb2-904a4c08dda6") }, // 协议121的温湿度计在122
        { new Guid("00860af7-1e65-4085-ab2f-7af2396fd13d"),new Guid("a8bd6cc6-29b0-4e96-8eb1-e444df27de2d") }, // 协议202-1的温湿度计在111
        { new Guid("916edd0e-df70-4137-806c-41817587e438"),new Guid("a8bd6cc6-29b0-4e96-8eb1-e444df27de2d") }, // 协议202-2的温湿度计在111(因为我也不知道协议的202是202-1还是202-2)
        { new Guid("7d6b322d-bf48-4963-bfe6-579560e84530"),new Guid("7d6b322d-bf48-4963-bfe6-579560e84530") }, // 协议103的温湿度计在103
        { new Guid("ddd64e08-5f2a-4578-84cb-2f90caa898e9"),new Guid("ddd64e08-5f2a-4578-84cb-2f90caa898e9") }, // 协议112的温湿度计在112
        { new Guid("a6ce46f1-d51f-45c8-a22e-2db3126da6cf"),new Guid("a6ce46f1-d51f-45c8-a22e-2db3126da6cf") }, // 协议119的温湿度计在119
    };

    /// <summary>
    /// 根据仿真试验系统识别编码获取房间号
    /// </summary>
    /// <param name="systemId"></param>
    /// <returns></returns>
    public static int GetRoom(int systemId) => systemId switch
    {
        1 => 310,
        2 => 307,
        3 => 314,
        4 => 109,
        5 => 108,
        6 => 121,
        7 => 202,
        8 => 103,
        9 => 112,
        10 => 119,
        _ => 0
    };

    /// <summary>
    /// 设备类型识别编码获取设备类型名称
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string GetEquipTypeName(int id) => id switch
    {
        1 => "阵列馈电",
        2 => "雷达转台",
        3 => "红外转台",
        4 => "固定电源",
        5 => "移动电源",
        6 => "雷达源",
        7 => "红外源",
        8 => "标准源",
        _ => ""
    };

#if DEBUG
    public static TestEquipData[] Seeds
    {
        get
        {
            List<string> uuids = [
                "0198BBB2-7F42-727E-B6F4-5022B4A43D01",
                "0198BBB2-7F42-727E-B6F4-5496B92E75E4",
                "0198BBB2-7F42-727E-B6F4-5B8AE3AC62D9",
                "0198BBB2-7F42-727E-B6F4-5C663EAE3336",
                "0198BBB2-7F42-727E-B6F4-60153153C45C",
                "0198BBB2-7F42-727E-B6F4-65C642145755",
                "0198BBB2-7F42-727E-B6F4-68E883E1A6B4",
                "0198BBB2-7F42-727E-B6F4-6D5550D5CFEE",
                "0198BBB2-7F42-727E-B6F4-735AE47D3773",
                "0198BBB2-7F42-727E-B6F4-76D401A8E56E",
                "0198BBB2-7F42-727E-B6F4-788374221197",
                "0198BBB2-7F42-727E-B6F4-7DC357D544E7",
                "0198BBB2-7F42-727E-B6F4-81D65BA3E6D2",
                "0198BBB2-7F42-727E-B6F4-8762D4BFFD02",
                "0198BBB2-7F42-727E-B6F4-8B2FCEE54C5C",
                "0198BBB2-7F42-727E-B6F4-8D9FDFA083E0",
                "0198BBB2-7F42-727E-B6F4-90FB289681AA",
                "0198BBB2-7F42-727E-B6F4-97C6D956B37F",
                "0198BBB2-7F42-727E-B6F4-99EE535AAFDF",
                "0198BBB2-7F42-727E-B6F4-9C09FE87EC24",
                "0198BBB2-7F42-727E-B6F4-A1AAA67A828B",
                "0198BBB2-7F42-727E-B6F4-A68D9702EFE6",
                "0198BBB2-7F42-727E-B6F4-AB3FBC007942",
                "0198BBB2-7F42-727E-B6F4-ACE6DF594ABC",
                "0198BBB2-7F42-727E-B6F4-B382B5DAB7E5",
                "0198BBB2-7F42-727E-B6F4-B5169125885C",
                "0198BBB2-7F42-727E-B6F4-BB0D2FB237AE",
                "0198BBB2-7F42-727E-B6F4-BC1D86A9055D",
                "0198BBB2-7F42-727E-B6F4-C3CF4DB5D446",
                "0198BBB2-7F42-727E-B6F4-C7C00A3A738E",
                "0198BBB2-7F42-727E-B6F4-CA73BF2C2E67",
                "0198BBB2-7F42-727E-B6F4-CF3960E0347A",
                "0198BBB2-7F42-727E-B6F4-D39596158FB4",
                "0198BBB2-7F42-727E-B6F4-D67DA0E89319",
                "0198BBB2-7F42-727E-B6F4-D805950EC7B9",
                "0198BBB2-7F42-727E-B6F4-DCA4D3D7E937",
                "0198BBB2-7F42-727E-B6F4-E0D22136C705",
                "0198BBB2-7F42-727E-B6F4-E67DC4FE3A30",
                "0198BBB2-7F42-727E-B6F4-EBC1339F1CF0",
                "0198BBB2-7F42-727E-B6F4-EC9D3742AA3F",
                "0198BBB2-7F42-727E-B6F4-F0047E330F68",
                "0198BBB2-7F42-727E-B6F4-F7DE4B5A4DB8",
                "0198BBB2-7F42-727E-B6F4-FAE2D191CAB8",
                "0198BBB2-7F42-727E-B6F4-FF6D958A5153",
                "0198BBB2-7F42-727E-B6F5-00E959E747AB",
                "0198BBB2-7F42-727E-B6F5-07FBBAD32384",
                "0198BBB2-7F42-727E-B6F5-0B6D25D90999",
                "0198BBB2-7F42-727E-B6F5-0D8555C0EAAB",
                "0198BBB2-7F42-727E-B6F5-13823C5D61D3",
                "0198BBB2-7F42-727E-B6F5-1702BBC8C010",
                "0198BBB2-7F42-727E-B6F5-180CA97F4987",
                "0198BBB2-7F42-727E-B6F5-1E4F2BF67E0D",
                "0198BBB2-7F42-727E-B6F5-2282456494D4",
                "0198BBB2-7F42-727E-B6F5-264FA2A3F8C6",
                "0198BBB2-7F42-727E-B6F5-285E94B1E1A4",
                "0198BBB2-7F42-727E-B6F5-2D9D7CB085D1",
                "0198BBB2-7F42-727E-B6F5-308190A269EE",
                "0198BBB2-7F42-727E-B6F5-377B83C914D0",
                "0198BBB2-7F42-727E-B6F5-3A51433E3856",
                "0198BBB2-7F42-727E-B6F5-3DFB78DD0054",
                "0198BBB2-7F42-727E-B6F5-4125464DB218",
                "0198BBB2-7F42-727E-B6F5-4782B83D3997",
                "0198BBB2-7F42-727E-B6F5-49178A649FAC",
                "0198BBB2-7F42-727E-B6F5-4EBBB74BDDB3",
                "0198BBB2-7F42-727E-B6F5-5162661A3560",
                "0198BBB2-7F42-727E-B6F5-5619E0EDB0BD",
                "0198BBB2-7F42-727E-B6F5-5A604A1BF332",
                "0198BBB2-7F42-727E-B6F5-5ED8F1193DE4",
                "0198BBB2-7F42-727E-B6F5-62109B8AAA44",
                "0198BBB2-7F42-727E-B6F5-65F338DB8A9C",
                "0198BBB2-7F42-727E-B6F5-6BAEE80704D0",
                "0198BBB2-7F42-727E-B6F5-6EC65C7170A8",
                "0198BBB2-7F42-727E-B6F5-7106638AE14A",
                "0198BBB2-7F42-727E-B6F5-74F53363C1E5",
                "0198BBB2-7F42-727E-B6F5-7A8CE459EA21",
                "0198BBB2-7F42-727E-B6F5-7FD701A727D7",
                "0198BBB2-7F42-727E-B6F5-8167FCEABD52",
                "0198BBB2-7F42-727E-B6F5-8600DC3A1EAA",
                "0198BBB2-7F42-727E-B6F5-89FE4A56747C",
                "0198BBB2-7F43-750A-AA63-3ACE96ECC733",
                "0198BBB2-7F43-750A-AA63-3CF1174268A1",
                "0198BBB2-7F43-750A-AA63-40FCBBF5BF33",
                "0198BBB2-7F43-750A-AA63-45770BCBF93C",
                "0198BBB2-7F43-750A-AA63-4906946E6EBC",
                "0198BBB2-7F43-750A-AA63-4EF5A2FC50DB",
                "0198BBB2-7F43-750A-AA63-52613FB16651",
                "0198BBB2-7F43-750A-AA63-5535C0A5BEE0",
                "0198BBB2-7F43-750A-AA63-584C9C88C667",
                "0198BBB2-7F43-750A-AA63-5E98280D5010",
                "0198BBB2-7F43-750A-AA63-6327E0C8B831",
                "0198BBB2-7F43-750A-AA63-66EB51DBD90E",
                "0198BBB2-7F43-750A-AA63-68681DED616B",
                "0198BBB2-7F43-750A-AA63-6E27CA6D1B2F",
                "0198BBB2-7F43-750A-AA63-7162B3FA1A3A",
                "0198BBB2-7F43-750A-AA63-756026BBB238",
                "0198BBB2-7F43-750A-AA63-783107083E6D",
                "0198BBB2-7F43-750A-AA63-7D9D168FE765",
                "0198BBB2-7F43-750A-AA63-81B8D2033FD8",
                "0198BBB2-7F43-750A-AA63-847A9A146AF9",
                "0198BBB2-7F43-750A-AA63-8A9490329F2F",
                "0198BBB3-1BD8-703D-B5F1-40CDA96D2681",
                "0198BBB3-1BD8-703D-B5F1-45AC3BC2EFEB",
                "0198BBB3-1BD8-703D-B5F1-4A56651930F7",
                "0198BBB3-1BD8-703D-B5F1-4C9FA080359F",
                "0198BBB3-1BD8-703D-B5F1-525B56FFA57B",
                "0198BBB3-1BD8-703D-B5F1-54F27879A72F",
                "0198BBB3-1BD8-703D-B5F1-5AE4E5AB2A3E",
                "0198BBB3-1BD8-703D-B5F1-5D3EDF854943",
                "0198BBB3-1BD8-703D-B5F1-60EE1442291B",
                "0198BBB3-1BD8-703D-B5F1-674210F49C0C",
                "0198BBB3-1BD8-703D-B5F1-6BC27BC34F7B",
                "0198BBB3-1BD8-703D-B5F1-6F17F28622F8",
                "0198BBB3-1BD8-703D-B5F1-70B4E664A66A",
                "0198BBB3-1BD8-703D-B5F1-744DCD0CFBA0",
                "0198BBB3-1BD8-703D-B5F1-7869B54162DD",
                "0198BBB3-1BD8-703D-B5F1-7E4CBEEB5ED6",
                "0198BBB3-1BD8-703D-B5F1-81493C6C77E8",
                "0198BBB3-1BD8-703D-B5F1-842E18327954",
                "0198BBB3-1BD8-703D-B5F1-8A253733A839",
                "0198BBB3-1BD8-703D-B5F1-8EB450AF8400",
                "0198BBB3-1BD8-703D-B5F1-927F5C308F3C",
                "0198BBB3-1BD8-703D-B5F1-97F6D4771AA8",
                "0198BBB3-1BD8-703D-B5F1-9AFFFC0126AC",
                "0198BBB3-1BD8-703D-B5F1-9E13F3D3EE86",
                "0198BBB3-1BD8-703D-B5F1-A147B34710A9",
                "0198BBB3-1BD8-703D-B5F1-A5A5985C568E",
                "0198BBB3-1BD8-703D-B5F1-AB8D9971EC11",
                "0198BBB3-1BD8-703D-B5F1-AFB3CECC104A",
                "0198BBB3-1BD8-703D-B5F1-B3B8C76E21AB",
                "0198BBB3-1BD8-703D-B5F1-B6278D59F351",
                "0198BBB3-1BD8-703D-B5F1-BAD5F8718271",
                "0198BBB3-1BD8-703D-B5F1-BC1ACD1530D4",
                "0198BBB3-1BD8-703D-B5F1-C09E4BCA0723",
                "0198BBB3-1BD8-703D-B5F1-C40405EC3F8B",
                "0198BBB3-1BD8-703D-B5F1-C9783CF855AB",
                "0198BBB3-1BD8-703D-B5F1-CC7CBE752AC7",
                "0198BBB3-1BD8-703D-B5F1-D184928F07A9",
                "0198BBB3-1BD8-703D-B5F1-D49D9613D275",
                "0198BBB3-1BD8-703D-B5F1-D8B086079C62",
                "0198BBB3-1BD8-703D-B5F1-DF1271AFFE9F",
                "0198BBB3-1BD8-703D-B5F1-E0C020732F65",
                "0198BBB3-1BD8-703D-B5F1-E4E8A88DB407",
                "0198BBB3-1BD8-703D-B5F1-E897C90C0239",
                "0198BBB3-1BD8-703D-B5F1-EE5D070B679C",
                "0198BBB3-1BD8-703D-B5F1-F3779080DB15",
                "0198BBB3-1BD8-703D-B5F1-F77B878A4DF8",
                "0198BBB3-1BD8-703D-B5F1-FACCBF289672",
                "0198BBB3-1BD8-703D-B5F1-FFA10AF7257B",
                "0198BBB3-1BD8-703D-B5F2-0174F915DEC4",
                "0198BBB3-1BD8-703D-B5F2-0527AF9CCFC5",
                "0198BBB3-1BD8-703D-B5F2-08485E16C26F",
                "0198BBB3-1BD8-703D-B5F2-0F284C5C5BED",
                "0198BBB3-1BD8-703D-B5F2-11EDFCA68D4C",
                "0198BBB3-1BD8-703D-B5F2-16624839DEC8",
                "0198BBB3-1BD8-703D-B5F2-1B5BAC1F28E2",
                "0198BBB3-1BD8-703D-B5F2-1E7F3FC15D8C",
                "0198BBB3-1BD8-703D-B5F2-20FA9B7DFD14",
                "0198BBB3-1BD8-703D-B5F2-271CBA89C90A",
                "0198BBB3-1BD8-703D-B5F2-2A7CD7D297D4",
                "0198BBB3-1BD8-703D-B5F2-2F0F28645B77",
                "0198BBB3-1BD8-703D-B5F2-333F118B53AF",
                "0198BBB3-1BD8-703D-B5F2-3573EB877E31",
                "0198BBB3-1BD8-703D-B5F2-38957691B95E",
                "0198BBB3-1BD8-703D-B5F2-3EF5DB55E133",
                "0198BBB3-1BD8-703D-B5F2-40FD05ECDA44",
                "0198BBB3-1BD8-703D-B5F2-47845464371E",
                "0198BBB3-1BD8-703D-B5F2-491013B5CE56",
                "0198BBB3-1BD8-703D-B5F2-4F0297061DDB",
                "0198BBB3-1BD8-703D-B5F2-50EF779D32C5",
                "0198BBB3-1BD8-703D-B5F2-546BBEF11B11",
                "0198BBB3-1BD8-703D-B5F2-585A8BC6C76B",
                "0198BBB3-1BD8-703D-B5F2-5C9E7983AD28",
                "0198BBB3-1BD8-703D-B5F2-610B4B44844D",
                "0198BBB3-1BD8-703D-B5F2-65F222C63C8D",
                "0198BBB3-1BD8-703D-B5F2-6AC16DF3759A",
                "0198BBB3-1BD8-703D-B5F2-6F592312F5A9",
                "0198BBB3-1BD8-703D-B5F2-71897C7E85E6",
                "0198BBB3-1BD8-703D-B5F2-765607DAA598",
                "0198BBB3-1BD8-703D-B5F2-79EBAB440713",
                "0198BBB3-1BD8-703D-B5F2-7D3BFF4EAD1A",
                "0198BBB3-1BD8-703D-B5F2-828621041CDB",
                "0198BBB3-1BD8-703D-B5F2-85386CA0D878",
                "0198BBB3-1BD8-703D-B5F2-88AF65B2F23D",
                "0198BBB3-1BD8-703D-B5F2-8C8B76FC1E95",
                "0198BBB3-1BD8-703D-B5F2-91CD663CF052",
                "0198BBB3-1BD8-703D-B5F2-9655E948778A",
                "0198BBB3-1BD9-716D-A5EB-00A448257464",
                "0198BBB3-1BD9-716D-A5EB-070A691D2E13",
                "0198BBB3-1BD9-716D-A5EB-0BE00D3731AB",
                "0198BBB3-1BD9-716D-A5EB-0EEAA93E06E1",
                "0198BBB3-1BD9-716D-A5EB-13DAA096CC71",
                "0198BBB3-1BD9-716D-A5EB-14AAE025B57C",
                "0198BBB3-1BD9-716D-A5EB-193D88695540",
                "0198BBB3-1BD9-716D-A5EB-1D8342BFB54D",
                "0198BBB3-1BD9-716D-A5EB-20668727126C",
                "0198BBB3-1BD9-716D-A5EB-243AEF0C303A",
                "0198BBB3-1BD9-716D-A5EB-29B3DAB51D56",
                "0198BBB3-1BD9-716D-A5EB-2D6C8ABF20E7",
                "0198BBB3-1BD9-716D-A5EB-31717F99D843",
                "0198BBB3-1BD9-716D-A5EB-37B4655A79FE",
                "0198BBB3-A649-726D-BD9E-9AB79DBB7294",
                "0198BBB3-A649-726D-BD9E-9DA5B6DDFBAC",
                "0198BBB3-A649-726D-BD9E-A1470CD1C96F",
                "0198BBB3-A649-726D-BD9E-A60121B0F04A",
                "0198BBB3-A649-726D-BD9E-A940D04D20D0",
                "0198BBB3-A649-726D-BD9E-AEB5DE45695A",
                "0198BBB3-A649-726D-BD9E-B3B738EA3876",
                "0198BBB3-A649-726D-BD9E-B7A50C5307F5",
                "0198BBB3-A649-726D-BD9E-BB8CEC658372",
                "0198BBB3-A649-726D-BD9E-BFCC731E07F0",
                "0198BBB3-A649-726D-BD9E-C0AD28E9E41E",
                "0198BBB3-A649-726D-BD9E-C7ABC009B742",
                "0198BBB3-A649-726D-BD9E-CA9DFCC610D9",
                "0198BBB3-A649-726D-BD9E-CE060E697007",
                "0198BBB3-A649-726D-BD9E-D1946AD220CC",
                "0198BBB3-A649-726D-BD9E-D497D886BB4D",
                "0198BBB3-A649-726D-BD9E-DBC4AE90A46B",
                "0198BBB3-A649-726D-BD9E-DD9B6A5909CA",
                "0198BBB3-A649-726D-BD9E-E24E65AF7F75",
                "0198BBB3-A649-726D-BD9E-E4DD99018535",
                "0198BBB3-A649-726D-BD9E-E8928FA743B6",
                "0198BBB3-A649-726D-BD9E-EE4F2CB78199",
                "0198BBB3-A649-726D-BD9E-F12B4123AD64",
                "0198BBB3-A649-726D-BD9E-F4DCA2883266",
                "0198BBB3-A649-726D-BD9E-FBF02A0B7FB9",
                "0198BBB3-A649-726D-BD9E-FC4A17A9CB79",
                "0198BBB3-A649-726D-BD9F-001C62338EF0",
                "0198BBB3-A649-726D-BD9F-04E7E98AD250",
                "0198BBB3-A649-726D-BD9F-0A90F1533D80",
                "0198BBB3-A649-726D-BD9F-0F460F70191E",
                "0198BBB3-A649-726D-BD9F-1135B95E8335",
                "0198BBB3-A649-726D-BD9F-145DC874CE0D",
                "0198BBB3-A649-726D-BD9F-18E67BCEA3C9",
                "0198BBB3-A649-726D-BD9F-1C0EAA1630A7",
                "0198BBB3-A649-726D-BD9F-220C5EB1B9A5",
                "0198BBB3-A649-726D-BD9F-27B84B6B9874",
                "0198BBB3-A649-726D-BD9F-2A5E984296AE",
                "0198BBB3-A649-726D-BD9F-2CC6B10EB679",
                "0198BBB3-A649-726D-BD9F-31786CCEA17E",
                "0198BBB3-A649-726D-BD9F-35996AAD2AC9",
                "0198BBB3-A649-726D-BD9F-39C24621EE28",
                "0198BBB3-A649-726D-BD9F-3F86347A4F3B",
                "0198BBB3-A649-726D-BD9F-429A5B516C54",
                "0198BBB3-A649-726D-BD9F-44BD1D35845E",
                "0198BBB3-A649-726D-BD9F-489D62FD4807",
                "0198BBB3-A649-726D-BD9F-4CAA65048A8B",
                "0198BBB3-A649-726D-BD9F-5296D75F43CB",
                "0198BBB3-A649-726D-BD9F-5630FE1291DC",
                "0198BBB3-A649-726D-BD9F-5AEA5B8CD4E0",
                "0198BBB3-A649-726D-BD9F-5FCEDCBF04BD",
                "0198BBB3-A649-726D-BD9F-60682462B0B0",
                "0198BBB3-A649-726D-BD9F-648070A10222",
                "0198BBB3-A649-726D-BD9F-687CDF069C0E",
                "0198BBB3-A649-726D-BD9F-6E1DE0BABBB1",
                "0198BBB3-A649-726D-BD9F-719218D7D417",
                "0198BBB3-A649-726D-BD9F-75673C37938F",
                "0198BBB3-A649-726D-BD9F-7B0B2A68A69F",
                "0198BBB3-A649-726D-BD9F-7E7AD72E8FC2",
                "0198BBB3-A649-726D-BD9F-807E21B45F26",
                "0198BBB3-A649-726D-BD9F-87312AFA35E0",
                "0198BBB3-A649-726D-BD9F-898DF400EEF0",
                "0198BBB3-A649-726D-BD9F-8F151B534718",
                "0198BBB3-A649-726D-BD9F-91F5EB81DDDC",
                "0198BBB3-A649-726D-BD9F-95F3CB04DFE3",
                "0198BBB3-A649-726D-BD9F-990DA119567F",
                "0198BBB3-A649-726D-BD9F-9C1C954EE25C",
                "0198BBB3-A649-726D-BD9F-A0881F89E448",
                "0198BBB3-A649-726D-BD9F-A457CFBA6886",
                "0198BBB3-A649-726D-BD9F-AA865260A3CD",
                "0198BBB3-A649-726D-BD9F-AE83D82E90A6"
            ];
            List<TestEquipData> list = [];
            int index = 0;
            for (int i = 1; i < 11; i++)
            {
                var count = EquipTypeCount(i);
                foreach (var c in count)
                {
                    for (int uuidIndex = index; uuidIndex < index + 10; uuidIndex++)
                    {
                        list.Add(new TestEquipData
                        {
                            Id = Guid.Parse(uuids[uuidIndex]),
                            SimuTestSysld = Convert.ToByte(i),
                            DevTypeld = Convert.ToByte(c),
                            Compld = string.Join("", uuids[uuidIndex].Split('-'))
                        });
                    }
                    index += 10;
                }
            }
            return [.. list];
        }
    }
#endif
}
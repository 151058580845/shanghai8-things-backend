using System.ComponentModel;
using System.Reflection;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_307_ReceiveDatas;

/// <summary>
/// 307_阵列馈电
/// </summary>
[Description("接收的数据")]
public class XT_307_SL_1_ReceiveData : UniversalEntity, IAudited
{
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }

    [Description("仿真试验系统识别编码")]
    public byte SimuTestSysld { get; set; }

    [Description("设备类型识别编码")]
    public byte DevTypeld { get; set; }

    [Description("本机识别编码")]
    public string? Compld { get; set; }

    #region 工作模式信息 10个

    [Description("微波/毫米波")]
    public byte MicroWare { get; set; }

    [Description("通道接入")]
    public byte Channel { get; set; }

    [Description("模式是否有效")]
    public byte ModelValid { get; set; }

    [Description("阵面末端极化方式")]
    public byte ArrayEndPolarizationMode { get; set; }

    [Description("阵面末端功率模式")]
    public byte ArrayEndPowerMode { get; set; }

    [Description("阵列通道复用")]
    public byte ArrayChannelMultiplexing { get; set; }

    [Description("通道极化方式1")]
    public byte ChannelPolarizationMode1 { get; set; }

    [Description("通道极化方式2")]
    public byte ChannelPolarizationMode2 { get; set; }

    [Description("通道功率模式")]
    public byte ChannelPowerMode { get; set; }

    [Description("预留")]
    public byte Reserved { get; set; }

    #endregion 工作模式信息

    #region 健康状态信息 3个

    [Description("状态类型")]
    public byte StateType { get; set; }

    [Description("自检状态")]
    public uint SelfTest { get; set; }

    [Description("电源电压状态")]
    public uint SupplyVoltageState { get; set; }

    #endregion 健康状态信息

    #region 物理量

    #region 物理量数量

    [Description("物理量数量")]
    public uint PhysicalQuantityCount { get; set; }

    #endregion

    #region 解析,精控器件

    [Description("通道1解析器件12V")]
    public float Channel1ParserDevice12V { get; set; }

    [Description("通道1解析器件-12V")]
    public float Channel1ParserDeviceNegative12V { get; set; }

    [Description("通道1解析器件5V")]
    public float Channel1ParserDevice5V { get; set; }

    [Description("通道1垂直精控器件12V")]
    public float Channel1VerticalPrecisionDevice12V { get; set; }

    [Description("通道1垂直精控器件-12V")]
    public float Channel1VerticalPrecisionDeviceNegative12V { get; set; }

    [Description("通道1垂直精控器件5V")]
    public float Channel1VerticalPrecisionDevice5V { get; set; }

    [Description("通道1水平精控器件12V")]
    public float Channel1HorizontalPrecisionDevice12V { get; set; }

    [Description("通道1水平精控器件-12V")]
    public float Channel1HorizontalPrecisionDeviceNegative12V { get; set; }

    [Description("通道1水平精控器件5V")]
    public float Channel1HorizontalPrecisionDevice5V { get; set; }

    [Description("通道1解析控制12V")]
    public float Channel1ParserControl12V { get; set; }

    [Description("通道1垂直精控控制12V")]
    public float Channel1VerticalPrecisionControl12V { get; set; }

    [Description("通道1水平精控控制12V")]
    public float Channel1HorizontalPrecisionControl12V { get; set; }

    [Description("通道1解析精控共用12V")]
    public float Channel1ParserPrecisionShared12V { get; set; }

    [Description("通道1解析精控风扇12V")]
    public float Channel1ParserPrecisionFan12V { get; set; }

    [Description("通道2解析器件12V")]
    public float Channel2ParserDevice12V { get; set; }

    [Description("通道2解析器件-12V")]
    public float Channel2ParserDeviceNegative12V { get; set; }

    [Description("通道2解析器件5V")]
    public float Channel2ParserDevice5V { get; set; }

    [Description("通道2垂直精控器件12V")]
    public float Channel2VerticalPrecisionDevice12V { get; set; }

    [Description("通道2垂直精控器件-12V")]
    public float Channel2VerticalPrecisionDeviceNegative12V { get; set; }

    [Description("通道2垂直精控器件5V")]
    public float Channel2VerticalPrecisionDevice5V { get; set; }

    [Description("通道2水平精控器件12V")]
    public float Channel2HorizontalPrecisionDevice12V { get; set; }

    [Description("通道2水平精控器件-12V")]
    public float Channel2HorizontalPrecisionDeviceNegative12V { get; set; }

    [Description("通道2水平精控器件5V")]
    public float Channel2HorizontalPrecisionDevice5V { get; set; }

    [Description("通道2解析控制12V")]
    public float Channel2ParserControl12V { get; set; }

    [Description("通道2垂直精控控制12V")]
    public float Channel2VerticalPrecisionControl12V { get; set; }

    [Description("通道2水平精控控制12V")]
    public float Channel2HorizontalPrecisionControl12V { get; set; }

    [Description("通道2解析精控共用12V")]
    public float Channel2ParserPrecisionShared12V { get; set; }

    [Description("通道2解析精控风扇12V")]
    public float Channel2ParserPrecisionFan12V { get; set; }

    [Description("通道3解析器件12V")]
    public float Channel3ParserDevice12V { get; set; }

    [Description("通道3解析器件-12V")]
    public float Channel3ParserDeviceNegative12V { get; set; }

    [Description("通道3解析器件5V")]
    public float Channel3ParserDevice5V { get; set; }

    [Description("通道3垂直精控器件12V")]
    public float Channel3VerticalPrecisionDevice12V { get; set; }

    [Description("通道3垂直精控器件-12V")]
    public float Channel3VerticalPrecisionDeviceNegative12V { get; set; }

    [Description("通道3垂直精控器件5V")]
    public float Channel3VerticalPrecisionDevice5V { get; set; }

    [Description("通道3水平精控器件12V")]
    public float Channel3HorizontalPrecisionDevice12V { get; set; }

    [Description("通道3水平精控器件-12V")]
    public float Channel3HorizontalPrecisionDeviceNegative12V { get; set; }

    [Description("通道3水平精控器件5V")]
    public float Channel3HorizontalPrecisionDevice5V { get; set; }

    [Description("通道3解析控制12V")]
    public float Channel3ParserControl12V { get; set; }

    [Description("通道3垂直精控控制12V")]
    public float Channel3VerticalPrecisionControl12V { get; set; }

    [Description("通道3水平精控控制12V")]
    public float Channel3HorizontalPrecisionControl12V { get; set; }

    [Description("通道3解析精控共用12V")]
    public float Channel3ParserPrecisionShared12V { get; set; }

    [Description("通道3解析精控风扇12V")]
    public float Channel3ParserPrecisionFan12V { get; set; }

    [Description("通道4解析器件12V")]
    public float Channel4ParserDevice12V { get; set; }

    [Description("通道4解析器件-12V")]
    public float Channel4ParserDeviceNegative12V { get; set; }

    [Description("通道4解析器件5V")]
    public float Channel4ParserDevice5V { get; set; }

    [Description("通道4垂直精控器件12V")]
    public float Channel4VerticalPrecisionDevice12V { get; set; }

    [Description("通道4垂直精控器件-12V")]
    public float Channel4VerticalPrecisionDeviceNegative12V { get; set; }

    [Description("通道4垂直精控器件5V")]
    public float Channel4VerticalPrecisionDevice5V { get; set; }

    [Description("通道4水平精控器件12V")]
    public float Channel4HorizontalPrecisionDevice12V { get; set; }

    [Description("通道4水平精控器件-12V")]
    public float Channel4HorizontalPrecisionDeviceNegative12V { get; set; }

    [Description("通道4水平精控器件5V")]
    public float Channel4HorizontalPrecisionDevice5V { get; set; }

    [Description("通道4解析控制12V")]
    public float Channel4ParserControl12V { get; set; }

    [Description("通道4垂直精控控制12V")]
    public float Channel4VerticalPrecisionControl12V { get; set; }

    [Description("通道4水平精控控制12V")]
    public float Channel4HorizontalPrecisionControl12V { get; set; }

    [Description("通道4解析精控共用12V")]
    public float Channel4ParserPrecisionShared12V { get; set; }

    [Description("通道4解析精控风扇12V")]
    public float Channel4ParserPrecisionFan12V { get; set; }

    [Description("通道5解析器件12V")]
    public float Channel5ParserDevice12V { get; set; }

    [Description("通道5解析器件-12V")]
    public float Channel5ParserDeviceNegative12V { get; set; }

    [Description("通道5解析器件5V")]
    public float Channel5ParserDevice5V { get; set; }

    [Description("通道5垂直精控器件12V")]
    public float Channel5VerticalPrecisionDevice12V { get; set; }

    [Description("通道5垂直精控器件-12V")]
    public float Channel5VerticalPrecisionDeviceNegative12V { get; set; }

    [Description("通道5垂直精控器件5V")]
    public float Channel5VerticalPrecisionDevice5V { get; set; }

    [Description("通道5水平精控器件12V")]
    public float Channel5HorizontalPrecisionDevice12V { get; set; }

    [Description("通道5水平精控器件-12V")]
    public float Channel5HorizontalPrecisionDeviceNegative12V { get; set; }

    [Description("通道5水平精控器件5V")]
    public float Channel5HorizontalPrecisionDevice5V { get; set; }

    [Description("通道5解析控制12V")]
    public float Channel5ParserControl12V { get; set; }

    [Description("通道5垂直精控控制12V")]
    public float Channel5VerticalPrecisionControl12V { get; set; }

    [Description("通道5水平精控控制12V")]
    public float Channel5HorizontalPrecisionControl12V { get; set; }

    [Description("通道5解析精控共用12V")]
    public float Channel5ParserPrecisionShared12V { get; set; }

    [Description("通道5解析精控风扇12V")]
    public float Channel5ParserPrecisionFan12V { get; set; }

    [Description("通道6解析器件12V")]
    public float Channel6ParserDevice12V { get; set; }

    [Description("通道6解析器件-12V")]
    public float Channel6ParserDeviceNegative12V { get; set; }

    [Description("通道6解析器件5V")]
    public float Channel6ParserDevice5V { get; set; }

    [Description("通道6垂直精控器件12V")]
    public float Channel6VerticalPrecisionDevice12V { get; set; }

    [Description("通道6垂直精控器件-12V")]
    public float Channel6VerticalPrecisionDeviceNegative12V { get; set; }

    [Description("通道6垂直精控器件5V")]
    public float Channel6VerticalPrecisionDevice5V { get; set; }

    [Description("通道6水平精控器件12V")]
    public float Channel6HorizontalPrecisionDevice12V { get; set; }

    [Description("通道6水平精控器件-12V")]
    public float Channel6HorizontalPrecisionDeviceNegative12V { get; set; }

    [Description("通道6水平精控器件5V")]
    public float Channel6HorizontalPrecisionDevice5V { get; set; }

    [Description("通道6解析控制12V")]
    public float Channel6ParserControl12V { get; set; }

    [Description("通道6垂直精控控制12V")]
    public float Channel6VerticalPrecisionControl12V { get; set; }

    [Description("通道6水平精控控制12V")]
    public float Channel6HorizontalPrecisionControl12V { get; set; }

    [Description("通道6解析精控共用12V")]
    public float Channel6ParserPrecisionShared12V { get; set; }

    [Description("通道6解析精控风扇12V")]
    public float Channel6ParserPrecisionFan12V { get; set; }

    #endregion 解析,精控器件

    #region 放大器

    // 通道1放大器
    [Description("通道1解析放大器15V")]
    public float Channel1ParserAmplifier15V { get; set; }

    [Description("通道1垂直精控放大器15V")]
    public float Channel1VerticalPrecisionAmplifier15V { get; set; }

    [Description("通道1水平精控放大器15V")]
    public float Channel1HorizontalPrecisionAmplifier15V { get; set; }

    [Description("通道1放大器共用12V")]
    public float Channel1AmplifierShared12V { get; set; }

    [Description("通道1放大器风扇12V")]
    public float Channel1AmplifierFan12V { get; set; }

    // 通道2放大器
    [Description("通道2解析放大器15V")]
    public float Channel2ParserAmplifier15V { get; set; }

    [Description("通道2垂直精控放大器15V")]
    public float Channel2VerticalPrecisionAmplifier15V { get; set; }

    [Description("通道2水平精控放大器15V")]
    public float Channel2HorizontalPrecisionAmplifier15V { get; set; }

    [Description("通道2放大器共用12V")]
    public float Channel2AmplifierShared12V { get; set; }

    [Description("通道2放大器风扇12V")]
    public float Channel2AmplifierFan12V { get; set; }

    // 通道3放大器
    [Description("通道3解析放大器15V")]
    public float Channel3ParserAmplifier15V { get; set; }

    [Description("通道3垂直精控放大器15V")]
    public float Channel3VerticalPrecisionAmplifier15V { get; set; }

    [Description("通道3水平精控放大器15V")]
    public float Channel3HorizontalPrecisionAmplifier15V { get; set; }

    [Description("通道3放大器共用12V")]
    public float Channel3AmplifierShared12V { get; set; }

    [Description("通道3放大器风扇12V")]
    public float Channel3AmplifierFan12V { get; set; }

    // 通道4放大器
    [Description("通道4解析放大器15V")]
    public float Channel4ParserAmplifier15V { get; set; }

    [Description("通道4垂直精控放大器15V")]
    public float Channel4VerticalPrecisionAmplifier15V { get; set; }

    [Description("通道4水平精控放大器15V")]
    public float Channel4HorizontalPrecisionAmplifier15V { get; set; }

    [Description("通道4放大器共用12V")]
    public float Channel4AmplifierShared12V { get; set; }

    [Description("通道4放大器风扇12V")]
    public float Channel4AmplifierFan12V { get; set; }

    // 通道5放大器
    [Description("通道5解析放大器15V")]
    public float Channel5ParserAmplifier15V { get; set; }

    [Description("通道5垂直精控放大器15V")]
    public float Channel5VerticalPrecisionAmplifier15V { get; set; }

    [Description("通道5水平精控放大器15V")]
    public float Channel5HorizontalPrecisionAmplifier15V { get; set; }

    [Description("通道5放大器共用12V")]
    public float Channel5AmplifierShared12V { get; set; }

    [Description("通道5放大器风扇12V")]
    public float Channel5AmplifierFan12V { get; set; }

    // 通道6放大器
    [Description("通道6解析放大器15V")]
    public float Channel6ParserAmplifier15V { get; set; }

    [Description("通道6垂直精控放大器15V")]
    public float Channel6VerticalPrecisionAmplifier15V { get; set; }

    [Description("通道6水平精控放大器15V")]
    public float Channel6HorizontalPrecisionAmplifier15V { get; set; }

    [Description("通道6放大器共用12V")]
    public float Channel6AmplifierShared12V { get; set; }

    [Description("通道6放大器风扇12V")]
    public float Channel6AmplifierFan12V { get; set; }

    #endregion 放大器

    #region 垂直

    #region 垂直粗控,公用和垂直粗控风扇 分区1

    #region 垂直粗控

    // 通道1分区1垂直粗控
    [Description("通道1分区1垂直粗控5V")]
    public float Channel1Zone1VerticalCoarseControl5V { get; set; }

    [Description("通道1分区1垂直粗控-12V")]
    public float Channel1Zone1VerticalCoarseControlNegative12V { get; set; }

    [Description("通道1分区1垂直粗控15V")]
    public float Channel1Zone1VerticalCoarseControl15V { get; set; }

    // 通道2分区1垂直粗控电压
    [Description("通道2分区1垂直粗控5V")]
    public float Channel2Zone1VerticalCoarseControl5V { get; set; }

    [Description("通道2分区1垂直粗控-12V")]
    public float Channel2Zone1VerticalCoarseControlNegative12V { get; set; }

    [Description("通道2分区1垂直粗控15V")]
    public float Channel2Zone1VerticalCoarseControl15V { get; set; }

    // 通道3分区1垂直粗控电压
    [Description("通道3分区1垂直粗控5V")]
    public float Channel3Zone1VerticalCoarseControl5V { get; set; }

    [Description("通道3分区1垂直粗控-12V")]
    public float Channel3Zone1VerticalCoarseControlNegative12V { get; set; }

    [Description("通道3分区1垂直粗控15V")]
    public float Channel3Zone1VerticalCoarseControl15V { get; set; }

    // 通道4分区1垂直粗控电压
    [Description("通道4分区1垂直粗控5V")]
    public float Channel4Zone1VerticalCoarseControl5V { get; set; }

    [Description("通道4分区1垂直粗控-12V")]
    public float Channel4Zone1VerticalCoarseControlNegative12V { get; set; }

    [Description("通道4分区1垂直粗控15V")]
    public float Channel4Zone1VerticalCoarseControl15V { get; set; }

    // 通道5分区1垂直粗控电压
    [Description("通道5分区1垂直粗控5V")]
    public float Channel5Zone1VerticalCoarseControl5V { get; set; }

    [Description("通道5分区1垂直粗控-12V")]
    public float Channel5Zone1VerticalCoarseControlNegative12V { get; set; }

    [Description("通道5分区1垂直粗控15V")]
    public float Channel5Zone1VerticalCoarseControl15V { get; set; }

    // 通道6分区1垂直粗控电压
    [Description("通道6分区1垂直粗控5V")]
    public float Channel6Zone1VerticalCoarseControl5V { get; set; }

    [Description("通道6分区1垂直粗控-12V")]
    public float Channel6Zone1VerticalCoarseControlNegative12V { get; set; }

    [Description("通道6分区1垂直粗控15V")]
    public float Channel6Zone1VerticalCoarseControl15V { get; set; }

    #endregion 垂直粗控

    #region 公用和垂直粗控风扇

    // 分区1垂直粗控共用供电
    [Description("分区1垂直粗控共用供电12V")]
    public float Zone1VerticalCoarseControlSharedPower12V { get; set; }

    // 分区1垂直粗控共用控制
    [Description("分区1垂直粗控共用控制12V")]
    public float Zone1VerticalCoarseControlSharedControl12V { get; set; }

    // 分区1垂直粗控风扇
    [Description("分区1垂直粗控风扇12V")]
    public float Zone1VerticalCoarseControlFan12V { get; set; }

    #endregion 公用和垂直粗控风扇

    #endregion 垂直粗控,公用和垂直粗控风扇 分区1

    #region 垂直粗控,公用和垂直粗控风扇 分区2

    #region 垂直粗控

    // 通道1分区2垂直粗控
    [Description("通道1分区2垂直粗控5V")]
    public float Channel1Zone2VerticalCoarseControl5V { get; set; }

    [Description("通道1分区2垂直粗控-12V")]
    public float Channel1Zone2VerticalCoarseControlNegative12V { get; set; }

    [Description("通道1分区2垂直粗控15V")]
    public float Channel1Zone2VerticalCoarseControl15V { get; set; }

    // 通道2分区2垂直粗控电压
    [Description("通道2分区2垂直粗控5V")]
    public float Channel2Zone2VerticalCoarseControl5V { get; set; }

    [Description("通道2分区2垂直粗控-12V")]
    public float Channel2Zone2VerticalCoarseControlNegative12V { get; set; }

    [Description("通道2分区2垂直粗控15V")]
    public float Channel2Zone2VerticalCoarseControl15V { get; set; }

    // 通道3分区2垂直粗控电压
    [Description("通道3分区2垂直粗控5V")]
    public float Channel3Zone2VerticalCoarseControl5V { get; set; }

    [Description("通道3分区2垂直粗控-12V")]
    public float Channel3Zone2VerticalCoarseControlNegative12V { get; set; }

    [Description("通道3分区2垂直粗控15V")]
    public float Channel3Zone2VerticalCoarseControl15V { get; set; }

    // 通道4分区2垂直粗控电压
    [Description("通道4分区2垂直粗控5V")]
    public float Channel4Zone2VerticalCoarseControl5V { get; set; }

    [Description("通道4分区2垂直粗控-12V")]
    public float Channel4Zone2VerticalCoarseControlNegative12V { get; set; }

    [Description("通道4分区2垂直粗控15V")]
    public float Channel4Zone2VerticalCoarseControl15V { get; set; }

    // 通道5分区2垂直粗控电压
    [Description("通道5分区2垂直粗控5V")]
    public float Channel5Zone2VerticalCoarseControl5V { get; set; }

    [Description("通道5分区2垂直粗控-12V")]
    public float Channel5Zone2VerticalCoarseControlNegative12V { get; set; }

    [Description("通道5分区2垂直粗控15V")]
    public float Channel5Zone2VerticalCoarseControl15V { get; set; }

    // 通道6分区2垂直粗控电压
    [Description("通道6分区2垂直粗控5V")]
    public float Channel6Zone2VerticalCoarseControl5V { get; set; }

    [Description("通道6分区2垂直粗控-12V")]
    public float Channel6Zone2VerticalCoarseControlNegative12V { get; set; }

    [Description("通道6分区2垂直粗控15V")]
    public float Channel6Zone2VerticalCoarseControl15V { get; set; }

    #endregion 垂直粗控

    #region 公用和垂直粗控风扇

    // 分区2垂直粗控共用供电
    [Description("分区2垂直粗控共用供电12V")]
    public float Zone2VerticalCoarseControlSharedPower12V { get; set; }

    // 分区2垂直粗控共用控制
    [Description("分区2垂直粗控共用控制12V")]
    public float Zone2VerticalCoarseControlSharedControl12V { get; set; }

    // 分区2垂直粗控风扇
    [Description("分区2垂直粗控风扇12V")]
    public float Zone2VerticalCoarseControlFan12V { get; set; }

    #endregion 公用和垂直粗控风扇



    #endregion 垂直粗控,公用和垂直粗控风扇 分区2

    #region 垂直粗控,公用和垂直粗控风扇 分区3

    #region 垂直粗控

    // 通道1分区3垂直粗控
    [Description("通道1分区3垂直粗控5V")]
    public float Channel1Zone3VerticalCoarseControl5V { get; set; }

    [Description("通道1分区3垂直粗控-12V")]
    public float Channel1Zone3VerticalCoarseControlNegative12V { get; set; }

    [Description("通道1分区3垂直粗控15V")]
    public float Channel1Zone3VerticalCoarseControl15V { get; set; }

    // 通道2分区3垂直粗控电压
    [Description("通道2分区3垂直粗控5V")]
    public float Channel2Zone3VerticalCoarseControl5V { get; set; }

    [Description("通道2分区3垂直粗控-12V")]
    public float Channel2Zone3VerticalCoarseControlNegative12V { get; set; }

    [Description("通道2分区3垂直粗控15V")]
    public float Channel2Zone3VerticalCoarseControl15V { get; set; }

    // 通道3分区3垂直粗控电压
    [Description("通道3分区3垂直粗控5V")]
    public float Channel3Zone3VerticalCoarseControl5V { get; set; }

    [Description("通道3分区3垂直粗控-12V")]
    public float Channel3Zone3VerticalCoarseControlNegative12V { get; set; }

    [Description("通道3分区3垂直粗控15V")]
    public float Channel3Zone3VerticalCoarseControl15V { get; set; }

    // 通道4分区3垂直粗控电压
    [Description("通道4分区3垂直粗控5V")]
    public float Channel4Zone3VerticalCoarseControl5V { get; set; }

    [Description("通道4分区3垂直粗控-12V")]
    public float Channel4Zone3VerticalCoarseControlNegative12V { get; set; }

    [Description("通道4分区3垂直粗控15V")]
    public float Channel4Zone3VerticalCoarseControl15V { get; set; }

    // 通道5分区3垂直粗控电压
    [Description("通道5分区3垂直粗控5V")]
    public float Channel5Zone3VerticalCoarseControl5V { get; set; }

    [Description("通道5分区3垂直粗控-12V")]
    public float Channel5Zone3VerticalCoarseControlNegative12V { get; set; }

    [Description("通道5分区3垂直粗控15V")]
    public float Channel5Zone3VerticalCoarseControl15V { get; set; }

    // 通道6分区3垂直粗控电压
    [Description("通道6分区3垂直粗控5V")]
    public float Channel6Zone3VerticalCoarseControl5V { get; set; }

    [Description("通道6分区3垂直粗控-12V")]
    public float Channel6Zone3VerticalCoarseControlNegative12V { get; set; }

    [Description("通道6分区3垂直粗控15V")]
    public float Channel6Zone3VerticalCoarseControl15V { get; set; }

    #endregion 垂直粗控

    #region 公用和垂直粗控风扇

    // 分区3垂直粗控共用供电
    [Description("分区3垂直粗控共用供电12V")]
    public float Zone3VerticalCoarseControlSharedPower12V { get; set; }

    // 分区3垂直粗控共用控制
    [Description("分区3垂直粗控共用控制12V")]
    public float Zone3VerticalCoarseControlSharedControl12V { get; set; }

    // 分区3垂直粗控风扇
    [Description("分区3垂直粗控风扇12V")]
    public float Zone3VerticalCoarseControlFan12V { get; set; }

    #endregion 公用和垂直粗控风扇



    #endregion 垂直粗控,公用和垂直粗控风扇 分区3

    #region 垂直粗控,公用和垂直粗控风扇 分区4

    #region 垂直粗控

    // 通道1分区4垂直粗控
    [Description("通道1分区4垂直粗控5V")]
    public float Channel1Zone4VerticalCoarseControl5V { get; set; }

    [Description("通道1分区4垂直粗控-12V")]
    public float Channel1Zone4VerticalCoarseControlNegative12V { get; set; }

    [Description("通道1分区4垂直粗控15V")]
    public float Channel1Zone4VerticalCoarseControl15V { get; set; }

    // 通道2分区4垂直粗控电压
    [Description("通道2分区4垂直粗控5V")]
    public float Channel2Zone4VerticalCoarseControl5V { get; set; }

    [Description("通道2分区4垂直粗控-12V")]
    public float Channel2Zone4VerticalCoarseControlNegative12V { get; set; }

    [Description("通道2分区4垂直粗控15V")]
    public float Channel2Zone4VerticalCoarseControl15V { get; set; }

    // 通道3分区4垂直粗控电压
    [Description("通道3分区4垂直粗控5V")]
    public float Channel3Zone4VerticalCoarseControl5V { get; set; }

    [Description("通道3分区4垂直粗控-12V")]
    public float Channel3Zone4VerticalCoarseControlNegative12V { get; set; }

    [Description("通道3分区4垂直粗控15V")]
    public float Channel3Zone4VerticalCoarseControl15V { get; set; }

    // 通道4分区4垂直粗控电压
    [Description("通道4分区4垂直粗控5V")]
    public float Channel4Zone4VerticalCoarseControl5V { get; set; }

    [Description("通道4分区4垂直粗控-12V")]
    public float Channel4Zone4VerticalCoarseControlNegative12V { get; set; }

    [Description("通道4分区4垂直粗控15V")]
    public float Channel4Zone4VerticalCoarseControl15V { get; set; }

    // 通道5分区4垂直粗控电压
    [Description("通道5分区4垂直粗控5V")]
    public float Channel5Zone4VerticalCoarseControl5V { get; set; }

    [Description("通道5分区4垂直粗控-12V")]
    public float Channel5Zone4VerticalCoarseControlNegative12V { get; set; }

    [Description("通道5分区4垂直粗控15V")]
    public float Channel5Zone4VerticalCoarseControl15V { get; set; }

    // 通道6分区4垂直粗控电压
    [Description("通道6分区4垂直粗控5V")]
    public float Channel6Zone4VerticalCoarseControl5V { get; set; }

    [Description("通道6分区4垂直粗控-12V")]
    public float Channel6Zone4VerticalCoarseControlNegative12V { get; set; }

    [Description("通道6分区4垂直粗控15V")]
    public float Channel6Zone4VerticalCoarseControl15V { get; set; }

    #endregion 垂直粗控

    #region 公用和垂直粗控风扇

    // 分区4垂直粗控共用供电
    [Description("分区4垂直粗控共用供电12V")]
    public float Zone4VerticalCoarseControlSharedPower12V { get; set; }

    // 分区4垂直粗控共用控制
    [Description("分区4垂直粗控共用控制12V")]
    public float Zone4VerticalCoarseControlSharedControl12V { get; set; }

    // 分区4垂直粗控风扇
    [Description("分区4垂直粗控风扇12V")]
    public float Zone4VerticalCoarseControlFan12V { get; set; }

    #endregion 公用和垂直粗控风扇



    #endregion 垂直粗控,公用和垂直粗控风扇 分区4

    #region 垂直粗控,公用和垂直粗控风扇 分区5

    #region 垂直粗控

    // 通道1分区5垂直粗控
    [Description("通道1分区5垂直粗控5V")]
    public float Channel1Zone5VerticalCoarseControl5V { get; set; }

    [Description("通道1分区5垂直粗控-12V")]
    public float Channel1Zone5VerticalCoarseControlNegative12V { get; set; }

    [Description("通道1分区5垂直粗控15V")]
    public float Channel1Zone5VerticalCoarseControl15V { get; set; }

    // 通道2分区5垂直粗控电压
    [Description("通道2分区5垂直粗控5V")]
    public float Channel2Zone5VerticalCoarseControl5V { get; set; }

    [Description("通道2分区5垂直粗控-12V")]
    public float Channel2Zone5VerticalCoarseControlNegative12V { get; set; }

    [Description("通道2分区5垂直粗控15V")]
    public float Channel2Zone5VerticalCoarseControl15V { get; set; }

    // 通道3分区5垂直粗控电压
    [Description("通道3分区5垂直粗控5V")]
    public float Channel3Zone5VerticalCoarseControl5V { get; set; }

    [Description("通道3分区5垂直粗控-12V")]
    public float Channel3Zone5VerticalCoarseControlNegative12V { get; set; }

    [Description("通道3分区5垂直粗控15V")]
    public float Channel3Zone5VerticalCoarseControl15V { get; set; }

    // 通道4分区5垂直粗控电压
    [Description("通道4分区5垂直粗控5V")]
    public float Channel4Zone5VerticalCoarseControl5V { get; set; }

    [Description("通道4分区5垂直粗控-12V")]
    public float Channel4Zone5VerticalCoarseControlNegative12V { get; set; }

    [Description("通道4分区5垂直粗控15V")]
    public float Channel4Zone5VerticalCoarseControl15V { get; set; }

    // 通道5分区5垂直粗控电压
    [Description("通道5分区5垂直粗控5V")]
    public float Channel5Zone5VerticalCoarseControl5V { get; set; }

    [Description("通道5分区5垂直粗控-12V")]
    public float Channel5Zone5VerticalCoarseControlNegative12V { get; set; }

    [Description("通道5分区5垂直粗控15V")]
    public float Channel5Zone5VerticalCoarseControl15V { get; set; }

    // 通道6分区5垂直粗控电压
    [Description("通道6分区5垂直粗控5V")]
    public float Channel6Zone5VerticalCoarseControl5V { get; set; }

    [Description("通道6分区5垂直粗控-12V")]
    public float Channel6Zone5VerticalCoarseControlNegative12V { get; set; }

    [Description("通道6分区5垂直粗控15V")]
    public float Channel6Zone5VerticalCoarseControl15V { get; set; }

    #endregion 垂直粗控

    #region 公用和垂直粗控风扇

    // 分区5垂直粗控共用供电
    [Description("分区5垂直粗控共用供电12V")]
    public float Zone5VerticalCoarseControlSharedPower12V { get; set; }

    // 分区5垂直粗控共用控制
    [Description("分区5垂直粗控共用控制12V")]
    public float Zone5VerticalCoarseControlSharedControl12V { get; set; }

    // 分区5垂直粗控风扇
    [Description("分区5垂直粗控风扇12V")]
    public float Zone5VerticalCoarseControlFan12V { get; set; }

    #endregion 公用和垂直粗控风扇



    #endregion 垂直粗控,公用和垂直粗控风扇 分区5

    #region 垂直粗控,公用和垂直粗控风扇 分区6

    #region 垂直粗控

    // 通道1分区6垂直粗控
    [Description("通道1分区6垂直粗控5V")]
    public float Channel1Zone6VerticalCoarseControl5V { get; set; }

    [Description("通道1分区6垂直粗控-12V")]
    public float Channel1Zone6VerticalCoarseControlNegative12V { get; set; }

    [Description("通道1分区6垂直粗控15V")]
    public float Channel1Zone6VerticalCoarseControl15V { get; set; }

    // 通道2分区6垂直粗控电压
    [Description("通道2分区6垂直粗控5V")]
    public float Channel2Zone6VerticalCoarseControl5V { get; set; }

    [Description("通道2分区6垂直粗控-12V")]
    public float Channel2Zone6VerticalCoarseControlNegative12V { get; set; }

    [Description("通道2分区6垂直粗控15V")]
    public float Channel2Zone6VerticalCoarseControl15V { get; set; }

    // 通道3分区6垂直粗控电压
    [Description("通道3分区6垂直粗控5V")]
    public float Channel3Zone6VerticalCoarseControl5V { get; set; }

    [Description("通道3分区6垂直粗控-12V")]
    public float Channel3Zone6VerticalCoarseControlNegative12V { get; set; }

    [Description("通道3分区6垂直粗控15V")]
    public float Channel3Zone6VerticalCoarseControl15V { get; set; }

    // 通道4分区6垂直粗控电压
    [Description("通道4分区6垂直粗控5V")]
    public float Channel4Zone6VerticalCoarseControl5V { get; set; }

    [Description("通道4分区6垂直粗控-12V")]
    public float Channel4Zone6VerticalCoarseControlNegative12V { get; set; }

    [Description("通道4分区6垂直粗控15V")]
    public float Channel4Zone6VerticalCoarseControl15V { get; set; }

    // 通道5分区6垂直粗控电压
    [Description("通道5分区6垂直粗控5V")]
    public float Channel5Zone6VerticalCoarseControl5V { get; set; }

    [Description("通道5分区6垂直粗控-12V")]
    public float Channel5Zone6VerticalCoarseControlNegative12V { get; set; }

    [Description("通道5分区6垂直粗控15V")]
    public float Channel5Zone6VerticalCoarseControl15V { get; set; }

    // 通道6分区6垂直粗控电压
    [Description("通道6分区6垂直粗控5V")]
    public float Channel6Zone6VerticalCoarseControl5V { get; set; }

    [Description("通道6分区6垂直粗控-12V")]
    public float Channel6Zone6VerticalCoarseControlNegative12V { get; set; }

    [Description("通道6分区6垂直粗控15V")]
    public float Channel6Zone6VerticalCoarseControl15V { get; set; }

    #endregion 垂直粗控

    #region 公用和垂直粗控风扇

    // 分区6垂直粗控共用供电
    [Description("分区6垂直粗控共用供电12V")]
    public float Zone6VerticalCoarseControlSharedPower12V { get; set; }

    // 分区6垂直粗控共用控制
    [Description("分区6垂直粗控共用控制12V")]
    public float Zone6VerticalCoarseControlSharedControl12V { get; set; }

    // 分区6垂直粗控风扇
    [Description("分区6垂直粗控风扇12V")]
    public float Zone6VerticalCoarseControlFan12V { get; set; }

    #endregion 公用和垂直粗控风扇

    #endregion 垂直粗控,公用和垂直粗控风扇 分区6

    #region 垂直粗控控制 分区1

    // 通道1分区1垂直粗控控制
    [Description("通道1分区1垂直粗控控制12V")]
    public float Channel1Zone1VerticalCoarseControlControl12V { get; set; }

    // 通道2分区1垂直粗控控制
    [Description("通道2分区1垂直粗控控制12V")]
    public float Channel2Zone1VerticalCoarseControlControl12V { get; set; }

    // 通道3分区1垂直粗控控制
    [Description("通道3分区1垂直粗控控制12V")]
    public float Channel3Zone1VerticalCoarseControlControl12V { get; set; }

    // 通道4分区1垂直粗控控制
    [Description("通道4分区1垂直粗控控制12V")]
    public float Channel4Zone1VerticalCoarseControlControl12V { get; set; }

    // 通道5分区1垂直粗控控制
    [Description("通道5分区1垂直粗控控制12V")]
    public float Channel5Zone1VerticalCoarseControlControl12V { get; set; }

    // 通道6分区1垂直粗控控制
    [Description("通道6分区1垂直粗控控制12V")]
    public float Channel6Zone1VerticalCoarseControlControl12V { get; set; }

    // 分区1垂直粗控控制共用
    [Description("分区1垂直粗控控制共用12V")]
    public float Zone1VerticalCoarseControlCommonControl12V { get; set; }

    // 分区1垂直粗控控制风扇
    [Description("分区1垂直粗控控制风扇12V")]
    public float Zone1VerticalCoarseControlFanControl12V { get; set; }

    #endregion 垂直粗控控制 分区1

    #region 垂直粗控控制 分区2

    // 通道1分区2垂直粗控控制
    [Description("通道1分区2垂直粗控控制12V")]
    public float Channel1Zone2VerticalCoarseControlControl12V { get; set; }

    // 通道2分区2垂直粗控控制
    [Description("通道2分区2垂直粗控控制12V")]
    public float Channel2Zone2VerticalCoarseControlControl12V { get; set; }

    // 通道3分区2垂直粗控控制
    [Description("通道3分区2垂直粗控控制12V")]
    public float Channel3Zone2VerticalCoarseControlControl12V { get; set; }

    // 通道4分区2垂直粗控控制
    [Description("通道4分区2垂直粗控控制12V")]
    public float Channel4Zone2VerticalCoarseControlControl12V { get; set; }

    // 通道5分区2垂直粗控控制
    [Description("通道5分区2垂直粗控控制12V")]
    public float Channel5Zone2VerticalCoarseControlControl12V { get; set; }

    // 通道6分区2垂直粗控控制
    [Description("通道6分区2垂直粗控控制12V")]
    public float Channel6Zone2VerticalCoarseControlControl12V { get; set; }

    // 分区2垂直粗控控制共用
    [Description("分区2垂直粗控控制共用12V")]
    public float Zone2VerticalCoarseControlCommonControl12V { get; set; }

    // 分区2垂直粗控控制风扇
    [Description("分区2垂直粗控控制风扇12V")]
    public float Zone2VerticalCoarseControlFanControl12V { get; set; }

    #endregion 垂直粗控控制 分区2

    #region 垂直粗控控制 分区3

    // 通道1分区3垂直粗控控制
    [Description("通道1分区3垂直粗控控制12V")]
    public float Channel1Zone3VerticalCoarseControlControl12V { get; set; }

    // 通道2分区3垂直粗控控制
    [Description("通道2分区3垂直粗控控制12V")]
    public float Channel2Zone3VerticalCoarseControlControl12V { get; set; }

    // 通道3分区3垂直粗控控制
    [Description("通道3分区3垂直粗控控制12V")]
    public float Channel3Zone3VerticalCoarseControlControl12V { get; set; }

    // 通道4分区3垂直粗控控制
    [Description("通道4分区3垂直粗控控制12V")]
    public float Channel4Zone3VerticalCoarseControlControl12V { get; set; }

    // 通道5分区3垂直粗控控制
    [Description("通道5分区3垂直粗控控制12V")]
    public float Channel5Zone3VerticalCoarseControlControl12V { get; set; }

    // 通道6分区3垂直粗控控制
    [Description("通道6分区3垂直粗控控制12V")]
    public float Channel6Zone3VerticalCoarseControlControl12V { get; set; }

    // 分区3垂直粗控控制共用
    [Description("分区3垂直粗控控制共用12V")]
    public float Zone3VerticalCoarseControlCommonControl12V { get; set; }

    // 分区3垂直粗控控制风扇
    [Description("分区3垂直粗控控制风扇12V")]
    public float Zone3VerticalCoarseControlFanControl12V { get; set; }

    #endregion 垂直粗控控制 分区3

    #region 垂直粗控控制 分区4

    // 通道1分区4垂直粗控控制
    [Description("通道1分区4垂直粗控控制12V")]
    public float Channel1Zone4VerticalCoarseControlControl12V { get; set; }

    // 通道2分区4垂直粗控控制
    [Description("通道2分区4垂直粗控控制12V")]
    public float Channel2Zone4VerticalCoarseControlControl12V { get; set; }

    // 通道3分区4垂直粗控控制
    [Description("通道3分区4垂直粗控控制12V")]
    public float Channel3Zone4VerticalCoarseControlControl12V { get; set; }

    // 通道4分区4垂直粗控控制
    [Description("通道4分区4垂直粗控控制12V")]
    public float Channel4Zone4VerticalCoarseControlControl12V { get; set; }

    // 通道5分区4垂直粗控控制
    [Description("通道5分区4垂直粗控控制12V")]
    public float Channel5Zone4VerticalCoarseControlControl12V { get; set; }

    // 通道6分区4垂直粗控控制
    [Description("通道6分区4垂直粗控控制12V")]
    public float Channel6Zone4VerticalCoarseControlControl12V { get; set; }

    // 分区4垂直粗控控制共用
    [Description("分区4垂直粗控控制共用12V")]
    public float Zone4VerticalCoarseControlCommonControl12V { get; set; }

    // 分区4垂直粗控控制风扇
    [Description("分区4垂直粗控控制风扇12V")]
    public float Zone4VerticalCoarseControlFanControl12V { get; set; }

    #endregion 垂直粗控控制 分区4

    #region 垂直粗控控制 分区5

    // 通道1分区5垂直粗控控制
    [Description("通道1分区5垂直粗控控制12V")]
    public float Channel1Zone5VerticalCoarseControlControl12V { get; set; }

    // 通道2分区5垂直粗控控制
    [Description("通道2分区5垂直粗控控制12V")]
    public float Channel2Zone5VerticalCoarseControlControl12V { get; set; }

    // 通道3分区5垂直粗控控制
    [Description("通道3分区5垂直粗控控制12V")]
    public float Channel3Zone5VerticalCoarseControlControl12V { get; set; }

    // 通道4分区5垂直粗控控制
    [Description("通道4分区5垂直粗控控制12V")]
    public float Channel4Zone5VerticalCoarseControlControl12V { get; set; }

    // 通道5分区5垂直粗控控制
    [Description("通道5分区5垂直粗控控制12V")]
    public float Channel5Zone5VerticalCoarseControlControl12V { get; set; }

    // 通道6分区5垂直粗控控制
    [Description("通道6分区5垂直粗控控制12V")]
    public float Channel6Zone5VerticalCoarseControlControl12V { get; set; }

    // 分区5垂直粗控控制共用
    [Description("分区5垂直粗控控制共用12V")]
    public float Zone5VerticalCoarseControlCommonControl12V { get; set; }

    // 分区5垂直粗控控制风扇
    [Description("分区5垂直粗控控制风扇12V")]
    public float Zone5VerticalCoarseControlFanControl12V { get; set; }

    #endregion 垂直粗控控制 分区5

    #region 垂直粗控控制 分区6

    // 通道1分区6垂直粗控控制
    [Description("通道1分区6垂直粗控控制12V")]
    public float Channel1Zone6VerticalCoarseControlControl12V { get; set; }

    // 通道2分区6垂直粗控控制
    [Description("通道2分区6垂直粗控控制12V")]
    public float Channel2Zone6VerticalCoarseControlControl12V { get; set; }

    // 通道3分区6垂直粗控控制
    [Description("通道3分区6垂直粗控控制12V")]
    public float Channel3Zone6VerticalCoarseControlControl12V { get; set; }

    // 通道4分区6垂直粗控控制
    [Description("通道4分区6垂直粗控控制12V")]
    public float Channel4Zone6VerticalCoarseControlControl12V { get; set; }

    // 通道5分区6垂直粗控控制
    [Description("通道5分区6垂直粗控控制12V")]
    public float Channel5Zone6VerticalCoarseControlControl12V { get; set; }

    // 通道6分区6垂直粗控控制
    [Description("通道6分区6垂直粗控控制12V")]
    public float Channel6Zone6VerticalCoarseControlControl12V { get; set; }

    // 分区6垂直粗控控制共用
    [Description("分区6垂直粗控控制共用12V")]
    public float Zone6VerticalCoarseControlCommonControl12V { get; set; }

    // 分区6垂直粗控控制风扇
    [Description("分区6垂直粗控控制风扇12V")]
    public float Zone6VerticalCoarseControlFanControl12V { get; set; }

    #endregion 垂直粗控控制 分区6

    #endregion 垂直

    #region 水平

    #region 水平粗控,公用和水平粗控风扇 分区1

    #region 水平粗控

    // 通道1分区1水平粗控
    [Description("通道1分区1水平粗控5V")]
    public float Channel1Zone1HorizontalCoarseControl5V { get; set; }

    [Description("通道1分区1水平粗控-12V")]
    public float Channel1Zone1HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道1分区1水平粗控15V")]
    public float Channel1Zone1HorizontalCoarseControl15V { get; set; }

    // 通道2分区1水平粗控电压
    [Description("通道2分区1水平粗控5V")]
    public float Channel2Zone1HorizontalCoarseControl5V { get; set; }

    [Description("通道2分区1水平粗控-12V")]
    public float Channel2Zone1HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道2分区1水平粗控15V")]
    public float Channel2Zone1HorizontalCoarseControl15V { get; set; }

    // 通道3分区1水平粗控电压
    [Description("通道3分区1水平粗控5V")]
    public float Channel3Zone1HorizontalCoarseControl5V { get; set; }

    [Description("通道3分区1水平粗控-12V")]
    public float Channel3Zone1HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道3分区1水平粗控15V")]
    public float Channel3Zone1HorizontalCoarseControl15V { get; set; }

    // 通道4分区1水平粗控电压
    [Description("通道4分区1水平粗控5V")]
    public float Channel4Zone1HorizontalCoarseControl5V { get; set; }

    [Description("通道4分区1水平粗控-12V")]
    public float Channel4Zone1HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道4分区1水平粗控15V")]
    public float Channel4Zone1HorizontalCoarseControl15V { get; set; }

    // 通道5分区1水平粗控电压
    [Description("通道5分区1水平粗控5V")]
    public float Channel5Zone1HorizontalCoarseControl5V { get; set; }

    [Description("通道5分区1水平粗控-12V")]
    public float Channel5Zone1HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道5分区1水平粗控15V")]
    public float Channel5Zone1HorizontalCoarseControl15V { get; set; }

    // 通道6分区1水平粗控电压
    [Description("通道6分区1水平粗控5V")]
    public float Channel6Zone1HorizontalCoarseControl5V { get; set; }

    [Description("通道6分区1水平粗控-12V")]
    public float Channel6Zone1HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道6分区1水平粗控15V")]
    public float Channel6Zone1HorizontalCoarseControl15V { get; set; }

    #endregion 水平粗控

    #region 公用和水平粗控风扇

    // 分区1水平粗控共用供电
    [Description("分区1水平粗控共用供电12V")]
    public float Zone1HorizontalCoarseControlSharedPower12V { get; set; }

    // 分区1水平粗控共用控制
    [Description("分区1水平粗控共用控制12V")]
    public float Zone1HorizontalCoarseControlSharedControl12V { get; set; }

    // 分区1水平粗控风扇
    [Description("分区1水平粗控风扇12V")]
    public float Zone1HorizontalCoarseControlFan12V { get; set; }

    #endregion 公用和水平粗控风扇

    #endregion 水平粗控,公用和水平粗控风扇 分区1

    #region 水平粗控,公用和水平粗控风扇 分区2

    #region 水平粗控

    // 通道1分区2水平粗控
    [Description("通道1分区2水平粗控5V")]
    public float Channel1Zone2HorizontalCoarseControl5V { get; set; }

    [Description("通道1分区2水平粗控-12V")]
    public float Channel1Zone2HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道1分区2水平粗控15V")]
    public float Channel1Zone2HorizontalCoarseControl15V { get; set; }

    // 通道2分区2水平粗控电压
    [Description("通道2分区2水平粗控5V")]
    public float Channel2Zone2HorizontalCoarseControl5V { get; set; }

    [Description("通道2分区2水平粗控-12V")]
    public float Channel2Zone2HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道2分区2水平粗控15V")]
    public float Channel2Zone2HorizontalCoarseControl15V { get; set; }

    // 通道3分区2水平粗控电压
    [Description("通道3分区2水平粗控5V")]
    public float Channel3Zone2HorizontalCoarseControl5V { get; set; }

    [Description("通道3分区2水平粗控-12V")]
    public float Channel3Zone2HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道3分区2水平粗控15V")]
    public float Channel3Zone2HorizontalCoarseControl15V { get; set; }

    // 通道4分区2水平粗控电压
    [Description("通道4分区2水平粗控5V")]
    public float Channel4Zone2HorizontalCoarseControl5V { get; set; }

    [Description("通道4分区2水平粗控-12V")]
    public float Channel4Zone2HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道4分区2水平粗控15V")]
    public float Channel4Zone2HorizontalCoarseControl15V { get; set; }

    // 通道5分区2水平粗控电压
    [Description("通道5分区2水平粗控5V")]
    public float Channel5Zone2HorizontalCoarseControl5V { get; set; }

    [Description("通道5分区2水平粗控-12V")]
    public float Channel5Zone2HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道5分区2水平粗控15V")]
    public float Channel5Zone2HorizontalCoarseControl15V { get; set; }

    // 通道6分区2水平粗控电压
    [Description("通道6分区2水平粗控5V")]
    public float Channel6Zone2HorizontalCoarseControl5V { get; set; }

    [Description("通道6分区2水平粗控-12V")]
    public float Channel6Zone2HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道6分区2水平粗控15V")]
    public float Channel6Zone2HorizontalCoarseControl15V { get; set; }

    #endregion 水平粗控

    #region 公用和水平粗控风扇

    // 分区2水平粗控共用供电
    [Description("分区2水平粗控共用供电12V")]
    public float Zone2HorizontalCoarseControlSharedPower12V { get; set; }

    // 分区2水平粗控共用控制
    [Description("分区2水平粗控共用控制12V")]
    public float Zone2HorizontalCoarseControlSharedControl12V { get; set; }

    // 分区2水平粗控风扇
    [Description("分区2水平粗控风扇12V")]
    public float Zone2HorizontalCoarseControlFan12V { get; set; }

    #endregion 公用和水平粗控风扇



    #endregion 水平粗控,公用和水平粗控风扇 分区2

    #region 水平粗控,公用和水平粗控风扇 分区3

    #region 水平粗控

    // 通道1分区3水平粗控
    [Description("通道1分区3水平粗控5V")]
    public float Channel1Zone3HorizontalCoarseControl5V { get; set; }

    [Description("通道1分区3水平粗控-12V")]
    public float Channel1Zone3HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道1分区3水平粗控15V")]
    public float Channel1Zone3HorizontalCoarseControl15V { get; set; }

    // 通道2分区3水平粗控电压
    [Description("通道2分区3水平粗控5V")]
    public float Channel2Zone3HorizontalCoarseControl5V { get; set; }

    [Description("通道2分区3水平粗控-12V")]
    public float Channel2Zone3HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道2分区3水平粗控15V")]
    public float Channel2Zone3HorizontalCoarseControl15V { get; set; }

    // 通道3分区3水平粗控电压
    [Description("通道3分区3水平粗控5V")]
    public float Channel3Zone3HorizontalCoarseControl5V { get; set; }

    [Description("通道3分区3水平粗控-12V")]
    public float Channel3Zone3HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道3分区3水平粗控15V")]
    public float Channel3Zone3HorizontalCoarseControl15V { get; set; }

    // 通道4分区3水平粗控电压
    [Description("通道4分区3水平粗控5V")]
    public float Channel4Zone3HorizontalCoarseControl5V { get; set; }

    [Description("通道4分区3水平粗控-12V")]
    public float Channel4Zone3HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道4分区3水平粗控15V")]
    public float Channel4Zone3HorizontalCoarseControl15V { get; set; }

    // 通道5分区3水平粗控电压
    [Description("通道5分区3水平粗控5V")]
    public float Channel5Zone3HorizontalCoarseControl5V { get; set; }

    [Description("通道5分区3水平粗控-12V")]
    public float Channel5Zone3HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道5分区3水平粗控15V")]
    public float Channel5Zone3HorizontalCoarseControl15V { get; set; }

    // 通道6分区3水平粗控电压
    [Description("通道6分区3水平粗控5V")]
    public float Channel6Zone3HorizontalCoarseControl5V { get; set; }

    [Description("通道6分区3水平粗控-12V")]
    public float Channel6Zone3HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道6分区3水平粗控15V")]
    public float Channel6Zone3HorizontalCoarseControl15V { get; set; }

    #endregion 水平粗控

    #region 公用和水平粗控风扇

    // 分区3水平粗控共用供电
    [Description("分区3水平粗控共用供电12V")]
    public float Zone3HorizontalCoarseControlSharedPower12V { get; set; }

    // 分区3水平粗控共用控制
    [Description("分区3水平粗控共用控制12V")]
    public float Zone3HorizontalCoarseControlSharedControl12V { get; set; }

    // 分区3水平粗控风扇
    [Description("分区3水平粗控风扇12V")]
    public float Zone3HorizontalCoarseControlFan12V { get; set; }

    #endregion 公用和水平粗控风扇



    #endregion 水平粗控,公用和水平粗控风扇 分区3

    #region 水平粗控,公用和水平粗控风扇 分区4

    #region 水平粗控

    // 通道1分区4水平粗控
    [Description("通道1分区4水平粗控5V")]
    public float Channel1Zone4HorizontalCoarseControl5V { get; set; }

    [Description("通道1分区4水平粗控-12V")]
    public float Channel1Zone4HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道1分区4水平粗控15V")]
    public float Channel1Zone4HorizontalCoarseControl15V { get; set; }

    // 通道2分区4水平粗控电压
    [Description("通道2分区4水平粗控5V")]
    public float Channel2Zone4HorizontalCoarseControl5V { get; set; }

    [Description("通道2分区4水平粗控-12V")]
    public float Channel2Zone4HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道2分区4水平粗控15V")]
    public float Channel2Zone4HorizontalCoarseControl15V { get; set; }

    // 通道3分区4水平粗控电压
    [Description("通道3分区4水平粗控5V")]
    public float Channel3Zone4HorizontalCoarseControl5V { get; set; }

    [Description("通道3分区4水平粗控-12V")]
    public float Channel3Zone4HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道3分区4水平粗控15V")]
    public float Channel3Zone4HorizontalCoarseControl15V { get; set; }

    // 通道4分区4水平粗控电压
    [Description("通道4分区4水平粗控5V")]
    public float Channel4Zone4HorizontalCoarseControl5V { get; set; }

    [Description("通道4分区4水平粗控-12V")]
    public float Channel4Zone4HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道4分区4水平粗控15V")]
    public float Channel4Zone4HorizontalCoarseControl15V { get; set; }

    // 通道5分区4水平粗控电压
    [Description("通道5分区4水平粗控5V")]
    public float Channel5Zone4HorizontalCoarseControl5V { get; set; }

    [Description("通道5分区4水平粗控-12V")]
    public float Channel5Zone4HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道5分区4水平粗控15V")]
    public float Channel5Zone4HorizontalCoarseControl15V { get; set; }

    // 通道6分区4水平粗控电压
    [Description("通道6分区4水平粗控5V")]
    public float Channel6Zone4HorizontalCoarseControl5V { get; set; }

    [Description("通道6分区4水平粗控-12V")]
    public float Channel6Zone4HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道6分区4水平粗控15V")]
    public float Channel6Zone4HorizontalCoarseControl15V { get; set; }

    #endregion 水平粗控

    #region 公用和水平粗控风扇

    // 分区4水平粗控共用供电
    [Description("分区4水平粗控共用供电12V")]
    public float Zone4HorizontalCoarseControlSharedPower12V { get; set; }

    // 分区4水平粗控共用控制
    [Description("分区4水平粗控共用控制12V")]
    public float Zone4HorizontalCoarseControlSharedControl12V { get; set; }

    // 分区4水平粗控风扇
    [Description("分区4水平粗控风扇12V")]
    public float Zone4HorizontalCoarseControlFan12V { get; set; }

    #endregion 公用和水平粗控风扇



    #endregion 水平粗控,公用和水平粗控风扇 分区4

    #region 水平粗控,公用和水平粗控风扇 分区5

    #region 水平粗控

    // 通道1分区5水平粗控
    [Description("通道1分区5水平粗控5V")]
    public float Channel1Zone5HorizontalCoarseControl5V { get; set; }

    [Description("通道1分区5水平粗控-12V")]
    public float Channel1Zone5HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道1分区5水平粗控15V")]
    public float Channel1Zone5HorizontalCoarseControl15V { get; set; }

    // 通道2分区5水平粗控电压
    [Description("通道2分区5水平粗控5V")]
    public float Channel2Zone5HorizontalCoarseControl5V { get; set; }

    [Description("通道2分区5水平粗控-12V")]
    public float Channel2Zone5HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道2分区5水平粗控15V")]
    public float Channel2Zone5HorizontalCoarseControl15V { get; set; }

    // 通道3分区5水平粗控电压
    [Description("通道3分区5水平粗控5V")]
    public float Channel3Zone5HorizontalCoarseControl5V { get; set; }

    [Description("通道3分区5水平粗控-12V")]
    public float Channel3Zone5HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道3分区5水平粗控15V")]
    public float Channel3Zone5HorizontalCoarseControl15V { get; set; }

    // 通道4分区5水平粗控电压
    [Description("通道4分区5水平粗控5V")]
    public float Channel4Zone5HorizontalCoarseControl5V { get; set; }

    [Description("通道4分区5水平粗控-12V")]
    public float Channel4Zone5HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道4分区5水平粗控15V")]
    public float Channel4Zone5HorizontalCoarseControl15V { get; set; }

    // 通道5分区5水平粗控电压
    [Description("通道5分区5水平粗控5V")]
    public float Channel5Zone5HorizontalCoarseControl5V { get; set; }

    [Description("通道5分区5水平粗控-12V")]
    public float Channel5Zone5HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道5分区5水平粗控15V")]
    public float Channel5Zone5HorizontalCoarseControl15V { get; set; }

    // 通道6分区5水平粗控电压
    [Description("通道6分区5水平粗控5V")]
    public float Channel6Zone5HorizontalCoarseControl5V { get; set; }

    [Description("通道6分区5水平粗控-12V")]
    public float Channel6Zone5HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道6分区5水平粗控15V")]
    public float Channel6Zone5HorizontalCoarseControl15V { get; set; }

    #endregion 水平粗控

    #region 公用和水平粗控风扇

    // 分区5水平粗控共用供电
    [Description("分区5水平粗控共用供电12V")]
    public float Zone5HorizontalCoarseControlSharedPower12V { get; set; }

    // 分区5水平粗控共用控制
    [Description("分区5水平粗控共用控制12V")]
    public float Zone5HorizontalCoarseControlSharedControl12V { get; set; }

    // 分区5水平粗控风扇
    [Description("分区5水平粗控风扇12V")]
    public float Zone5HorizontalCoarseControlFan12V { get; set; }

    #endregion 公用和水平粗控风扇



    #endregion 水平粗控,公用和水平粗控风扇 分区5

    #region 水平粗控,公用和水平粗控风扇 分区6

    #region 水平粗控

    // 通道1分区6水平粗控
    [Description("通道1分区6水平粗控5V")]
    public float Channel1Zone6HorizontalCoarseControl5V { get; set; }

    [Description("通道1分区6水平粗控-12V")]
    public float Channel1Zone6HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道1分区6水平粗控15V")]
    public float Channel1Zone6HorizontalCoarseControl15V { get; set; }

    // 通道2分区6水平粗控电压
    [Description("通道2分区6水平粗控5V")]
    public float Channel2Zone6HorizontalCoarseControl5V { get; set; }

    [Description("通道2分区6水平粗控-12V")]
    public float Channel2Zone6HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道2分区6水平粗控15V")]
    public float Channel2Zone6HorizontalCoarseControl15V { get; set; }

    // 通道3分区6水平粗控电压
    [Description("通道3分区6水平粗控5V")]
    public float Channel3Zone6HorizontalCoarseControl5V { get; set; }

    [Description("通道3分区6水平粗控-12V")]
    public float Channel3Zone6HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道3分区6水平粗控15V")]
    public float Channel3Zone6HorizontalCoarseControl15V { get; set; }

    // 通道4分区6水平粗控电压
    [Description("通道4分区6水平粗控5V")]
    public float Channel4Zone6HorizontalCoarseControl5V { get; set; }

    [Description("通道4分区6水平粗控-12V")]
    public float Channel4Zone6HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道4分区6水平粗控15V")]
    public float Channel4Zone6HorizontalCoarseControl15V { get; set; }

    // 通道5分区6水平粗控电压
    [Description("通道5分区6水平粗控5V")]
    public float Channel5Zone6HorizontalCoarseControl5V { get; set; }

    [Description("通道5分区6水平粗控-12V")]
    public float Channel5Zone6HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道5分区6水平粗控15V")]
    public float Channel5Zone6HorizontalCoarseControl15V { get; set; }

    // 通道6分区6水平粗控电压
    [Description("通道6分区6水平粗控5V")]
    public float Channel6Zone6HorizontalCoarseControl5V { get; set; }

    [Description("通道6分区6水平粗控-12V")]
    public float Channel6Zone6HorizontalCoarseControlNegative12V { get; set; }

    [Description("通道6分区6水平粗控15V")]
    public float Channel6Zone6HorizontalCoarseControl15V { get; set; }

    #endregion 水平粗控

    #region 公用和水平粗控风扇

    // 分区6水平粗控共用供电
    [Description("分区6水平粗控共用供电12V")]
    public float Zone6HorizontalCoarseControlSharedPower12V { get; set; }

    // 分区6水平粗控共用控制
    [Description("分区6水平粗控共用控制12V")]
    public float Zone6HorizontalCoarseControlSharedControl12V { get; set; }

    // 分区6水平粗控风扇
    [Description("分区6水平粗控风扇12V")]
    public float Zone6HorizontalCoarseControlFan12V { get; set; }

    #endregion 公用和水平粗控风扇

    #endregion 水平粗控,公用和水平粗控风扇 分区6

    #region 水平粗控控制 分区1

    // 通道1分区1水平粗控控制
    [Description("通道1分区1水平粗控控制12V")]
    public float Channel1Zone1HorizontalCoarseControlControl12V { get; set; }

    // 通道2分区1水平粗控控制
    [Description("通道2分区1水平粗控控制12V")]
    public float Channel2Zone1HorizontalCoarseControlControl12V { get; set; }

    // 通道3分区1水平粗控控制
    [Description("通道3分区1水平粗控控制12V")]
    public float Channel3Zone1HorizontalCoarseControlControl12V { get; set; }

    // 通道4分区1水平粗控控制
    [Description("通道4分区1水平粗控控制12V")]
    public float Channel4Zone1HorizontalCoarseControlControl12V { get; set; }

    // 通道5分区1水平粗控控制
    [Description("通道5分区1水平粗控控制12V")]
    public float Channel5Zone1HorizontalCoarseControlControl12V { get; set; }

    // 通道6分区1水平粗控控制
    [Description("通道6分区1水平粗控控制12V")]
    public float Channel6Zone1HorizontalCoarseControlControl12V { get; set; }

    // 分区1水平粗控控制共用
    [Description("分区1水平粗控控制共用12V")]
    public float Zone1HorizontalCoarseControlCommonControl12V { get; set; }

    // 分区1水平粗控控制风扇
    [Description("分区1水平粗控控制风扇12V")]
    public float Zone1HorizontalCoarseControlFanControl12V { get; set; }

    #endregion 水平粗控控制 分区1

    #region 水平粗控控制 分区2

    // 通道1分区2水平粗控控制
    [Description("通道1分区2水平粗控控制12V")]
    public float Channel1Zone2HorizontalCoarseControlControl12V { get; set; }

    // 通道2分区2水平粗控控制
    [Description("通道2分区2水平粗控控制12V")]
    public float Channel2Zone2HorizontalCoarseControlControl12V { get; set; }

    // 通道3分区2水平粗控控制
    [Description("通道3分区2水平粗控控制12V")]
    public float Channel3Zone2HorizontalCoarseControlControl12V { get; set; }

    // 通道4分区2水平粗控控制
    [Description("通道4分区2水平粗控控制12V")]
    public float Channel4Zone2HorizontalCoarseControlControl12V { get; set; }

    // 通道5分区2水平粗控控制
    [Description("通道5分区2水平粗控控制12V")]
    public float Channel5Zone2HorizontalCoarseControlControl12V { get; set; }

    // 通道6分区2水平粗控控制
    [Description("通道6分区2水平粗控控制12V")]
    public float Channel6Zone2HorizontalCoarseControlControl12V { get; set; }

    // 分区2水平粗控控制共用
    [Description("分区2水平粗控控制共用12V")]
    public float Zone2HorizontalCoarseControlCommonControl12V { get; set; }

    // 分区2水平粗控控制风扇
    [Description("分区2水平粗控控制风扇12V")]
    public float Zone2HorizontalCoarseControlFanControl12V { get; set; }

    #endregion 水平粗控控制 分区2

    #region 水平粗控控制 分区3

    // 通道1分区3水平粗控控制
    [Description("通道1分区3水平粗控控制12V")]
    public float Channel1Zone3HorizontalCoarseControlControl12V { get; set; }

    // 通道2分区3水平粗控控制
    [Description("通道2分区3水平粗控控制12V")]
    public float Channel2Zone3HorizontalCoarseControlControl12V { get; set; }

    // 通道3分区3水平粗控控制
    [Description("通道3分区3水平粗控控制12V")]
    public float Channel3Zone3HorizontalCoarseControlControl12V { get; set; }

    // 通道4分区3水平粗控控制
    [Description("通道4分区3水平粗控控制12V")]
    public float Channel4Zone3HorizontalCoarseControlControl12V { get; set; }

    // 通道5分区3水平粗控控制
    [Description("通道5分区3水平粗控控制12V")]
    public float Channel5Zone3HorizontalCoarseControlControl12V { get; set; }

    // 通道6分区3水平粗控控制
    [Description("通道6分区3水平粗控控制12V")]
    public float Channel6Zone3HorizontalCoarseControlControl12V { get; set; }

    // 分区3水平粗控控制共用
    [Description("分区3水平粗控控制共用12V")]
    public float Zone3HorizontalCoarseControlCommonControl12V { get; set; }

    // 分区3水平粗控控制风扇
    [Description("分区3水平粗控控制风扇12V")]
    public float Zone3HorizontalCoarseControlFanControl12V { get; set; }

    #endregion 水平粗控控制 分区3

    #region 水平粗控控制 分区4

    // 通道1分区4水平粗控控制
    [Description("通道1分区4水平粗控控制12V")]
    public float Channel1Zone4HorizontalCoarseControlControl12V { get; set; }

    // 通道2分区4水平粗控控制
    [Description("通道2分区4水平粗控控制12V")]
    public float Channel2Zone4HorizontalCoarseControlControl12V { get; set; }

    // 通道3分区4水平粗控控制
    [Description("通道3分区4水平粗控控制12V")]
    public float Channel3Zone4HorizontalCoarseControlControl12V { get; set; }

    // 通道4分区4水平粗控控制
    [Description("通道4分区4水平粗控控制12V")]
    public float Channel4Zone4HorizontalCoarseControlControl12V { get; set; }

    // 通道5分区4水平粗控控制
    [Description("通道5分区4水平粗控控制12V")]
    public float Channel5Zone4HorizontalCoarseControlControl12V { get; set; }

    // 通道6分区4水平粗控控制
    [Description("通道6分区4水平粗控控制12V")]
    public float Channel6Zone4HorizontalCoarseControlControl12V { get; set; }

    // 分区4水平粗控控制共用
    [Description("分区4水平粗控控制共用12V")]
    public float Zone4HorizontalCoarseControlCommonControl12V { get; set; }

    // 分区4水平粗控控制风扇
    [Description("分区4水平粗控控制风扇12V")]
    public float Zone4HorizontalCoarseControlFanControl12V { get; set; }

    #endregion 水平粗控控制 分区4

    #region 水平粗控控制 分区5

    // 通道1分区5水平粗控控制
    [Description("通道1分区5水平粗控控制12V")]
    public float Channel1Zone5HorizontalCoarseControlControl12V { get; set; }

    // 通道2分区5水平粗控控制
    [Description("通道2分区5水平粗控控制12V")]
    public float Channel2Zone5HorizontalCoarseControlControl12V { get; set; }

    // 通道3分区5水平粗控控制
    [Description("通道3分区5水平粗控控制12V")]
    public float Channel3Zone5HorizontalCoarseControlControl12V { get; set; }

    // 通道4分区5水平粗控控制
    [Description("通道4分区5水平粗控控制12V")]
    public float Channel4Zone5HorizontalCoarseControlControl12V { get; set; }

    // 通道5分区5水平粗控控制
    [Description("通道5分区5水平粗控控制12V")]
    public float Channel5Zone5HorizontalCoarseControlControl12V { get; set; }

    // 通道6分区5水平粗控控制
    [Description("通道6分区5水平粗控控制12V")]
    public float Channel6Zone5HorizontalCoarseControlControl12V { get; set; }

    // 分区5水平粗控控制共用
    [Description("分区5水平粗控控制共用12V")]
    public float Zone5HorizontalCoarseControlCommonControl12V { get; set; }

    // 分区5水平粗控控制风扇
    [Description("分区5水平粗控控制风扇12V")]
    public float Zone5HorizontalCoarseControlFanControl12V { get; set; }

    #endregion 水平粗控控制 分区5

    #region 水平粗控控制 分区6

    // 通道1分区6水平粗控控制
    [Description("通道1分区6水平粗控控制12V")]
    public float Channel1Zone6HorizontalCoarseControlControl12V { get; set; }

    // 通道2分区6水平粗控控制
    [Description("通道2分区6水平粗控控制12V")]
    public float Channel2Zone6HorizontalCoarseControlControl12V { get; set; }

    // 通道3分区6水平粗控控制
    [Description("通道3分区6水平粗控控制12V")]
    public float Channel3Zone6HorizontalCoarseControlControl12V { get; set; }

    // 通道4分区6水平粗控控制
    [Description("通道4分区6水平粗控控制12V")]
    public float Channel4Zone6HorizontalCoarseControlControl12V { get; set; }

    // 通道5分区6水平粗控控制
    [Description("通道5分区6水平粗控控制12V")]
    public float Channel5Zone6HorizontalCoarseControlControl12V { get; set; }

    // 通道6分区6水平粗控控制
    [Description("通道6分区6水平粗控控制12V")]
    public float Channel6Zone6HorizontalCoarseControlControl12V { get; set; }

    // 分区6水平粗控控制共用
    [Description("分区6水平粗控控制共用12V")]
    public float Zone6HorizontalCoarseControlCommonControl12V { get; set; }

    // 分区6水平粗控控制风扇
    [Description("分区6水平粗控控制风扇12V")]
    public float Zone6HorizontalCoarseControlFanControl12V { get; set; }

    #endregion 水平粗控控制 分区6

    #endregion 水平 

    #endregion

    [Description("运行时间")]
    public uint? RunTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public int CreatorLevel { get; set; } = 0;

#if DEBUG
    public static XT_307_SL_1_ReceiveData[] Seeds
    {
        get
        {
            List<XT_307_SL_1_ReceiveData> list = [];
            List<string> uuids = [
                "0198BFBC-2382-762B-8C99-AD1E7FFC047B",
                "0198BFBC-2382-762B-8C99-B316283CF7B7",
                "0198BFBC-2382-762B-8C99-B641B482AF27",
                "0198BFBC-2382-762B-8C99-B8EE325C8EAC",
                "0198BFBC-2382-762B-8C99-BDF08284F050",
                "0198BFBC-2382-762B-8C99-C1757578755F",
                "0198BFBC-2382-762B-8C99-C45F7FD09501",
                "0198BFBC-2382-762B-8C99-C9A015AB7406",
                "0198BFBC-2382-762B-8C99-CDDC14CF275A",
                "0198BFBC-2382-762B-8C99-D26CC8F98D08",
                "0198BFBC-2382-762B-8C99-D6C0AB71FA2A",
                "0198BFBC-2382-762B-8C99-D986267A162D",
                "0198BFBC-2382-762B-8C99-DCD1C9E91CEA",
                "0198BFBC-2382-762B-8C99-E115176A5BF4",
                "0198BFBC-2382-762B-8C99-E5DC6A8CC042",
                "0198BFBC-2382-762B-8C99-E8C822AC5DB4",
                "0198BFBC-2382-762B-8C99-ED203932858B",
                "0198BFBC-2382-762B-8C99-F13916C88A89",
                "0198BFBC-2382-762B-8C99-F4EBC7BAAE9A",
                "0198BFBC-2382-762B-8C99-F893F27DF255",
                "0198BFBC-2382-762B-8C99-FC310E176296",
                "0198BFBC-2382-762B-8C9A-03499F013EDA",
                "0198BFBC-2382-762B-8C9A-068644E74446",
                "0198BFBC-2382-762B-8C9A-0AB327526FEE",
                "0198BFBC-2382-762B-8C9A-0F6910A2320A",
                "0198BFBC-2382-762B-8C9A-1104B2C3BC7A",
                "0198BFBC-2382-762B-8C9A-144A5B602297",
                "0198BFBC-2382-762B-8C9A-1826B6C55706",
                "0198BFBC-2382-762B-8C9A-1DAC067921FE",
                "0198BFBC-2382-762B-8C9A-233BB40FD271",
                "0198BFBC-2382-762B-8C9A-24EF36CE907D",
                "0198BFBC-2382-762B-8C9A-2886110DDDD7",
                "0198BFBC-2382-762B-8C9A-2D51A4CE4D07",
                "0198BFBC-2383-7058-8BB1-ACBFEFFBA2C1",
                "0198BFBC-2383-7058-8BB1-B356C1977905",
                "0198BFBC-2383-7058-8BB1-B72E9F540C7C",
                "0198BFBC-2383-7058-8BB1-BBF8C91A276D",
                "0198BFBC-2383-7058-8BB1-BC5761EEEADE",
                "0198BFBC-2383-7058-8BB1-C2060E57821E",
                "0198BFBC-2383-7058-8BB1-C61F1E7861F6",
                "0198BFBC-2383-7058-8BB1-C8929556434A",
                "0198BFBC-2383-7058-8BB1-CD7E5CEB0410",
                "0198BFBC-2383-7058-8BB1-D2348890D991",
                "0198BFBC-2383-7058-8BB1-D5DD5EEFB527",
                "0198BFBC-2383-7058-8BB1-DBBCA29CB2A7",
                "0198BFBC-2383-7058-8BB1-DC59CE2B4B12",
                "0198BFBC-2383-7058-8BB1-E0BA6859329A",
                "0198BFBC-2383-7058-8BB1-E6024D445BE5",
                "0198BFBC-2383-7058-8BB1-E9D24E67FBB9",
                "0198BFBC-2383-7058-8BB1-EDB572144A34",
                "0198BFBC-2383-7058-8BB1-F0180841F9F7",
                "0198BFBC-2383-7058-8BB1-F7B97BA7F964",
                "0198BFBC-2383-7058-8BB1-F9B70FE5EC18",
                "0198BFBC-2383-7058-8BB1-FEB236CBDC27",
                "0198BFBC-2383-7058-8BB2-03F34DF9E1C2",
                "0198BFBC-2383-7058-8BB2-049BDAB7E610",
                "0198BFBC-2383-7058-8BB2-0B0889C48E18",
                "0198BFBC-2383-7058-8BB2-0F11F6561DEB",
                "0198BFBC-2383-7058-8BB2-10A30D318950",
                "0198BFBC-2383-7058-8BB2-16BC5C41EF02",
                "0198BFBC-2383-7058-8BB2-1B471198CE5A",
                "0198BFBC-2383-7058-8BB2-1D983040011E",
                "0198BFBC-2383-7058-8BB2-2091633B0D7F",
                "0198BFBC-2383-7058-8BB2-244A9E60DC72",
                "0198BFBC-2383-7058-8BB2-29D33ECEF404",
                "0198BFBC-2383-7058-8BB2-2C0695BDEF82",
                "0198BFBC-2383-7058-8BB2-3148FA36F817",
                "0198BFBC-2383-7058-8BB2-342EFA3EDCFC",
                "0198BFBC-2383-7058-8BB2-38D7CE5DBE98",
                "0198BFBC-2383-7058-8BB2-3E36566002DE",
                "0198BFBC-2383-7058-8BB2-404334898304",
                "0198BFBC-2383-7058-8BB2-45953BA29AE3",
                "0198BFBC-2383-7058-8BB2-487ED59A36DC",
                "0198BFBC-2383-7058-8BB2-4EDD14987C10",
                "0198BFBC-2383-7058-8BB2-506163C37764",
                "0198BFBC-2383-7058-8BB2-56B3E9476FAA",
                "0198BFBC-2383-7058-8BB2-582FDCDBD908",
                "0198BFBC-2383-7058-8BB2-5D6ADED9B03F",
                "0198BFBC-2383-7058-8BB2-6023560DB433",
                "0198BFBC-2383-7058-8BB2-66D46A4FA9AE",
                "0198BFBC-2383-7058-8BB2-6BA0233B4DCD",
                "0198BFBC-2383-7058-8BB2-6E7DEABA62ED",
                "0198BFBC-2383-7058-8BB2-72FC04AAC627",
                "0198BFBC-2383-7058-8BB2-7535BFAC0DA9",
                "0198BFBC-2383-7058-8BB2-7AACE3F85E9B",
                "0198BFBC-2383-7058-8BB2-7FF25A4CFC45",
                "0198BFBC-2383-7058-8BB2-80992183A6F9",
                "0198BFBC-2383-7058-8BB2-86DBF455A0D0",
                "0198BFBC-2383-7058-8BB2-8832E34FBEDB",
                "0198BFBC-2383-7058-8BB2-8ECA07BDDAFF",
                "0198BFBC-2383-7058-8BB2-93FDA1B3B54F",
                "0198BFBC-2383-7058-8BB2-9515777931DE",
                "0198BFBC-2383-7058-8BB2-984A550B5FE8",
                "0198BFBC-2383-7058-8BB2-9DA01030AC32",
                "0198BFBC-2383-7058-8BB2-A158EC114609",
                "0198BFBC-2383-7058-8BB2-A42AC6ED09C3",
                "0198BFBC-2383-7058-8BB2-A968296A4794",
                "0198BFBC-2383-7058-8BB2-ACF62F2AE834",
                "0198BFBC-2383-7058-8BB2-B0C0AA02EFBC",
                "0198BFBC-2383-7058-8BB2-B5C0B018CE86"
            ];
            List<string> equipUuids = [
                "0198BBB27F42727EB6F4A1AAA67A828B",
                "0198BBB27F42727EB6F4A68D9702EFE6",
                "0198BBB27F42727EB6F4AB3FBC007942",
                "0198BBB27F42727EB6F4ACE6DF594ABC",
                "0198BBB27F42727EB6F4B382B5DAB7E5",
                "0198BBB27F42727EB6F4B5169125885C",
                "0198BBB27F42727EB6F4BB0D2FB237AE",
                "0198BBB27F42727EB6F4BC1D86A9055D",
                "0198BBB27F42727EB6F4C3CF4DB5D446",
                "0198BBB27F42727EB6F4C7C00A3A738E"
            ];
            var rand = new Random();
            var now = DateTime.Now;
            int a = 0;
            List<Attribute> attributes = [];
            Type? type = null;
            Dictionary<PropertyInfo, int> properties = [];
            foreach (var uuid in uuids)
            {
                var b = Activator.CreateInstance<XT_307_SL_1_ReceiveData>();
                type ??= b.GetType();
                if (!properties.Any())
                {
                    var tempProperties = type.GetProperties();
                    foreach (var property in tempProperties)
                    {
                        var attrs = property.GetCustomAttributes();
                        foreach (var attr in attrs)
                        {
                            if (attr is DescriptionAttribute descattr)
                            {
                                if (descattr.Description.Contains("-5V"))
                                {
                                    properties.Add(property, -5);
                                    break;
                                }
                                else if (descattr.Description.Contains("-12V"))
                                {
                                    properties.Add(property, -12);
                                    break;
                                }
                                else if (descattr.Description.Contains("5V"))
                                {
                                    properties.Add(property, 5);
                                    break;
                                }
                                else if (descattr.Description.Contains("12V"))
                                {
                                    properties.Add(property, 12);
                                    break;
                                }
                            }
                        }
                    }
                }
                
                b.Id = Guid.Parse(uuid);
                b.SimuTestSysld = 2;
                b.DevTypeld = 1;
                b.Compld = equipUuids[rand.Next(equipUuids.Count - 1)];
                b.MicroWare = 1;
                b.Channel = 1;
                b.ModelValid = 1;
                b.ArrayEndPolarizationMode = 1;
                b.ArrayEndPowerMode = 1;
                b.ArrayChannelMultiplexing = 1;
                b.ChannelPolarizationMode1 = 1;
                b.ChannelPolarizationMode2 = 1;
                b.ChannelPowerMode = 1;
                b.Reserved = 1;
                b.StateType = 1;
                b.SelfTest = 1;
                b.SupplyVoltageState = 1;
                b.PhysicalQuantityCount = 462;
                b.RunTime = (uint?)new DateTimeOffset(now.AddSeconds(a)).ToUnixTimeSeconds();
                b.CreationTime = now.AddSeconds(a);
                foreach (var dic in properties)
                {
                    dic.Key.SetValue(b, dic.Value);
                }
                list.Add(b);
                a++;
            }
            return [.. list];
        }
    }
#endif
}
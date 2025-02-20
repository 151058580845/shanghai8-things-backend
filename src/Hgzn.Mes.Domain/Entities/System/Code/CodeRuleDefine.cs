using Hgzn.Mes.Domain.Entities.Base;
using System.ComponentModel;

namespace Hgzn.Mes.Domain.Entities.System.Code;

public class CodeRuleDefine : UniversalEntity, IOrder
{
    [Description("编码规则")]
    public Guid CodeRuleId { get; set; }

    [Description("类型")]
    public string? CodeRuleType { get; set; }

    [Description("排序号")]
    public int OrderNum { get; set; }

    [Description("流水长度")]
    public int? MaxFlow { get; set; }

    [Description("当前流水号")]
    public int? NowFlow { get; set; }

    [Description("原始流水号")]
    public int? InitialNowFlow { get; set; }

    [Description("确认当前编码被应用")]
    public bool? NowFlowIsSure { get; set; }

    [Description("补位符")]
    public char? CodeCover { get; set; }

    [Description("日期格式")]
    public string? DateFormat { get; set; }

    [Description("常量")]
    public string? ConstantChar { get; set; }

    [Description("元素键值")]
    public string? SourceKey { get; set; }

    [Description("元素属性")]
    public string? SourceValue { get; set; }

    #region static CodeRuleDefine

    public static readonly CodeRuleDefine EquipmentLedgerDefineByConstant = new()
    {
        Id = new Guid("7197E4CE-98E8-ACD9-5D85-C5F139AE3453"),
        CodeRuleId = new Guid("80CAD9AD-8473-7FC7-CA56-057DBB91448B"),
        CodeRuleType = "Constant",
        ConstantChar = "TEX",
        OrderNum = 1
    };

    public static readonly CodeRuleDefine EquipmentLedgerDefineByDate = new()
    {
        Id = new Guid("C444DF4E-DC26-4922-732E-C18D99952823"),
        CodeRuleId = new Guid("80CAD9AD-8473-7FC7-CA56-057DBB91448B"),
        CodeRuleType = "Date",
        DateFormat = "yyyyMMddmmss"
    };

    public static readonly CodeRuleDefine EquipmentLedgerDefineBySerialNumber = new()
    {
        Id = new Guid("B10CD4B1-6037-D34D-C3E2-F5C1DDB8829A"),
        CodeRuleId = new Guid("80CAD9AD-8473-7FC7-CA56-057DBB91448B"),
        CodeRuleType = "SerialNumber",
        OrderNum = 0,
        MaxFlow = 17,
        NowFlow = 20536,
        InitialNowFlow= 20536,
        CodeCover = '*'
    };


    public static readonly CodeRuleDefine EquipmentParameterDefineByConstant = new()
    {
        Id = new Guid("6A05FDAB-9468-1E69-7DCF-CEF7ACDF1EB7"),
        CodeRuleId = new Guid("9BB2FF2E-4907-57C3-8DEE-EA526CB9C844"),
        CodeRuleType = "Constant",
        ConstantChar = "TEX",
        OrderNum = 1
    };


    public static readonly CodeRuleDefine EquipmentParameterDefineByDate = new()
    {
        Id = new Guid("5D40EDAA-8B05-4DBA-651F-BFEDF95082E6"),
        CodeRuleId = new Guid("9BB2FF2E-4907-57C3-8DEE-EA526CB9C844"),
        CodeRuleType = "Date",
        DateFormat = "yyyyMMddmmss"
    };

    public static readonly CodeRuleDefine EquipmentParameterDefineBySerialNumber = new()
    {
        Id = new Guid("5FC96497-7D0F-FCE9-B97C-7351087D5593"),
        CodeRuleId = new Guid("9BB2FF2E-4907-57C3-8DEE-EA526CB9C844"),
        CodeRuleType = "SerialNumber",
        OrderNum = 0,
        MaxFlow = 17,
        NowFlow = 20536,
        InitialNowFlow = 20536,
        CodeCover = '*'
    };

    #endregion

    public static CodeRuleDefine[] Seeds { get; } =
      [
           EquipmentLedgerDefineByConstant,
            EquipmentLedgerDefineByDate,
        EquipmentParameterDefineByConstant,
        EquipmentParameterDefineByDate,
        EquipmentParameterDefineBySerialNumber,
        EquipmentLedgerDefineBySerialNumber
       ];
}
using Hgzn.Mes.Domain.Entities.Base;
using System.ComponentModel;

namespace Hgzn.Mes.Domain.Entities.System.Code;

public class CodeRule : UniversalEntity, ISoftDelete, IState, IOrder, ISeedsGeneratable
{
    [Description("创建时间")]
    public DateTime CreationTime { get; set; }

    [Description("创建者ID")]
    public Guid? CreatorId { get; set; }

    [Description("最后修改者ID")]
    public Guid? LastModifierId { get; set; }

    [Description("最后修改时间")]
    public DateTime? LastModificationTime { get; set; }

    [Description("排序")]
    public int OrderNum { get; set; }

    [Description("状态")]
    public bool State { get; set; }

    [Description("规则名称")]
    public string CodeName { get; set; } = null!;

    [Description("规则编号")]
    public string? CodeNumber { get; set; }

    //[Description("基础元素")]
    //public string? BasicDomain { get; set; }

    [Description("备注")]
    public string? Remark { get; set; }

    [Description("规则列表")]
    public List<CodeRuleDefine>? CodeRuleRules { get; set; }

    #region delete filter

    [Description("软删除标志")]
    public bool SoftDeleted { get; set; } = false;

    [Description("删除时间")]
    public DateTime? DeleteTime { get; set; } = null;

    #endregion delete filter


    #region static CodeRule

    /// <summary>
    /// 设备台账种子数据
    /// </summary>
    public static readonly CodeRule EquipmentLedgerCodeRule = new()
    {
        Id = new Guid("80CAD9AD-8473-7FC7-CA56-057DBB91448B"),
        OrderNum = 1,
        State = true,
        CodeName = "设备台账",
        CodeNumber = "SBTZ",
        CodeRuleRules = new List<CodeRuleDefine>() {
          new CodeRuleDefine(){
            Id=new Guid("B10CD4B1-6037-D34D-C3E2-F5C1DDB8829A"),
            CodeRuleId=new Guid("80CAD9AD-8473-7FC7-CA56-057DBB91448B"),
            CodeRuleType = "SerialNumber",
            OrderNum=0,
            MaxFlow=17,
            NowFlow=20536,
            CodeCover = '*'
          },
          new CodeRuleDefine(){
              Id=new Guid("7197E4CE-98E8-ACD9-5D85-C5F139AE3453"),
              CodeRuleId=new Guid("80CAD9AD-8473-7FC7-CA56-057DBB91448B"),
              CodeRuleType = "Constant",
              ConstantChar="TEX",
              OrderNum=1
          },
            new CodeRuleDefine(){
              Id=new Guid("C444DF4E-DC26-4922-732E-C18D99952823"),
              CodeRuleId=new Guid("80CAD9AD-8473-7FC7-CA56-057DBB91448B"),
              CodeRuleType = "Date",
              ConstantChar="yyyy-MM-dd"
          }

        }
    };


    /// <summary>
    /// 设备参数
    /// </summary>
    public static readonly CodeRule EquipmentParameterCodeRule = new()
    {
        Id = new Guid("9BB2FF2E-4907-57C3-8DEE-EA526CB9C844"),
        OrderNum = 1,
        State = true,
        CodeName = "设备参数",
        CodeNumber = "SBCS",
        CodeRuleRules = new List<CodeRuleDefine>() {
          new CodeRuleDefine(){
            Id=new Guid("5FC96497-7D0F-FCE9-B97C-7351087D5593"),
            CodeRuleId=new Guid("9BB2FF2E-4907-57C3-8DEE-EA526CB9C844"),
            CodeRuleType = "SerialNumber",
            OrderNum=0,
            MaxFlow=17,
            NowFlow=20536,
            CodeCover = '*'
          },
          new CodeRuleDefine(){
              Id=new Guid("6A05FDAB-9468-1E69-7DCF-CEF7ACDF1EB7"),
              CodeRuleId=new Guid("9BB2FF2E-4907-57C3-8DEE-EA526CB9C844"),
              CodeRuleType = "Constant",
              ConstantChar="TEX",
              OrderNum=1
          },
            new CodeRuleDefine(){
              Id=new Guid("5D40EDAA-8B05-4DBA-651F-BFEDF95082E6"),
              CodeRuleId=new Guid("9BB2FF2E-4907-57C3-8DEE-EA526CB9C844"),
              CodeRuleType = "Date",
              ConstantChar="yyyy-MM-dd"
          }

        }
    };
    #endregion

    public static CodeRule[] Seeds { get; } =
       [
            EquipmentLedgerCodeRule,
            EquipmentParameterCodeRule
        ];
}
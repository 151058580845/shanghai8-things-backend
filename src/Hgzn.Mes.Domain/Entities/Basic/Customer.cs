using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Basic;

[Description("客户管理")]
public class Customer: UniversalEntity, IAudited
{
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }

    public int CreatorLevel { get; set; } = 0;
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    
    [Description("客户编码")]
    public string Code { get; set; } = null!;
    
    [Description("客户名称")]
    public string Name { get; set; } = null!;

    [Description("结算方式")]
    public string SettlementMethod { get; set; } = null!;
    
    [Description("是否包含税")]
    public bool TaxIncluded { get; set; }
    
    [Description("是否包含运费")]
    public bool FreightIncluded { get; set; }
    
    [Description("客户简称")]
    public string? ShortName { get; set; }
    
    [Description("客户英文名称")]
    public string? EnglishName { get; set; }
    
    [Description("客户介绍")]
    public string? Description { get; set; }
    
    [Description("客户官网地址")]
    public string? Website { get; set; }
    
    [Description("客户邮箱")]
    public string? Email { get; set; }
    
    [Description("客户电话")]
    public string? Phone { get; set; }
    
    [Description("社会信用代码")]
    public string? SocialCreditCode { get; set; }
    
    [Description("备注")]
    public string? Remark { get; set; }
    
    public List<Contact>? Contacts { get; set; }
    public List<AddressB>? AddressBs { get; set; }
}
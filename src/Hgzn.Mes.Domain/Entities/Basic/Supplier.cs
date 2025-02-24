using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Basic;


[Description("供应商管理")]
public class Supplier: UniversalEntity, IAudited
{
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    
    [Description("供应商编码")]
    public string Code { get; set; } = null!;
    
    [Description("供应商名称")]
    public string Name { get; set; } = null!;
    
    [Description("供应商英文名称")]
    public string? EnglishName { get; set; }
    
    [Description("供应商类型")]
    public string? Type { get; set; }
    
    [Description("供应商简介")]
    public string? Description { get; set; }
    
    [Description("供应商官网地址")]
    public string? Website { get; set; }
    
    [Description("供应商邮箱")]
    public string? Email { get; set; }
    
    [Description("供应商电话")]
    public string? Phone { get; set; }
    
    [Description("供应商等级")]
    public string? Level { get; set; }
    
    [Description("供应商评分")]
    public decimal? Rating { get; set; }
    
    [Description("社会信用代码")]
    public string? SocialCreditCode { get; set; }
    
    [Description("备注")]
    public string? Remark { get; set; }
    
    public List<Contact>? Contacts { get; set; }
    public List<AddressB>? AddressBs { get; set; }
}
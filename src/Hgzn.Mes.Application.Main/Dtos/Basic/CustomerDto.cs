using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Entities.Basic;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Dtos.Basic
{
    public class CustomerDto
    {
    }

    public class CustomerReadDto : ReadDto {

        [Description("客户编码")]
        public string Code { get; set; }  

        [Description("客户名称")]
        public string Name { get; set; }  

        [Description("结算方式")]
        public string SettlementMethod { get; set; }  

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

        public List<SupplierDetilDto>? ContactEntities { get; set; }
    }

    public class CustomerCreateDto : CreateDto {
        [Description("客户编码")]
        public string Code { get; set; }  

        [Description("客户名称")]
        public string Name { get; set; }  

        [Description("结算方式")]
        public string SettlementMethod { get; set; }  

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

        public List<SupplierDetilDto>? ContactEntities { get; set; }
    }


    public class CustomerUpdateDto : UpdateDto
    {
        [Description("客户编码")]
        public string Code { get; set; }  

        [Description("客户名称")]
        public string Name { get; set; }  

        [Description("结算方式")]
        public string SettlementMethod { get; set; }  

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

        public List<SupplierDetilDto>? ContactEntities { get; set; }
    }

    public class CustomerQueryDto : PaginatedQueryDto
    {
        [Description("客户编码")]
        public string Code { get; set; }  

        [Description("客户名称")]
        public string Name { get; set; }  
    }
}

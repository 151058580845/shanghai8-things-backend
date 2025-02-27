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
    /// <summary>
    /// 供应商DTO
    /// </summary>
    public class SupplierDto
    {
    }

    public class SupplierReadDto : ReadDto
    {

        [Description("供应商编码")]
        public string Code { get; set; } 

        [Description("供应商名称")]
        public string Name { get; set; }

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

        public List<SupplierDetilDto>? ContactEntities { get; set; }
    }

    public class SupplierCreateDto : CreateDto
    {
        [Description("供应商编码")]
        public string Code { get; set; } 

        [Description("供应商名称")]
        public string Name { get; set; }

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

        public List<SupplierDetilDto> ContactEntities { get; set; }

        public List<Contact> Contacts { get; set; } = new List<Contact>();
        public List<AddressB> AddressBs { get; set; } = new List<AddressB>();
    }

    public class SupplierUpdateDto : UpdateDto {
        [Description("供应商编码")]
        public string Code { get; set; }

        [Description("供应商名称")]
        public string Name { get; set; }

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

        public List<SupplierDetilDto> ContactEntities { get; set; }

        public List<Contact> Contacts { get; set; } = new List<Contact>();
        public List<AddressB> AddressBs { get; set; } = new List<AddressB>();
    }

    public class SupplierDetilDto
    {
        public int? OrderNum { get; set; }

        public string? Name { get; set; }

        public string? Address { get; set; }

        public string? Phone { get; set; }

    }

    public class SupplierQueryDto : PaginatedQueryDto {

        [Description("供应商编码")]
        public string Code { get; set; } 

        [Description("供应商名称")]
        public string Name { get; set; } 
    }
}

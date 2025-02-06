using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Code;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Utilities;
using Hgzn.Mes.Infrastructure.Utilities;

using Microsoft.Extensions.Logging;

using System.Text;

namespace Hgzn.Mes.Application.Main.Services.System;

/// <summary>
/// 编码规则
/// </summary>
public class CodeRuleService : SugarCrudAppService<
    CodeRule, Guid,
    CodeRuleReadDto, CodeRuleQueryDto,
    CodeRuleCreateDto, CodeRuleUpdateDto>,
    ICodeRuleService
{
    public CodeRuleService(
            ICodeRuleDefineService codeRuleDefineService,
            ILogger<UserService> logger
        )
    {
        _userDomainService = codeRuleDefineService;
    }


    private readonly ICodeRuleDefineService _userDomainService;

    public override async Task<PaginatedList<CodeRuleReadDto>> GetPaginatedListAsync(CodeRuleQueryDto input)
    {
        var entities = await Queryable
            .Where(x => input.CodeName != null && x.CodeName.Contains(input.CodeName))
            .Where(x => x.CodeNumber != null && input.CodeNumber != null && x.CodeNumber.Contains(input.CodeNumber))
            .Where(x => x.BasicDomain != null && input.BasicDomain != null && x.BasicDomain.Contains(input.BasicDomain))
            .OrderBy(x => x.OrderNum)
            .ToPaginatedListAsync(input.PageIndex, input.PageSize);
        return Mapper.Map<PaginatedList<CodeRuleReadDto>>(entities);
    }

    /// <summary>
    /// 返回对应的编码
    /// </summary>
    /// <param name="codeNumber"></param>
    /// <returns></returns>
    public async Task<string> GenerateCodeByCodeAsync(string codeNumber)
    {
        var code = await Queryable.FirstAsync(t => t.CodeNumber == codeNumber);

        if (code == null)
        {
            return "";
        }
        var codeRules = await DbContext.Queryable<CodeRuleDefine>()
            .Where(t => t.CodeRuleId == code.Id)
            .OrderBy(t => t.OrderNum)
            .ToListAsync();
        if (codeRules.Count < 1)
        {
            return "";
        }
        StringBuilder sb = new StringBuilder();
        try
        {
            foreach (var rule in codeRules)
            {
                rule.CodeRuleId = code.Id;
                switch (rule.CodeRuleType)
                {   
                    case "SerialNumber":
                        rule.NowFlow++;
                        sb.Append((rule.NowFlow).ToString()?.PadLeft((int)rule.MaxFlow!, (char)rule.CodeCover!));
                        rule.NowFlowIsSure = false;
                        break;

                    case "Constant":
                        sb.Append(rule.ConstantChar);
                        break;

                    case "Date":
                        sb.Append(DateTime.Now.ToString(rule.DateFormat));
                        break;

                    case "AttributeValue":
                        break;
                }
            }
            return sb.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// 获取数据列表
    /// </summary>
    /// <param name="queryDto"></param>
    /// <returns></returns>
    public override async Task<IEnumerable<CodeRuleReadDto>> GetListAsync(CodeRuleQueryDto? queryDto)
    {
        var users = await Queryable
        .WhereIF(!string.IsNullOrEmpty(queryDto.CodeName), x => x.CodeName.Contains(queryDto.CodeName))
        .WhereIF(!string.IsNullOrEmpty(queryDto.CodeNumber), x => x.CodeNumber.Contains(queryDto.CodeNumber))
        .WhereIF(!string.IsNullOrEmpty(queryDto.BasicDomain), x => x.BasicDomain.Contains(queryDto.BasicDomain))
        .OrderBy(x => x.OrderNum)
        .ToArrayAsync();

        return Mapper.Map<IEnumerable<CodeRuleReadDto>>(users);
    }

    /// <summary>
    /// 获取实例
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public override async Task<CodeRuleReadDto?> GetAsync(Guid key)
    {
        var user = await Queryable
                 .Where(u => u.Id == key)
                 .Includes(u => u.CodeRuleRules)
                 .FirstAsync();

        return Mapper.Map<CodeRuleReadDto>(user);
    }

    /// <summary>
    /// 创建编码规则
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public override async Task<CodeRuleReadDto> CreateAsync(CodeRuleCreateDto dto)
    {
        var entity = await base.CreateAsync(dto);
        foreach (var rule in dto.codeRuleRules)
        {
            rule.CodeRuleId = entity.Id;
            if (rule.Id.IsGuidEmpty())
            {
                rule.Id = new Guid();
            }
        }

        // 修改编码
        var list = new List<CodeRuleDefine>();
        list.AddRange(dto.codeRuleRules.Select(id => new CodeRuleDefine() { 
          CodeCover= id.CodeCover,
            CodeRuleId = id.CodeRuleId.Value,
            CodeRuleType = id.CodeRuleType,
            OrderNum = id.OrderNum,
            MaxFlow = id.MaxFlow,
            NowFlow = id.NowFlow,
            DateFormat = id.DateFormat,
            ConstantChar = id.ConstantChar,
            SourceKey = id.SourceKey,
            SourceValue = id.SourceValue,

        }));
        await ModifyCodeRuleDefine(entity.Id, list);

        return entity;
    }

    /// <summary>
    /// 修改编码规则
    /// </summary>
    /// <param name="key"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async override Task<CodeRuleReadDto?> UpdateAsync(Guid key, CodeRuleUpdateDto dto)
    {
        var entity = await base.UpdateAsync(key,dto);
        foreach (var rule in dto.CodeRuleDefines)
        {
            rule.CodeRuleId = entity.Id;
            if (rule.Id.IsGuidEmpty())
            {
                rule.Id = new Guid();
            }
        }

        // 修改编码
        var list = new List<CodeRuleDefine>();
        list.AddRange(dto.CodeRuleDefines.Select(id => new CodeRuleDefine()
        {
            CodeCover = id.CodeCover,
            CodeRuleId = id.CodeRuleId.Value,
            CodeRuleType = id.CodeRuleType,
            OrderNum = id.OrderNum,
            MaxFlow = id.MaxFlow,
            NowFlow = id.NowFlow,
            DateFormat = id.DateFormat,
            ConstantChar = id.ConstantChar,
            SourceKey = id.SourceKey,
            SourceValue = id.SourceValue,

        }));
        await ModifyCodeRuleDefine(entity.Id, list);

        return entity;
    }
  

    /// <summary>
    /// 修改编码规则
    /// </summary>
    /// <param name="codeRuleId"></param>
    /// <param name="targetsEntities"></param>
    public async Task ModifyCodeRuleDefine(Guid codeRuleId, List<CodeRuleDefine> targetsEntities)
    {
        try
        {
            await DbContext.Ado.BeginTranAsync();
            DbContext.Deleteable<CodeRuleDefine>().Where(t => t.CodeRuleId == codeRuleId);
            DbContext.Insertable(targetsEntities);
            await DbContext.Ado.CommitTranAsync();
        }
        catch
        {
            await DbContext.Ado.RollbackTranAsync();
            throw;
        }
    }


}
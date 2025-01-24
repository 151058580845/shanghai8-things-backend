using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Code;
using Hgzn.Mes.Domain.Shared;
using System.Text;
using Hgzn.Mes.Infrastructure.Utilities;

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

    public override Task<IEnumerable<CodeRuleReadDto>> GetListAsync(CodeRuleQueryDto? queryDto)
    {
        throw new NotImplementedException();
    }
}
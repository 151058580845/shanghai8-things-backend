using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Code;
using Hgzn.Mes.Domain.Shared;

namespace Hgzn.Mes.Application.Main.Services.System;

/// <summary>
/// 编码规则详细内容
/// </summary>
public class CodeRuleDefineDefineService : CrudAppServiceSugar<CodeRuleDefine
        , Guid, CodeRuleDefineQueryDto, CodeRuleDefineReadDto, CodeRuleDefineCreateDto, CodeRuleDefineUpdateDto>,
    ICodeRuleDefineService
{
    public override async Task<PaginatedList<CodeRuleDefineReadDto>> GetListAsync(CodeRuleDefineQueryDto input)
    {
        var entities = await Queryable()
            .Where(x=>x.CodeRuleId == input.CodeRuleId)
            .Where(x=>x.CodeRuleType != null && input.CodeRuleType != null && x.CodeRuleType.Contains(input.CodeRuleType))
            .Where(x=>x.SourceKey != null && input.SourceKey != null && x.SourceKey.Contains(input.SourceKey))
            .Where(x=>x.SourceValue != null && input.SourceValue != null && x.SourceValue.Contains(input.SourceValue))
            .OrderBy(x=>x.OrderNum)
            .ToPageListAsync(input.PageIndex, input.PageSize);
        return Mapper.Map<PaginatedList<CodeRuleDefineReadDto>>(entities);
    }
}
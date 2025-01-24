using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Code;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;

namespace Hgzn.Mes.Application.Main.Services.System;

/// <summary>
/// 编码规则详细内容
/// </summary>
public class CodeRuleDefineService : SugarCrudAppService<
    CodeRuleDefine, Guid,
    CodeRuleDefineReadDto, CodeRuleDefineQueryDto,
    CodeRuleDefineCreateDto, CodeRuleDefineUpdateDto>,
    ICodeRuleDefineService
{
    public override Task<IEnumerable<CodeRuleDefineReadDto>> GetListAsync(CodeRuleDefineQueryDto? queryDto)
    {
        throw new NotImplementedException();
    }

    public override async Task<PaginatedList<CodeRuleDefineReadDto>> GetPaginatedListAsync(CodeRuleDefineQueryDto input)
    {
        var entities = await Queryable
            .Where(x => x.CodeRuleId == input.CodeRuleId)
            .Where(x => x.CodeRuleType != null && input.CodeRuleType != null && x.CodeRuleType.Contains(input.CodeRuleType))
            .Where(x => x.SourceKey != null && input.SourceKey != null && x.SourceKey.Contains(input.SourceKey))
            .Where(x => x.SourceValue != null && input.SourceValue != null && x.SourceValue.Contains(input.SourceValue))
            .OrderBy(x => x.OrderNum)
            .ToPaginatedListAsync(input.PageIndex, input.PageSize);
        return Mapper.Map<PaginatedList<CodeRuleDefineReadDto>>(entities);
    }
}
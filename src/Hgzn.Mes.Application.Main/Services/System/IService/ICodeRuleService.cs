using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Code;

namespace Hgzn.Mes.Application.Main.Services.System.IService;

public interface ICodeRuleService : ICrudAppService<
    CodeRule, Guid,
    CodeRuleReadDto, CodeRuleQueryDto,
    CodeRuleCreateDto, CodeRuleUpdateDto>
{
    Task<string> GenerateCodeByCodeAsync(string codeNumber);
}
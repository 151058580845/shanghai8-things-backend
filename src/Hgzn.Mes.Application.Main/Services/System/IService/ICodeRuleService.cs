using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Code;

namespace Hgzn.Mes.Application.Main.Services.System.IService;

public interface ICodeRuleService : ICrudAppService<
    CodeRule, Guid,
    CodeRuleReadDto, CodeRuleQueryDto,
    CodeRuleCreateDto, CodeRuleUpdateDto>
{
    Task<string> GenerateCodeByCodeAsync(string codeNumber);

    /// <summary>
    /// 修改状态
    /// </summary>
    /// <param name="id"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    Task<CodeRuleReadDto> GetGenerateCodeByCodeAsync(Guid id, bool? state);
}
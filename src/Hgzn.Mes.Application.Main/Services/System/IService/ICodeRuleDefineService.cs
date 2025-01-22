using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Code;

namespace Hgzn.Mes.Application.Main.Services.System.IService;

public interface ICodeRuleDefineService : ICrudAppService<
    CodeRuleDefine, Guid,
    CodeRuleDefineReadDto, CodeRuleDefineQueryDto>
{
}
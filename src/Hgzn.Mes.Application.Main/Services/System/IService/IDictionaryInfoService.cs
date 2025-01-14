using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Dictionary;

namespace Hgzn.Mes.Application.Main.Services.System.IService;

public interface IDictionaryInfoService:ICrudAppService<DictionaryInfo
    ,Guid,DictionaryInfoQueryDto,DictionaryInfoReadDto,DictionaryInfoCreateDto,DictionaryInfoUpdateDto>
{
    Task<List<NameValueDto>> GetNameValueByTypeAsync(string dictLabel);
    Task<List<NameValueDto>> GetNameValueByTypeIdAsync(Guid dictTypeId);
}
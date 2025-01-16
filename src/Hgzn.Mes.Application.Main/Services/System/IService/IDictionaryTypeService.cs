using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Dictionary;

namespace Hgzn.Mes.Application.Main.Services.System.IService;

public interface IDictionaryTypeService : ICrudAppService<DictionaryType
    , Guid, DictionaryTypeQueryDto, DictionaryTypeReadDto, DictionaryTypeCreateDto, DictionaryTypeUpdateDto>
{
}
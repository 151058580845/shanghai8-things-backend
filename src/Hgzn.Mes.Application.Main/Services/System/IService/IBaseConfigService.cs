using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Config;
using Hgzn.Mes.Domain.Entities.System.Location;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.System.IService
{
    public interface IBaseConfigService : ICrudAppService<
    BaseConfig, Guid,
    BaseConfigReadDto, BaseConfigQueryDto,
    BaseConfigCreateDto, BaseConfigUpdateDto>
    {
        Task<string> GetValueByKeyAsync(string key);
    }
}

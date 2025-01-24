using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService
{
    public interface ICollectionConfigService : ICrudAppService<
    CollectionConfig, Guid,
    CollectionConfigReadDto, CollectionConfigQueryDto,
    CollectionConfigCreateDto, CollectionConfigUpdateDto>
    {
    }
}

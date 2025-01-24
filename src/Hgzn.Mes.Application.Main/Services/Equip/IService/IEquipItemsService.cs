using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService
{
    public interface IEquipItemsService : ICrudAppService<
    EquipItems, Guid,
    EquipItemsReadDto, EquipItemsQueryDto,
    EquipItemsCreateDto, EquipItemsUpdateDto>
    {
        Task<EquipItemsEnumReadDto> GetEnumsAsync();
    }
}

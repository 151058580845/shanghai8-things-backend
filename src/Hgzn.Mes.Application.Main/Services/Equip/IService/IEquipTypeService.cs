<<<<<<< Updated upstream
﻿using Hgzn.Mes.Application;
using Hgzn.Mes.Application.Main.Dtos.Equip;
=======
﻿using Hgzn.Mes.Application.BaseS;
using Hgzn.Mes.Application.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services;
>>>>>>> Stashed changes
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService;

public interface IEquipTypeService : ICrudAppService<EquipType, Guid
    , EquipTypeReadDto, EquipTypeCreateDto, EquipTypeUpdateDto>
{
    Task<IEnumerable<EquipTypeReadDto>> GetListAsync(EquipTypeQueryDto queryDto);
}
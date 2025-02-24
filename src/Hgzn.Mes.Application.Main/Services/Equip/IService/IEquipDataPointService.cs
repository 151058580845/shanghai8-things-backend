using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService
{
    public interface IEquipDataPointService : ICrudAppService<
    EquipDataPoint, Guid,
    EquipDataPointReadDto, EquipDataPointQueryDto,
    EquipDataPointCreateDto, EquipDataPointUpdateDto>
    {
        Task PutStartConnect(Guid id);
    }
}

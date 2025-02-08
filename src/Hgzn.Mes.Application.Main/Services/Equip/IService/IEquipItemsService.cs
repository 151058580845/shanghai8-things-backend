using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;

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

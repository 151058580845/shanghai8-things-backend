
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.Equip;
using Hgzn.Mes.Domain.Shared;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService
{
    public interface ILocationLabelService :
        ICrudAppService<LocationLabel, Guid,
        LocationLabelReadDto, LocationLabelQueryDto,
        LocationLabelCreateDto, LocationLabelUpdateDto>
    {
        Task<int> DeleteRangesAsync(IEnumerable<Guid> ids);

        Task<PaginatedList<RoomLocationLabelReadDto>> GetRoomLabelAsync(int pageIndex, int pageSize);

        Task<PaginatedList<EquipLocationLabelReadDto>> GetEquipLabelAsync(int pageIndex, int pageSize);

        Task<int> BindingLabelsAsync(IEnumerable<BindingLabelDto> dtos);
    }
}

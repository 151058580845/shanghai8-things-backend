
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService
{
    public interface ILocationLabelService :
        ICrudAppService<LocationLabel, Guid,
        LocationLabelReadDto, LocationLabelQueryDto,
        LocationLabelCreateDto, LocationLabelUpdateDto>
    {
        Task<int> DeleteRangesAsync(IEnumerable<Guid> ids);
    }
}

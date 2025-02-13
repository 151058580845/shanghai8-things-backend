using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService;

public interface IEquipLedgerService : ICrudAppService<
    EquipLedger, Guid,
    EquipLedgerReadDto, EquipLedgerQueryDto,
    EquipLedgerCreateDto, EquipLedgerUpdateDto>
{
    Task<IEnumerable<NameValueDto>> GetNameValueListAsync();

    Task<IEnumerable<RfidEquipReadDto>> GetRfidEquipsListAsync(Guid equipId);

    Task<IEnumerable<EquipLedgerReadDto>> GetEquipsListAsync(string? equipCode, string? equipName);
    Task<int> UpdateEquipRoomId(Dictionary<Guid,Guid> equipIds);
    Task<IEnumerable<EquipLedgerReadDto>> GetEquipsListInIdsAsync(List<Guid> equipIds);
    Task<IEnumerable<EquipLedgerReadDto>> GetAppSearchAsync();
}
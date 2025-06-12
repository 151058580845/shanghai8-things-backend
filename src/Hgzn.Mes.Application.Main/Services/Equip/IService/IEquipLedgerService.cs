using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Dtos.System;
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
    Task<int> UpdateEquipRoomId(Dictionary<string,Guid> equipIds);
    Task<IEnumerable<EquipLedgerReadDto>> GetEquipsListInIdsAsync(List<Guid> equipIds);
    Task<IEnumerable<EquipLedgerSearchReadDto>> GetAppSearchAsync();

    Task<IEnumerable<EquipLedger>> GetEquipsListByRoomAsync(IEnumerable<Guid> rooms);

    /// <summary>
    /// ����api���빦��
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    Task<int> PostImportDatas(string url);

    Task<IEnumerable<EquipLedgerReadDto>> GetMissingDevicesAlarmAsync();
    Task<IEnumerable<EquipLedgerReadDto>?> GetListByTypeAsync(string? type);

    Task<IEnumerable<EquipResponsibleUserReadDto>> GetEquipResponsibleUsersAsync();
}
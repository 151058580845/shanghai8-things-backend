using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService;

public interface IEquipLedgerService : ICrudAppService<
    EquipLedger, Guid,
    EquipLedgerReadDto, EquipLedgerQueryDto,
    EquipLedgerCreateDto, EquipLedgerUpdateDto>
{
    Task<IEnumerable<NameValueDto>> GetNameValueListAsync();

    Task<IEnumerable<RfidEquipReadDto>> GetRfidEquipsListAsync(Guid equipId);

    Task<IEnumerable<EquipLedgerReadDto>> GetEquipsListAsync(string? equipCode, string? equipName);
    Task<int> UpdateEquipRoomId(Dictionary<string, Guid> equipIds);
    Task<IEnumerable<EquipLedgerReadDto>> GetEquipsListInIdsAsync(List<Guid> equipIds);
    Task<PaginatedList<EquipLedgerSearchReadDto>> GetAppSearchAsync(int pageIndex, int pageSize);

    Task<IEnumerable<EquipLedger>> GetEquipsListByRoomAsync(IEnumerable<Guid> rooms);

    /// <summary>
    /// ����api���빦��
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    Task<int> PostImportDatas(string url);

    Task<bool?> ImportAsync(IFormFile file);

    Task<IEnumerable<EquipLedgerReadDto>> GetMissingDevicesAlarmAsync();
    Task<IEnumerable<EquipLedgerReadDto>?> GetListByTypeAsync(string? type);

    Task<bool?> SetEquipExistByAssetNumber(string? assetNumber);

    Task<IEnumerable<EquipLedgerReadDto>> GetListByAssetNumberAsync(string? assetNumber);

    Task<IEnumerable<EquipResponsibleUserReadDto>> GetEquipResponsibleUsersAsync();

    /// <summary>
    /// 根据设备ID获取设备名称
    /// </summary>
    /// <param name="equipId"></param>
    /// <returns></returns>
    Task<string> GetEquipName(Guid equipId);
}
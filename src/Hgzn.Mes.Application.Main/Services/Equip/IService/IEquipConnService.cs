using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService;

public interface IEquipConnService : ICrudAppService<
    EquipConnect, Guid,
    EquipConnectReadDto, EquipConnectQueryDto,
    EquipConnectCreateDto, EquipConnectUpdateDto>
{
    Task<List<EquipConnectReadDto>> MapToGetListOutputDtosAsync(List<EquipConnect> equipLedgerQueryDtos);

    Task PutStartConnect(Guid id);

    Task StopConnectAsync(Guid connectId);

    Task TestConnection(ConnType protocolEnum, string connectionString);
    Task<IEnumerable<NameValueDto>> GetNameValueListAsync();

    Task<IEnumerable<EquipConnectReadDto>> GetRfidIssuerConnectionsAsync();
}
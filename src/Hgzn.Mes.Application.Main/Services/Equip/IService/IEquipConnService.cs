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

    Task TestConnection(Protocol protocolEnum, string connectionString);
}
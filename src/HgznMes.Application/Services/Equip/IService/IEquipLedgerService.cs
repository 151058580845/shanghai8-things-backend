using HgznMes.Application.BaseS;
using HgznMes.Application.Dtos.Base;
using HgznMes.Application.Dtos.Equip;
using HgznMes.Domain.Entities.Equip.EquipManager;

namespace HgznMes.Application.Services.Equip.IService;

public interface IEquipLedgerService:ICrudAppService<EquipLedgerAggregateRoot
    ,EquipLedgerReadDto,EquipLedgerReadDto,Guid,EquipLedgerQueryDto,EquipLedgerCreateDto,EquipLedgerUpdateDto>
{
    /// <summary>
    /// 获取设备列表
    /// </summary>
    /// <param name="queryDto"></param>
    /// <returns></returns>
    Task<IEnumerable<EquipLedgerReadDto>> GetEquipsListAsync(EquipLedgerQueryDto queryDto);
    
    /// <summary>
    /// 根据Ip地址获取设备信息
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <returns></returns>
    Task<EquipLedgerAggregateRoot> GetEquipByIpAsync(string ipAddress);

    /// <summary>
    /// 获取名称列表
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<NameValueDto>> GetNameValueListAsync();
}
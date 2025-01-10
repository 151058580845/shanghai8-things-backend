using Hgzn.Mes.Application.BaseS;
using Hgzn.Mes.Application.Dtos.Base;
using Hgzn.Mes.Application.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;

namespace Hgzn.Mes.Application.Services.Equip.IService;

public interface IEquipLedgerService:ICrudAppService<EquipLedger
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
    Task<EquipLedger> GetEquipByIpAsync(string ipAddress);

    /// <summary>
    /// 获取名称列表
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<NameValueDto>> GetNameValueListAsync();
}
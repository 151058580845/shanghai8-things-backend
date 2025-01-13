<<<<<<< Updated upstream
﻿using Hgzn.Mes.Application;
using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.Equip;
=======
﻿using Hgzn.Mes.Application.BaseS;
using Hgzn.Mes.Application.Dtos.Base;
using Hgzn.Mes.Application.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services;
>>>>>>> Stashed changes
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService;

public interface IEquipLedgerService : ICrudAppService<EquipLedger, Guid,
     EquipLedgerReadDto, EquipLedgerCreateDto, EquipLedgerUpdateDto>
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
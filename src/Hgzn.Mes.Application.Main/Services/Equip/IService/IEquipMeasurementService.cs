using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipMeasurementManager;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService
{
    public interface IEquipMeasurementService : ICrudAppService<
    EquipMeasurement, Guid,
    EquipMeasurementReadDto, EquipMeasurementQueryDto,
    EquipMeasurementCreateDto, EquipMeasurementUpdateDto>
    {
        /// <summary>
        /// 刷新计量数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool?> EquipMeasurementRefreshAsync(IFormFile formfile);

        /// <summary>
        /// 获取即将到期的计量数据
        /// </summary>
        /// <returns></returns>
        Task<List<EquipMeasurementReadDto>> GetMeasurementDue();
    }
}

using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService
{
    public interface IAssetDataServer : ICrudAppService<
    AssetData, Guid,
    AssetDataReadDto, AssetDataQueryDto, AssetDataCreateDto, AssetDataUpdateDto>
    {
        /// <summary>
        /// 获取成本计算明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AssetDataCalculationDetailsDto> GetCalculationDetailsAsync(Guid id);
    }
}

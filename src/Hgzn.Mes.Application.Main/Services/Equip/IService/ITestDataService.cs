using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService;

public interface ITestDataService : ICrudAppService<
    TestData, Guid,
    TestDataReadDto, TestDataQueryDto,
    TestDataCreateDto, TestDataUpdateDto>
{
    /// <summary>
    /// 批量api导入功能
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    Task<int> GetDataFromThirdPartyAsync(string url);
}
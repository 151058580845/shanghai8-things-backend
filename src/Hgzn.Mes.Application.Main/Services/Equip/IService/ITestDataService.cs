using Hgzn.Mes.Application.Main.Dtos.App;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService;

public interface ITestDataService : ICrudAppService<
    TestData, Guid,
    TestDataReadDto, TestDataQueryDto,
    TestDataCreateDto, TestDataUpdateDto>
{
    public Task<int> CreateAsync(IEnumerable<TestDataCreateDto> data);
    Task<IEnumerable<TestDataListReadDto>> GetListByTestAsync(string testName);

    /// <summary>
    /// 获取历史的试验计划数据
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<TestDataReadDto>> GetHistoryListByTestAsync();

    /// <summary>
    /// 获取当下的试验计划数据
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<TestDataReadDto>> GetCurrentListByTestAsync();

    /// <summary>
    /// 获取未来的试验计划数据
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<TestDataReadDto>> GetFeatureListByTestAsync();

    /// <summary>
    /// 批量api导入功能
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    Task<int> GetDataFromThirdPartyAsync(string url);
}
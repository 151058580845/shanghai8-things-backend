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
}
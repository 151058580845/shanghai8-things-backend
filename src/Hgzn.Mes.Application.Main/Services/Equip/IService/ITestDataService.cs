using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService;

public interface ITestDataService : ICrudAppService<
    TestData, Guid,
    TestDataReadDto, TestDataQueryDto,
    TestDataCreateDto, TestDataUpdateDto>
{
    
}
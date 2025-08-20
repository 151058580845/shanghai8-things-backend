using Hgzn.Mes.Application.Main.Dtos.Equip;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService;

public interface ITestEquipDataService
{
    Task<List<AssetNumberObj>> GetAssetNumbersAsync(int systemId, int equipTypeId);
    Task<List<ColumnObj>> GetColumnsAsync(int systemId, int equipTypeId);
    Task<object> GetDatasAsync(TestEquipDataQueryDto query);
}

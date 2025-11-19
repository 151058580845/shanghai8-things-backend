using Hgzn.Mes.Application.Main.Dtos.Equip;

namespace Hgzn.Mes.Application.Main.Services.Equip.IService;

public interface ITestEquipDataService
{
    Task<List<AssetNumberObj>> GetAssetNumbersAsync(int systemId, int equipTypeId);
    Task<List<ColumnObj>> GetColumnsAsync(int systemId, int equipTypeId);
    Task<object> GetDatasAsync(TestEquipDataQueryDto query);
    /// <summary>
    /// 导出Excel文件（流式处理，支持大量数据）
    /// </summary>
    Task<byte[]> ExportToExcelAsync(TestEquipDataQueryDto query);
}

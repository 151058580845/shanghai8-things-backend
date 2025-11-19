using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Equip;

[Route("api/[controller]")]
[ApiController]
public class TestEquipDataController(ITestEquipDataService _service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("assetnumbers/{systemId}/{equipTypeId}")]
    [Authorize(Policy = $"equip:testequipdata:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<List<AssetNumberObj>>> GetAssetNumberObjListAsync(int systemId, int equipTypeId)
        => (await _service.GetAssetNumbersAsync(systemId, equipTypeId)).Wrap();

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("columns/{systemId}/{equipTypeId}")]
    [Authorize(Policy = $"equip:testequipdata:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<List<ColumnObj>>> GetColumnObjListAsync(int systemId, int equipTypeId)
        => (await _service.GetColumnsAsync(systemId, equipTypeId)).Wrap();
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("page")]
    [Authorize(Policy = $"equip:testequipdata:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<object>> GetDatasAsync(TestEquipDataQueryDto query)
        => (await _service.GetDatasAsync(query)).Wrap();
    
    /// <summary>
    /// 导出Excel文件（流式处理，支持大量数据）
    /// 注意：前端已设置10分钟超时，后端处理时间应在此范围内完成
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("export")]
    [Authorize(Policy = $"equip:testequipdata:{ScopeMethodType.Query}")]
    public async Task<IActionResult> ExportToExcelAsync([FromBody] TestEquipDataQueryDto query)
    {
        try
        {
            var excelData = await _service.ExportToExcelAsync(query);
            var fileName = $"试验数据_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            
            return File(
                excelData,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"导出Excel失败: {ex.Message}" });
        }
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;
using Hgzn.Mes.Application.Main.Dtos.App;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Entities.System.Code;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class TestDataService : SugarCrudAppService<
        TestData, Guid,
        TestDataReadDto, TestDataQueryDto,
        TestDataCreateDto, TestDataUpdateDto>,
    ITestDataService
{

    private readonly HttpClient _httpClient;
    public TestDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public override async Task<IEnumerable<TestDataReadDto>> GetListAsync(TestDataQueryDto? queryDto = null)
    {
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(queryDto.SysName), t => t.SysName.Contains(queryDto.SysName))
            .WhereIF(!string.IsNullOrEmpty(queryDto.ProjectName), t => t.ProjectName.Contains(queryDto.ProjectName))
            .WhereIF(!string.IsNullOrEmpty(queryDto.TaskName), t => t.TaskName.Contains(queryDto.TaskName))
            .WhereIF(!string.IsNullOrEmpty(queryDto.DevPhase), t => t.DevPhase.Contains(queryDto.DevPhase))
            .WhereIF(!string.IsNullOrEmpty(queryDto.TaskStartTime), t => t.TaskStartTime.Contains(queryDto.TaskStartTime))
            .WhereIF(!string.IsNullOrEmpty(queryDto.TaskEndTime), t => t.TaskEndTime.Contains(queryDto.TaskEndTime))
            .WhereIF(!string.IsNullOrEmpty(queryDto.ReqDep), t => t.ReqDep.Contains(queryDto.ReqDep))
            .WhereIF(!string.IsNullOrEmpty(queryDto.ReqManager), t => t.ReqManager.Contains(queryDto.ReqManager))
            .WhereIF(!string.IsNullOrEmpty(queryDto.ReqManagerCode), t => t.ReqManagerCode.Contains(queryDto.ReqManagerCode))
            .WhereIF(!string.IsNullOrEmpty(queryDto.GncResp), t => t.GncResp.Contains(queryDto.GncResp))
            .WhereIF(!string.IsNullOrEmpty(queryDto.GncRespCode), t => t.GncRespCode.Contains(queryDto.GncRespCode))
            .WhereIF(!string.IsNullOrEmpty(queryDto.SimuResp), t => t.SimuResp.Contains(queryDto.SimuResp))
            .WhereIF(!string.IsNullOrEmpty(queryDto.simuRespCode), t => t.simuRespCode.Contains(queryDto.GncRespCode))
            .WhereIF(!string.IsNullOrEmpty(queryDto.SimuStaff), t => t.SimuStaff.Contains(queryDto.SimuStaff))
            .WhereIF(!string.IsNullOrEmpty(queryDto.simuStaffCodes), t => t.simuStaffCodes.Contains(queryDto.simuStaffCodes))
            .WhereIF(!string.IsNullOrEmpty(queryDto.QncResp), t => t.QncResp.Contains(queryDto.QncResp))
            .Includes(x => x.UUT)
            .ToListAsync();
        return Mapper.Map<IEnumerable<TestDataReadDto>>(entities);
    }

    public override async Task<PaginatedList<TestDataReadDto>> GetPaginatedListAsync(TestDataQueryDto queryDto)
    {
        var entities = await Queryable
              .WhereIF(!string.IsNullOrEmpty(queryDto.SysName), t => t.SysName.Contains(queryDto.SysName))
              .WhereIF(!string.IsNullOrEmpty(queryDto.ProjectName), t => t.ProjectName.Contains(queryDto.ProjectName))
              .WhereIF(!string.IsNullOrEmpty(queryDto.TaskName), t => t.TaskName.Contains(queryDto.TaskName))
              .WhereIF(!string.IsNullOrEmpty(queryDto.DevPhase), t => t.DevPhase.Contains(queryDto.DevPhase))
              .WhereIF(!string.IsNullOrEmpty(queryDto.TaskStartTime), t => t.TaskStartTime.Contains(queryDto.TaskStartTime))
              .WhereIF(!string.IsNullOrEmpty(queryDto.TaskEndTime), t => t.TaskEndTime.Contains(queryDto.TaskEndTime))
              .WhereIF(!string.IsNullOrEmpty(queryDto.ReqDep), t => t.ReqDep.Contains(queryDto.ReqDep))
              .WhereIF(!string.IsNullOrEmpty(queryDto.ReqManager), t => t.ReqManager.Contains(queryDto.ReqManager))
              .WhereIF(!string.IsNullOrEmpty(queryDto.ReqManagerCode), t => t.ReqManagerCode.Contains(queryDto.ReqManagerCode))
              .WhereIF(!string.IsNullOrEmpty(queryDto.GncResp), t => t.GncResp.Contains(queryDto.GncResp))
              .WhereIF(!string.IsNullOrEmpty(queryDto.GncRespCode), t => t.GncRespCode.Contains(queryDto.GncRespCode))
              .WhereIF(!string.IsNullOrEmpty(queryDto.SimuResp), t => t.SimuResp.Contains(queryDto.SimuResp))
              .WhereIF(!string.IsNullOrEmpty(queryDto.simuRespCode), t => t.simuRespCode.Contains(queryDto.simuRespCode))
              .WhereIF(!string.IsNullOrEmpty(queryDto.SimuStaff), t => t.SimuStaff.Contains(queryDto.SimuStaff))
              .WhereIF(!string.IsNullOrEmpty(queryDto.simuStaffCodes), t => t.simuStaffCodes.Contains(queryDto.simuStaffCodes))
              .WhereIF(!string.IsNullOrEmpty(queryDto.QncResp), t => t.QncResp.Contains(queryDto.QncResp))
              .Includes(x => x.UUT)
              .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
        return Mapper.Map<PaginatedList<TestDataReadDto>>(entities);
    }

    public async Task<int> CreateAsync(IEnumerable<TestDataCreateDto> data)
    {
        var entities = Mapper.Map<IEnumerable<TestData>>(data);
        return await DbContext.Insertable<List<TestData>>(entities).ExecuteCommandAsync();
    }

    public async Task<IEnumerable<TestDataListReadDto>> GetListByTestAsync(string testName)
    {
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(testName), t => t.SysName == testName)
            .ToListAsync();
        return Mapper.Map<IEnumerable<TestDataListReadDto>>(entities);
    }

    /// <summary>
    /// 批量api导入
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public async Task<int> GetDataFromThirdPartyAsync(string url)
    {
        try
        {
            // 发送 GET 请求
            var response = await _httpClient.GetAsync(url);

            // 确保请求成功
            response.EnsureSuccessStatusCode();

            // 读取响应内容
            var jsonResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // 忽略大小写
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // 忽略 null 值
            };

            var result = JsonSerializer.Deserialize<List<TestDataCreateDto>>(jsonResponse, options);

            var changeCount = 0;
            if (result.Any())
            {
                foreach (var item in result)
                {
                    var info = Mapper.Map<TestData>(item);
                    // 首先检查数据库中是否已存在相同 TestDataId 的记录
                    var existingData = DbContext.Queryable<TestData>()
                        .Where(x => x.TestDataId == info.TestDataId)
                        .First();

                    if (existingData != null)
                    {
                        // 如果存在，则更新主表和子表
                        DbContext.UpdateNav(existingData)
                            .Include(x => x.UUT) // 包含子表
                            .ExecuteCommand();
                    }
                    else
                    {
                        // 如果不存在，则插入新记录
                        var inData = DbContext.InsertNav<TestData>(info)
                            .Include(x => x.UUT) // 包含子表
                            .ExecuteCommand();
                        if (inData)
                        {
                            changeCount++;
                        }
                    }
                }
            }

            return changeCount;
        }
        catch (HttpRequestException ex)
        {
            // 处理 HTTP 请求异常
            LoggerAdapter.LogError($"HTTP 请求失败: {ex.Message}");
            throw;
        }
        catch (JsonException ex)
        {
            // 处理 JSON 反序列化异常
            LoggerAdapter.LogError($"JSON 反序列化失败: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            // 处理其他异常
            LoggerAdapter.LogError($"发生错误: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<TestDataReadDto>> GetHistoryListByTestAsync()
    {
        List<TestData> entities = await Queryable
            .Where(x => x.TaskEndTime != null && DateTime.Parse(x.TaskEndTime) < DateTime.Now.ToLocalTime())
            .Includes(x => x.UUT)
            .ToListAsync();
        return Mapper.Map<IEnumerable<TestDataReadDto>>(entities);
    }

    public async Task<IEnumerable<TestDataReadDto>> GetCurrentListByTestAsync()
    {
        List<TestData> entities = await Queryable
            .Where(x => x.TaskEndTime != null && DateTime.Parse(x.TaskEndTime) > DateTime.Now.ToLocalTime() &&
                        x.TaskStartTime != null && DateTime.Parse(x.TaskStartTime) < DateTime.Now.ToLocalTime())
            .Includes(x => x.UUT)
            .ToListAsync();
        return Mapper.Map<IEnumerable<TestDataReadDto>>(entities);
    }

    public async Task<IEnumerable<TestDataReadDto>> GetFeatureListByTestAsync()
    {
        List<TestData> entities = await Queryable
            .Where(x => x.TaskStartTime != null && DateTime.Parse(x.TaskStartTime) > DateTime.Now.ToLocalTime())
            .Includes(x => x.UUT)
            .ToListAsync();
        return Mapper.Map<IEnumerable<TestDataReadDto>>(entities);
    }
}
using System.Text.Json;
using System.Text.Json.Serialization;
using Hgzn.Mes.Application.Main.Dtos.App;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Entities.System.Code;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;

using System.Runtime.Serialization.Json;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Application.Main.Services.System;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class TestDataService : SugarCrudAppService<
        TestData, Guid,
        TestDataReadDto, TestDataQueryDto,
        TestDataCreateDto, TestDataUpdateDto>,
    ITestDataService
{

    private readonly HttpClient _httpClient;
    private readonly IBaseConfigService _baseConfigService;
    public TestDataService(HttpClient httpClient, IBaseConfigService baseConfigService)
    {
        _httpClient = httpClient;
        _baseConfigService = baseConfigService;
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
            .Includes(x => x.UST)
            .OrderByDescending(x => x.TaskStartTime)
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
            .Includes(x => x.UST)
              .OrderByDescending(x => x.TaskStartTime)
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

    public static string ObjectToJson(object obj)
    {
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
        using (MemoryStream stream = new MemoryStream())
        {
            serializer.WriteObject(stream, obj);
            return global::System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    /// <summary>
    /// 批量api导入
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public async Task<int> GetDataFromThirdPartyAsync()
    {
        try
        {
            LoggerAdapter.LogInformation($"AG - 试验计划导入 - 开始执行API批量导入");
            
            var url = await _baseConfigService.GetValueByKeyAsync("import_plan_url");
            LoggerAdapter.LogInformation($"AG - 试验计划导入 - 获取到导入URL: {url}");

            // 发送 GET 请求
            LoggerAdapter.LogInformation($"AG - 试验计划导入 - 正在发送HTTP GET请求...");
            var response = await _httpClient.GetAsync(url);

            // 确保请求成功
            LoggerAdapter.LogInformation($"AG - 试验计划导入 - HTTP响应状态码: {response.StatusCode}");
            response.EnsureSuccessStatusCode();

            // 读取响应内容
            var jsonResponse = await response.Content.ReadAsStringAsync();
            LoggerAdapter.LogInformation($"AG - 试验计划导入 - 接收到JSON响应, 长度: {jsonResponse?.Length ?? 0} 字符");
            LoggerAdapter.LogInformation($"AG - 试验计划导入 - JSON内容前500字符: {(jsonResponse?.Length > 500 ? jsonResponse.Substring(0, 500) : jsonResponse)}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // 忽略大小写
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // 忽略 null 值
            };

            LoggerAdapter.LogInformation($"AG - 试验计划导入 - 开始反序列化JSON数据...");
            var result = JsonSerializer.Deserialize<List<TestDataCreateDto>>(jsonResponse, options);
            LoggerAdapter.LogInformation($"AG - 试验计划导入 - 反序列化完成, 共解析到 {result?.Count ?? 0} 条数据");

            var changeCount = 0;
            if (result.Any())
            {
                LoggerAdapter.LogInformation($"AG - 试验计划导入 - 开始处理 {result.Count} 条试验计划数据...");
                
                var processedCount = 0;
                foreach (var item in result)
                {
                    processedCount++;
                    LoggerAdapter.LogInformation($"AG - 试验计划导入 - 处理第 {processedCount}/{result.Count} 条: TestDataId={item.TestDataId}, 试验名称={item.TaskName}");
                    LoggerAdapter.LogInformation($"AG - 试验计划导入 - 该计划包含 UUT数量: {item.UUT?.Count ?? 0}, UST数量: {item.UST?.Count ?? 0}");
                    
                    // 记录UST详细信息
                    if (item.UST != null && item.UST.Any())
                    {
                        foreach (var ust in item.UST)
                        {
                            LoggerAdapter.LogInformation($"AG - 试验计划导入 - UST设备: 编号={ust.Code}, 名称={ust.Name}, 型号={ust.ModelSpecification}, 有效期={ust.ValidityPeriod}");
                        }
                    }
                    else
                    {
                        LoggerAdapter.LogInformation($"AG - 试验计划导入 - 该计划未包含UST数据");
                    }
                    
                    var info = Mapper.Map<TestData>(item);
                    LoggerAdapter.LogInformation($"AG - 试验计划导入 - Mapper映射完成, UUT映射后数量: {info.UUT?.Count ?? 0}, UST映射后数量: {info.UST?.Count ?? 0}");
                    
                    // 首先检查数据库中是否已存在相同 TestDataId 的记录
                    LoggerAdapter.LogInformation($"AG - 试验计划导入 - 查询数据库是否存在TestDataId: {info.TestDataId}");
                    var existingData = DbContext.Queryable<TestData>()
                        .Where(x => x.TestDataId == info.TestDataId)
                        .First();

                    if (existingData != null)
                    {
                        LoggerAdapter.LogInformation($"AG - 试验计划导入 - 找到已存在记录, ID={existingData.Id}, 执行更新操作");
                        LoggerAdapter.LogInformation($"AG - 试验计划导入 - 更新前: 现有UUT数量: {existingData.UUT?.Count ?? 0}, 现有UST数量: {existingData.UST?.Count ?? 0}");
                        
                        // 更新现有数据的属性
                        existingData.SysName = info.SysName;
                        existingData.ProjectName = info.ProjectName;
                        existingData.TaskName = info.TaskName;
                        existingData.DevPhase = info.DevPhase;
                        existingData.TaskStartTime = info.TaskStartTime;
                        existingData.TaskEndTime = info.TaskEndTime;
                        existingData.ReqDep = info.ReqDep;
                        existingData.ReqManager = info.ReqManager;
                        existingData.ReqManagerCode = info.ReqManagerCode;
                        existingData.GncResp = info.GncResp;
                        existingData.GncRespCode = info.GncRespCode;
                        existingData.SimuResp = info.SimuResp;
                        existingData.simuRespCode = info.simuRespCode;
                        existingData.SimuStaff = info.SimuStaff;
                        existingData.simuStaffCodes = info.simuStaffCodes;
                        existingData.QncResp = info.QncResp;
                        existingData.UUT = info.UUT;
                        existingData.UST = info.UST;
                        
                        LoggerAdapter.LogInformation($"AG - 试验计划导入 - 更新后: 新UUT数量: {existingData.UUT?.Count ?? 0}, 新UST数量: {existingData.UST?.Count ?? 0}");
                        
                        // 如果存在，则更新主表和子表
                        var updateResult = DbContext.UpdateNav(existingData)
                            .Include(x => x.UUT) // 包含UUT子表
                            .Include(x => x.UST) // 包含UST子表
                            .ExecuteCommand();
                        
                        LoggerAdapter.LogInformation($"AG - 试验计划导入 - 更新操作完成, 更新结果: {updateResult}");
                        changeCount++;
                    }
                    else
                    {
                        LoggerAdapter.LogInformation($"AG - 试验计划导入 - 未找到已存在记录, 执行新增操作");
                        LoggerAdapter.LogInformation($"AG - 试验计划导入 - 新增数据: UUT数量: {info.UUT?.Count ?? 0}, UST数量: {info.UST?.Count ?? 0}");
                        
                        // 如果不存在，则插入新记录
                        var inData = DbContext.InsertNav<TestData>(info)
                            .Include(x => x.UUT) // 包含UUT子表
                            .Include(x => x.UST) // 包含UST子表
                            .ExecuteCommand();
                        
                        LoggerAdapter.LogInformation($"AG - 试验计划导入 - 新增操作完成, 插入结果: {inData}");
                        if (inData)
                        {
                            changeCount++;
                        }
                    }
                }
                
                LoggerAdapter.LogInformation($"AG - 试验计划导入 - 所有数据处理完成");
            }
            else
            {
                LoggerAdapter.LogInformation($"AG - 试验计划导入 - 未解析到任何数据或数据为空");
            }

            LoggerAdapter.LogInformation($"AG - 试验计划导入 - 导入完成, 总共处理 {changeCount} 条记录");
            return changeCount;
        }
        catch (HttpRequestException ex)
        {
            // 处理 HTTP 请求异常
            LoggerAdapter.LogError($"AG - 试验计划导入 - HTTP 请求失败: {ex.Message}");
            LoggerAdapter.LogError($"AG - 试验计划导入 - 异常堆栈: {ex.StackTrace}");
        }
        catch (JsonException ex)
        {
            // 处理 JSON 反序列化异常
            LoggerAdapter.LogError($"AG - 试验计划导入 - JSON 反序列化失败: {ex.Message}");
            LoggerAdapter.LogError($"AG - 试验计划导入 - 异常堆栈: {ex.StackTrace}");
        }
        catch (Exception ex)
        {
            // 处理其他异常
            LoggerAdapter.LogError($"AG - 试验计划导入 - 发生未知错误: {ex.Message}");
            LoggerAdapter.LogError($"AG - 试验计划导入 - 异常类型: {ex.GetType().Name}");
            LoggerAdapter.LogError($"AG - 试验计划导入 - 异常堆栈: {ex.StackTrace}");
        }
        return 0;
    }

    public async Task<IEnumerable<TestDataReadDto>> GetHistoryListByTestAsync()
    {
        DateTime firstDayOfYear = new DateTime(DateTime.Now.Year, 1, 1);
        List<TestData> entities = await Queryable
            .Where(x => x.TaskEndTime != null && DateTime.Parse(x.TaskEndTime) < DateTime.Now.ToLocalTime() && DateTime.Parse(x.TaskEndTime) >= firstDayOfYear)
            .Includes(x => x.UUT)
            .Includes(x => x.UST)
            .OrderByDescending(x => x.TaskStartTime)
            .ToListAsync();
        return Mapper.Map<IEnumerable<TestDataReadDto>>(entities);
    }

    public async Task<IEnumerable<TestDataReadDto>> GetCurrentListByTestAsync()
    {
        List<TestData> entities = await Queryable
            .Where(x => x.TaskEndTime != null && DateTime.Parse(x.TaskEndTime) >= DateTime.Now.ToLocalTime() &&
                        x.TaskStartTime != null && DateTime.Parse(x.TaskStartTime) <= DateTime.Now.ToLocalTime())
            .Includes(x => x.UUT)
            .Includes(x => x.UST)
            .OrderByDescending(x => x.TaskStartTime)
            .ToListAsync();
        IEnumerable<TestDataReadDto> ret = Mapper.Map<IEnumerable<TestDataReadDto>>(entities);
        return ret;
    }

    public async Task<IEnumerable<TestDataReadDto>> GetFeatureListByTestAsync()
    {
        List<TestData> entities = await Queryable
            .Where(x => x.TaskStartTime != null && DateTime.Parse(x.TaskStartTime) > DateTime.Now.ToLocalTime())
            .Includes(x => x.UUT)
            .Includes(x => x.UST)
            .OrderByDescending(x => x.TaskStartTime)
            .ToListAsync();
        return Mapper.Map<IEnumerable<TestDataReadDto>>(entities);
    }
}
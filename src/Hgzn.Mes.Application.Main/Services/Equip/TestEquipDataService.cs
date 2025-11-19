using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Attributes;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_307_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Services;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.Record;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using StackExchange.Redis;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class TestEquipDataService
    : SugarCrudAppService<TestEquipData, Guid, TestEquipDataReadDto>, ITestEquipDataService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger<TestEquipDataService> _logger;
    private readonly SplitTableService _splitTableService;

    public TestEquipDataService(IConnectionMultiplexer connectionMultiplexer, ILogger<TestEquipDataService> logger,
        SplitTableService splitTableService)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _logger = logger;
        _splitTableService = splitTableService;
    }

    public async Task<List<AssetNumberObj>> GetAssetNumbersAsync(int systemId, int equipTypeId)
    {
        // 优化：先获取去重后的资产编号列表
        var distinctAssetNumbers = await DbContext.Queryable<TestEquipData>()
            .Where(x => x.SimuTestSysld == systemId && x.DevTypeld == equipTypeId && !string.IsNullOrEmpty(x.Compld))
            .GroupBy(x => x.Compld)
            .Select(x => x.Compld)
            .ToListAsync();
        
        if (distinctAssetNumbers == null || distinctAssetNumbers.Count == 0)
        {
            return new List<AssetNumberObj>();
        }
        
        // 过滤掉 null 值，并去掉前后引号和空格
        var validAssetNumbers = distinctAssetNumbers
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(x => TrimQuotes(x!)) // 🔥 去掉前后引号和空格
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct()
            .ToList();
        
        if (validAssetNumbers.Count == 0)
        {
            return new List<AssetNumberObj>();
        }
        
        // 批量查询 EquipLedger 数据，避免 N+1 查询问题
        var equipLedgers = await DbContext.Queryable<EquipLedger>()
            .Where(x => x.AssetNumber != null && validAssetNumbers.Contains(x.AssetNumber) && !x.SoftDeleted)
            .Select(x => new { x.AssetNumber, x.EquipName })
            .ToListAsync();
        
        // 构建字典以提高查找效率（AssetNumber 已经过滤了 null，所以可以安全使用）
        Dictionary<string, string?> equipLedgerDict = new Dictionary<string, string?>();
        if (equipLedgers != null)
        {
            foreach (var item in equipLedgers.Where(x => !string.IsNullOrEmpty(x.AssetNumber)))
            {
                if (!equipLedgerDict.ContainsKey(item.AssetNumber!))
                {
                    equipLedgerDict[item.AssetNumber!] = item.EquipName;
                }
            }
        }
        
        // 组装结果，保持 EquipName 可能为 null，由前端处理显示逻辑
        var list = validAssetNumbers.Select(assetNumber => new AssetNumberObj
        {
            AssetNumber = assetNumber,
            EquipName = equipLedgerDict.TryGetValue(assetNumber, out var equipName) ? equipName : null
        }).ToList();
        
        return list;
    }

    public Task<List<ColumnObj>> GetColumnsAsync(int systemId, int equipTypeId)
    {
        var assem = Assembly.Load("Hgzn.Mes.Domain");
        var room = TestEquipData.GetRoom(systemId);
        var type = assem.GetType(
            $"Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_{room}_ReceiveDatas.XT_{room}_SL_{equipTypeId}_ReceiveData");
        if (type is null)
        {
            return Task.FromResult<List<ColumnObj>>(new List<ColumnObj>());
        }

        var properties = type.GetProperties();
        List<ColumnObj> list = [];
        list.AddRange(from property in properties
            where property.Name != "Seeds" && !property.IsDefined(typeof(TableNotShowAttribute))
            select new ColumnObj
            {
                Label = property.Name == "CreationTime"
                    ? "数据获取时间"
                    : (property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? property.Name),
                Prop = property.Name,   
                Hide = property.Name == "Id" ? true : false,
                Width =  property.Name is "CreationTime" or "Compld"?200:150
            });
        return Task.FromResult(list);
    }

    public async Task<object> GetDatasAsync(TestEquipDataQueryDto query)
    {
        var assem = Assembly.Load("Hgzn.Mes.Domain");
        var room = TestEquipData.GetRoom(query.SystemId);
        var type = assem.GetType(
            $"Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_{room}_ReceiveDatas.XT_{room}_SL_{query.EquipTypeId}_ReceiveData");
        if (type is null)
        {
            LoggerAdapter.LogWarning($"GetDatasAsync - Type not found for SystemId: {query.SystemId}, EquipTypeId: {query.EquipTypeId}, Room: {room}");
            return new PaginatedList<object>(new List<object>(), 0, query.PageIndex, query.PageSize);
        }

        // 直接查询 equip_x_t_{room}_s_l_{equipTypeId}_receive_data 表
        var tableName = $"equip_x_t_{room}_s_l_{query.EquipTypeId}_receive_data";
        
        LoggerAdapter.LogInformation($"GetDatasAsync - SystemId: {query.SystemId}, EquipTypeId: {query.EquipTypeId}, Room: {room}, TableName: {tableName}, AssetNumbers: {(query.AssetNumbers == null || query.AssetNumbers.Count == 0 ? "null/empty" : string.Join(",", query.AssetNumbers))}");

        // 构建 WHERE 条件和参数
        var whereConditions = new List<string> { "simu_test_sysld = @SystemId", "dev_typeld = @EquipTypeId" };
        var parameters = new List<SugarParameter>
        {
            new SugarParameter("@SystemId", query.SystemId),
            new SugarParameter("@EquipTypeId", query.EquipTypeId)
        };
        
        // 资产编号过滤
        if (query.AssetNumbers != null && query.AssetNumbers.Count > 0)
        {
            var assetNumberParams = new List<string>();
            for (int i = 0; i < query.AssetNumbers.Count; i++)
            {
                var paramName = $"@AssetNumber{i}";
                assetNumberParams.Add(paramName);
                parameters.Add(new SugarParameter(paramName, query.AssetNumbers[i]));
            }
            whereConditions.Add($"compld IN ({string.Join(", ", assetNumberParams)})");
        }
        
        // 时间范围过滤
        if (query.StartDateTime != null)
        {
            whereConditions.Add("creation_time >= @StartDateTime");
            parameters.Add(new SugarParameter("@StartDateTime", query.StartDateTime.Value));
        }
        if (query.EndDateTime != null)
        {
            whereConditions.Add("creation_time <= @EndDateTime");
            parameters.Add(new SugarParameter("@EndDateTime", query.EndDateTime.Value));
        }
        
        var whereClause = string.Join(" AND ", whereConditions);
        
        // 获取总数
        var countSql = $"SELECT COUNT(*) FROM {tableName} WHERE {whereClause}";
        var totalCount = await DbContext.Ado.GetIntAsync(countSql, parameters.ToArray());
        
        // 分页查询
        // 如果 pageSize 为 0，表示导出所有数据，不使用 LIMIT
        string pageSql;
        var queryParameters = new List<SugarParameter>(parameters);
        if (query.PageSize > 0)
        {
            pageSql = $"SELECT * FROM {tableName} WHERE {whereClause} ORDER BY creation_time DESC LIMIT @PageSize OFFSET @Offset";
            queryParameters.Add(new SugarParameter("@PageSize", query.PageSize));
            queryParameters.Add(new SugarParameter("@Offset", (query.PageIndex - 1) * query.PageSize));
        }
        else
        {
            // 导出所有数据，不使用分页
            pageSql = $"SELECT * FROM {tableName} WHERE {whereClause} ORDER BY creation_time DESC";
        }
        
        var items = await DbContext.Ado.SqlQueryAsync<dynamic>(pageSql, queryParameters.ToArray());
        
        LoggerAdapter.LogInformation($"GetDatasAsync - Returned {items?.Count ?? 0} items, TotalCount: {totalCount}, PageSize: {query.PageSize}");
        
        return new PaginatedList<object>(items?.Cast<object>().ToList() ?? new List<object>(), totalCount, query.PageIndex, query.PageSize);
        // var dd = rawData.Items.Select(t => JsonSerializer.Deserialize(t.ToString(), type));
        // return new PaginatedList<object>(dd!, rawData.TotalCount, query.PageIndex, query.PageSize);
        // return rawData;
        // var data = rawData.Items
        //     .Select(json => JsonSerializer.Deserialize(JsonSerializer.Serialize(json),type))
        //     .ToList();
        // return new PaginatedList<object>(data!, rawData.TotalCount, query.PageIndex, query.PageSize);
        // var data = await DbContext.Queryable<Receive>()
        //     .WhereIF(query.StartDateTime != null, it => it.CreateTime >= query.StartDateTime)
        //     .WhereIF(query.EndDateTime != null, it => it.CreateTime <= query.EndDateTime)
        //     .SplitTable(tas => tas.Where(y => y.TableName.Contains(tableName)
        //                                       && query.AssetNumbers != null
        //                                       && query.AssetNumbers.Any(t => y.TableName.Contains(t)))
        //         .ToList())
        //     .Select(t => JsonSerializer.Deserialize(t.Content, type, JsonSerializerOptions.Default))
        //     .ToListAsync();
        // return data;
        // try
        // {
        //      int room = TestEquipData.GetRoom(query.SystemId);
        //      string selectsql = $"SELECT * ";
        //      string basesql = $"FROM equip_x_t_{room}_s_l_{query.EquipTypeId}_receive_data WHERE simu_test_sysld={query.SystemId} AND dev_typeld={query.EquipTypeId}";
        //      string conditionalsql = "";
        //      if (query.AssetNumbers != null && query.AssetNumbers.Count > 0)
        //      {
        //          for (var i = 0; i < query.AssetNumbers.Count; i++)
        //          {
        //              query.AssetNumbers[i] = "'" + query.AssetNumbers[i] + "'";
        //          }
        //          string condition = " AND compld IN (" + string.Join(',', query.AssetNumbers) + ")";
        //          conditionalsql += condition;
        //      }
        //      if (query.StartDateTime != null)
        //      {
        //          conditionalsql += " AND creation_time >= '" + query.StartDateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";
        //      }
        //      if (query.EndDateTime != null)
        //      {
        //          conditionalsql += " AND creation_time <= '" + query.EndDateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";
        //      }
        //      string pagesql = " ORDER BY creation_time desc ";
        //      if (query.PageIndex != 0 && query.PageSize != 0)
        //      {
        //          pagesql = $" ORDER BY creation_time desc LIMIT {query.PageSize} OFFSET {query.PageSize * (query.PageIndex - 1)};";
        //      }
        //      string sqlCount = $"SELECT COUNT(*) " + basesql + conditionalsql;
        //      string sql = selectsql + basesql + conditionalsql + pagesql;
        //      var count = await DbContext.Ado.SqlQueryAsync<int>(sqlCount);
        //      var result = await DbContext.Ado.SqlQueryAsync<object>(sql);
        //     _logger.LogInformation(sql);
        //      return new { Items = result, PageSize = query.PageSize, TotalCount = count[0] };
        //     // var list = await DbContext.Queryable<XT_307_SL_1_ReceiveData>()
        //     //     .Where(t=>t.)
        //     //     .WhereIF(query.StartDateTime != null, x => x.CreationTime >= query.StartDateTime)
        //     //     .WhereIF(query.EndDateTime != null, x => x.CreationTime <= query.StartDateTime)
        //     //     .WhereIF(query.AssetNumbers != null && query.AssetNumbers.Count > 0, x => query.AssetNumbers.Contains(x.Compld))
        //     //     .ToPaginatedListAsync(query.PageIndex, query.PageSize);
        //     // return new { Items = list.Items, PageSize = query.PageSize, TotalCount = list.TotalCount };
        // }
        // catch (Exception ex)
        // {
        //     throw new Exception(ex.Message);
        // }
    }

    /// <summary>
    /// 导出Excel文件（流式处理，支持大量数据）
    /// </summary>
    public async Task<byte[]> ExportToExcelAsync(TestEquipDataQueryDto query)
    {
        var assem = Assembly.Load("Hgzn.Mes.Domain");
        var room = TestEquipData.GetRoom(query.SystemId);
        var type = assem.GetType(
            $"Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_{room}_ReceiveDatas.XT_{room}_SL_{query.EquipTypeId}_ReceiveData");
        if (type is null)
        {
            LoggerAdapter.LogWarning($"ExportToExcelAsync - Type not found for SystemId: {query.SystemId}, EquipTypeId: {query.EquipTypeId}, Room: {room}");
            throw new Exception($"未找到对应的数据类型: SystemId={query.SystemId}, EquipTypeId={query.EquipTypeId}");
        }

        // 获取列信息
        var columns = await GetColumnsAsync(query.SystemId, query.EquipTypeId);
        if (columns == null || columns.Count == 0)
        {
            throw new Exception("未找到列信息");
        }

        // 过滤隐藏的列
        var visibleColumns = columns.Where(c => !(c.Hide ?? false)).OrderBy(c => c.Prop).ToList();

        // 直接查询 equip_x_t_{room}_s_l_{equipTypeId}_receive_data 表
        var tableName = $"equip_x_t_{room}_s_l_{query.EquipTypeId}_receive_data";
        
        LoggerAdapter.LogInformation($"ExportToExcelAsync - SystemId: {query.SystemId}, EquipTypeId: {query.EquipTypeId}, Room: {room}, TableName: {tableName}");

        // 构建 WHERE 条件和参数
        var whereConditions = new List<string> { "simu_test_sysld = @SystemId", "dev_typeld = @EquipTypeId" };
        var parameters = new List<SugarParameter>
        {
            new SugarParameter("@SystemId", query.SystemId),
            new SugarParameter("@EquipTypeId", query.EquipTypeId)
        };
        
        // 资产编号过滤
        if (query.AssetNumbers != null && query.AssetNumbers.Count > 0)
        {
            var assetNumberParams = new List<string>();
            for (int i = 0; i < query.AssetNumbers.Count; i++)
            {
                var paramName = $"@AssetNumber{i}";
                assetNumberParams.Add(paramName);
                parameters.Add(new SugarParameter(paramName, query.AssetNumbers[i]));
            }
            whereConditions.Add($"compld IN ({string.Join(", ", assetNumberParams)})");
        }
        
        // 时间范围过滤
        if (query.StartDateTime != null)
        {
            whereConditions.Add("creation_time >= @StartDateTime");
            parameters.Add(new SugarParameter("@StartDateTime", query.StartDateTime.Value));
        }
        if (query.EndDateTime != null)
        {
            whereConditions.Add("creation_time <= @EndDateTime");
            parameters.Add(new SugarParameter("@EndDateTime", query.EndDateTime.Value));
        }
        
        var whereClause = string.Join(" AND ", whereConditions);
        
        // 创建Excel工作簿
        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = workbook.CreateSheet("数据报表");
        
        // 创建标题行
        IRow headerRow = sheet.CreateRow(0);
        for (int i = 0; i < visibleColumns.Count; i++)
        {
            var cell = headerRow.CreateCell(i);
            cell.SetCellValue(visibleColumns[i].Label ?? visibleColumns[i].Prop ?? "");
            
            // 设置标题行样式
            ICellStyle headerStyle = workbook.CreateCellStyle();
            IFont headerFont = workbook.CreateFont();
            headerFont.IsBold = true;
            headerStyle.SetFont(headerFont);
            cell.CellStyle = headerStyle;
        }
        
        // 🔥 关键优化：分批查询数据，避免一次性加载所有数据到内存
        const int batchSize = 1000; // 每批处理1000条数据
        int currentRow = 1; // 从第2行开始（第1行是标题）
        int offset = 0;
        bool hasMoreData = true;
        
        LoggerAdapter.LogInformation($"ExportToExcelAsync - 开始分批导出数据，每批 {batchSize} 条");
        
        while (hasMoreData)
        {
            // 分批查询数据
            var batchSql = $"SELECT * FROM {tableName} WHERE {whereClause} ORDER BY creation_time DESC LIMIT @BatchSize OFFSET @Offset";
            var batchParameters = new List<SugarParameter>(parameters)
            {
                new SugarParameter("@BatchSize", batchSize),
                new SugarParameter("@Offset", offset)
            };
            
            var batchItems = await DbContext.Ado.SqlQueryAsync<dynamic>(batchSql, batchParameters.ToArray());
            
            if (batchItems == null || batchItems.Count == 0)
            {
                hasMoreData = false;
                break;
            }
            
            LoggerAdapter.LogInformation($"ExportToExcelAsync - 处理第 {offset / batchSize + 1} 批数据，共 {batchItems.Count} 条，当前总行数: {currentRow}");
            
            // 将数据写入Excel
            foreach (var item in batchItems)
            {
                IRow dataRow = sheet.CreateRow(currentRow);
                
                for (int colIndex = 0; colIndex < visibleColumns.Count; colIndex++)
                {
                    var column = visibleColumns[colIndex];
                    var cell = dataRow.CreateCell(colIndex);
                    
                    // 获取字段值（snake_case 转 PascalCase）
                    var propName = column.Prop ?? "";
                    object? cellValue = null;
                    
                    // 尝试从动态对象中获取值
                    if (item != null)
                    {
                        var dict = (IDictionary<string, object>)item;
                        // 先尝试直接匹配（后端返回的是 snake_case）
                        if (dict.ContainsKey(propName))
                        {
                            cellValue = dict[propName];
                        }
                        else
                        {
                            // 尝试转换为 snake_case 查找
                            var snakeCaseName = ConvertToSnakeCase(propName);
                            if (dict.ContainsKey(snakeCaseName))
                            {
                                cellValue = dict[snakeCaseName];
                            }
                        }
                    }
                    
                    // 设置单元格值
                    if (cellValue != null)
                    {
                        if (propName == "CreationTime" && cellValue is DateTime dt)
                        {
                            cell.SetCellValue(dt.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        else if (cellValue is DateTime dateTime)
                        {
                            cell.SetCellValue(dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        else
                        {
                            cell.SetCellValue(cellValue.ToString() ?? "");
                        }
                    }
                    else
                    {
                        cell.SetCellValue("");
                    }
                }
                
                currentRow++;
            }
            
            // 如果这批数据少于 batchSize，说明已经是最后一批了
            if (batchItems.Count < batchSize)
            {
                hasMoreData = false;
            }
            else
            {
                offset += batchSize;
            }
        }
        
        LoggerAdapter.LogInformation($"ExportToExcelAsync - 导出完成，共 {currentRow - 1} 条数据");
        
        // 将工作簿写入内存流
        using var memoryStream = new MemoryStream();
        workbook.Write(memoryStream, true);
        workbook.Close();
        
        return memoryStream.ToArray();
    }
    
    /// <summary>
    /// 去掉字符串前后的引号（单引号、双引号）和空格
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns>处理后的字符串</returns>
    private string TrimQuotes(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        
        // 先去掉前后空格
        var trimmed = input.Trim();
        
        // 去掉前后的单引号或双引号
        if (trimmed.Length >= 2)
        {
            char firstChar = trimmed[0];
            char lastChar = trimmed[trimmed.Length - 1];
            
            // 如果前后都是引号（单引号或双引号），则去掉
            if ((firstChar == '"' && lastChar == '"') || 
                (firstChar == '\'' && lastChar == '\''))
            {
                trimmed = trimmed.Substring(1, trimmed.Length - 2);
            }
        }
        
        return trimmed.Trim(); // 再次去掉可能存在的空格
    }
    
    /// <summary>
    /// 将 PascalCase 转换为 snake_case
    /// </summary>
    private string ConvertToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        
        var result = new StringBuilder();
        result.Append(char.ToLowerInvariant(input[0]));
        
        for (int i = 1; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
            {
                result.Append('_');
                result.Append(char.ToLowerInvariant(input[i]));
            }
            else
            {
                result.Append(input[i]);
            }
        }
        
        return result.ToString();
    }
}

public class AssetNumberObj
{
    public string? EquipName { get; set; }
    public string? AssetNumber { get; set; }
}

public class ColumnObj
{
    /// <summary>
    /// 名称
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// 属性名
    /// </summary>
    public string? Prop { get; set; }

    /// <summary>
    /// 固定
    /// </summary>
    public string? Fixed { get; set; }

    /// <summary>
    /// 宽度
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// 隐藏
    /// </summary>
    public bool? Hide { get; set; }
}
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Dynamic;
using Hgzn.Mes.Application.Main.Dtos.Audit;
using Hgzn.Mes.Application.Main.Services.Audit.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using StackExchange.Redis;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services.Audit;

/// <summary>
/// 增量备份服务实现
/// </summary>
public class IncrementalBackupService : IIncrementalBackupService
{
    private readonly SqlSugarContext _dbContext;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private const string BackupTimestampKeyPrefix = "incremental_backup:timestamp:";

    public IncrementalBackupService(
        SqlSugarContext dbContext,
        IConnectionMultiplexer connectionMultiplexer)
    {
        _dbContext = dbContext;
        _connectionMultiplexer = connectionMultiplexer;
    }

    /// <summary>
    /// 增量备份导出（只导出JSON格式，包含所有数据）
    /// </summary>
    public async Task<byte[]> ExportIncrementalBackupAsync(IncrementalBackupRequestDto request)
    {
        if (string.IsNullOrEmpty(request.ClientId))
        {
            throw new ArgumentException("ClientId不能为空", nameof(request));
        }

        try
        {
            // 1. 获取上次导出时间
            DateTime? lastExportTime = request.LastExportTime;
            if (!lastExportTime.HasValue)
            {
                lastExportTime = await GetLastExportTimestampAsync(request.ClientId);
            }

            // 2. 获取要导出的表列表
            var tables = await GetTablesToExportAsync(request.TableName, request.DatabaseName);

            // 3. 查询增量数据（只查询新增/修改的数据，不处理删除）
            var allData = new Dictionary<string, List<Dictionary<string, object>>>();

            foreach (var table in tables)
            {
                // 查询新增/修改的数据
                var tableData = await QueryInsertUpdateDataAsync(
                    table, lastExportTime, false); // 不包含已删除的数据
                
                if (tableData.Count > 0)
                {
                    // 获取表的UUID列信息，用于规范化UUID值
                    var uuidColumns = await GetUuidColumnsAsync(table);
                    
                    // #region agent log
                    try
                    {
                        var logData = new
                        {
                            tableName = table,
                            uuidColumns = uuidColumns.ToList(),
                            recordCount = tableData.Count
                        };
                        var logJson = JsonSerializer.Serialize(new
                        {
                            sessionId = "debug-session",
                            runId = "run1",
                            hypothesisId = "EXPORT",
                            location = $"{nameof(IncrementalBackupService)}.cs:{new StackTrace().GetFrame(0)?.GetFileLineNumber()}",
                            message = "导出前：获取表的UUID列信息",
                            data = logData,
                            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                        });
                        await File.AppendAllTextAsync(@"d:\ALast\.cursor\debug.log", logJson + Environment.NewLine);
                    }
                    catch { }
                    // #endregion
                    
                    // 将dynamic对象转换为Dictionary，并规范化UUID列的值
                    var normalizedRecords = new List<Dictionary<string, object>>();
                    foreach (var record in tableData)
                    {
                        var normalizedRecord = NormalizeRecordForExport(record, uuidColumns);
                        normalizedRecords.Add(normalizedRecord);
                        
                        // #region agent log - 只记录前3条
                        try
                        {
                            if (normalizedRecords.Count <= 3)
                            {
                                var uuidValues = new Dictionary<string, object>();
                                foreach (var uuidCol in uuidColumns)
                                {
                                    if (normalizedRecord.ContainsKey(uuidCol))
                                    {
                                        var val = normalizedRecord[uuidCol];
                                        uuidValues[uuidCol] = new
                                        {
                                            value = val?.ToString(),
                                            type = val?.GetType().Name ?? "null"
                                        };
                                    }
                                }
                                var logData = new
                                {
                                    tableName = table,
                                    recordIndex = normalizedRecords.Count - 1,
                                    uuidValues
                                };
                                var logJson = JsonSerializer.Serialize(new
                                {
                                    sessionId = "debug-session",
                                    runId = "run1",
                                    hypothesisId = "EXPORT",
                                    location = $"{nameof(IncrementalBackupService)}.cs:{new StackTrace().GetFrame(0)?.GetFileLineNumber()}",
                                    message = "导出时：规范化后的UUID值",
                                    data = logData,
                                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                                });
                                await File.AppendAllTextAsync(@"d:\ALast\.cursor\debug.log", logJson + Environment.NewLine);
                            }
                        }
                        catch { }
                        // #endregion
                    }
                    
                    allData[table] = normalizedRecords;
                }
            }

            // 4. 导出为JSON格式
            var result = new
            {
                version = "2.0",
                exportTime = DateTime.Now.ToString("O"),
                lastExportTime = lastExportTime?.ToString("O"),
                clientId = request.ClientId,
                database = request.DatabaseName ?? _dbContext.DbContext.Ado.Connection.Database,
                statistics = new
                {
                    tableCount = allData.Count,
                    totalRecordCount = allData.Sum(t => t.Value.Count)
                },
                data = allData.Select(kvp => new
                {
                    tableName = kvp.Key,
                    records = kvp.Value
                })
            };

            // 使用自定义转换器确保UUID值被正确序列化
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = global::System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            
            // 添加自定义转换器，确保Dictionary中的Guid值被序列化为标准格式的字符串
            jsonOptions.Converters.Add(new GuidToStringConverter());
            
            var json = JsonSerializer.Serialize(result, jsonOptions);

            var exportData = Encoding.UTF8.GetBytes(json);

            // 5. 压缩（如果需要）
            if (request.Compress)
            {
                exportData = CompressData(exportData);
            }

            // 6. 更新导出时间戳
            if (request.UpdateTimestamp)
            {
                await SaveExportTimestampAsync(request.ClientId, DateTime.Now);
            }

            LoggerAdapter.LogInformation(
                $"增量备份导出完成 - ClientId: {request.ClientId}, " +
                $"表数: {allData.Count}, " +
                $"总记录数: {allData.Sum(t => t.Value.Count)}");

            return exportData;
        }
        catch (Exception ex)
        {
            LoggerAdapter.LogError($"增量备份导出失败 - ClientId: {request.ClientId}, 错误: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 获取增量备份状态
    /// </summary>
    public async Task<IncrementalBackupStatusDto> GetBackupStatusAsync(string clientId)
    {
        var timestamp = await GetLastExportTimestampAsync(clientId);
        var key = $"{BackupTimestampKeyPrefix}{clientId}:count";
        var db = _connectionMultiplexer.GetDatabase();
        var countStr = await db.StringGetAsync(key);

        return new IncrementalBackupStatusDto
        {
            ClientId = clientId,
            LastExportTime = timestamp,
            HasExportHistory = timestamp.HasValue,
            LastExportRecordCount = countStr.HasValue && int.TryParse(countStr, out var count) ? count : null
        };
    }

    /// <summary>
    /// 获取增量备份元数据信息
    /// </summary>
    public async Task<IncrementalBackupResultDto> GetBackupMetadataAsync(IncrementalBackupRequestDto request)
    {
        var lastExportTime = request.LastExportTime ?? 
            await GetLastExportTimestampAsync(request.ClientId);

        var tables = await GetTablesToExportAsync(request.TableName, request.DatabaseName);
        
        int recordCount = 0;

        foreach (var table in tables)
        {
            var tableData = await QueryInsertUpdateDataAsync(
                table, lastExportTime, false); // 不包含已删除的数据
            recordCount += tableData.Count;
        }

        return new IncrementalBackupResultDto
        {
            ExportTime = DateTime.Now,
            LastExportTime = lastExportTime,
            RecordCount = recordCount
        };
    }

    #region 私有方法

    /// <summary>
    /// 获取要导出的表列表
    /// </summary>
    private async Task<List<string>> GetTablesToExportAsync(string? tableName, string? databaseName)
    {
        if (!string.IsNullOrEmpty(tableName))
        {
            return new List<string> { tableName };
        }

        // 查询所有用户表
        var dbName = databaseName ?? _dbContext.DbContext.Ado.Connection.Database;
        var sql = @"
            SELECT table_name 
            FROM information_schema.tables 
            WHERE table_schema = 'public' 
            AND table_type = 'BASE TABLE'
            ORDER BY table_name";

        var tables = await _dbContext.DbContext.Ado.SqlQueryAsync<string>(sql);
        return tables?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// 查询新增/修改的数据
    /// </summary>
    private async Task<List<dynamic>> QueryInsertUpdateDataAsync(
        string tableName, 
        DateTime? lastExportTime, 
        bool includeDeleted)
    {
        var whereConditions = new List<string>();
        var parameters = new List<SugarParameter>();

        // 增量时间过滤
        if (lastExportTime.HasValue)
        {
            // 检查表是否有creation_time和last_modification_time字段
            var hasCreationTime = await HasColumnAsync(tableName, "creation_time");
            var hasModificationTime = await HasColumnAsync(tableName, "last_modification_time");

            if (hasCreationTime && hasModificationTime)
            {
                whereConditions.Add(
                    "(creation_time > @LastExportTime OR last_modification_time > @LastExportTime)");
            }
            else if (hasCreationTime)
            {
                whereConditions.Add("creation_time > @LastExportTime");
            }
            else if (hasModificationTime)
            {
                whereConditions.Add("last_modification_time > @LastExportTime");
            }

            if (whereConditions.Any())
            {
                parameters.Add(new SugarParameter("@LastExportTime", lastExportTime.Value));
            }
        }

        // 软删除过滤
        var hasSoftDeleted = await HasColumnAsync(tableName, "soft_deleted");
        if (hasSoftDeleted && !includeDeleted)
        {
            whereConditions.Add("soft_deleted = 0");
        }

        var whereClause = whereConditions.Any() 
            ? $"WHERE {string.Join(" AND ", whereConditions)}" 
            : "";

        // PostgreSQL/OpenGauss表名需要用双引号包裹
        var orderByClause = await HasColumnAsync(tableName, "creation_time") 
            ? "ORDER BY creation_time ASC" 
            : "";

        var sql = $@"SELECT * FROM ""{tableName}"" {whereClause} {orderByClause} LIMIT 10000";
        
        try
        {
            return await _dbContext.DbContext.Ado.SqlQueryAsync<dynamic>(sql, parameters.ToArray());
        }
        catch (Exception ex)
        {
            LoggerAdapter.LogWarning($"查询表 {tableName} 数据失败，可能表不存在或字段不匹配: {ex.Message}");
            return new List<dynamic>();
        }
    }


    /// <summary>
    /// 检查表是否存在
    /// </summary>
    private async Task<bool> TableExistsAsync(string tableName)
    {
        try
        {
            // PostgreSQL/OpenGauss中，使用information_schema.tables检查表是否存在
            var sql = @"
                SELECT COUNT(*) 
                FROM information_schema.tables 
                WHERE table_schema = 'public'
                AND LOWER(table_name) = LOWER(@TableName)";

            var count = await _dbContext.DbContext.Ado.GetIntAsync(
                sql, 
                new SugarParameter("@TableName", tableName));

            return count > 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 检查表是否有指定列
    /// </summary>
    private async Task<bool> HasColumnAsync(string tableName, string columnName)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) 
                FROM information_schema.columns 
                WHERE table_schema = 'public'
                AND LOWER(table_name) = LOWER(@TableName) 
                AND LOWER(column_name) = LOWER(@ColumnName)";

            var count = await _dbContext.DbContext.Ado.GetIntAsync(
                sql, 
                new SugarParameter("@TableName", tableName),
                new SugarParameter("@ColumnName", columnName));

            return count > 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 获取表的UUID类型列列表
    /// </summary>
    private async Task<HashSet<string>> GetUuidColumnsAsync(string tableName)
    {
        try
        {
            // PostgreSQL的information_schema中table_name是小写的，使用LOWER确保匹配，并指定schema
            var sql = @"
                SELECT column_name 
                FROM information_schema.columns 
                WHERE table_schema = 'public'
                AND LOWER(table_name) = LOWER(@TableName) 
                AND data_type = 'uuid'";

            var columns = await _dbContext.DbContext.Ado.SqlQueryAsync<string>(
                sql, 
                new SugarParameter("@TableName", tableName));

            return new HashSet<string>(columns ?? new List<string>(), StringComparer.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            var errorMsg = ex.Message ?? "";
            // 检查是否是事务中止错误
            var isTransactionAborted = errorMsg.Contains("25P02") || 
                                      errorMsg.Contains("current transaction is aborted");
            
            if (isTransactionAborted)
            {
                LoggerAdapter.LogError($"查询表 {tableName} 的UUID列失败，事务已中止: {ex.Message}");
                throw; // 重新抛出异常，让事务回滚
            }
            
            LoggerAdapter.LogWarning($"查询表 {tableName} 的UUID列失败: {ex.Message}");
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// 规范化记录以便导出（确保UUID列的值是标准格式的字符串）
    /// </summary>
    private Dictionary<string, object> NormalizeRecordForExport(dynamic record, HashSet<string> uuidColumns)
    {
        var normalizedRecord = new Dictionary<string, object>();
        
        // 将dynamic对象转换为字典
        IDictionary<string, object>? recordDict = null;
        if (record is IDictionary<string, object> dict)
        {
            recordDict = dict;
        }
        else if (record is global::System.Dynamic.ExpandoObject expando)
        {
            recordDict = (IDictionary<string, object>)expando;
        }
        else
        {
            // 尝试通过反射获取属性
            recordDict = new Dictionary<string, object>();
            var type = record.GetType();
            foreach (var prop in type.GetProperties())
            {
                try
                {
                    var value = prop.GetValue(record);
                    recordDict[prop.Name] = value ?? (object?)null!;
                }
                catch
                {
                    // 忽略无法访问的属性
                }
            }
        }
        
        if (recordDict == null)
        {
            return normalizedRecord;
        }
        
        // 处理每个字段
        foreach (var kvp in recordDict)
        {
            var columnName = kvp.Key;
            var value = kvp.Value;
            
            // 如果是UUID列，确保值是标准格式的字符串
            if (uuidColumns.Contains(columnName, StringComparer.OrdinalIgnoreCase))
            {
                if (value == null || value == DBNull.Value)
                {
                    normalizedRecord[columnName] = null!;
                }
                else if (value is Guid guid)
                {
                    // 已经是Guid类型，转换为标准格式的字符串
                    normalizedRecord[columnName] = guid.ToString("D"); // D格式：xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
                }
                else if (value is string strValue)
                {
                    // 如果是字符串，验证并规范化格式
                    if (Guid.TryParse(strValue, out var parsedGuid))
                    {
                        normalizedRecord[columnName] = parsedGuid.ToString("D");
                    }
                    else
                    {
                        // 无法解析为Guid，保持原值（可能会在导入时失败，但至少记录原始值）
                        LoggerAdapter.LogWarning($"导出时发现UUID列 {columnName} 的值不是有效的GUID格式: {strValue}");
                        normalizedRecord[columnName] = strValue;
                    }
                }
                else
                {
                    // 其他类型，尝试转换为字符串再解析
                    var str = value.ToString();
                    if (!string.IsNullOrEmpty(str) && Guid.TryParse(str, out var parsedGuid2))
                    {
                        normalizedRecord[columnName] = parsedGuid2.ToString("D");
                    }
                    else
                    {
                        LoggerAdapter.LogWarning($"导出时发现UUID列 {columnName} 的值无法转换为GUID: {value} (类型: {value?.GetType().Name ?? "null"})");
                        normalizedRecord[columnName] = str ?? "";
                    }
                }
            }
            else
            {
                // 非UUID列，直接使用原值
                normalizedRecord[columnName] = value ?? null!;
            }
        }
        
        return normalizedRecord;
    }

    /// <summary>
    /// 获取表的JSON类型列列表
    /// </summary>
    private async Task<HashSet<string>> GetJsonColumnsAsync(string tableName)
    {
        try
        {
            // PostgreSQL的information_schema中table_name是小写的，使用LOWER确保匹配，并指定schema
            var sql = @"
                SELECT column_name
                FROM information_schema.columns 
                WHERE table_schema = 'public'
                AND LOWER(table_name) = LOWER(@TableName) 
                AND (data_type = 'json' OR data_type = 'jsonb')";

            var columns = await _dbContext.DbContext.Ado.SqlQueryAsync<string>(
                sql, 
                new SugarParameter("@TableName", tableName));

            var result = new HashSet<string>(columns ?? new List<string>(), StringComparer.OrdinalIgnoreCase);
            
            // #region agent log
            try
            {
                // 同时查询所有列的类型，用于调试
                var allColumnsSql = @"
                    SELECT column_name, data_type
                    FROM information_schema.columns 
                    WHERE table_schema = 'public'
                    AND LOWER(table_name) = LOWER(@TableName)
                    ORDER BY ordinal_position";
                var allColumns = await _dbContext.DbContext.Ado.SqlQueryAsync<dynamic>(
                    allColumnsSql,
                    new SugarParameter("@TableName", tableName));
                
                var allColumnsList = new List<object>();
                if (allColumns != null)
                {
                    foreach (var c in allColumns.Take(10))
                    {
                        try
                        {
                            var dict = (IDictionary<string, object>)c;
                            allColumnsList.Add(new { 
                                column_name = dict.ContainsKey("column_name") ? dict["column_name"]?.ToString() : null,
                                data_type = dict.ContainsKey("data_type") ? dict["data_type"]?.ToString() : null
                            });
                        }
                        catch { }
                    }
                }
                
                var logData = new
                {
                    tableName,
                    jsonColumnsCount = result.Count,
                    jsonColumns = result.ToList(),
                    allColumns = allColumnsList
                };
                var logJson = JsonSerializer.Serialize(new
                {
                    sessionId = "debug-session",
                    runId = "run1",
                    hypothesisId = "JSON",
                    location = $"{nameof(IncrementalBackupService)}.cs:{new StackTrace().GetFrame(0)?.GetFileLineNumber()}",
                    message = "GetJsonColumnsAsync查询结果（修复后）",
                    data = logData,
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                });
                await File.AppendAllTextAsync(@"d:\ALast\.cursor\debug.log", logJson + Environment.NewLine);
            }
            catch { }
            // #endregion
            
            return result;
        }
        catch (Exception ex)
        {
            var errorMsg = ex.Message ?? "";
            // 检查是否是事务中止错误
            var isTransactionAborted = errorMsg.Contains("25P02") || 
                                      errorMsg.Contains("current transaction is aborted");
            
            if (isTransactionAborted)
            {
                LoggerAdapter.LogError($"查询表 {tableName} 的JSON列失败，事务已中止: {ex.Message}");
                throw; // 重新抛出异常，让事务回滚
            }
            
            LoggerAdapter.LogWarning($"查询表 {tableName} 的JSON列失败: {ex.Message}");
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
    }


    /// <summary>
    /// 压缩数据
    /// </summary>
    private byte[] CompressData(byte[] data)
    {
        using var output = new MemoryStream();
        using (var gzip = new global::System.IO.Compression.GZipStream(
            output, global::System.IO.Compression.CompressionLevel.Optimal))
        {
            gzip.Write(data, 0, data.Length);
        }
        return output.ToArray();
    }

    /// <summary>
    /// 获取上次导出时间戳
    /// </summary>
    private async Task<DateTime?> GetLastExportTimestampAsync(string clientId)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();
            var key = $"{BackupTimestampKeyPrefix}{clientId}";
            var value = await db.StringGetAsync(key);

            if (value.HasValue && DateTime.TryParse(value, out var timestamp))
            {
                return timestamp;
            }

            return null;
        }
        catch (Exception ex)
        {
            LoggerAdapter.LogWarning($"获取导出时间戳失败 - ClientId: {clientId}, 错误: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 保存导出时间戳
    /// </summary>
    private async Task SaveExportTimestampAsync(string clientId, DateTime timestamp)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();
            var key = $"{BackupTimestampKeyPrefix}{clientId}";
            await db.StringSetAsync(key, timestamp.ToString("O")); // ISO 8601格式
        }
        catch (Exception ex)
        {
            LoggerAdapter.LogWarning($"保存导出时间戳失败 - ClientId: {clientId}, 错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 导入增量备份文件并更新数据库（使用Storageable逻辑）
    /// </summary>
    public async Task<IncrementalBackupImportResultDto> ImportIncrementalBackupAsync(IncrementalBackupImportRequestDto request)
    {
        try
        {
            LoggerAdapter.LogInformation($"开始导入增量备份文件 - Compress: {request.Compress}");

            // 1. 解压文件（如果需要）
            byte[] fileData = request.FileData;
            if (request.Compress)
            {
                fileData = DecompressData(fileData);
            }

            // 2. 解析JSON并执行导入
            var result = await ImportFromJsonAsync(fileData);

            LoggerAdapter.LogInformation(
                $"增量备份导入完成 - 新增/更新: {result.insertUpdateCount}");

            return new IncrementalBackupImportResultDto
            {
                Success = true,
                InsertUpdateCount = result.insertUpdateCount,
                ImportTime = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            LoggerAdapter.LogError($"增量备份导入失败: {ex.Message}");
            return new IncrementalBackupImportResultDto
            {
                Success = false,
                ErrorMessage = ex.Message,
                ImportTime = DateTime.Now
            };
        }
    }

    #endregion

    #region 导入相关私有方法

    /// <summary>
    /// 从JSON文件导入（使用Storageable逻辑：根据id判断插入或更新）
    /// </summary>
    private async Task<(int insertUpdateCount, int deleteCount)> ImportFromJsonAsync(byte[] fileData)
    {
        var text = Encoding.UTF8.GetString(fileData);
        var jsonDoc = JsonDocument.Parse(text);
        var root = jsonDoc.RootElement;

        int insertUpdateCount = 0;
        int deleteCount = 0; // 不再处理删除，保留为0

        // 使用事务执行所有操作
        await _dbContext.DbContext.Ado.BeginTranAsync();
        try
        {
            // 解析JSON结构（新格式：data是数组，每个元素包含tableName和records）
            if (root.TryGetProperty("data", out var dataElement))
            {
                foreach (var tableDataElement in dataElement.EnumerateArray())
                {
                    if (tableDataElement.TryGetProperty("tableName", out var tableNameElement))
                    {
                        var tableName = tableNameElement.GetString();
                        if (tableDataElement.TryGetProperty("records", out var recordsElement))
                        {
                            // 先检查表是否存在
                            if (!await TableExistsAsync(tableName!))
                            {
                                LoggerAdapter.LogWarning($"表 {tableName} 在目标数据库中不存在，跳过导入。这通常发生在跨数据库导入时，某些动态表可能不存在。");
                                continue; // 跳过不存在的表，继续处理下一个表
                            }

                            try
                            {
                                // 使用Storageable逻辑处理该表的数据
                                var count = await ApplyTableDataWithStorageableAsync(tableName!, recordsElement);
                                insertUpdateCount += count;
                            }
                            catch (Exception ex)
                            {
                                var errorMsg = ex.Message ?? "";
                                // 检查是否是表不存在的错误（42P01）
                                var isTableNotExists = errorMsg.Contains("42P01") || 
                                                      errorMsg.Contains("does not exist") ||
                                                      (ex.InnerException != null && 
                                                       (ex.InnerException.Message.Contains("42P01") || 
                                                        ex.InnerException.Message.Contains("does not exist")));
                                
                                if (isTableNotExists)
                                {
                                    // 表不存在错误，记录警告并跳过，不中断整个导入过程
                                    LoggerAdapter.LogWarning($"处理表 {tableName} 时发现表不存在，跳过导入: {ex.Message}");
                                    continue;
                                }
                                
                                // 其他错误，记录详细的错误信息，包括表名
                                LoggerAdapter.LogError($"处理表 {tableName} 时发生错误: {ex.Message}");
                                LoggerAdapter.LogError($"错误详情: {ex}");
                                // 重新抛出异常，让事务回滚
                                throw new Exception($"处理表 {tableName} 时发生错误: {ex.Message}", ex);
                            }
                        }
                    }
                }
            }

            await _dbContext.DbContext.Ado.CommitTranAsync();
        }
        catch (Exception ex)
        {
            await _dbContext.DbContext.Ado.RollbackTranAsync();
            LoggerAdapter.LogError($"增量备份导入事务回滚: {ex.Message}");
            throw;
        }

        return (insertUpdateCount, deleteCount);
    }

    /// <summary>
    /// 使用Storageable逻辑应用表数据（根据id判断插入或更新）
    /// </summary>
    private async Task<int> ApplyTableDataWithStorageableAsync(string tableName, JsonElement recordsElement)
    {
        if (recordsElement.ValueKind != JsonValueKind.Array)
        {
            return 0;
        }

        // 再次检查表是否存在（双重保险）
        if (!await TableExistsAsync(tableName))
        {
            LoggerAdapter.LogWarning($"表 {tableName} 在目标数据库中不存在，跳过导入");
            return 0;
        }

        // 查询表的UUID类型列和JSON类型列，确保这些列的值类型正确
        var uuidColumns = await GetUuidColumnsAsync(tableName);
        var jsonColumns = await GetJsonColumnsAsync(tableName);
        LoggerAdapter.LogInformation($"表 {tableName} 的UUID列: {string.Join(", ", uuidColumns)}");
        LoggerAdapter.LogInformation($"表 {tableName} 的JSON列: {string.Join(", ", jsonColumns)}");

        var records = new List<Dictionary<string, object>>();
        foreach (var recordElement in recordsElement.EnumerateArray())
        {
            var record = new Dictionary<string, object>();
            foreach (var prop in recordElement.EnumerateObject())
            {
                var value = GetJsonValue(prop.Value);
                
                // 如果该列是UUID类型，确保值是Guid类型
                if (uuidColumns.Contains(prop.Name, StringComparer.OrdinalIgnoreCase))
                {
                    if (value == null)
                    {
                        // null值保持不变
                        record[prop.Name] = null!;
                        continue;
                    }
                    
                    // 如果已经是Guid类型，直接使用
                    if (value is Guid)
                    {
                        record[prop.Name] = value;
                        continue;
                    }
                    
                    // 尝试转换为Guid
                    Guid? guidValue = null;
                    if (value is string strValue)
                    {
                        if (Guid.TryParse(strValue, out var parsedGuid))
                        {
                            guidValue = parsedGuid;
                        }
                    }
                    else if (value is Guid existingGuid)
                    {
                        guidValue = existingGuid;
                    }
                    else
                    {
                        // 尝试将值转换为字符串再解析
                        var str = value.ToString();
                        if (!string.IsNullOrEmpty(str) && Guid.TryParse(str, out var parsedGuid2))
                        {
                            guidValue = parsedGuid2;
                        }
                    }
                    
                    if (guidValue.HasValue)
                    {
                        record[prop.Name] = guidValue.Value;
                        LoggerAdapter.LogInformation($"表 {tableName} 的列 {prop.Name} 已转换为Guid类型: {guidValue.Value} (类型: {guidValue.Value.GetType().Name})");
                    }
                    else
                    {
                        LoggerAdapter.LogWarning($"表 {tableName} 的列 {prop.Name} 应该是UUID类型，但无法转换为Guid。原始值: {value} (类型: {value?.GetType().Name ?? "null"})");
                        // 如果无法转换，仍然使用原值（可能会失败，但至少会记录警告）
                        record[prop.Name] = value!;
                    }
                }
                else
                {
                    // 非UUID列，直接使用原值
                    // SqlSugar Storageable 需要 null 而不是 DBNull.Value，object 类型可以直接接受 null
                    record[prop.Name] = value!;
                }
            }
            records.Add(record);
        }

        if (records.Count == 0)
        {
            return 0;
        }

        LoggerAdapter.LogInformation($"处理表 {tableName}，共 {records.Count} 条记录");

        // 检查是否有id字段（支持id或Id）
        var firstRecord = records.First();
        var idKey = firstRecord.ContainsKey("id") ? "id" : 
                   firstRecord.ContainsKey("Id") ? "Id" : 
                   firstRecord.ContainsKey("uuid") ? "uuid" :
                   firstRecord.ContainsKey("Uuid") ? "Uuid" : null;
        
        if (string.IsNullOrEmpty(idKey))
        {
            // 没有id字段，直接批量插入
            LoggerAdapter.LogWarning($"表 {tableName} 没有id/uuid字段，将直接插入所有记录");
            
            // 如果有UUID列，使用自定义SQL插入
            if (uuidColumns.Count > 0)
            {
                var count = await InsertRecordsWithCustomSqlAsync(tableName, records, uuidColumns, jsonColumns);
                LoggerAdapter.LogInformation($"表 {tableName} 插入完成 - 插入: {count}");
                return count;
            }
            else
            {
                var insertable = _dbContext.DbContext.Insertable(records).AS(tableName);
                var count = await insertable.ExecuteCommandAsync();
                LoggerAdapter.LogInformation($"表 {tableName} 插入完成 - 插入: {count}");
                return count;
            }
        }

        // 如果有UUID列，使用自定义SQL方式导入（确保UUID类型正确）
        if (uuidColumns.Count > 0)
        {
            LoggerAdapter.LogInformation($"表 {tableName} 包含UUID列，使用自定义SQL方式导入");
            
            // 先查询已存在的记录ID
            var ids = records.Select(r => r[idKey]).ToList();
            var existingIds = await QueryExistingIdsAsync(tableName, idKey, ids);
            
            // 分离插入和更新
            var toInsert = records.Where(r =>
            {
                var recordId = r[idKey]?.ToString() ?? "";
                return !existingIds.Contains(recordId);
            }).ToList();
            
            var toUpdate = records.Where(r =>
            {
                var recordId = r[idKey]?.ToString() ?? "";
                return existingIds.Contains(recordId);
            }).ToList();
            
            int insertCount = 0;
            int updateCount = 0;
            
            if (toInsert.Count > 0)
            {
                insertCount = await InsertRecordsWithCustomSqlAsync(tableName, toInsert, uuidColumns, jsonColumns);
            }
            
            if (toUpdate.Count > 0)
            {
                updateCount = await UpdateRecordsWithCustomSqlAsync(tableName, toUpdate, idKey, uuidColumns, jsonColumns);
            }
            
            LoggerAdapter.LogInformation($"表 {tableName} 处理完成 - 插入: {insertCount}, 更新: {updateCount}");
            return insertCount + updateCount;
        }

        // 没有UUID列，使用 Storageable 方法
        // Storageable 方法可以接受字典列表和表名作为参数，WhereColumns 接受字符串列名
        var storage = _dbContext.DbContext.Storageable(records, tableName)
            .WhereColumns(idKey!)
            .ToStorage();
        
        // 不存在插入
        var insertCountStorageable = await storage.AsInsertable.ExecuteCommandAsync();
        
        // 存在更新
        var updateCountStorageable = await storage.AsUpdateable.ExecuteCommandAsync();

        LoggerAdapter.LogInformation($"表 {tableName} 处理完成 - 插入: {insertCountStorageable}, 更新: {updateCountStorageable}");

        return insertCountStorageable + updateCountStorageable;
    }

    /// <summary>
    /// 查询已存在的记录ID（返回HashSet<string>以便统一比较）
    /// </summary>
    private async Task<HashSet<string>> QueryExistingIdsAsync(string tableName, string idColumn, List<object> ids)
    {
        if (ids.Count == 0)
        {
            return new HashSet<string>();
        }

        try
        {
            // 构建IN查询
            var idValues = new List<string>();
            var parameters = new List<SugarParameter>();
            
            for (int i = 0; i < ids.Count; i++)
            {
                var idValue = ids[i];
                // 确保ID是Guid类型（如果是字符串，尝试转换）
                if (idValue is string idStr && Guid.TryParse(idStr, out var idGuid))
                {
                    idValue = idGuid;
                }
                else if (idValue is not Guid && idValue != null)
                {
                    var str = idValue.ToString();
                    if (!string.IsNullOrEmpty(str) && Guid.TryParse(str, out var parsedIdGuid))
                    {
                        idValue = parsedIdGuid;
                    }
                }
                
                var paramName = $"@id{i}";
                idValues.Add(paramName);
                parameters.Add(new SugarParameter(paramName, idValue));
            }

            var idList = string.Join(", ", idValues);
            var sql = $@"SELECT ""{idColumn}"" FROM ""{tableName}"" WHERE ""{idColumn}"" IN ({idList})";
            
            // 使用dynamic类型查询，然后提取ID值
            var existingIdsRaw = await _dbContext.DbContext.Ado.SqlQueryAsync<dynamic>(sql, parameters.ToArray());
            
            // 从dynamic对象中提取ID值，统一转换为字符串
            var result = new HashSet<string>();
            if (existingIdsRaw != null)
            {
                foreach (var item in existingIdsRaw)
                {
                    object? idValue = null;
                    
                    // 如果返回的是ExpandoObject，尝试从属性中获取ID
                    if (item is global::System.Dynamic.ExpandoObject expando)
                    {
                        var dict = (IDictionary<string, object>)expando;
                        if (dict.ContainsKey(idColumn))
                        {
                            idValue = dict[idColumn];
                        }
                        else if (dict.ContainsKey(idColumn.ToLower()))
                        {
                            idValue = dict[idColumn.ToLower()];
                        }
                        else if (dict.Count > 0)
                        {
                            // 取第一个值
                            idValue = dict.Values.First();
                        }
                    }
                    else
                    {
                        // 如果不是ExpandoObject，直接使用
                        idValue = item;
                    }
                    
                    // 转换为字符串用于比较
                    if (idValue != null)
                    {
                        var idStr = idValue is Guid guid ? guid.ToString() : idValue.ToString() ?? "";
                        if (!string.IsNullOrEmpty(idStr))
                        {
                            result.Add(idStr);
                        }
                    }
                }
            }
            
            return result;
        }
        catch (Exception ex)
        {
            var errorMsg = ex.Message ?? "";
            // 检查是否是事务中止错误
            var isTransactionAborted = errorMsg.Contains("25P02") || 
                                      errorMsg.Contains("current transaction is aborted");
            
            if (isTransactionAborted)
            {
                LoggerAdapter.LogError($"查询表 {tableName} 已存在ID失败，事务已中止: {ex.Message}");
                throw; // 重新抛出异常，让事务回滚
            }
            
            LoggerAdapter.LogWarning($"查询表 {tableName} 已存在ID失败: {ex.Message}，将尝试插入所有记录");
            return new HashSet<string>();
        }
    }

    /// <summary>
    /// 使用自定义SQL插入记录（确保UUID列和JSON列类型正确）
    /// </summary>
    private async Task<int> InsertRecordsWithCustomSqlAsync(
        string tableName, 
        List<Dictionary<string, object>> records, 
        HashSet<string> uuidColumns,
        HashSet<string> jsonColumns)
    {
        if (records.Count == 0)
        {
            return 0;
        }

        int totalInserted = 0;
        foreach (var record in records)
        {
            string? sql = null;
            List<SugarParameter>? parameters = null;
            try
            {
                var columns = record.Keys.ToList();
                var columnsQuoted = string.Join(", ", columns.Select(c => $@"""{c}"""));
                
                parameters = new List<SugarParameter>();
                var valueParams = new List<string>();
                int paramIndex = 0; // 参数索引计数器，只有非null值才使用参数
                
                for (int i = 0; i < columns.Count; i++)
                {
                    var column = columns[i];
                    var value = record[column];
                    
                    // 确保UUID列的值是Guid类型（包括null值）
                    if (uuidColumns.Contains(column, StringComparer.OrdinalIgnoreCase))
                    {
                        if (value == null)
                        {
                            // 对于UUID列的null值，直接在SQL中使用NULL，不使用参数
                            valueParams.Add("NULL");
                        }
                        else
                        {
                            var paramName = $"@p{paramIndex}";
                            valueParams.Add(paramName);
                            paramIndex++;
                            
                            // 确保值是Guid类型
                            if (value is Guid guid)
                            {
                                parameters.Add(new SugarParameter(paramName, guid));
                            }
                            else if (value is string strValue && Guid.TryParse(strValue, out var parsedGuid))
                            {
                                parameters.Add(new SugarParameter(paramName, parsedGuid));
                            }
                            else
                            {
                                // 尝试转换为字符串再解析
                                var str = value.ToString();
                                if (!string.IsNullOrEmpty(str) && Guid.TryParse(str, out var parsedGuid2))
                                {
                                    parameters.Add(new SugarParameter(paramName, parsedGuid2));
                                }
                                else
                                {
                                    LoggerAdapter.LogWarning($"表 {tableName} 的列 {column} 应该是UUID类型，但无法转换为Guid: {value}");
                                    parameters.Add(new SugarParameter(paramName, value));
                                }
                            }
                        }
                    }
                    else if (jsonColumns.Contains(column, StringComparer.OrdinalIgnoreCase))
                    {
                        // JSON类型列，需要显式转换为json类型
                        if (value == null)
                        {
                            valueParams.Add("NULL");
                        }
                        else
                        {
                            var paramName = $"@p{paramIndex}";
                            valueParams.Add($"CAST({paramName} AS json)");
                            paramIndex++;
                            
                            if (value is string jsonStr)
                            {
                                parameters.Add(new SugarParameter(paramName, jsonStr));
                            }
                            else
                            {
                                var jsonString = JsonSerializer.Serialize(value);
                                parameters.Add(new SugarParameter(paramName, jsonString));
                            }
                        }
                    }
                    else
                    {
                        // 非UUID列，非JSON列
                        if (value == null)
                        {
                            valueParams.Add("NULL");
                        }
                        else
                        {
                            var paramName = $"@p{paramIndex}";
                            valueParams.Add(paramName);
                            paramIndex++;
                            parameters.Add(new SugarParameter(paramName, value));
                        }
                    }
                }
                
                var valuesStr = string.Join(", ", valueParams);
                sql = $@"INSERT INTO ""{tableName}"" ({columnsQuoted}) VALUES ({valuesStr})";
                
                var count = await _dbContext.DbContext.Ado.ExecuteCommandAsync(sql, parameters.ToArray());
                totalInserted += count;
            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message ?? "";
                var isTransactionAborted = errorMsg.Contains("25P02") || 
                                          errorMsg.Contains("current transaction is aborted");
                
                if (isTransactionAborted)
                {
                    LoggerAdapter.LogError($"插入表 {tableName} 记录失败，事务已中止: {ex.Message}");
                    throw;
                }
                
                LoggerAdapter.LogWarning($"插入表 {tableName} 记录失败: {ex.Message}，SQL: {sql?.Substring(0, Math.Min(200, sql?.Length ?? 0))}");
                throw;
            }
        }
        
        return totalInserted;
    }

    /// <summary>
    /// 使用自定义SQL更新记录（确保UUID列和JSON列类型正确）
    /// </summary>
    private async Task<int> UpdateRecordsWithCustomSqlAsync(
        string tableName, 
        List<Dictionary<string, object>> records, 
        string idKey,
        HashSet<string> uuidColumns,
        HashSet<string> jsonColumns)
    {
        if (records.Count == 0)
        {
            return 0;
        }

        int totalUpdated = 0;
        foreach (var record in records)
        {
            try
            {
                var idValue = record[idKey];
                
                if (idValue == null)
                {
                    LoggerAdapter.LogWarning($"表 {tableName} 的记录ID为null，跳过更新");
                    continue;
                }
                
                var columns = record.Keys.Where(k => !k.Equals(idKey, StringComparison.OrdinalIgnoreCase)).ToList();
                var setClause = new List<string>();
                var parameters = new List<SugarParameter>();
                int paramIndex = 0;
                
                for (int i = 0; i < columns.Count; i++)
                {
                    var column = columns[i];
                    var value = record[column];
                    
                    // 确保UUID列的值是Guid类型（包括null值）
                    if (uuidColumns.Contains(column, StringComparer.OrdinalIgnoreCase))
                    {
                        if (value == null)
                        {
                            setClause.Add($@"""{column}"" = NULL");
                        }
                        else
                        {
                            var paramName = $"@set_{paramIndex}";
                            setClause.Add($@"""{column}"" = {paramName}");
                            paramIndex++;
                            
                            // 确保值是Guid类型
                            if (value is Guid guid)
                            {
                                parameters.Add(new SugarParameter(paramName, guid));
                            }
                            else if (value is string strValue && Guid.TryParse(strValue, out var parsedGuid))
                            {
                                parameters.Add(new SugarParameter(paramName, parsedGuid));
                            }
                            else
                            {
                                var str = value.ToString();
                                if (!string.IsNullOrEmpty(str) && Guid.TryParse(str, out var parsedGuid2))
                                {
                                    parameters.Add(new SugarParameter(paramName, parsedGuid2));
                                }
                                else
                                {
                                    LoggerAdapter.LogWarning($"表 {tableName} 的列 {column} 应该是UUID类型，但无法转换为Guid: {value}");
                                    parameters.Add(new SugarParameter(paramName, value));
                                }
                            }
                        }
                    }
                    else if (jsonColumns.Contains(column, StringComparer.OrdinalIgnoreCase))
                    {
                        // JSON类型列，需要显式转换为json类型
                        if (value == null)
                        {
                            setClause.Add($@"""{column}"" = NULL");
                        }
                        else
                        {
                            var paramName = $"@set_{paramIndex}";
                            setClause.Add($@"""{column}"" = CAST({paramName} AS json)");
                            paramIndex++;
                            
                            if (value is string jsonStr)
                            {
                                parameters.Add(new SugarParameter(paramName, jsonStr));
                            }
                            else
                            {
                                var jsonString = JsonSerializer.Serialize(value);
                                parameters.Add(new SugarParameter(paramName, jsonString));
                            }
                        }
                    }
                    else
                    {
                        // 非UUID列，非JSON列
                        if (value == null)
                        {
                            setClause.Add($@"""{column}"" = NULL");
                        }
                        else
                        {
                            var paramName = $"@set_{paramIndex}";
                            setClause.Add($@"""{column}"" = {paramName}");
                            paramIndex++;
                            parameters.Add(new SugarParameter(paramName, value));
                        }
                    }
                }
                
                // 确保ID参数也是Guid类型
                var idParamName = $"@where_id";
                if (idValue is Guid idGuid)
                {
                    parameters.Add(new SugarParameter(idParamName, idGuid));
                }
                else if (idValue is string idStrValue && Guid.TryParse(idStrValue, out var idGuidValue))
                {
                    parameters.Add(new SugarParameter(idParamName, idGuidValue));
                }
                else
                {
                    var idStr = idValue.ToString();
                    if (!string.IsNullOrEmpty(idStr) && Guid.TryParse(idStr, out var parsedIdGuid))
                    {
                        parameters.Add(new SugarParameter(idParamName, parsedIdGuid));
                    }
                    else
                    {
                        LoggerAdapter.LogWarning($"表 {tableName} 的ID列 {idKey} 无法转换为Guid: {idValue}");
                        parameters.Add(new SugarParameter(idParamName, idValue));
                    }
                }
                
                var setStr = string.Join(", ", setClause);
                var sql = $@"UPDATE ""{tableName}"" SET {setStr} WHERE ""{idKey}"" = {idParamName}";
                
                var count = await _dbContext.DbContext.Ado.ExecuteCommandAsync(sql, parameters.ToArray());
                totalUpdated += count;
            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message ?? "";
                var isTransactionAborted = errorMsg.Contains("25P02") || 
                                          errorMsg.Contains("current transaction is aborted");
                
                if (isTransactionAborted)
                {
                    LoggerAdapter.LogError($"更新表 {tableName} 记录失败，事务已中止: {ex.Message}");
                    throw;
                }
                
                LoggerAdapter.LogWarning($"更新表 {tableName} 记录失败: {ex.Message}");
                throw;
            }
        }
        
        return totalUpdated;
    }

    /// <summary>
    /// 获取JSON值
    /// </summary>
    private object? GetJsonValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            // 不要自动解析UUID字符串为Guid对象，保持为字符串，让导入代码根据列的UUID类型进行转换
            // 这样可以让SqlSugar的Storageable方法在导入代码转换后正确识别Guid类型
            JsonValueKind.String => element.GetString() ?? "",
            JsonValueKind.Number => element.GetDecimal(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,  // SqlSugar Storageable 不支持 DBNull.Value，使用 null
            JsonValueKind.Object => element.GetRawText(),
            JsonValueKind.Array => element.GetRawText(),
            _ => element.GetRawText()
        };
    }

    /// <summary>
    /// 解压数据
    /// </summary>
    private byte[] DecompressData(byte[] compressedData)
    {
        using var input = new MemoryStream(compressedData);
        using var gzip = new global::System.IO.Compression.GZipStream(
            input, global::System.IO.Compression.CompressionMode.Decompress);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return output.ToArray();
    }

    #endregion

    #region 辅助类

    private class TableChangeData
    {
        public string TableName { get; set; } = string.Empty;
        public List<dynamic> Data { get; set; } = new();
        public string OperationType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Guid到字符串的JSON转换器，确保Guid值被序列化为标准格式的字符串
    /// </summary>
    private class GuidToStringConverter : JsonConverter<Guid>
    {
        public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString();
                if (Guid.TryParse(str, out var guid))
                {
                    return guid;
                }
            }
            throw new JsonException($"无法将值转换为Guid");
        }

        public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            // 使用标准格式（D格式：xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx）
            writer.WriteStringValue(value.ToString("D"));
        }
    }

    #endregion
}

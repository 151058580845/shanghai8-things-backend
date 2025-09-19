using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Extensions;
using Hgzn.Mes.Domain.Shared.Utilities;
using Hgzn.Mes.Infrastructure.Utilities;
using StackExchange.Redis;
using System.Text.Json.Serialization;
using System.Text.Json;
using Hgzn.Mes.Domain.Shared.Enum;
using Hgzn.Mes.Domain.Entities.Equip;
using Hgzn.Mes.Domain.Shared.Enums;
using SqlSugar;
using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Hgzn.Mes.Application.Main.Services.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Account;
using System.Collections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using System.Reflection.Metadata;
using Hgzn.Mes.Domain.Entities.System.Location;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Application.Main.Services.App;
using StackExchange.Redis;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipLedgerService : SugarCrudAppService<
        EquipLedger, Guid,
        EquipLedgerReadDto, EquipLedgerQueryDto,
        EquipLedgerCreateDto, EquipLedgerUpdateDto>,
    IEquipLedgerService
{
    private readonly HttpClient _httpClient;
    private readonly ICodeRuleService _codeRuleService;
    private readonly IotMessageHandler _handler;
    private readonly SystemInfoManager _systemInfoManager;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public EquipLedgerService(HttpClient httpClient, ICodeRuleService codeRuleService, IotMessageHandler handler, IMqttExplorer mqttExplorer, SystemInfoManager systemInfoManager, IConnectionMultiplexer connectionMultiplexer)
    {
        _httpClient = httpClient;
        _codeRuleService = codeRuleService;
        _handler = handler;
        _systemInfoManager = systemInfoManager;
        _connectionMultiplexer = connectionMultiplexer;
        _handler.Initialize(mqttExplorer);
    }

    public async Task<EquipLedger> GetEquipByIpAsync(string ipAddress)
    {
        return await Queryable.FirstAsync(t => t.IpAddress == ipAddress);
    }

    public async Task<IEnumerable<NameValueDto>> GetNameValueListAsync()
    {
        var entities = await Queryable
            .Select(t => new NameValueDto
            {
                Id = t.Id,
                Name = t.AssetNumber,
                Value = t.Id.ToString()
            }).ToListAsync();
        return entities;
    }

    public async Task<IEnumerable<EquipLedgerReadDto>> GetEquipsListAsync(string? equipCode, string? equipName)
    {
        var entities = await DbContext.Queryable<EquipLedger>()
            .WhereIF(!string.IsNullOrEmpty(equipCode), t => t.EquipCode == equipCode)
            .WhereIF(!string.IsNullOrEmpty(equipName), t => t.EquipName == equipName)
            .ToListAsync();
        return Mapper.Map<IEnumerable<EquipLedgerReadDto>>(entities);
    }

    public Task<int> UpdateEquipRoomId(Dictionary<string, Guid> equipIds)
    {
        string[] keys = equipIds.Keys.ToArray();
        List<EquipLedger> list = Queryable.Where(t => keys.Contains(t.AssetNumber)).ToList();
        foreach (var equipLedger in list)
        {
            if (equipLedger.AssetNumber == null) continue;
            equipLedger.RoomId = equipIds[equipLedger.AssetNumber];
        }

        return DbContext.Updateable(list).ExecuteCommandAsync();
    }


    /// <summary>
    /// 获取设备列表
    /// </summary>
    /// <param name="equipIds"></param>
    /// <returns></returns>
    public async Task<IEnumerable<EquipLedgerReadDto>> GetEquipsListInIdsAsync(List<Guid> equipIds)
    {
        var entities = await Queryable.Where(t => equipIds.Contains(t.Id)).ToListAsync();
        return Mapper.Map<IEnumerable<EquipLedgerReadDto>>(entities);
    }

    /// <summary>
    /// 获取待搜索记录
    /// </summary>
    /// <returns></returns>
    public async Task<PaginatedList<EquipLedgerSearchReadDto>> GetAppSearchAsync(int pageIndex, int pageSize)
    {
        var entities = await Queryable.Where(t => t.DeviceStatus == DeviceStatus.Lost).Includes(t => t.EquipType)
            .Select(t => new EquipLedgerSearchReadDto()
            {
                Id = t.Id,
                EquipCode = t.EquipCode,
                EquipName = t.EquipName,
                TypeId = t.TypeId,
                TypeName = t.EquipType!.TypeName,
                Model = t.Model,
                RoomId = t.RoomId,
                AssetNumber = t.AssetNumber,
                ResponsibleUserId = t.ResponsibleUserId,
                ResponsibleUserName = t.ResponsibleUserName
            }).ToPaginatedListAsync(pageIndex, pageSize);
        return entities;
    }

    public async Task<IEnumerable<EquipLedger>> GetEquipsListByRoomAsync(IEnumerable<Guid> rooms)
    {
        var equipList = await Queryable.Where(t => t.RoomId != null && rooms.Contains(t.RoomId!.Value)).ToListAsync();
        return Mapper.Map<IEnumerable<EquipLedger>>(equipList);
    }

    public override async Task<PaginatedList<EquipLedgerReadDto>> GetPaginatedListAsync(EquipLedgerQueryDto query)
    {
        var queryable = Queryable
            .WhereIF(!string.IsNullOrEmpty(query.EquipCode), m => m.EquipCode.Contains(query.EquipCode!))
            .WhereIF(!string.IsNullOrEmpty(query.AssetNumber), m => m.AssetNumber!.Contains(query.AssetNumber))
            .WhereIF(query.ResponsibleUserId is not null, m => m.ResponsibleUserId.Equals(query.ResponsibleUserId))
            .WhereIF(!string.IsNullOrEmpty(query.Query),
                m => m.EquipName.Contains(query.Query!) || m.Model!.Contains(query.Query!))
            .WhereIF(!query.TypeId.IsNullableGuidEmpty(), m => m.TypeId.Equals(query.TypeId))
            .WhereIF(query.NoRfidDevice == true, m => m.TypeId == null ||
                                                      (m.TypeId != EquipType.RfidIssuerType.Id &&
                                                       m.TypeId != EquipType.RfidReaderType.Id))
            .WhereIF(!query.RoomId.IsNullableGuidEmpty(), m => m.RoomId.Equals(query.RoomId))
            .WhereIF(query.StartTime != null, m => m.CreationTime >= query.StartTime)
            .WhereIF(query.EndTime != null, m => m.CreationTime <= query.EndTime)
            .WhereIF(query.State != null, m => m.State == query.State);

        if (query.BindingTagCount is not null)
        {
            queryable = queryable.Includes(eq => eq.Labels);
            queryable = query.BindingTagCount == -1
                ? queryable.Where(eq => SqlFunc.Subqueryable<LocationLabel>()
                    .Where(l => l.EquipLedgerId == eq.Id)
                    .Count() > 0)
                : queryable.Where(eq => SqlFunc.Subqueryable<LocationLabel>()
                    .Where(l => l.EquipLedgerId == eq.Id)
                    .Count() == 0);
        }

        var entities = await queryable
            .Includes(t => t.Room)
            .Includes(t => t.EquipType)
            .OrderByDescending(m => m.OrderNum)
            .OrderByDescending(m => m.CreationTime)
            .ToPaginatedListAsync(query.PageIndex, query.PageSize);
        return Mapper.Map<PaginatedList<EquipLedgerReadDto>>(entities);
    }

    public override async Task<IEnumerable<EquipLedgerReadDto>> GetListAsync(EquipLedgerQueryDto? query = null)
    {
        var queryable = query is null
            ? Queryable
            : Queryable
                .WhereIF(!string.IsNullOrEmpty(query.EquipCode), m => m.EquipCode.Contains(query.EquipCode!))
                .WhereIF(!string.IsNullOrEmpty(query.AssetNumber), m => m.AssetNumber == query.AssetNumber)
                .WhereIF(!string.IsNullOrEmpty(query.Query),
                    m => m.EquipName.Contains(query.Query!) ||
                         m.Model!.Contains(query.Query!))
                .WhereIF(query.ResponsibleUserId is not null, m => m.ResponsibleUserId.Equals(query.ResponsibleUserId))
                .WhereIF(!query.TypeId.IsNullableGuidEmpty(), m => m.TypeId.Equals(query.TypeId))
                .WhereIF(query.NoRfidDevice == true, m => m.TypeId == null ||
                                                          (m.TypeId != EquipType.RfidIssuerType.Id &&
                                                           m.TypeId != EquipType.RfidReaderType.Id))
                .WhereIF(!query.RoomId.IsNullableGuidEmpty(), m => m.RoomId.Equals(query.RoomId))
                .WhereIF(query.StartTime != null, m => m.CreationTime >= query.StartTime)
                .WhereIF(query.EndTime != null, m => m.CreationTime <= query.EndTime)
                .WhereIF(query.State != null, m => m.State == query.State);

        var entities = await queryable
            .Includes(t => t.Room)
            .Includes(t => t.EquipType)
            .OrderByDescending(m => m.OrderNum)
            .OrderByDescending(m => m.CreationTime)
            .ToListAsync();
        return Mapper.Map<IEnumerable<EquipLedgerReadDto>>(entities);
    }

    public async Task<IEnumerable<RfidEquipReadDto>> GetRfidEquipsListAsync(Guid equipId)
    {
        List<RfidEquipReadDto> list =
            await DbContext.Queryable<RfidEquipReadDto>().Where(t => t.EquipId == equipId).ToListAsync();
        return list;
    }

    /// <summary>
    ///  Api批量导入
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<int> PostImportDatas(string url)
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

            var result = JsonSerializer.Deserialize<List<EquipLedgerCreateDto>>(jsonResponse, options);

            var changeCount = 0;
            if (result != null && result.Any())
            {
                foreach (var item in result)
                {
                    var info = Mapper.Map<EquipLedger>(item);
                    changeCount += DbContext.Insertable<EquipLedger>(info).ExecuteCommand();
                }
            }

            return changeCount;
        }
        catch (HttpRequestException ex)
        {
            // 处理 HTTP 请求异常
            LoggerAdapter.LogWarning($"HTTP 请求失败: {ex.Message}");
            throw;
        }
        catch (JsonException ex)
        {
            // 处理 JSON 反序列化异常
            LoggerAdapter.LogWarning($"JSON 反序列化失败: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            // 处理其他异常
            LoggerAdapter.LogWarning($"发生错误: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<EquipLedgerReadDto>> GetMissingDevicesAlarmAsync()
    {
        var entities = await DbContext.Queryable<EquipLedger>()
            .Includes(e => e.Room)
            .Where(e => e.IsMovable &&
                        (e.RoomId == null || (int)e.Room!.Purpose >= (int)RoomPurpose.Hallway) &&
                        DateTime.Now.ToLocalTime().AddMinutes(-30) >= e.LastMoveTime).ToArrayAsync();
        return Mapper.Map<IEnumerable<EquipLedgerReadDto>>(entities);
    }

    public async Task<IEnumerable<EquipLedgerReadDto>?> GetListByTypeAsync(string? protocolEnum)
    {
        // var protocol = Enum.Parse<EquipConnType>(type, true);
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(protocolEnum),
                m => m.EquipType!.ProtocolEnum == protocolEnum &&
                     (!m.EquipName.Contains("测试") || !m.EquipCode.Contains("测试")))
            .Includes(t => t.Room)
            .Includes(t => t.EquipType)
            .OrderByDescending(m => m.OrderNum)
            .ToListAsync();
        return Mapper.Map<IEnumerable<EquipLedgerReadDto>>(entities);
    }

    public async Task<IEnumerable<EquipResponsibleUserReadDto>> GetEquipResponsibleUsersAsync()
    {
        var result = await Queryable
            .Where(eq => eq.ResponsibleUserId != null)
            .Distinct()
            .Select(t => new EquipResponsibleUserReadDto
            {
                ResponsibleUserId = t.ResponsibleUserId,
                ResponsibleUserName = t.ResponsibleUserName,
            })
            .ToArrayAsync();
        return result;
    }

    public async Task<bool?> ImportAsync(IFormFile file)
    {
        var directoryPath = Path.Combine(Environment.CurrentDirectory, "attachs");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var fullPath = Path.Combine(directoryPath, $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}计量.xlsx");
        if (!File.Exists(fullPath))
        {
            using var reader = file.OpenReadStream();
            using var writer = new FileStream(fullPath, FileMode.Create);
            await reader.CopyToAsync(writer);
            writer.Close();
            reader.Close();
            await CheckExpiringMeasurementDevices(fullPath);
        }

        return true;
    }

    public async Task CheckExpiringMeasurementDevices(string filePath)
    {
        // 用于存储结果的列表
        var results = new List<string>();

        IWorkbook workbook;
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            // 根据文件扩展名选择不同的 Workbook 实现
            if (filePath.EndsWith(".xlsx"))
                workbook = new XSSFWorkbook(fileStream);
            else if (filePath.EndsWith(".xls"))
                workbook = new HSSFWorkbook(fileStream);
            else
                throw new NotSupportedException("文件格式不支持，仅支持 .xls 或 .xlsx 文件。");
        }

        // 获取第一个工作表
        ISheet sheet = workbook.GetSheetAt(0);

        // 获取标题行（假设第一行是标题）
        IRow headerRow = sheet.GetRow(0);
        if (headerRow == null)
            throw new InvalidDataException("Excel 文件没有标题行！");

        // 构建列名到列索引的映射
        Dictionary<string, int> columnIndices = new Dictionary<string, int>();
        for (int col = 0; col < headerRow.LastCellNum; col++)
        {
            string? header = headerRow.GetCell(col)?.ToString()?.Trim();
            if (!string.IsNullOrEmpty(header))
                columnIndices[header] = col;
        }

        // 检查必要的列是否存在
        string[] requiredColumns = { "是否计量设备仪器", "责任人", "本地化资产编号", "型号", "资产名称", "有效期" };
        foreach (var column in requiredColumns)
        {
            if (!columnIndices.ContainsKey(column))
                throw new InvalidDataException($"Excel 文件中缺少必要的列：{column}");
        }

        // 在检查必要列的代码部分之后，添加对"出厂日期"列的可选处理
        bool hasManufactureDateColumn = columnIndices.ContainsKey("出厂日期");

        // 在检查必要列的代码部分之后，添加对"Rfid系统导出位置"列的可选处理
        bool hasRoomLocation = columnIndices.ContainsKey("Rfid定位");
        // 获取当前日期
        DateTime currentDate = DateTime.Now.ToLocalTime();

        EquipLedger[] oldEquipMeasurements = await DbContext.Queryable<EquipLedger>().ToArrayAsync();
        try
        {
            await DbContext.Ado.BeginTranAsync();

            // 遍历每一行（从第 2 行开始）
            for (int row = 1; row <= sheet.LastRowNum; row++)
            {
                IRow dataRow = sheet.GetRow(row);
                if (dataRow == null)
                    continue; // 跳过空行

                // 检查是否是计量设备仪器
                bool? isMeasurementDevice =
                    dataRow.GetCell(columnIndices["是否计量设备仪器"])?.ToString()?.Trim() == "是" ? true : false;
                // 检查有效期
                string? expiryDateStr = dataRow.GetCell(columnIndices["有效期"])?.ToString()?.Trim();
                // 责任人
                string? responsiblePersonStr = dataRow.GetCell(columnIndices["责任人"])?.ToString()?.Trim();
                // 本地化资产编号
                string? localAssetNumberStr = dataRow.GetCell(columnIndices["本地化资产编号"])?.ToString()?.Trim();
                // 型号
                string? modelStr = dataRow.GetCell(columnIndices["型号"])?.ToString()?.Trim();
                // 资产名称
                string? assetNameStr = dataRow.GetCell(columnIndices["资产名称"])?.ToString()?.Trim();
                // 出厂日期
                string? manufactureDateStr = null;
                if (hasManufactureDateColumn)
                {
                    manufactureDateStr = dataRow.GetCell(columnIndices["出厂日期"])?.ToString()?.Trim();
                }

                // Rfid定位
                string? roomStr = null;
                if (hasRoomLocation)
                {
                    roomStr = dataRow.GetCell(columnIndices["Rfid定位"])?.ToString()?.Trim();
                }

                bool isExist = false;
                var entity = oldEquipMeasurements.FirstOrDefault(t => t.AssetNumber == localAssetNumberStr);
                if (entity == null)
                {
                    Guid? userId = (await DbContext.Queryable<User>().FirstAsync(x => x.Name == responsiblePersonStr))
                        ?.Id;
                    DateTime.TryParse(expiryDateStr, out var dt);
                    DateTime.TryParse(manufactureDateStr, out var PurchaseDate);
                    EquipLedger input = new EquipLedger()
                    {
                        EquipCode = await _codeRuleService.GenerateCodeByCodeAsync("SBTZ"),
                        ValidityDate = dt,
                        ResponsibleUserId = userId,
                        ResponsibleUserName = responsiblePersonStr,
                        AssetNumber = localAssetNumberStr,
                        Model = modelStr,
                        EquipName = assetNameStr ?? "未知设备",
                        IsMeasurementDevice = isMeasurementDevice,
                        DeviceStatus = DeviceStatus.Normal,
                        EquipLevel = EquipLevelEnum.Basic,
                        PurchaseDate = PurchaseDate
                    };
                    if (Guid.TryParse(roomStr, out var roomGuid))
                    {
                        input.RoomId = roomGuid;
                        input.RoomIdSourceType = 4;
                    }
                    await DbContext.Insertable(input).ExecuteCommandAsync();
                }
                else
                {
                    Guid? userId = (await DbContext.Queryable<User>().FirstAsync(x => x.Name == responsiblePersonStr))
                        ?.Id;
                    // 进入此判断说明有效期不一致,需要更新
                    entity.ResponsibleUserId = userId;
                    entity.ResponsibleUserId = userId;
                    entity.ResponsibleUserName = responsiblePersonStr;
                    entity.AssetNumber = localAssetNumberStr;
                    entity.Model = modelStr;
                    if (!string.IsNullOrEmpty(assetNameStr))
                    {
                        entity.EquipName = assetNameStr;
                    }

                    entity.IsMeasurementDevice = isMeasurementDevice;
                    if (Guid.TryParse(roomStr, out var roomGuid))
                    {
                        entity.RoomId = roomGuid;
                        entity.RoomIdSourceType = 4;
                    }

                    if (DateTime.TryParse(expiryDateStr, out var time))
                    {
                        entity.ValidityDate = time;
                    }

                    if (DateTime.TryParse(manufactureDateStr, out var manufactureDate))
                    {
                        entity.PurchaseDate = manufactureDate;
                    }

                    await DbContext.Updateable(entity).ExecuteCommandAsync();
                }

                // foreach (EquipLedger item in oldEquipMeasurements)
                // {
                //     if (item.AssetNumber == localAssetNumberStr && item.IsMeasurementDevice == isMeasurementDevice &&
                //         !string.IsNullOrEmpty(expiryDateStr) && item.ValidityDate.ToString() != expiryDateStr)
                //     {
                //         Guid? userId = (await DbContext.Queryable<User>().FirstAsync(x => x.Name == responsiblePersonStr))?.Id;
                //         // 进入此判断说明有效期不一致,需要更新
                //         DbContext.Updateable(item).SetColumns(it => new EquipLedger()
                //         {
                //             ValidityDate = DateTime.Parse(expiryDateStr),
                //             ResponsibleUserId = userId,
                //             ResponsibleUserName = responsiblePersonStr,
                //             AssetNumber = localAssetNumberStr,
                //             Model = modelStr,
                //             EquipName = assetNameStr,
                //             IsMeasurementDevice = isMeasurementDevice,
                //             PurchaseDate = manufactureDateStr != null ? DateTime.Parse(manufactureDateStr) : item.PurchaseDate
                //         }).ExecuteCommand();
                //         isExist = true;
                //     }
                // }
                // if (!isExist)
                // {
                //     Guid? userId = (await DbContext.Queryable<User>().FirstAsync(x => x.Name == responsiblePersonStr))?.Id;
                //     DateTime? dt = null;
                //     if (!string.IsNullOrEmpty(expiryDateStr))
                //         dt = DateTime.Parse(expiryDateStr);
                //     // 在创建新记录的代码块中，添加出厂日期的设置
                //     DateTime? manufactureDate = null;
                //     if (hasManufactureDateColumn && !string.IsNullOrEmpty(manufactureDateStr))
                //     {
                //         manufactureDate = DateTime.Parse(manufactureDateStr);
                //     }
                //     EquipLedgerCreateDto input = new EquipLedgerCreateDto()
                //     {
                //         EquipCode = await _codeRuleService.GenerateCodeByCodeAsync("SBTZ"),
                //         ValidityDate = dt,
                //         ResponsibleUserId = userId,
                //         ResponsibleUserName = responsiblePersonStr,
                //         AssetNumber = localAssetNumberStr,
                //         Model = modelStr,
                //         EquipName = assetNameStr,
                //         IsMeasurementDevice = isMeasurementDevice,
                //         DeviceStatus = DeviceStatus.Normal.ToString(),
                //         DeviceLevel = EquipLevelEnum.Basic.ToString(),
                //         PurchaseDate = manufactureDate,
                //     };
                //     try
                //     {
                //         await CreateAsync(input);
                //     }
                //     catch (Exception e) { }
                // }
            }

            await DbContext.Ado.CommitTranAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await DbContext.Ado.RollbackTranAsync();
            throw;
        }
    }

    /// <summary>
    /// 根据设备ID获取设备名称
    /// </summary>
    /// <param name="equipId"></param>
    /// <returns></returns>
    public async Task<string> GetEquipName(Guid equipId)
    {
        string equipName = null!;
        try
        {
            equipName = await Queryable.Where(x => x.Id == equipId).Select(x => x.EquipName).FirstAsync();
        }
        catch (Exception)
        {
        }

        return equipName;
    }

    /// <summary>
    /// 返回所有设备的导出数据
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<EquipLedgerSearchReadDto>> GetEquipExportRfid()
    {
        var entities = await DbContext.Queryable<EquipLedger>()
            .Select(e => new EquipLedgerSearchReadDto
            {
                Id = e.Id,
                EquipCode = e.EquipCode,
                EquipName = e.EquipName,
                RoomId = e.RoomId,
                AssetNumber = e.AssetNumber,
                ResponsibleUserId = e.ResponsibleUserId
            }).ToListAsync();
        return entities;
    }

    /// <summary>
    /// 根据资产编号获取设备
    /// </summary>
    /// <param name="assetNumber"></param>
    /// <returns></returns>
    public async Task<bool?> SetEquipExistByAssetNumber(string? assetNumber)
    {
        var abnormalEntities = await Queryable
            .Where(x => x.AssetNumber == assetNumber && x.DeviceStatus != DeviceStatus.Normal)
            .ToListAsync();
        if (!abnormalEntities.Any())
            return false;
        abnormalEntities.ForEach(item => item.DeviceStatus = DeviceStatus.Normal);
        await DbContext.Updateable(abnormalEntities)
            .UpdateColumns(x => new { x.DeviceStatus })
            .ExecuteCommandAsync();
        return true;
    }

    public async Task<IEnumerable<EquipLedgerReadDto>> GetListByAssetNumberAsync(string? assetNumber)
    {
        var entitys = await Queryable.Where(x => x.AssetNumber == assetNumber).ToArrayAsync();
        return Mapper.Map<IEnumerable<EquipLedgerReadDto>>(entitys);
    }

    /// <summary>
    /// 导出温湿度数据到Excel
    /// </summary>
    public async Task<byte[]> ExportTemperatureHumidityAsync(TemperatureHumidityExportRequestDto request)
    {
        // 构建查询条件
        var query = DbContext.Queryable<TemperatureHumidityRecord>()
            .LeftJoin<EquipLedger>((t, e) => t.EquipId == e.Id)
            .WhereIF(request.EquipCodes.Any(), (t, e) => request.EquipCodes.Contains(e.EquipCode));

        // 日期筛选
        if (!string.IsNullOrEmpty(request.StartDate) && DateTime.TryParse(request.StartDate, out var startDate))
        {
            query = query.Where((t, e) => t.RecordTime >= startDate);
        }

        if (!string.IsNullOrEmpty(request.EndDate) && DateTime.TryParse(request.EndDate, out var endDate))
        {
            var endDateTime = endDate.AddDays(1).AddSeconds(-1); // 包含结束日期的整天
            query = query.Where((t, e) => t.RecordTime <= endDateTime);
        }

        // 查询数据并排序
        var data = await query
            .OrderBy((t, e) => t.RecordTime)
            .Select((t, e) => new TemperatureHumidityExportDataDto
            {
                EquipName = e.EquipName,
                EquipCode = e.EquipCode,
                RoomName = t.RoomName,
                Temperature = t.Temperature,
                Humidness = t.Humidness,
                IpAddress = t.IpAddress,
                CreateTime = t.RecordTime.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToListAsync();

        // 添加序号
        for (int i = 0; i < data.Count; i++)
        {
            data[i].SequenceNumber = i + 1;
        }

        // 生成Excel文件
        return GenerateTemperatureHumidityExcel(data);
    }

    /// <summary>
    /// 生成温湿度Excel文件
    /// </summary>
    private byte[] GenerateTemperatureHumidityExcel(List<TemperatureHumidityExportDataDto> data)
    {
        var workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet("温湿度数据");

        // 创建标题行
        var headerRow = sheet.CreateRow(0);
        var headers = new[] { "序号", "设备名称", "设备编码", "房间名称", "温度(°C)", "湿度(%)", "IP地址", "记录时间" };
        
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = headerRow.CreateCell(i);
            cell.SetCellValue(headers[i]);
            
            // 设置标题样式
            var style = workbook.CreateCellStyle();
            var font = workbook.CreateFont();
            font.IsBold = true;
            style.SetFont(font);
            cell.CellStyle = style;
        }

        // 填充数据行
        for (int i = 0; i < data.Count; i++)
        {
            var row = sheet.CreateRow(i + 1);
            var item = data[i];

            row.CreateCell(0).SetCellValue(item.SequenceNumber);
            row.CreateCell(1).SetCellValue(item.EquipName ?? "");
            row.CreateCell(2).SetCellValue(item.EquipCode ?? "");
            row.CreateCell(3).SetCellValue(item.RoomName ?? "");
            row.CreateCell(4).SetCellValue(item.Temperature?.ToString() ?? "");
            row.CreateCell(5).SetCellValue(item.Humidness?.ToString() ?? "");
            row.CreateCell(6).SetCellValue(item.IpAddress ?? "");
            row.CreateCell(7).SetCellValue(item.CreateTime ?? "");
        }

        // 自动调整列宽
        for (int i = 0; i < headers.Length; i++)
        {
            sheet.AutoSizeColumn(i);
        }

        // 转换为字节数组
        using var stream = new MemoryStream();
        workbook.Write(stream);
        return stream.ToArray();
    }

    /// <summary>
    /// 导出关键设备工作时长数据
    /// </summary>
    /// <param name="queryDto"></param>
    /// <returns></returns>
    public async Task<IEnumerable<KeyEquipWorkingHoursExportDto>> ExportKeyEquipWorkingHoursAsync(EquipLedgerQueryDto queryDto)
    {
        // 构建查询条件，添加关键设备筛选
        var query = Queryable
            .LeftJoin<Room>((el, r) => el.RoomId == r.Id)
            .LeftJoin<User>((el, r, u) => el.ResponsibleUserId == u.Id)
            .WhereIF(!string.IsNullOrEmpty(queryDto.AssetNumber), (el, r, u) => el.AssetNumber!.Contains(queryDto.AssetNumber!))
            .WhereIF(!string.IsNullOrEmpty(queryDto.EquipName), (el, r, u) => el.EquipName.Contains(queryDto.EquipName!))
            .WhereIF(!string.IsNullOrEmpty(queryDto.EquipCode), (el, r, u) => el.EquipCode.Contains(queryDto.EquipCode!))
            .WhereIF(queryDto.TypeId.HasValue, (el, r, u) => el.TypeId == queryDto.TypeId)
            .WhereIF(queryDto.RoomId.HasValue, (el, r, u) => el.RoomId == queryDto.RoomId)
            .WhereIF(queryDto.ResponsibleUserId.HasValue, (el, r, u) => el.ResponsibleUserId == queryDto.ResponsibleUserId)
            .WhereIF(queryDto.State.HasValue, (el, r, u) => el.State == queryDto.State)
            .WhereIF(!string.IsNullOrEmpty(queryDto.Query), (el, r, u) => 
                el.EquipName.Contains(queryDto.Query!) || el.Model!.Contains(queryDto.Query!))
            .Where((el, r, u) => !el.SoftDeleted)
            .Where((el, r, u) => el.EquipLevel == EquipLevelEnum.Important); // 只筛选关键设备

        // 查询设备基础数据，包含设备ID用于计算工作时长
        var equipDataQuery = await query
            .Select((el, r, u) => new
            {
                Id = el.Id,
                AssetNumber = el.AssetNumber,
                EquipName = el.EquipName,
                Model = el.Model,
                RoomName = r.Name,
                RoomId = el.RoomId,
                IsMeasurementDevice = el.IsMeasurementDevice,
                ResponsibleUserName = u.Name
            })
            .ToListAsync();

        var equipData = new List<KeyEquipWorkingHoursExportDto>();

        // 计算每个设备的工作时长
        foreach (var item in equipDataQuery)
        {
            var workingHours = await CalculateEquipWorkingHours(item.Id, item.RoomId);
            
            equipData.Add(new KeyEquipWorkingHoursExportDto
            {
                AssetNumber = item.AssetNumber,
                EquipName = item.EquipName,
                Model = item.Model,
                RoomName = item.RoomName,
                IsMeasurementDevice = item.IsMeasurementDevice,
                ResponsibleUserName = item.ResponsibleUserName,
                WorkingHours = workingHours
            });
        }

        return equipData;
    }

    /// <summary>
    /// 计算设备工作时长（优先从数据库获取，Redis作为备用）
    /// </summary>
    /// <param name="equipId">设备ID</param>
    /// <param name="roomId">房间ID</param>
    /// <returns>工作时长（小时）</returns>
    private async Task<decimal> CalculateEquipWorkingHours(Guid equipId, Guid? roomId)
    {
        try
        {
            if (!roomId.HasValue)
            {
                return 0; // 没有房间信息则返回0
            }

            // 根据房间ID获取系统编号
            var systemNumber = GetSystemNumberByRoomId(roomId.Value);
            if (systemNumber == 0)
            {
                return 0; // 无法确定系统编号则返回0
            }

            // 方案1：优先从数据库获取历史累计运行时长（更可靠）
            var totalHoursFromDb = await GetTotalRuntimeFromDatabase(equipId, systemNumber);
            if (totalHoursFromDb > 0)
            {
                return totalHoursFromDb;
            }

            // 方案2：从Redis获取近30天运行时长（备用方案）
            var runTimeSeconds = await _systemInfoManager.GetRunTime((byte)systemNumber, equipId);
            var runTimeHours = Math.Round((decimal)runTimeSeconds / 3600, 2);
            
            return runTimeHours;
        }
        catch (Exception ex)
        {
            // 记录异常日志
            Console.WriteLine($"计算设备工作时长失败: {ex.Message}");
            return 0; // 出现异常时返回0
        }
    }

    /// <summary>
    /// 从数据库获取设备的总运行时长
    /// </summary>
    /// <param name="equipId">设备ID</param>
    /// <param name="systemNumber">系统编号</param>
    /// <returns>总运行时长（小时）</returns>
    private async Task<decimal> GetTotalRuntimeFromDatabase(Guid equipId, byte systemNumber)
    {
        try
        {
            // 计算最近30天的运行时长总和
            var thirtyDaysAgo = DateTime.Now.AddDays(-30).Date;
            
            var totalSeconds = await DbContext.Queryable<EquipDailyRuntime>()
                .Where(x => x.EquipId == equipId && 
                           x.SystemNumber == systemNumber &&
                           x.RecordDate >= thirtyDaysAgo)
                .SumAsync(x => x.RunningSeconds);

            return Math.Round((decimal)totalSeconds / 3600, 2);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"从数据库获取设备运行时长失败: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 根据房间ID获取系统编号
    /// </summary>
    /// <param name="roomId">房间ID</param>
    /// <returns>系统编号，如果找不到则返回0</returns>
    private static byte GetSystemNumberByRoomId(Guid roomId)
    {
        var roomIdString = roomId.ToString().ToUpper();
        
        // 使用TestEquipData中的映射关系反向查找
        for (int systemId = 1; systemId <= 10; systemId++)
        {
            var mappedRoomId = TestEquipData.GetRoomId(systemId);
            if (string.Equals(mappedRoomId, roomIdString, StringComparison.OrdinalIgnoreCase))
            {
                return (byte)systemId;
            }
        }
        
        return 0; // 未找到对应的系统编号
    }
}
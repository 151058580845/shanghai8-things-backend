﻿using Hgzn.Mes.Application.Main.Dtos.Base;
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
using Hgzn.Mes.Domain.Entities.System.Location;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Application.Main.Services.App;
using StackExchange.Redis;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

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
            .Where(t => !string.IsNullOrEmpty(t.EquipName) && !string.IsNullOrEmpty(t.AssetNumber)) // 过滤没有设备名称或资产编号的记录
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
        var entities = await Queryable
            .Where(t => equipIds.Contains(t.Id))
            .Where(t => !string.IsNullOrEmpty(t.EquipName) && !string.IsNullOrEmpty(t.AssetNumber)) // 过滤没有设备名称或资产编号的记录
            .ToListAsync();
        return Mapper.Map<IEnumerable<EquipLedgerReadDto>>(entities);
    }

    /// <summary>
    /// 获取待搜索记录
    /// </summary>
    /// <returns></returns>
    public async Task<PaginatedList<EquipLedgerSearchReadDto>> GetAppSearchAsync(int pageIndex, int pageSize)
    {
        var entities = await Queryable
            .Where(t => t.DeviceStatus == DeviceStatus.Lost)
            .Where(t => !string.IsNullOrEmpty(t.EquipName) && !string.IsNullOrEmpty(t.AssetNumber)) // 过滤没有设备名称或资产编号的记录
            .Includes(t => t.EquipType)
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
        var equipList = await Queryable
            .Where(t => t.RoomId != null && rooms.Contains(t.RoomId!.Value))
            .Where(t => !string.IsNullOrEmpty(t.EquipName) && !string.IsNullOrEmpty(t.AssetNumber)) // 过滤没有设备名称或资产编号的记录
            .ToListAsync();
        return Mapper.Map<IEnumerable<EquipLedger>>(equipList);
    }

    public override async Task<PaginatedList<EquipLedgerReadDto>> GetPaginatedListAsync(EquipLedgerQueryDto query)
    {
        var queryable = Queryable
            .Where(m => !string.IsNullOrEmpty(m.EquipName) && !string.IsNullOrEmpty(m.AssetNumber)) // 过滤没有设备名称或资产编号的记录
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
            ? Queryable.Where(m => !string.IsNullOrEmpty(m.EquipName) && !string.IsNullOrEmpty(m.AssetNumber)) // 过滤没有设备名称或资产编号的记录
            : Queryable
                .Where(m => !string.IsNullOrEmpty(m.EquipName) && !string.IsNullOrEmpty(m.AssetNumber)) // 过滤没有设备名称或资产编号的记录
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
            LoggerAdapter.LogInformation($"AG - 设备导入 - 开始执行API批量导入, URL: {url}");

            // 发送 GET 请求
            LoggerAdapter.LogInformation($"AG - 设备导入 - 正在发送HTTP GET请求...");
            var response = await _httpClient.GetAsync(url);

            // 确保请求成功
            LoggerAdapter.LogInformation($"AG - 设备导入 - HTTP响应状态码: {response.StatusCode}");
            response.EnsureSuccessStatusCode();

            // 读取响应内容
            var jsonResponse = await response.Content.ReadAsStringAsync();
            LoggerAdapter.LogInformation($"AG - 设备导入 - 接收到JSON响应, 长度: {jsonResponse?.Length ?? 0} 字符");
            LoggerAdapter.LogInformation($"AG - 设备导入 - JSON内容前500字符: {(jsonResponse?.Length > 500 ? jsonResponse.Substring(0, 500) : jsonResponse)}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // 忽略大小写
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // 忽略 null 值
            };

            LoggerAdapter.LogInformation($"AG - 设备导入 - 开始反序列化JSON数据...");
            List<EquipLedgerImportDto>? result = JsonSerializer.Deserialize<List<EquipLedgerImportDto>>(jsonResponse, options);
            LoggerAdapter.LogInformation($"AG - 设备导入 - 反序列化完成, 共解析到 {result?.Count ?? 0} 条数据");

            var changeCount = 0;
            if (result != null && result.Any())
            {
                LoggerAdapter.LogInformation($"AG - 设备导入 - 开始处理 {result.Count} 条设备数据...");

                var processedCount = 0;
                foreach (var item in result)
                {
                    processedCount++;
                    LoggerAdapter.LogInformation($"AG - 设备导入 - 处理第 {processedCount}/{result.Count} 条: 本地化资产编号={item.Localsn}, 设备名称={item.Assetname}");

                    // 映射新API格式到内部DTO
                    var equipLedgerDto = MapImportDtoToCreateDto(item);

                    // 补全与清洗：责任人、编码、名称等
                    try
                    {
                        // 去除首尾空格
                        equipLedgerDto.EquipName = equipLedgerDto.EquipName?.Trim() ?? string.Empty;
                        equipLedgerDto.AssetNumber = equipLedgerDto.AssetNumber?.Trim();
                        equipLedgerDto.Model = equipLedgerDto.Model?.Trim();
                        equipLedgerDto.Sn = equipLedgerDto.Sn?.Trim();
                        equipLedgerDto.ResponsibleUserName = equipLedgerDto.ResponsibleUserName?.Trim();

                        // 责任人：按姓名查询用户ID
                        if (!string.IsNullOrWhiteSpace(equipLedgerDto.ResponsibleUserName))
                        {
                            var user = await DbContext.Queryable<User>()
                                .FirstAsync(x => x.Name == equipLedgerDto.ResponsibleUserName);
                            if (user != null)
                            {
                                equipLedgerDto.ResponsibleUserId = user.Id;
                            }
                        }

                        // 状态与级别兜底
                        equipLedgerDto.State = equipLedgerDto.State;
                        equipLedgerDto.DeviceStatus ??= DeviceStatus.Normal.ToString();
                        equipLedgerDto.DeviceLevel ??= EquipLevelEnum.Basic.ToString();
                    }
                    catch (Exception ex)
                    {
                        LoggerAdapter.LogWarning($"AG - 设备导入 - 清洗/补全导入数据异常: {ex.Message}");
                    }

                    LoggerAdapter.LogInformation($"AG - 设备导入 - DTO映射完成: 设备编号={equipLedgerDto.EquipCode}, 资产编号={equipLedgerDto.AssetNumber}");

                    // 检查是否已存在相同的本地化资产编号
                    LoggerAdapter.LogInformation($"AG - 设备导入 - 查询数据库是否存在资产编号: {item.Localsn}");

                    var existingEquips = await DbContext.Queryable<EquipLedger>().ToListAsync();

                    var existingEquip = await DbContext.Queryable<EquipLedger>()
                        .Where(x => x.AssetNumber == item.Localsn && !x.SoftDeleted)
                        .FirstAsync();

                    if (existingEquip != null)
                    {
                        LoggerAdapter.LogInformation($"AG - 设备导入 - 找到已存在设备, ID={existingEquip.Id}, 执行选择性更新");

                        // 仅当新数据提供值时才覆盖；否则保留旧值
                        static bool HasText(string? s) => !string.IsNullOrWhiteSpace(s);

                        if (HasText(equipLedgerDto.EquipName)) existingEquip.EquipName = equipLedgerDto.EquipName!;
                        if (HasText(equipLedgerDto.AssetNumber)) existingEquip.AssetNumber = equipLedgerDto.AssetNumber!;
                        if (HasText(equipLedgerDto.Model)) existingEquip.Model = equipLedgerDto.Model!;
                        if (HasText(equipLedgerDto.Sn)) existingEquip.Sn = equipLedgerDto.Sn!;
                        if (HasText(equipLedgerDto.IpAddress)) existingEquip.IpAddress = equipLedgerDto.IpAddress!;

                        if (equipLedgerDto.PurchaseDate.HasValue) existingEquip.PurchaseDate = equipLedgerDto.PurchaseDate;
                        if (equipLedgerDto.ValidityDate.HasValue) existingEquip.ValidityDate = equipLedgerDto.ValidityDate;
                        if (equipLedgerDto.IsMeasurementDevice.HasValue) existingEquip.IsMeasurementDevice = equipLedgerDto.IsMeasurementDevice;

                        if (HasText(equipLedgerDto.DeviceStatus))
                        {
                            // 允许字符串到枚举的安全转换；失败则忽略
                            if (Enum.TryParse<DeviceStatus>(equipLedgerDto.DeviceStatus, true, out var ds))
                                existingEquip.DeviceStatus = ds;
                        }
                        if (HasText(equipLedgerDto.DeviceLevel))
                        {
                            if (Enum.TryParse<EquipLevelEnum>(equipLedgerDto.DeviceLevel, true, out var dl))
                                existingEquip.EquipLevel = dl;
                        }

                        if (equipLedgerDto.ResponsibleUserId.HasValue) existingEquip.ResponsibleUserId = equipLedgerDto.ResponsibleUserId;
                        if (HasText(equipLedgerDto.ResponsibleUserName)) existingEquip.ResponsibleUserName = equipLedgerDto.ResponsibleUserName!;

                        existingEquip.State = equipLedgerDto.State; // State 为 bool，维持新值（若需要保留旧值请告知）
                        existingEquip.LastModificationTime = DateTime.Now;

                        // 如果已存在设备没有设备编码,那么就分配一个
                        if (!HasText(existingEquip.EquipCode))
                            existingEquip.EquipCode = await _codeRuleService.GenerateCodeByCodeAsync("SBTZ");

                        var updateResult = DbContext.Updateable(existingEquip)
                            .IgnoreColumns(x => new { x.CreationTime, x.CreatorId, x.SoftDeleted, x.DeleteTime })
                            .ExecuteCommand();

                        LoggerAdapter.LogInformation($"AG - 设备导入 - 选择性更新完成, 影响行数: {updateResult}");
                        changeCount += updateResult;
                    }
                    else
                    {
                        LoggerAdapter.LogInformation($"AG - 设备导入 - 未找到已存在设备, 执行新增操作");

                        // 新增设备
                        EquipLedger info = Mapper.Map<EquipLedger>(equipLedgerDto);
                        // 新增设备自动分配一个设备编码
                        info.EquipCode = await _codeRuleService.GenerateCodeByCodeAsync("SBTZ");
                        var insertResult = DbContext.Insertable<EquipLedger>(info).ExecuteCommand();

                        LoggerAdapter.LogInformation($"AG - 设备导入 - 新增操作完成, 影响行数: {insertResult}");
                        changeCount += insertResult;
                    }
                }

                LoggerAdapter.LogInformation($"AG - 设备导入 - 所有数据处理完成");
            }
            else
            {
                LoggerAdapter.LogInformation($"AG - 设备导入 - 未解析到任何数据或数据为空");
            }

            LoggerAdapter.LogInformation($"AG - 设备导入 - 导入完成, 总共影响 {changeCount} 条记录");
            return changeCount;
        }
        catch (Exception ex)
        {
            LoggerAdapter.LogWarning($"AG - 设备导入 - 异常: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 将导入DTO映射为创建DTO
    /// </summary>
    /// <param name="importDto"></param>
    /// <returns></returns>
    private EquipLedgerCreateDto MapImportDtoToCreateDto(EquipLedgerImportDto importDto)
    {
        return new EquipLedgerCreateDto
        {
            EquipName = importDto.Assetname,
            ResponsibleUserName = importDto.Dutyman,
            IsMeasurementDevice = ParseIsCalibrate(importDto.Iscalibrate),
            AssetNumber = importDto.Localsn,
            Model = importDto.Model,
            PurchaseDate = ParseDate(importDto.Factorydate),
            ValidityDate = ParseDate(importDto.ValidPeriod),
            Sn = importDto.Sn,
            State = true,
            DeviceStatus = "Normal", // 默认设备状态为正常
            DeviceLevel = "Basic"    // 默认设备重要度为普通设备
        };
    }

    /// <summary>
    /// 解析是否校准字段
    /// </summary>
    /// <param name="isCalibrate"></param>
    /// <returns></returns>
    private bool? ParseIsCalibrate(string? isCalibrate)
    {
        if (string.IsNullOrEmpty(isCalibrate))
            return null;

        return isCalibrate.Equals("是", StringComparison.OrdinalIgnoreCase) ||
               isCalibrate.Equals("true", StringComparison.OrdinalIgnoreCase) ||
               isCalibrate.Equals("1", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 解析日期字段
    /// </summary>
    /// <param name="dateString"></param>
    /// <returns></returns>
    private DateTime? ParseDate(string? dateString)
    {
        if (string.IsNullOrEmpty(dateString))
            return null;

        if (DateTime.TryParse(dateString, out var result))
            return result;

        return null;
    }

    public async Task<IEnumerable<EquipLedgerReadDto>> GetMissingDevicesAlarmAsync()
    {
        var entities = await DbContext.Queryable<EquipLedger>()
            .Includes(e => e.Room)
            .Where(e => e.IsMovable &&
                        (e.RoomId == null || (int)e.Room!.Purpose >= (int)RoomPurpose.Hallway) &&
                        DateTime.Now.ToLocalTime().AddMinutes(-30) >= e.LastMoveTime)
            .Where(e => !string.IsNullOrEmpty(e.EquipName) && !string.IsNullOrEmpty(e.AssetNumber)) // 过滤没有设备名称或资产编号的记录
            .ToArrayAsync();
        return Mapper.Map<IEnumerable<EquipLedgerReadDto>>(entities);
    }

    public async Task<IEnumerable<EquipLedgerReadDto>?> GetListByTypeAsync(string? protocolEnum)
    {
        // var protocol = Enum.Parse<EquipConnType>(type, true);
        var entities = await Queryable
            .Where(m => !string.IsNullOrEmpty(m.EquipName) && !string.IsNullOrEmpty(m.AssetNumber)) // 过滤没有设备名称或资产编号的记录
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

                // 如果设备名称或资产编号为空，跳过此条记录
                if (string.IsNullOrEmpty(assetNameStr) || string.IsNullOrEmpty(localAssetNumberStr))
                {
                    LoggerAdapter.LogInformation($"AG - 跳过导入记录: 设备名称={assetNameStr ?? "空"}, 资产编号={localAssetNumberStr ?? "空"}");
                    continue; // 跳过当前行
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
            .Where(e => !string.IsNullOrEmpty(e.EquipName) && !string.IsNullOrEmpty(e.AssetNumber)) // 过滤没有设备名称或资产编号的记录
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
        var entitys = await Queryable
            .Where(x => x.AssetNumber == assetNumber)
            .Where(x => !string.IsNullOrEmpty(x.EquipName) && !string.IsNullOrEmpty(x.AssetNumber)) // 过滤没有设备名称或资产编号的记录
            .ToArrayAsync();
        return Mapper.Map<IEnumerable<EquipLedgerReadDto>>(entitys);
    }

    /// <summary>
    /// 导出温湿度记录表到Word文档
    /// </summary>
    /// <param name="request">导出请求参数</param>
    /// <returns>Word文档字节数组</returns>
    public async Task<byte[]> ExportTemperatureHumidityAsync(TemperatureHumidityExportRequestDto request)
    {
        // 获取所有设备的房间信息
        var equipRooms = await DbContext.Queryable<EquipLedger>()
            .Where(e => request.EquipCodes.Contains(e.EquipCode))
            .Select(e => new { e.EquipCode, e.RoomId })
            .ToListAsync();

        if (!equipRooms.Any())
        {
            throw new ArgumentException("未找到指定的设备");
        }

        // 按房间分组
        var roomGroups = equipRooms
            .Where(e => e.RoomId.HasValue)
            .GroupBy(e => e.RoomId.Value)
            .ToList();

        if (!roomGroups.Any())
        {
            throw new ArgumentException("所有设备都没有关联的房间信息");
        }

        // 解析日期
        var startDate = !string.IsNullOrEmpty(request.StartDate) && DateTime.TryParse(request.StartDate, out var start)
            ? start
            : DateTime.Now.AddDays(-30);
        var endDate = !string.IsNullOrEmpty(request.EndDate) && DateTime.TryParse(request.EndDate, out var end)
            ? end
            : DateTime.Now;

        // 如果只有一个房间，直接生成Word文档
        if (roomGroups.Count == 1)
        {
            var roomId = roomGroups.First().Key;
            var newRequest = new TemperatureHumidityRecordExportRequestDto
            {
                RoomId = roomId,
                StartDate = startDate,
                EndDate = endDate
            };
            return await ExportTemperatureHumidityRecordToWordAsync(newRequest);
        }

        // 多个房间时，为每个房间生成一个Word文档，然后合并
        var wordDocuments = new List<byte[]>();

        foreach (var roomGroup in roomGroups)
        {
            var roomId = roomGroup.Key;
            var newRequest = new TemperatureHumidityRecordExportRequestDto
            {
                RoomId = roomId,
                StartDate = startDate,
                EndDate = endDate
            };

            var roomWordData = await ExportTemperatureHumidityRecordToWordAsync(newRequest);
            wordDocuments.Add(roomWordData);
        }

        // 合并多个Word文档（简单实现：返回第一个房间的文档，后续可以优化为真正的合并）
        return wordDocuments.First();
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
            .Where((el, r, u) => !string.IsNullOrEmpty(el.EquipName) && !string.IsNullOrEmpty(el.AssetNumber)) // 过滤没有设备名称或资产编号的记录
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

    /// <summary>
    /// 导出温湿度记录表到Word文档
    /// </summary>
    /// <param name="request">导出请求参数</param>
    /// <returns>Word文档字节数组</returns>
    public async Task<byte[]> ExportTemperatureHumidityRecordToWordAsync(TemperatureHumidityRecordExportRequestDto request)
    {
        // 获取房间信息和系统名称
        var room = await DbContext.Queryable<Room>().FirstAsync(r => r.Id == request.RoomId);
        var systemName = GetSystemNameByRoomId(request.RoomId);
        var roomNumber = GetRoomNumberByRoomId(request.RoomId);

        // 获取指定日期范围内的温湿度数据
        var recordData = await GetTemperatureHumidityRecordDataAsync(request.RoomId, request.StartDate, request.EndDate);

        // 生成Word文档
        return GenerateTemperatureHumidityRecordWord(systemName, roomNumber, recordData);
    }

    /// <summary>
    /// 根据房间ID获取系统名称
    /// </summary>
    /// <param name="roomId">房间ID</param>
    /// <returns>系统名称</returns>
    private string GetSystemNameByRoomId(Guid roomId)
    {
        var roomIdString = roomId.ToString().ToUpper();

        for (int systemId = 1; systemId <= 10; systemId++)
        {
            var mappedRoomId = TestEquipData.GetRoomId(systemId);
            if (string.Equals(mappedRoomId, roomIdString, StringComparison.OrdinalIgnoreCase))
            {
                return TestEquipData.GetSystemName(systemId);
            }
        }

        return "未知系统";
    }

    /// <summary>
    /// 根据房间ID获取房间号
    /// </summary>
    /// <param name="roomId">房间ID</param>
    /// <returns>房间号</returns>
    private string GetRoomNumberByRoomId(Guid roomId)
    {
        var roomIdString = roomId.ToString().ToUpper();

        for (int systemId = 1; systemId <= 10; systemId++)
        {
            var mappedRoomId = TestEquipData.GetRoomId(systemId);
            if (string.Equals(mappedRoomId, roomIdString, StringComparison.OrdinalIgnoreCase))
            {
                return TestEquipData.GetRoom(systemId).ToString();
            }
        }

        return "000";
    }

    /// <summary>
    /// 获取指定日期范围内的温湿度记录数据
    /// </summary>
    /// <param name="roomId">房间ID</param>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <returns>温湿度记录数据列表</returns>
    private async Task<List<TemperatureHumidityRecordDataDto>> GetTemperatureHumidityRecordDataAsync(Guid roomId, DateTime startDate, DateTime endDate)
    {
        var result = new List<TemperatureHumidityRecordDataDto>();

        // 遍历每一天
        for (var currentDate = startDate.Date; currentDate <= endDate.Date; currentDate = currentDate.AddDays(1))
        {
            // 获取当天9点或9点后的第一个温湿度记录
            var record = await DbContext.Queryable<TemperatureHumidityRecord>()
                .Where(t => t.RoomId == roomId &&
                           t.RecordTime.Date == currentDate &&
                           t.RecordTime.Hour >= 9)
                .OrderBy(t => t.RecordTime)
                .FirstAsync();

            if (record != null)
            {
                result.Add(new TemperatureHumidityRecordDataDto
                {
                    MeasurementDate = currentDate.ToString("yyyy-MM-dd"),
                    TemperatureValue = record.Temperature?.ToString("F1") ?? "--",
                    HumidityValue = record.Humidness?.ToString("F1") ?? "--",
                    MeasurementTime = record.RecordTime.ToString("HH:mm:ss"),
                    Measurer = "自动测量"
                });
            }
            else
            {
                // 如果没有找到9点后的数据，尝试获取当天任意时间的数据
                var anyRecord = await DbContext.Queryable<TemperatureHumidityRecord>()
                    .Where(t => t.RoomId == roomId && t.RecordTime.Date == currentDate)
                    .OrderBy(t => t.RecordTime)
                    .FirstAsync();

                if (anyRecord != null)
                {
                    result.Add(new TemperatureHumidityRecordDataDto
                    {
                        MeasurementDate = currentDate.ToString("yyyy-MM-dd"),
                        TemperatureValue = anyRecord.Temperature?.ToString("F1") ?? "--",
                        HumidityValue = anyRecord.Humidness?.ToString("F1") ?? "--",
                        MeasurementTime = anyRecord.RecordTime.ToString("HH:mm:ss"),
                        Measurer = "自动测量"
                    });
                }
                else
                {
                    // 如果当天没有任何数据，添加空记录
                    result.Add(new TemperatureHumidityRecordDataDto
                    {
                        MeasurementDate = currentDate.ToString("yyyy-MM-dd"),
                        TemperatureValue = "--",
                        HumidityValue = "--",
                        MeasurementTime = "--",
                        Measurer = "自动测量"
                    });
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 生成温湿度记录表Word文档
    /// </summary>
    /// <param name="systemName">系统名称</param>
    /// <param name="roomNumber">房间号</param>
    /// <param name="recordData">记录数据</param>
    /// <returns>Word文档字节数组</returns>
    private byte[] GenerateTemperatureHumidityRecordWord(string systemName, string roomNumber, List<TemperatureHumidityRecordDataDto> recordData)
    {
        using var stream = new MemoryStream();
        using var document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document);

        // 添加主文档部分
        var mainPart = document.AddMainDocumentPart();
        mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
        var body = mainPart.Document.AppendChild(new Body());

        // 设置页面大小和边距
        var sectionProperties = body.AppendChild(new SectionProperties());
        var pageSize = sectionProperties.AppendChild(new PageSize()
        {
            Width = 11906,  // A4宽度 (21cm = 595pt = 11906 twips)
            Height = 16838, // A4高度 (29.7cm = 842pt = 16838 twips)
            Orient = PageOrientationValues.Portrait
        });

        var pageMargin = sectionProperties.AppendChild(new PageMargin()
        {
            Top = 1440,     // 上边距 2.54cm (1 inch = 72pt = 1440 twips)
            Right = 1440,   // 右边距 2.54cm
            Bottom = 1440,  // 下边距 2.54cm
            Left = 1440,    // 左边距 2.54cm
            Header = 708,   // 页眉边距 1.25cm
            Footer = 708    // 页脚边距 1.25cm
        });

        // 1. 创建居中标题
        var titleParagraph = body.AppendChild(new Paragraph());
        var titleRun = titleParagraph.AppendChild(new Run(new Text($"{systemName}温湿度、相对湿度记录表")));

        // 设置标题字体属性：四号字体和粗体
        var titleRunProperties = titleRun.AppendChild(new RunProperties());
        titleRunProperties.AppendChild(new Bold());
        titleRunProperties.AppendChild(new FontSize() { Val = "28" }); // 四号字体 (14磅)

        // 设置标题居中
        var titleParagraphProperties = titleParagraph.AppendChild(new ParagraphProperties());
        titleParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Center });

        // 添加空行
        body.AppendChild(new Paragraph());

        // 2. 创建表头上方信息区域（使用空格对齐）
        var infoParagraph1 = body.AppendChild(new Paragraph());
        infoParagraph1.AppendChild(new Run(new Text($"部门(房间号)：2#{roomNumber}")));

        // 设置1.5倍行距
        var infoParagraph1Properties = infoParagraph1.AppendChild(new ParagraphProperties());
        infoParagraph1Properties.AppendChild(new SpacingBetweenLines() { Line = "360", LineRule = LineSpacingRuleValues.Auto }); // 1.5倍行距

        var infoParagraph2 = body.AppendChild(new Paragraph());
        infoParagraph2.AppendChild(new Run(new Text($"文明生产区类别：二类                                          静电控制类别：管控点")));

        // 设置1.5倍行距
        var infoParagraph2Properties = infoParagraph2.AppendChild(new ParagraphProperties());
        infoParagraph2Properties.AppendChild(new SpacingBetweenLines() { Line = "360", LineRule = LineSpacingRuleValues.Auto }); // 1.5倍行距

        var infoParagraph3 = body.AppendChild(new Paragraph());
        infoParagraph3.AppendChild(new Run(new Text($"本区域温度要求：15~30℃                                      本区域湿度要求：30~75%")));

        // 设置1.5倍行距
        var infoParagraph3Properties = infoParagraph3.AppendChild(new ParagraphProperties());
        infoParagraph3Properties.AppendChild(new SpacingBetweenLines() { Line = "360", LineRule = LineSpacingRuleValues.Auto }); // 1.5倍行距

        // 添加空行
        body.AppendChild(new Paragraph());

        // 3. 创建主数据表格
        var table = body.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Table());

        // 设置表格属性 - 表格宽度为100%
        var tableProperties = table.AppendChild(new TableProperties());
        tableProperties.AppendChild(new TableWidth() { Type = TableWidthUnitValues.Pct, Width = "5000" }); // 100%
        tableProperties.AppendChild(new TableBorders(
            new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
            new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
            new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
            new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
            new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
            new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }
        ));

        // 设置表格列宽 - 5列平均分配，每列20%
        var tableGrid = tableProperties.AppendChild(new TableGrid());
        for (int i = 0; i < 5; i++)
        {
            tableGrid.AppendChild(new GridColumn() { Width = "1000" }); // 每列20% (5000/5=1000)
        }

        // 创建表头行
        var headerDataRow = table.AppendChild(new TableRow());

        // 设置表头行高度
        var headerRowProperties = headerDataRow.AppendChild(new TableRowProperties());
        headerRowProperties.AppendChild(new TableRowHeight() { Val = 380, HeightType = HeightRuleValues.AtLeast }); // 行高约13磅

        var headerCells = new[] { "测量日期", "温度值(℃)", "湿度值(%)", "测量时间", "测量人员" };
        foreach (var header in headerCells)
        {
            var cell = headerDataRow.AppendChild(new TableCell());

            // 设置单元格垂直居中
            var cellProperties = cell.AppendChild(new TableCellProperties());
            cellProperties.AppendChild(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center });

            var paragraph = cell.AppendChild(new Paragraph());
            var run = paragraph.AppendChild(new Run(new Text(header)));
            run.AppendChild(new RunProperties(new Bold()));

            // 设置单元格内容居中和行高
            var cellParagraphProperties = paragraph.AppendChild(new ParagraphProperties());
            cellParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Center });
            cellParagraphProperties.AppendChild(new SpacingBetweenLines() { Line = "260", LineRule = LineSpacingRuleValues.Auto }); // 行高约12磅
        }

        // 创建数据行 - 分页处理，每页最多25行数据
        const int maxRowsPerPage = 25;
        var totalPages = (int)Math.Ceiling((double)recordData.Count / maxRowsPerPage);

        for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
        {
            // 如果不是第一页，添加分页符
            if (pageIndex > 0)
            {
                // 添加分页符
                var pageBreak = body.AppendChild(new Paragraph());
                pageBreak.AppendChild(new ParagraphProperties()).AppendChild(new PageBreakBefore());

                // 重新创建标题
                var newTitleParagraph = body.AppendChild(new Paragraph());
                var newTitleRun = newTitleParagraph.AppendChild(new Run(new Text($"{systemName}温湿度、相对湿度记录表")));

                // 设置标题字体属性：四号字体和粗体
                var newTitleRunProperties = newTitleRun.AppendChild(new RunProperties());
                newTitleRunProperties.AppendChild(new Bold());
                newTitleRunProperties.AppendChild(new FontSize() { Val = "28" }); // 四号字体 (14磅)

                // 设置标题居中
                var newTitleParagraphProperties = newTitleParagraph.AppendChild(new ParagraphProperties());
                newTitleParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Center });

                // 添加空行
                body.AppendChild(new Paragraph());

                // 重新创建表头上方信息区域（使用空格对齐）
                var newInfoParagraph1 = body.AppendChild(new Paragraph());
                newInfoParagraph1.AppendChild(new Run(new Text($"部门(房间号)：2#{roomNumber}")));

                // 设置1.5倍行距
                var newInfoParagraph1Properties = newInfoParagraph1.AppendChild(new ParagraphProperties());
                newInfoParagraph1Properties.AppendChild(new SpacingBetweenLines() { Line = "360", LineRule = LineSpacingRuleValues.Auto }); // 1.5倍行距

                var newInfoParagraph2 = body.AppendChild(new Paragraph());
                newInfoParagraph2.AppendChild(new Run(new Text($"文明生产区类别：二类                                          静电控制类别：管控点")));

                // 设置1.5倍行距
                var newInfoParagraph2Properties = newInfoParagraph2.AppendChild(new ParagraphProperties());
                newInfoParagraph2Properties.AppendChild(new SpacingBetweenLines() { Line = "360", LineRule = LineSpacingRuleValues.Auto }); // 1.5倍行距

                var newInfoParagraph3 = body.AppendChild(new Paragraph());
                newInfoParagraph3.AppendChild(new Run(new Text($"本区域温度要求：15~30℃                                      本区域湿度要求：30~75%")));

                // 设置1.5倍行距
                var newInfoParagraph3Properties = newInfoParagraph3.AppendChild(new ParagraphProperties());
                newInfoParagraph3Properties.AppendChild(new SpacingBetweenLines() { Line = "360", LineRule = LineSpacingRuleValues.Auto }); // 1.5倍行距

                // 添加空行
                body.AppendChild(new Paragraph());

                // 重新创建主数据表格
                table = body.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Table());

                // 设置表格属性 - 表格宽度为100%
                tableProperties = table.AppendChild(new TableProperties());
                tableProperties.AppendChild(new TableWidth() { Type = TableWidthUnitValues.Pct, Width = "5000" }); // 100%
                tableProperties.AppendChild(new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }
                ));

                // 设置表格列宽 - 5列平均分配，每列20%
                var newTableGrid = tableProperties.AppendChild(new TableGrid());
                for (int i = 0; i < 5; i++)
                {
                    newTableGrid.AppendChild(new GridColumn() { Width = "1000" }); // 每列20%
                }

                // 重新创建表头行
                var newHeaderDataRow = table.AppendChild(new TableRow());

                // 设置表头行高度
                var newHeaderRowProperties = newHeaderDataRow.AppendChild(new TableRowProperties());
                newHeaderRowProperties.AppendChild(new TableRowHeight() { Val = 380, HeightType = HeightRuleValues.AtLeast }); // 行高约13磅

                var newHeaderCells = new[] { "测量日期", "温度值(℃)", "湿度值(%)", "测量时间", "测量人员" };
                foreach (var header in newHeaderCells)
                {
                    var cell = newHeaderDataRow.AppendChild(new TableCell());

                    // 设置单元格垂直居中
                    var cellProperties = cell.AppendChild(new TableCellProperties());
                    cellProperties.AppendChild(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center });

                    var paragraph = cell.AppendChild(new Paragraph());
                    var run = paragraph.AppendChild(new Run(new Text(header)));
                    run.AppendChild(new RunProperties(new Bold()));

                    // 设置单元格内容居中和行高
                    var cellParagraphProperties = paragraph.AppendChild(new ParagraphProperties());
                    cellParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Center });
                    cellParagraphProperties.AppendChild(new SpacingBetweenLines() { Line = "260", LineRule = LineSpacingRuleValues.Auto }); // 行高约12磅
                }
            }

            // 计算当前页的数据范围
            var startIndex = pageIndex * maxRowsPerPage;
            var endIndex = Math.Min(startIndex + maxRowsPerPage, recordData.Count);
            var pageData = recordData.Skip(startIndex).Take(maxRowsPerPage).ToList();

            // 添加当前页的数据行
            foreach (var record in pageData)
            {
                var dataRow = table.AppendChild(new TableRow());

                // 设置数据行高度
                var dataRowProperties = dataRow.AppendChild(new TableRowProperties());
                dataRowProperties.AppendChild(new TableRowHeight() { Val = 320, HeightType = HeightRuleValues.AtLeast }); // 行高约11磅

                var cellData = new[]
                {
                    record.MeasurementDate,
                    record.TemperatureValue,
                    record.HumidityValue,
                    record.MeasurementTime,
                    record.Measurer
                };

                foreach (var data in cellData)
                {
                    var cell = dataRow.AppendChild(new TableCell());

                    // 设置单元格垂直居中
                    var cellProperties = cell.AppendChild(new TableCellProperties());
                    cellProperties.AppendChild(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center });

                    var paragraph = cell.AppendChild(new Paragraph());
                    paragraph.AppendChild(new Run(new Text(data)));

                    // 设置单元格内容居中和行高
                    var cellParagraphProperties = paragraph.AppendChild(new ParagraphProperties());
                    cellParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Center });
                    cellParagraphProperties.AppendChild(new SpacingBetweenLines() { Line = "260", LineRule = LineSpacingRuleValues.Auto }); // 行高约12磅
                }
            }

            // 为当前页添加记录要求和参照制度/标准（每一页都需要）
            // 4. 在表格内添加记录要求行（合并所有列为一个单元格）
            var requirementsRow = table.AppendChild(new TableRow());

            // 设置记录要求行高度
            var requirementsRowProperties = requirementsRow.AppendChild(new TableRowProperties());
            requirementsRowProperties.AppendChild(new TableRowHeight() { Val = 380, HeightType = HeightRuleValues.AtLeast }); // 行高约13磅

            // 创建合并的单元格，跨5列
            var requirementsCell = requirementsRow.AppendChild(new TableCell());

            // 设置单元格属性 - 跨5列
            var requirementsCellProperties = requirementsCell.AppendChild(new TableCellProperties());
            requirementsCellProperties.AppendChild(new GridSpan() { Val = 5 });

            // 记录要求内容
            var requirementsParagraph1 = requirementsCell.AppendChild(new Paragraph());
            requirementsParagraph1.AppendChild(new Run(new Text("记录要求：1、参考《八部工作场所与工作环境(物理因素)识别对照表》(详见附录B)执行。")));

            var requirementsParagraph2 = requirementsCell.AppendChild(new Paragraph());
            requirementsParagraph2.AppendChild(new Run(new Text(".         2、每次在试验开始前进行温湿度记录。")));

            // 5. 在表格内添加参照制度/标准行（合并所有列为一个单元格）
            var standardsRow = table.AppendChild(new TableRow());

            // 设置参照制度行高度
            var standardsRowProperties = standardsRow.AppendChild(new TableRowProperties());
            standardsRowProperties.AppendChild(new TableRowHeight() { Val = 500, HeightType = HeightRuleValues.AtLeast }); // 行高约18磅

            // 创建合并的单元格，跨5列
            var standardsCell = standardsRow.AppendChild(new TableCell());

            // 设置单元格属性 - 跨5列
            var standardsCellProperties = standardsCell.AppendChild(new TableCellProperties());
            standardsCellProperties.AppendChild(new GridSpan() { Val = 5 });

            // 参照制度/标准内容
            var standardsParagraph1 = standardsCell.AppendChild(new Paragraph());
            standardsParagraph1.AppendChild(new Run(new Text("参照制度/标准：")));

            var standardsParagraph2 = standardsCell.AppendChild(new Paragraph());
            standardsParagraph2.AppendChild(new Run(new Text("1) Q/RJ 90A-2014        《航天产品文明生产区域等级划分及其管理要求》")));

            var standardsParagraph3 = standardsCell.AppendChild(new Paragraph());
            standardsParagraph3.AppendChild(new Run(new Text("2) 沪八部行[2017]82号  《八部静电防护工作管理办法》")));

            var standardsParagraph4 = standardsCell.AppendChild(new Paragraph());
            standardsParagraph4.AppendChild(new Run(new Text("3) 沪八部档(2015)99号  《八部档案工作实施办法》")));
        }

        document.Save();
        return stream.ToArray();
    }
}
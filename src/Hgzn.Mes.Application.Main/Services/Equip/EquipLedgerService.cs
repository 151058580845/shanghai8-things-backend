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

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipLedgerService : SugarCrudAppService<
        EquipLedger, Guid,
        EquipLedgerReadDto, EquipLedgerQueryDto,
        EquipLedgerCreateDto, EquipLedgerUpdateDto>,
    IEquipLedgerService
{

    private readonly HttpClient _httpClient;
    private readonly ICodeRuleService _codeRuleService;

    public EquipLedgerService(HttpClient httpClient, ICodeRuleService codeRuleService)
    {
        _httpClient = httpClient;
        _codeRuleService = codeRuleService;
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
        var keys = equipIds.Keys.ToArray();
        var list = Queryable.Where(t => keys.Contains(t.EquipCode)).ToList();
        foreach (var equipLedger in list)
        {
            equipLedger.RoomId = equipIds[equipLedger.EquipCode];
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
    public async Task<IEnumerable<EquipLedgerSearchReadDto>> GetAppSearchAsync()
    {
        var entities = await Queryable.Where(t => t.State == false).Includes(t => t.EquipType)
            .Select(t => new EquipLedgerSearchReadDto()
            {
                Id = t.Id,
                EquipCode = t.EquipCode,
                EquipName = t.EquipName,
                TypeId = t.TypeId,
                TypeName = t.EquipType!.TypeName,
                Model = t.Model,
                RoomId = t.RoomId,
                AssetNumber = t.AssetNumber
            }).ToListAsync();
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
            .WhereIF(!string.IsNullOrEmpty(query.EquipCode), m => m.EquipName.Contains(query.EquipCode!))
            .WhereIF(!string.IsNullOrEmpty(query.AssetNumber), m => m.EquipName == query.AssetNumber)
            .WhereIF(query.ResponsibleUserId is not null, m => m.ResponsibleUserId.Equals(query.ResponsibleUserId))
            .WhereIF(!string.IsNullOrEmpty(query.Query),
                m => m.EquipName.Contains(query.Query!) ||
                m.Model!.Contains(query.Query!))
            .WhereIF(!query.TypeId.IsNullableGuidEmpty(), m => m.TypeId.Equals(query.TypeId))
            .WhereIF(query.NoRfidDevice == true, m => m.TypeId != EquipType.RfidIssuerType.Id && m.TypeId != EquipType.RfidReaderType.Id)
            .WhereIF(!query.RoomId.IsNullableGuidEmpty(), m => m.RoomId.Equals(query.RoomId))
            .WhereIF(query.StartTime != null, m => m.CreationTime >= query.StartTime)
            .WhereIF(query.EndTime != null, m => m.CreationTime <= query.EndTime)
            .WhereIF(query.State != null, m => m.State == query.State);

        if (query.BindingTagCount is not null)
        {
            queryable = queryable.Includes(eq => eq.Labels);
            queryable = query.BindingTagCount == -1 ?
                queryable.Where(eq => SqlFunc.Subqueryable<LocationLabel>()
                        .Where(l => l.EquipLedgerId == eq.Id)
                        .Count() > 0) :
                queryable.Where(eq => SqlFunc.Subqueryable<LocationLabel>()
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
        var queryable = query is null ? Queryable :
            Queryable
            .WhereIF(!string.IsNullOrEmpty(query.EquipCode), m => m.EquipName.Contains(query.EquipCode!))
            .WhereIF(!string.IsNullOrEmpty(query.AssetNumber), m => m.EquipName == query.AssetNumber)
            .WhereIF(!string.IsNullOrEmpty(query.Query),
                m => m.EquipName.Contains(query.Query!) ||
                m.Model!.Contains(query.Query!))
            .WhereIF(query.ResponsibleUserId is not null, m => m.ResponsibleUserId.Equals(query.ResponsibleUserId))
            .WhereIF(!query.TypeId.IsNullableGuidEmpty(), m => m.TypeId.Equals(query.TypeId))
            .WhereIF(query.NoRfidDevice == true, m => m.TypeId != EquipType.RfidIssuerType.Id && m.TypeId != EquipType.RfidReaderType.Id)
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
            DateTime.UtcNow.AddMinutes(-30) >= e.LastMoveTime).ToArrayAsync();
        return Mapper.Map<IEnumerable<EquipLedgerReadDto>>(entities);
    }

    public async Task<IEnumerable<EquipLedgerReadDto>?> GetListByTypeAsync(string? protocolEnum)
    {
        // var protocol = Enum.Parse<EquipConnType>(type, true);
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(protocolEnum), m => m.EquipType!.ProtocolEnum == protocolEnum && (!m.EquipName.Contains("测试") || !m.EquipCode.Contains("测试")))
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

        // 获取当前日期
        DateTime currentDate = DateTime.Now;

        EquipLedger[] oldEquipMeasurements = await DbContext.Queryable<EquipLedger>().ToArrayAsync();

        // 遍历每一行（从第 2 行开始）
        for (int row = 1; row <= sheet.LastRowNum; row++)
        {
            IRow dataRow = sheet.GetRow(row);
            if (dataRow == null)
                continue; // 跳过空行

            // 检查是否是计量设备仪器
            bool? isMeasurementDevice = dataRow.GetCell(columnIndices["是否计量设备仪器"])?.ToString()?.Trim() == "是" ? true : false;
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

            bool isExist = false;
            foreach (EquipLedger item in oldEquipMeasurements)
            {
                if (item.EquipName == assetNameStr && item.Model == modelStr && item.AssetNumber == localAssetNumberStr && item.IsMeasurementDevice == isMeasurementDevice &&
                    !string.IsNullOrEmpty(expiryDateStr) && item.ValidityDate.ToString() != expiryDateStr)
                {
                    // 进入此判断说明有效期不一致,需要更新
                    DbContext.Updateable(item).SetColumns(it => new EquipLedger()
                    {
                        ValidityDate = DateTime.Parse(expiryDateStr),
                        ResponsibleUserName = responsiblePersonStr,
                        AssetNumber = localAssetNumberStr,
                        Model = modelStr,
                        EquipName = assetNameStr,
                        IsMeasurementDevice = isMeasurementDevice,
                    }).ExecuteCommand();
                    isExist = true;
                }
            }
            if (!isExist)
            {
                Guid? userId = (await DbContext.Queryable<User>().FirstAsync(x => x.Name == responsiblePersonStr))?.Id;
                DateTime? dt = null;
                if (!string.IsNullOrEmpty(expiryDateStr))
                    dt = DateTime.Parse(expiryDateStr);
                EquipLedgerCreateDto input = new EquipLedgerCreateDto()
                {
                    EquipCode = await _codeRuleService.GenerateCodeByCodeAsync("SBTZ"),
                    ValidityDate = dt,
                    ResponsibleUserId = userId,
                    ResponsibleUserName = responsiblePersonStr,
                    AssetNumber = localAssetNumberStr,
                    Model = modelStr,
                    EquipName = assetNameStr,
                    IsMeasurementDevice = isMeasurementDevice,
                    DeviceStatus = DeviceStatus.Normal.ToString(),
                    DeviceLevel = EquipLevelEnum.Basic.ToString(),
                };
                try
                {

                    await CreateAsync(input);
                }
                catch (Exception e) { }
            }
        }
    }
}
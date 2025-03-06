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

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipLedgerService : SugarCrudAppService<
        EquipLedger, Guid,
        EquipLedgerReadDto, EquipLedgerQueryDto,
        EquipLedgerCreateDto, EquipLedgerUpdateDto>,
    IEquipLedgerService
{

    private readonly HttpClient _httpClient;

    public EquipLedgerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
                Name = t.EquipName,
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
            .Select<EquipLedgerSearchReadDto>(t => new EquipLedgerSearchReadDto()
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

    public async Task<IEnumerable<EquipLedgerReadDto>> GetEquipsListByRoomAsync(IEnumerable<Guid> rooms)
    {
        var equipList = await Queryable.Where(t =>t.RoomId != null && rooms.Contains(t.RoomId!.Value)).ToListAsync();
        return Mapper.Map<IEnumerable<EquipLedgerReadDto>>(equipList);
    }

    public override async Task<PaginatedList<EquipLedgerReadDto>> GetPaginatedListAsync(EquipLedgerQueryDto query)
    {
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(query.EquipName), m => m.EquipName.Contains(query.EquipName!))
            .WhereIF(!string.IsNullOrEmpty(query.EquipCode), m => m.EquipName.Contains(query.EquipCode!))
            .WhereIF(!query.TypeId.IsNullableGuidEmpty(), m => m.TypeId.Equals(query.TypeId))
            .WhereIF(!query.RoomId.IsNullableGuidEmpty(), m => m.RoomId.Equals(query.RoomId))
            .WhereIF(query.StartTime != null, m => m.CreationTime >= query.StartTime)
            .WhereIF(query.EndTime != null, m => m.CreationTime <= query.EndTime)
            .WhereIF(query.State != null, m => m.State == query.State)
            .Includes(t => t.Room)
            .Includes(t => t.EquipType)
            .OrderByDescending(m => m.OrderNum)
            .ToPaginatedListAsync(query.PageIndex, query.PageSize);
        return Mapper.Map<PaginatedList<EquipLedgerReadDto>>(entities);
    }

    public override async Task<IEnumerable<EquipLedgerReadDto>> GetListAsync(EquipLedgerQueryDto? query = null)
    {
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(query.EquipName), m => m.EquipName.Contains(query.EquipName!))
            .WhereIF(!string.IsNullOrEmpty(query.EquipCode), m => m.EquipName.Contains(query.EquipCode!))
            .WhereIF(!query.TypeId.IsNullableGuidEmpty(), m => m.TypeId.Equals(query.TypeId))
            .WhereIF(!query.RoomId.IsNullableGuidEmpty(), m => m.RoomId.Equals(query.RoomId))
            .WhereIF(query.StartTime != null, m => m.CreationTime >= query.StartTime)
            .WhereIF(query.EndTime != null, m => m.CreationTime <= query.EndTime)
            .WhereIF(query?.State != null, m => m.State == query.State)
            .Includes(t => t.Room)
            .Includes(t => t.EquipType)
            .OrderByDescending(m => m.OrderNum)
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
            if (result!=null && result.Any())
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
            Console.WriteLine($"HTTP 请求失败: {ex.Message}");
            throw;
        }
        catch (JsonException ex)
        {
            // 处理 JSON 反序列化异常
            Console.WriteLine($"JSON 反序列化失败: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            // 处理其他异常
            Console.WriteLine($"发生错误: {ex.Message}");
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
}
using System.ComponentModel;
using System.Reflection;
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
using StackExchange.Redis;

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
        IDatabase database = _connectionMultiplexer.GetDatabase();
        List<AssetNumberObj> list = await DbContext.Queryable<TestEquipData>()
            .Where(x => x.SimuTestSysld == systemId && x.DevTypeld == equipTypeId)
            .LeftJoin<EquipLedger>((testequipdate, equipledger) => testequipdate.Compld == equipledger.AssetNumber)
            .Select((testequipdate, equipledger) => new AssetNumberObj
            {
                EquipName = equipledger.EquipName,
                AssetNumber = testequipdate.Compld
            }).ToListAsync();
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
            return null;
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
            return null;
        }

        var text = new Receive()
        {
            SimuTestSysld = query.SystemId,
            DevTypeld = query.EquipTypeId,
            Compld = null
        };
        var tableName = _splitTableService.GetFieldValuesTableName(text);

        var rawData = await DbContext.Queryable<Receive>()
            .WhereIF(query.StartDateTime != null, it => it.CreateTime >= query.StartDateTime)
            .WhereIF(query.EndDateTime != null, it => it.CreateTime <= query.EndDateTime)
            .SplitTable(tas => tas.Where(y => y.TableName.Contains(tableName)
                                              && query.AssetNumbers != null
                                              && query.AssetNumbers.Any(t => y.TableName.Contains(t, StringComparison.CurrentCultureIgnoreCase)))
                .ToList())
            .Select(t => t.Content)
            .ToPaginatedListAsync(query.PageIndex, query.PageSize);
        return rawData;
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
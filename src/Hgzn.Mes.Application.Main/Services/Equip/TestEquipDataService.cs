using System.ComponentModel;
using System.Reflection;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Attributes;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class TestEquipDataService(IConnectionMultiplexer _connectionMultiplexer, ILogger<TestEquipDataService> _logger) : SugarCrudAppService<TestEquipData, Guid, TestEquipDataReadDto>, ITestEquipDataService
{
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

    public async Task<List<ColumnObj>> GetColumnsAsync(int systemId, int equipTypeId)
    {
        await Task.CompletedTask;
        var assem = Assembly.Load("Hgzn.Mes.Domain");
        int room = TestEquipData.GetRoom(systemId);
        var type = assem.GetType($"Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_{room}_ReceiveDatas.XT_{room}_SL_{equipTypeId}_ReceiveData");
        if (type is null)
        {
            return null;
        }
        var properties = type.GetProperties();
        List<ColumnObj> list = [];
        foreach (var property in properties)
        {
            if (property.Name == "Seeds" || property.IsDefined(typeof(TableNotShowAttribute))) continue;
            list.Add(new ColumnObj
            {
                Label = property.Name == "CreationTime" ? "数据获取时间" : (property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? property.Name),
                Prop = property.Name,
                Hide = property.Name == "Id" ? true : false,
                Width = 150
            });
        }
        return list;
    }

    public async Task<object> GetDatasAsync(TestEquipDataQueryDto query)
    {
        try
        {
            var assem = Assembly.Load("Hgzn.Mes.Domain");
            int room = TestEquipData.GetRoom(query.SystemId);
            string selectsql = $"SELECT * ";
            string basesql = $"FROM equip_x_t_{room}_s_l_{query.EquipTypeId}_receive_data WHERE simu_test_sysld={query.SystemId} AND dev_typeld={query.EquipTypeId}";
            string conditionalsql = "";
            if (query.AssetNumbers != null && query.AssetNumbers.Count > 0)
            {
                for (var i = 0; i < query.AssetNumbers.Count; i++)
                {
                    query.AssetNumbers[i] = "'" + query.AssetNumbers[i] + "'";
                }
                string condition = " AND compld IN (" + string.Join(',', query.AssetNumbers) + ")";
                conditionalsql += condition;
            }
            if (query.StartDateTime != null)
            {
                conditionalsql += " AND creation_time >= '" + query.StartDateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
            if (query.EndDateTime != null)
            {
                conditionalsql += " AND creation_time <= '" + query.EndDateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
            string pagesql = " ORDER BY creation_time desc ";
            if (query.PageIndex != 0 && query.PageSize != 0)
            {
                pagesql = $" ORDER BY creation_time desc LIMIT {query.PageSize} OFFSET {query.PageSize * (query.PageIndex - 1)};";
            }
            string sqlCount = $"SELECT COUNT(*) " + basesql + conditionalsql;
            string sql = selectsql + basesql + conditionalsql + pagesql;
            var count = await DbContext.Ado.SqlQueryAsync<int>(sqlCount);
            var result = await DbContext.Ado.SqlQueryAsync<object>(sql);
            //_logger.LogInformation(sql);
            return new { Items = result, PageSize = query.PageSize, TotalCount = count[0] };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
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

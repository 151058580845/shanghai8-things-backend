using System.Linq.Expressions;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Shared;
using SqlSugar;

namespace Hgzn.Mes.Domain.Services;

public interface ISplitTableQueryService : IDomainService
{
    /// <summary>
    /// 查询指定时间范围内的数据(适用于按时间分表的实体)
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="otherSplitFieldValues">其他分表字段值</param>
    /// <returns>查询结果</returns>
    Task<IEnumerable<Receive>?> QueryByTimeRangeAsync(
        DateTime? startTime,
        DateTime? endTime,
        object otherSplitFieldValues);

    /// <summary>
    /// 查询最后一条数据（优化：只查询最近两个月的表）
    /// </summary>
    /// <param name="otherSplitFieldValues">分表字段值（Receive对象）</param>
    /// <returns>最后一条数据，如果没有则返回null</returns>
    Task<Receive?> GetLatestDataAsync(object otherSplitFieldValues);
}
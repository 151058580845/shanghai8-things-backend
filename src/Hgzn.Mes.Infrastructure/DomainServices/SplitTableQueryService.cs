using System.Linq.Expressions;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Services;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Extensions;
using Hgzn.Mes.Infrastructure.DbContexts.Ef;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Hgzn.Mes.Infrastructure.Utilities;
using SqlSugar;

namespace Hgzn.Mes.Infrastructure.DomainServices;

public class SplitTableQueryService : ISplitTableQueryService
    {
        public ISqlSugarClient DbContext;
        private readonly SplitTableService _splitTableService;
        public SplitTableQueryService(ISqlSugarClient db, SplitTableService splitTableService)
        {
            DbContext = db;
            _splitTableService = splitTableService;
        }

        /// <summary>
        /// 查询指定时间范围内的数据(适用于按时间分表的实体)
        /// </summary>
        public async Task<IEnumerable<Receive>?> QueryByTimeRangeAsync(
            DateTime? startTime, 
            DateTime? endTime, 
            object otherSplitFieldValues
            ) 
        {
            // 计算需要查询的所有月份
            // var months = GetMonthsBetween(startTime, endTime);
            string tableName = _splitTableService.GetFieldValuesTableName(otherSplitFieldValues);
            var data = await  DbContext.Queryable<Receive>()
                .WhereIF(startTime != null, it => it.CreateTime >= startTime)
                .WhereIF(endTime != null, it => it.CreateTime <= endTime)
                .SplitTable(tas => tas.Where(y=>y.TableName.Contains(tableName))
                .ToList())
                .ToListAsync();

            // 遍历每个月份进行查询
            // foreach (var date in months)
            // {
            //     try
            //     {
            //         // 合并分表字段值
            //         var splitFieldValues = MergeSplitFieldValues(otherSplitFieldValues, timeFieldName, date);
            //         // 获取表名
            //         // string tableName = _splitTableService.GetFieldValuesTableName(otherSplitFieldValues);
            //         
            //         // 创建查询对象并指定表名
            //         var query = _db.Queryable<T>().AS(tableName);
            //         
            //         // 构建时间范围条件
            //         var parameter = Expression.Parameter(typeof(T), "x");
            //         var property = Expression.Property(parameter, timeFieldName);
            //         var startConstant = Expression.Constant(startTime);
            //         var endConstant = Expression.Constant(endTime);
            //         
            //         var greaterEqual = Expression.GreaterThanOrEqual(property, startConstant);
            //         var lessEqual = Expression.LessThanOrEqual(property, endConstant);
            //         var andExpression = Expression.AndAlso(greaterEqual, lessEqual);
            //         
            //         var timeRangeExpression = Expression.Lambda<Func<T, bool>>(andExpression, parameter);
            //         
            //         query = query.Where(timeRangeExpression);
            //         
            //         // 应用其他查询条件
            //         if (whereExpression != null)
            //         {
            //             query = query.Where(whereExpression);
            //         }
            //         
            //         // 执行查询并合并结果
            //         var monthData = await query.ToListAsync();
            //         result.AddRange(monthData);
            //     }
            //     catch (Exception ex)
            //     {
            //         // 表不存在或其他错误，继续下一个月
            //         continue;
            //     }
            // }
            
            return data;
        }

        /// <summary>
        /// 查询最后一条数据（优化：只查询最近两个月的表）
        /// </summary>
        public async Task<Receive?> GetLatestDataAsync(object otherSplitFieldValues)
        {
            // 优化：只查询当前月份和上个月的数据（确保能获取到最新数据）
            var currentMonth = DateTime.Now.ToString("yyyyMM");
            var lastMonth = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
            
            // 获取表名前缀（不包含月份部分）
            string baseTableName = _splitTableService.GetFieldValuesTableName(otherSplitFieldValues);
            
            // 如果传入的是 Receive 对象，提取过滤条件
            var query = DbContext.Queryable<Receive>();
            if (otherSplitFieldValues is Receive receiveFilter)
            {
                if (receiveFilter.SimuTestSysld.HasValue)
                    query = query.Where(it => it.SimuTestSysld == receiveFilter.SimuTestSysld);
                if (receiveFilter.DevTypeld.HasValue)
                    query = query.Where(it => it.DevTypeld == receiveFilter.DevTypeld);
                if (!string.IsNullOrEmpty(receiveFilter.Compld))
                    query = query.Where(it => it.Compld == receiveFilter.Compld);
            }
            
            // 查询最后一条数据 - 只查询当前月份和上个月的表
            var data = await query
                .SplitTable(tas => tas.Where(y => 
                    y.TableName.Contains(baseTableName) && 
                    (y.TableName.Contains($"_{currentMonth}") || y.TableName.Contains($"_{lastMonth}")))
                    .ToList())
                .OrderByDescending(x => x.CreateTime)
                .Take(1)
                .FirstAsync();
            
            return data;
        }
    }

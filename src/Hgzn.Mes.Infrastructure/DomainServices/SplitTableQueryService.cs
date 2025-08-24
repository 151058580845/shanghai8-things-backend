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
    }

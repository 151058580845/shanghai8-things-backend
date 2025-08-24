using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using SqlSugar;

namespace Hgzn.Mes.Domain.Services
{
    public class SplitTableService : ISplitTableService
    {
        private const string TableSuffix = "_sysj";

        // 缓存编译后的表达式树委托，避免重复编译
        private static readonly ConcurrentDictionary<Type, Func<object, List<string>>> CompiledGetters = new();

        // 缓存分表字段信息
        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> SplitFieldCache = new();

        // 缓存表名，避免重复构建
        private static readonly ConcurrentDictionary<string, string> TableNameCache = new();

        /// <summary>
        /// 返回数据库中所有分表
        /// </summary>
        public List<SplitTableInfo> GetAllTables(ISqlSugarClient db, EntityInfo entityInfo,
            List<DbTableInfo> tableInfos)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = tableInfos
                .Where(item => item.Name.Contains(TableSuffix))
                .Select(item => new SplitTableInfo { TableName = item.Name })
                .OrderBy(it => it.TableName)
                .ToList();

            stopwatch.Stop();
            Console.WriteLine($"GetAllTables 执行时间: {stopwatch.Elapsed}");

            return result;
        }

        /// <summary>
        /// 获取分表字段的值
        /// </summary>
        public object GetFieldValue(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object entityValue)
        {
            if (entityValue == null) return null;

            var stopwatch = Stopwatch.StartNew();

            var entityType = entityValue.GetType();

            // 从缓存获取或创建委托
            var getter = CompiledGetters.GetOrAdd(entityType, type =>
                CreateFieldValueGetter(type));

            // 执行委托获取值
            var result = getter(entityValue);

            stopwatch.Stop();
            Console.WriteLine(
                $"GetFieldValue 执行时间: {stopwatch.ElapsedTicks} ticks ({stopwatch.ElapsedMilliseconds} ms)");

            return result.Count > 0 ? result : null;
        }

        public string GetFieldValuesTableName<T>(T entity) where T : class, new()
        {
            if (entity == null) return "";

            var entityType = entity.GetType();

            // 从缓存获取或创建委托
            var getter = CompiledGetters.GetOrAdd(entityType, type =>
                CreateFieldValueGetter(type));

            // 执行委托获取值
            var result = getter(entity);

            return String.Join("_", result);
        }

        /// <summary>
        /// 创建获取字段值的表达式树委托
        /// </summary>
        private static Func<object, List<string>> CreateFieldValueGetter(Type entityType)
        {
            // 获取分表字段
            var splitFields = GetSplitFields(entityType)
                .OrderBy(p => p.GetCustomAttribute<SplitFieldAttribute>()?.Priority ?? 0)
                .ToList();
            if (splitFields.Count == 0)
                return _ => new List<string>();
            // 参数: object entity
            var parameter = Expression.Parameter(typeof(object), "entity");
            // 转换为具体类型: (EntityType)entity
            var typedEntity = Expression.Convert(parameter, entityType);
            // 创建结果列表: var result = new List<string>()
            var listType = typeof(List<string>);
            var listVariable = Expression.Variable(listType, "result");
            var newListExpression = Expression.New(listType);
            var assignListExpression = Expression.Assign(listVariable, newListExpression);
            // 获取Add方法
            var addMethod = listType.GetMethod("Add", new[] { typeof(string) });
            // 构建表达式列表
            var expressions = new List<Expression> { assignListExpression };
            // 为每个分表字段添加值到列表
            foreach (var property in splitFields)
            {
                // 获取属性值
                var propertyAccess = Expression.Property(typedEntity, property);

                // 检查属性是否为可空类型
                var isNullable = Nullable.GetUnderlyingType(property.PropertyType) != null;

                // 创建跳过该字段的条件和值表达式
                Expression skipCondition;
                Expression valueExpression;
            
                if (property.PropertyType == typeof(string))
                {
                    // 字符串类型: 检查是否为null或空
                    var isNullOrEmptyMethod = typeof(string).GetMethod("IsNullOrEmpty", new[] { typeof(string) });
                    skipCondition = Expression.Call(isNullOrEmptyMethod, propertyAccess);
                    valueExpression = propertyAccess;
                }
                else if (isNullable)
                {
                    // 可空类型: 检查HasValue属性
                    skipCondition = Expression.Not(Expression.Property(propertyAccess, "HasValue"));

                    // 获取Value属性
                    var valueProperty = Expression.Property(propertyAccess, "Value");

                    // 获取底层类型
                    var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);

                    // 根据类型处理
                    if (underlyingType == typeof(DateTime))
                    {
                        // 日期时间类型使用格式化
                        var toStringMethod = typeof(DateTime).GetMethod("ToString", new[] { typeof(string) });
                        var formatAttribute = property.GetCustomAttribute<SplitFieldAttribute>();
                        var format = !string.IsNullOrEmpty(formatAttribute?.Format) ? formatAttribute.Format : "yyyyMM";
                        var formatConstant = Expression.Constant(format);
        
                        valueExpression = Expression.Call(valueProperty, toStringMethod, formatConstant);
                    }
                    else
                    {
                        // 其他可空类型调用底层类型的ToString方法
                        var toStringMethod = underlyingType.GetMethod("ToString", Type.EmptyTypes);
                        valueExpression = Expression.Call(valueProperty, toStringMethod);
                    }
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    // DateTime类型: 不可能为null，但可以检查默认值
                    var defaultDateTime = Expression.Constant(default(DateTime));
                    skipCondition = Expression.Equal(propertyAccess, defaultDateTime);

                    // 格式化DateTime
                    var toStringMethod = typeof(DateTime).GetMethod("ToString", new[] { typeof(string) });
                    var formatAttribute = property.GetCustomAttribute<SplitFieldAttribute>();
                    var format = !string.IsNullOrEmpty(formatAttribute?.Format) ? formatAttribute.Format : "yyyyMM";
                    var formatConstant = Expression.Constant(format);
                    valueExpression = Expression.Call(propertyAccess, toStringMethod, formatConstant);
                }
                else
                {
                    // 其他非可空类型: 不跳过
                    skipCondition = Expression.Constant(false);

                    // 调用ToString方法
                    var toStringMethod = property.PropertyType.GetMethod("ToString", Type.EmptyTypes);
                    valueExpression = Expression.Call(propertyAccess, toStringMethod);
                }

                // 创建条件语句: if (!skipCondition) { result.Add(valueExpression); }
                var addToListExpression = Expression.Call(listVariable, addMethod, valueExpression);
                var conditionalExpression = Expression.IfThen(Expression.Not(skipCondition), addToListExpression);
                expressions.Add(conditionalExpression);
            }

            // 返回列表
            expressions.Add(listVariable);
            // 创建代码块
            var block = Expression.Block(new[] { listVariable }, expressions);
            // 编译表达式树
            var lambda = Expression.Lambda<Func<object, List<string>>>(block, parameter);
            return lambda.Compile();
        }


        /// <summary>
        /// 获取分表字段信息（带缓存）
        /// </summary>
        private static List<PropertyInfo> GetSplitFields(Type entityType)
        {
            return SplitFieldCache.GetOrAdd(entityType, type =>
            {
                return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.GetCustomAttribute<SplitFieldAttribute>() != null)
                    .ToList();
            });
        }

        /// <summary>
        /// 默认表名
        /// </summary>
        public string GetTableName(ISqlSugarClient db, EntityInfo entityInfo)
        {
            return $"{entityInfo.DbTableName}{TableSuffix}";
        }

        /// <summary>
        /// 按类型获取表名
        /// </summary>
        public string GetTableName(ISqlSugarClient db, EntityInfo entityInfo, SplitType type)
        {
            return $"{entityInfo.DbTableName}{TableSuffix}";
        }

        /// <summary>
        /// 按值获取表名
        /// </summary>
        public string GetTableName(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object fieldValue)
        {
            var stopwatch = Stopwatch.StartNew();

            string result;

            if (fieldValue is List<string> values && values.Count > 0)
            {
                // 使用缓存优化表名构建
                var cacheKey = $"{entityInfo.DbTableName}_{string.Join("_", values)}";

                result = TableNameCache.GetOrAdd(cacheKey, key =>
                {
                    var sb = new StringBuilder(entityInfo.DbTableName);
                    sb.Append(TableSuffix);
                    foreach (var value in values)
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            sb.Append('_').Append(value);
                        }
                    }
                    return sb.ToString();
                });
            }
            else
            {
                // 默认表名
                result = entityInfo.DbTableName + TableSuffix;
            }

            stopwatch.Stop();
            Console.WriteLine($"GetTableName 执行时间: {stopwatch.Elapsed})");

            return result;
        }
    }

    /// <summary>
    /// 分表字段标记特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SplitFieldAttribute : Attribute
    {
        /// <summary>
        /// 字段优先级（用于多字段排序）
        /// </summary>
        public int Priority { get; set; } = 0;

        /// <summary>
        /// 自定义格式化字符串（主要用于DateTime类型）
        /// </summary>
        public string Format { get; set; }
    }
}
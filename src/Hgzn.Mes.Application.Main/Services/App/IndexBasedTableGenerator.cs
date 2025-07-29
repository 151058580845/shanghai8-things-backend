using Hgzn.Mes.Application.Main.Dtos.App;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.App
{
    /// <summary>
    /// 基于属性索引的表格生成服务
    /// </summary>
    public class IndexBasedTableGenerator
    {
        /// <summary>
        /// 从实体实例生成表格数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="instance">实体实例</param>
        /// <param name="title">表格标题</param>
        /// <param name="startIndex">起始属性索引(从0开始)</param>
        /// <param name="endIndex">结束属性索引(包含)</param>
        /// <returns>TableDto对象</returns>
        public TableDto GenerateTableFromInstance<T>(
            T instance,
            string title,
            int startIndex = 0,
            int? endIndex = null) where T : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            // 获取所有公共属性并按声明顺序排序
            var properties = typeof(T)
                .GetProperties()
                .Where(p => p.CanRead)
                .OrderBy(p => p.MetadataToken)
                .ToList();

            // 设置默认结束索引
            endIndex ??= properties.Count - 1;

            // 验证索引范围
            if (startIndex < 0 || startIndex >= properties.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex),
                    $"起始索引{startIndex}超出有效范围(0-{properties.Count - 1})");
            }

            if (endIndex < startIndex || endIndex >= properties.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(endIndex),
                    $"结束索引{endIndex}无效，必须大于等于{startIndex}且小于{properties.Count}");
            }

            // 创建表格DTO
            var tableDto = new TableDto
            {
                Title = title,
                Header = CreateStandardHeader(),
                Data = new List<Dictionary<string, string>>()
            };

            // 处理选定范围内的属性
            for (int i = startIndex; i <= endIndex; i++)
            {
                var property = properties[i];
                var displayName = GetPropertyDisplayName(property);
                var value = property.GetValue(instance)?.ToString() ?? string.Empty;

                tableDto.Data.Add(CreateDataRow(displayName, value));
            }

            return tableDto;
        }

        /// <summary>
        /// 获取属性显示名称(优先使用Description特性)
        /// </summary>
        private string GetPropertyDisplayName(PropertyInfo property)
        {
            return property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? property.Name;
        }

        /// <summary>
        /// 创建标准表格表头
        /// </summary>
        private List<List<string>> CreateStandardHeader()
        {
            return new List<List<string>>
        {
            new() { "name", "物理量定义" },
            new() { "control", "物理量" }
        };
        }

        /// <summary>
        /// 创建表格数据行
        /// </summary>
        private Dictionary<string, string> CreateDataRow(string name, string controlValue)
        {
            return new Dictionary<string, string>
        {
            { "name", name },
            { "control", controlValue }
        };
        }
    }
}

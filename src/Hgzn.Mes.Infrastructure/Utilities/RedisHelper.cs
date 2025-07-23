using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Dm.net.buffer.ByteArrayBuffer;
using static NPOI.HSSF.Util.HSSFColor;

namespace Hgzn.Mes.Infrastructure.Utilities
{
    public class RedisHelper : Module
    {
        public IConnectionMultiplexer Redis { get; set; }  // Redis 连接对象
        private readonly IDatabase _db;                 // Redis 数据库对象

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration">配置</param>
        /// <param name="logger">日志记录器(可选)</param>
        public RedisHelper(IConnectionMultiplexer cm)
        {
            Redis = cm;
            // 获取默认数据库
            _db = Redis.GetDatabase();
        }

        /// <summary>
        /// 获取Redis键的层级结构树
        /// </summary>
        public async Task<RedisTreeNode> GetTreeAsync(string prefix = "")
        {
            var root = new RedisTreeNode("root", "");
            var endpoints = Redis.GetEndPoints();
            var server = Redis.GetServer(endpoints[0]);

            await foreach (var key in ScanKeysAsync(server, $"{prefix}*"))
            {
                await BuildTreeAsync(root, key);
            }

            return root;
        }

        /// <summary>
        /// 获取指定类型节点的子元素（适用于Hash/List/Set/SortedSet等）
        /// </summary>
        public async Task<IEnumerable<string>> GetTreeNodeChildrenValuesAsync(RedisTreeNode node, int maxCount = 100)
        {
            if (node == null || !node.IsLeaf)
                return Enumerable.Empty<string>();

            return node.KeyType switch
            {
                RedisKeyType.Hash => (await _db.HashGetAllAsync(node.FullPath))
                    .Take(maxCount)
                    .Select(entry => $"{entry.Name}:{entry.Value}"),

                RedisKeyType.List => (await _db.ListRangeAsync(node.FullPath, 0, maxCount - 1))
                    .Select(v => v.ToString()),

                RedisKeyType.Set => (await _db.SetMembersAsync(node.FullPath))
                    .Take(maxCount)
                    .Select(v => v.ToString()),

                RedisKeyType.SortedSet => (await _db.SortedSetRangeByRankWithScoresAsync(node.FullPath, 0, maxCount - 1))
                    .Select(entry => $"{entry.Element}:{entry.Score}"),

                RedisKeyType.Stream => (await _db.StreamRangeAsync(node.FullPath, count: maxCount))
                    .Select(entry => $"{entry.Id}:{string.Join(",", entry.Values)}"),

                _ => null!
            };
        }

        /// <summary>
        /// 在层级树中异步查找指定路径的节点（并更新节点类型信息）[注意,这个函数区别于使用名字查找的FindTreeNodeFirstByNameAsync,这个函数需要输入除root外的完整路径,路径之间用:分割]
        /// </summary>
        /// <param name="root">树根节点</param>
        /// <param name="path">要查找的路径(用:分隔)</param>
        /// <returns>找到的节点，未找到返回null</returns>
        public async Task<RedisTreeNode> FindTreeNodeByPathAsync(RedisTreeNode root, string path)
        {
            if (root == null) return null;
            var parts = path.Split(':');
            var currentNode = root;

            foreach (var part in parts)
            {
                currentNode = currentNode.Children.FirstOrDefault(n => n.Name == part);
                if (currentNode == null) return null;
            }

            // 如果是叶子节点，确保类型信息是最新的
            if (currentNode.IsLeaf && currentNode.KeyType == RedisKeyType.None)
            {
                currentNode.KeyType = await GetKeyTypeAsync(currentNode.FullPath);
            }

            return currentNode;
        }

        /// <summary>
        /// 递归查找树中第一个匹配指定名称的节点
        /// </summary>
        /// <param name="root">起始节点</param>
        /// <param name="targetName">要查找的节点名称</param>
        /// <param name="comparison">字符串比较方式</param>
        /// <returns>第一个匹配的节点，未找到返回null</returns>
        public async Task<RedisTreeNode> FindTreeNodeFirstByNameAsync(RedisTreeNode root, string targetName, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            // 检查当前节点
            if (root.Name.Equals(targetName, comparison))
            {
                // 如果是叶子节点，确保类型信息是最新的
                if (root.IsLeaf && root.KeyType == RedisKeyType.None)
                {
                    root.KeyType = await GetKeyTypeAsync(root.FullPath);
                }
                return root;
            }

            // 递归检查子节点
            foreach (var child in root.Children)
            {
                var found = await FindTreeNodeFirstByNameAsync(child, targetName, comparison);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// 递归查找树中所有匹配指定名称的节点
        /// </summary>
        /// <param name="root">起始节点</param>
        /// <param name="targetName">要查找的节点名称</param>
        /// <param name="comparison">字符串比较方式</param>
        /// <returns>所有匹配的节点列表</returns>
        public async Task<List<RedisTreeNode>> FindTreeNodesByNameAsync(RedisTreeNode root, string targetName, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var result = new List<RedisTreeNode>();
            await FindTreeNodesByNameInternalAsync(root, targetName, comparison, result);
            return result;
        }

        /// <summary>
        /// 递归查找树中是否存在指定名称的节点
        /// </summary>
        /// <param name="root">起始节点</param>
        /// <param name="targetName">要查找的节点名称</param>
        /// <param name="comparison">字符串比较方式</param>
        /// <returns>是否存在匹配的节点</returns>
        public async Task<bool> ContainsTreeNodeNameAsync(RedisTreeNode root, string targetName, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            // 检查当前节点
            if (root.Name.Equals(targetName, comparison))
            {
                return true;
            }

            // 递归检查子节点
            foreach (var child in root.Children)
            {
                if (await ContainsTreeNodeNameAsync(child, targetName, comparison))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 递归查找树中第一个匹配指定类型的节点
        /// </summary>
        /// <param name="root">起始节点</param>
        /// <param name="targetType">要查找的节点类型</param>
        /// <returns>第一个匹配的节点，未找到返回null</returns>
        public async Task<RedisTreeNode> FindTreeNodeFirstByTypeAsync(RedisTreeNode root, RedisKeyType targetType)
        {
            // 检查当前节点
            if (root.IsLeaf && (await GetKeyTypeAsync(root.FullPath)) == targetType)
            {
                return root;
            }

            // 递归检查子节点
            foreach (var child in root.Children)
            {
                var found = await FindTreeNodeFirstByTypeAsync(child, targetType);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// 递归查找树中所有匹配指定类型的节点
        /// </summary>
        /// <param name="root">起始节点</param>
        /// <param name="targetType">要查找的节点类型</param>
        /// <returns>所有匹配的节点列表</returns>
        public async Task<List<RedisTreeNode>> FindTreeNodesByTypeAsync(RedisTreeNode root, RedisKeyType targetType)
        {
            if (root == null) return new List<RedisTreeNode>();
            var result = new List<RedisTreeNode>();
            await FindTreeNodesByTypeInternalAsync(root, targetType, result);
            return result;
        }

        /// <summary>
        /// 递归查找树中是否存在指定类型的节点
        /// </summary>
        /// <param name="root">起始节点</param>
        /// <param name="targetType">要查找的节点类型</param>
        /// <returns>是否存在匹配的节点</returns>
        public async Task<bool> ContainsTreeNodeTypeAsync(RedisTreeNode root, RedisKeyType targetType)
        {
            // 检查当前节点
            if (root.IsLeaf && (await GetKeyTypeAsync(root.FullPath)) == targetType)
            {
                return true;
            }

            // 递归检查子节点
            foreach (var child in root.Children)
            {
                if (await ContainsTreeNodeTypeAsync(child, targetType))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取树节点对应的值（仅限叶子节点）
        /// </summary>
        /// <param name="node">Redis树节点</param>
        /// <returns>叶子节点的值，非叶子节点返回null</returns>
        public async Task<string> GetTreeNodeValueAsync(RedisTreeNode node)
        {
            if (node == null)
                return null;
            if (!node.IsLeaf)
                return null;
            return await _db.StringGetAsync(node.FullPath);
        }

        /// <summary>
        /// 获取树节点对应的值（泛型版本，支持自动反序列化）
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="node">Redis树节点</param>
        /// <returns>反序列化后的值，非叶子节点返回default(T)</returns>
        public async Task<T> GetTreeNodeValueAsync<T>(RedisTreeNode node)
        {
            if (node == null)
                return default;
            if (!node.IsLeaf)
                return default;
            var value = await _db.StringGetAsync(node.FullPath);
            return JsonSerializer.Deserialize<T>(value);
        }

        private async Task FindTreeNodesByTypeInternalAsync(RedisTreeNode node, RedisKeyType targetType, List<RedisTreeNode> outResult)
        {
            if (node == null) return;
            // 检查当前节点
            if (node.IsLeaf && (await GetKeyTypeAsync(node.FullPath)) == targetType)
            {
                outResult.Add(node);
            }

            // 递归检查子节点
            foreach (var child in node.Children)
            {
                await FindTreeNodesByTypeInternalAsync(child, targetType, outResult);
            }
        }

        private async Task FindTreeNodesByNameInternalAsync(RedisTreeNode node, string targetName, StringComparison comparison, List<RedisTreeNode> outResult)
        {
            // 检查当前节点
            if (node.Name.Equals(targetName, comparison))
            {
                // 如果是叶子节点，确保类型信息是最新的
                if (node.IsLeaf && node.KeyType == RedisKeyType.None)
                {
                    node.KeyType = await GetKeyTypeAsync(node.FullPath);
                }
                outResult.Add(node);
            }

            // 递归检查子节点
            foreach (var child in node.Children)
            {
                await FindTreeNodesByNameInternalAsync(child, targetName, comparison, outResult);
            }
        }

        private async IAsyncEnumerable<string> ScanKeysAsync(IServer server, string pattern)
        {
            const int pageSize = 1000;
            var cursor = 0L;
            do
            {
                var keys = server.KeysAsync(pattern: pattern, pageSize: pageSize, cursor: cursor);
                await foreach (var key in keys)
                {
                    yield return key.ToString();
                }
            } while (cursor != 0);
        }

        private async Task BuildTreeAsync(RedisTreeNode root, string key)
        {
            var parts = key.Split(':');
            var currentNode = root;
            var currentPath = "";

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                currentPath = i == 0 ? part : $"{currentPath}:{part}";
                var isLeaf = i == parts.Length - 1;

                var existingNode = FindOrAddNode(currentNode, part, currentPath, isLeaf);
                currentNode = existingNode;

                // 如果是叶子节点，获取键类型
                if (isLeaf)
                {
                    currentNode.KeyType = await GetKeyTypeAsync(key);
                }
            }
        }

        private async Task<RedisKeyType> GetKeyTypeAsync(string key)
        {
            try
            {
                var type = await _db.ExecuteAsync("TYPE", key);
                return type.ToString() switch
                {
                    "string" => RedisKeyType.String,
                    "hash" => RedisKeyType.Hash,
                    "list" => RedisKeyType.List,
                    "set" => RedisKeyType.Set,
                    "zset" => RedisKeyType.SortedSet,
                    "stream" => RedisKeyType.Stream,
                    _ => RedisKeyType.None
                };
            }
            catch
            {
                return RedisKeyType.None;
            }
        }

        private RedisTreeNode FindOrAddNode(RedisTreeNode parent, string name, string fullPath, bool isLeaf)
        {
            var existingNode = parent.Children.FirstOrDefault(n => n.Name == name);
            if (existingNode == null)
            {
                var newNode = new RedisTreeNode(name, fullPath, isLeaf);
                parent.Children.Add(newNode);
                return newNode;
            }

            // 如果现有节点不是叶子节点但当前是叶子节点，更新状态
            if (!existingNode.IsLeaf && isLeaf)
            {
                existingNode.IsLeaf = true;
                existingNode.FullPath = fullPath;
            }

            return existingNode;
        }
    }

    /// <summary>
    /// Redis树节点类
    /// </summary>
    public class RedisTreeNode
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public bool IsLeaf { get; set; }
        public RedisKeyType KeyType { get; set; }
        public List<RedisTreeNode> Children { get; } = new List<RedisTreeNode>();

        public RedisTreeNode(string name, string fullPath, bool isLeaf = false, RedisKeyType keyType = RedisKeyType.None)
        {
            Name = name;
            FullPath = fullPath;
            IsLeaf = isLeaf;
            KeyType = keyType;
        }
    }

    /// <summary>
    /// Redis键类型枚举
    /// </summary>
    public enum RedisKeyType
    {
        None = 0,     // 未知类型或非叶子节点
        String,       // 字符串类型
        Hash,         // 哈希表类型
        List,         // 列表类型
        Set,          // 集合类型
        SortedSet,    // 有序集合类型
        Stream,       // 流类型
        Module        // 模块类型（如RedisJSON等）
    }
}

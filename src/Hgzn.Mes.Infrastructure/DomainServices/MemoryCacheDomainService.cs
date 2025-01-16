using Hgzn.Mes.Domain.Services;
using Hgzn.Mes.Domain.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.DomainServices
{
    public class MemoryCacheDomainService : IMemoryCacheDomainService
    {
        private readonly ConcurrentDictionary<string, EquipStatus> _cache = new ConcurrentDictionary<string, EquipStatus>();

        public async Task<EquipStatus> GetOrAddAsync(string key, Func<Task<EquipStatus>> valueFactory)
        {
            // 尝试从缓存中获取值
            if (_cache.TryGetValue(key, out var existingValue))
            {
                return existingValue;
            }

            // 如果缓存中没有，则调用 valueFactory 创建新值
            var newValue = await valueFactory();
            _cache[key] = newValue; // 将新值添加到缓存
            return newValue;
        }
    }
}

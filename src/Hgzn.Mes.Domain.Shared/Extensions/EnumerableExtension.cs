using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Shared.Extensions
{
    public static class EnumerableExtension
    {
        public static Task<List<T>> ToListAsync<T>(this IEnumerable<T> sourceList)
        {
            return Task.FromResult(sourceList.ToList());
        }
    }
}

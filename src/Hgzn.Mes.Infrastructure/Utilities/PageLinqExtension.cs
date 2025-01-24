using Hgzn.Mes.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SqlSugar;

namespace Hgzn.Mes.Infrastructure.Utilities
{
    public static class PageLinqExtension
    {
        public static async Task<PaginatedList<TEntity>> ToPaginatedListAsync<TEntity>(this IQueryable<TEntity> entities, int pageIndex, int pageSize)
        {
            var count = await entities.LongCountAsync();
            var query = await entities
            .Skip((pageIndex-1) * pageSize).Take(pageSize).ToArrayAsync();
            return new PaginatedList<TEntity>(query, count, pageIndex, pageSize);
        }
        public static async Task<PaginatedList<TEntity>> ToPaginatedListAsync<TEntity>(this ISugarQueryable<TEntity> entities, int pageIndex, int pageSize)
        {
            var count = await entities.CountAsync();
            var query = await entities
                .Skip((pageIndex-1) * pageSize).Take(pageSize).ToArrayAsync();
            return new PaginatedList<TEntity>(query, count, pageIndex, pageSize);
        }
    }
}
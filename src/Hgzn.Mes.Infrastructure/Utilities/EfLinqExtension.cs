﻿using Hgzn.Mes.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Hgzn.Mes.Infrastructure.Utilities
{
    public static class EfLinqExtension
    {
        public static async Task<PaginatedList<TEntity>> ToPaginatedListAsync<TEntity>(this IQueryable<TEntity> entities, int pageIndex, int pageSize)
        {
            var count = await entities.LongCountAsync();
            var query = await entities
            .Skip(pageIndex * pageIndex).Take(pageSize).ToArrayAsync();
            return new PaginatedList<TEntity>(query, count, pageIndex, pageSize);
        }
    }
}
using Microsoft.EntityFrameworkCore;

namespace Hgzn.Mes.Infrastructure.Utilities
{
    public static class EfExtension
    {
        public static IQueryable<TEntity> WithDeleted<TEntity>(this DbSet<TEntity> entities) where TEntity : class =>
            entities.IgnoreQueryFilters();
    }
}
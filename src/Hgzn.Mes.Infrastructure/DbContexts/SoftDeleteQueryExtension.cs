using Hgzn.Mes.Domain.Entities.System.Base;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using System.Reflection;

namespace Hgzn.Mes.Infrastructure.DbContexts
{
    public static class SoftDeleteQueryExtension
    {
        public static void AddSoftDeleteQueryFilter(
            this IMutableEntityType entityData)
        {
            var methodToCall = typeof(SoftDeleteQueryExtension)
                .GetMethod(nameof(GetSoftDeleteFilter),
                    BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(entityData.ClrType);
            var filter = methodToCall.Invoke(null, Array.Empty<object>());
            entityData.SetQueryFilter((LambdaExpression)filter!);
        }

        private static LambdaExpression GetSoftDeleteFilter<TEntity>()
            where TEntity : class, ISoftDelete
        {
            Expression<Func<TEntity, bool>> filter = x => !x.SoftDeleted;
            return filter;
        }
    }
}
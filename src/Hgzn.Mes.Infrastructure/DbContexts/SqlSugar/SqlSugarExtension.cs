
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Shared.Exceptions;
using Hgzn.Mes.Infrastructure.Utilities.CurrentUser;
using Microsoft.AspNetCore.Http;
using SqlSugar;

namespace Hgzn.Mes.Infrastructure.DbContexts.SqlSugar
{
    public static class SqlSugarExtension
    {
        public async static Task<int> DeleteAsync<TEntity>(
            this ISqlSugarClient context, TEntity entity, IHttpContextAccessor? httpContextAccessor = null)
            where TEntity : AggregateRoot, new()
        {
            if(entity is ICreationAudited && httpContextAccessor is not null)
            {
                var levelPlain = httpContextAccessor?.HttpContext?.User.Claims
                    .FirstOrDefault(c => ClaimType.Level == c.Type);
                var level = levelPlain is null ? -1 : int.Parse(levelPlain!.Value);
                var dataLevel = (entity as ICreationAudited)!.CreatorLevel;
                if (dataLevel < level)
                    throw new ForbiddenException("not allowed to delete this data");
            }
            if (entity is ISoftDelete)
            {
                var target = entity as ISoftDelete;
                target!.SoftDeleted = true;
                target!.DeleteTime = DateTime.Now.ToLocalTime().ToLocalTime();
                return await context.Updateable(entity).ExecuteCommandAsync();
            }
            
            return await context.Deleteable(entity).ExecuteCommandAsync();
        }
    }
}

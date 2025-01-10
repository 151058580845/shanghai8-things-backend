using System.Reflection;
using HgznMes.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HgznMes.Infrastructure.DbContexts
{
    public class InitialDatabase
    {
        public InitialDatabase(
            ApiDbContext apiDbContext,
            ILogger<InitialDatabase> logger
            )
        {
            _apiDbContext = apiDbContext;
            _logger = logger;
           var ss = Assembly.Load("HgznMes." + nameof(Domain));
           ss.GetTypes().Where(t => t.FullName.Contains("123"));
        }

        private readonly ApiDbContext _apiDbContext;
        private readonly ILogger<InitialDatabase> _logger;

        public async Task Initialize()
        {
            if (!await _apiDbContext.Database.EnsureCreatedAsync())
            {
                _logger.LogInformation("database already created");
                //await _apiDbContext.Database.MigrateAsync();
            }
            //var scopes = await _apiDbContext.Scopes
            //    .ToArrayAsync();
            var transaction = await _apiDbContext.Database.BeginTransactionAsync();
            try
            {
                //if (scopes.Length != Scope.Seeds.Length || scopes.Any(ss => Scope.Seeds.Contains(ss)))
                //{
                //    _apiDbContext.Scopes.RemoveRange(scopes);
                //    await _apiDbContext.SaveChangesAsync();
                //    await _apiDbContext.AddRangeAsync(Scope.Seeds);
                //}
                //var count = await _apiDbContext.SaveChangesAsync();
                //await transaction.CommitAsync();
                //_logger.LogInformation($"scope table generate succeed, with {count} new scopes");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
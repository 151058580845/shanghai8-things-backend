using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;

public class InitialDatabaseSugar
{
    private readonly SqlSugarContext _context;
    private readonly ILogger<InitialDatabaseSugar> _logger;
    private readonly DbConnOptions _connOptions;

    public InitialDatabaseSugar(SqlSugarContext context, ILogger<InitialDatabaseSugar> logger, IOptions<DbConnOptions> options)
    {
        _context = context;
        _logger = logger;
        _connOptions = options.Value;
    }

    public Task InitDatabaseAsync()
    {
        throw new NotImplementedException();
    }
}
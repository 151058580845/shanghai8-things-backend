using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlSugar;

namespace Hgzn.Mes.Infrastructure.SqlSugarContext;

public class InitialDatabaseSugar
{
    private readonly SqlSugarContext _context;
    private readonly ILogger<InitialDatabaseSugar> _logger;
    private readonly DbConnOptions _connOptions;

    public InitialDatabaseSugar(SqlSugarContext context, ILogger<InitialDatabaseSugar> logger,IOptions<DbConnOptions> options)
    {
        _context = context;
        _logger = logger;
        _connOptions = options.Value;
    }

    public async Task InitDatabaseAsync()
    {
        if (_connOptions.EnabledCodeFirst)
        {
            _context.InitTables();
        }
    }
}
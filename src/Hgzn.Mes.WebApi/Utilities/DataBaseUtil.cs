using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Microsoft.Extensions.Options;

namespace Hgzn.Mes.WebApi.Utilities;

public class DataBaseUtil
{
    public static void Initalize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<SqlSugarContext>();
        var dbOptions = serviceProvider.GetRequiredService<IOptions<DbConnOptions>>();
        if (dbOptions.Value.EnabledCodeFirst)
            context.InitTables();
    }
}
using Hgzn.Mes.Domain.Entities.System.Audit;
using Hgzn.Mes.Domain.Shared.Exceptions;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Hgzn.Mes.Infrastructure.Utilities.CurrentUser;
using IPTools.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using SqlSugar;

namespace Hgzn.Mes.Infrastructure.Utilities.Filter;

public class OperLogFilterAttribute:ActionFilterAttribute
{
    private ICurrentUser _currentUser;
    private SqlSugarContext _sugarContext;
    private ICurrentPrincipalAccessor _currentPrincipalAccessor;
    

    public OperLogFilterAttribute(ICurrentUser currentUser, SqlSugarContext sqlSugarContext,
        ICurrentPrincipalAccessor currentPrincipalAccessor)
    {
        _currentUser = currentUser;
        _sugarContext = sqlSugarContext;
        _currentPrincipalAccessor = currentPrincipalAccessor;
    }

    public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var resultContext = await next.Invoke();
        _currentPrincipalAccessor.Change(context.HttpContext.User);
        var ip = resultContext.HttpContext.GetClientIp();
        var ipTool = IpTool.Search(ip);
        
        string location = ipTool.Province + "-" + ipTool.City;
        
        var logEntity = new OperatorLog
        {
            Id = Guid.NewGuid(),
            OperIp = ip,
            OperLocation = location,
            RequestMethod = resultContext.HttpContext.Request.Method,
            Method = resultContext.HttpContext.Request.Path.Value,
            OperUser = _currentUser.UserName
        };

        await _sugarContext.DbContext.Insertable(logEntity).ExecuteCommandAsync();
    }
}
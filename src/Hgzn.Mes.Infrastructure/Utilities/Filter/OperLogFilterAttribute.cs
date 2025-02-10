using Hgzn.Mes.Domain.Entities.Audit;
using Hgzn.Mes.Domain.Shared.Exceptions;
using Hgzn.Mes.Domain.Shared.Extensions;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Hgzn.Mes.Infrastructure.Utilities.CurrentUser;
using IPTools.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SqlSugar;

namespace Hgzn.Mes.Infrastructure.Utilities.Filter;

public class OperLogFilterAttribute : ActionFilterAttribute
{
    private ICurrentUser _currentUser;
    private SqlSugarContext _sugarContext;
    private ICurrentPrincipalAccessor _currentPrincipalAccessor;
    private readonly IActionContextAccessor _actionContextAccessor;


    public OperLogFilterAttribute(ICurrentUser currentUser, SqlSugarContext sqlSugarContext,
        ICurrentPrincipalAccessor currentPrincipalAccessor,  IActionContextAccessor actionContextAccessor)
    {
        _currentUser = currentUser;
        _sugarContext = sqlSugarContext;
        _currentPrincipalAccessor = currentPrincipalAccessor;
        _actionContextAccessor = actionContextAccessor;
    }

    public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var resultContext = await next.Invoke();
        _currentPrincipalAccessor.Change(context.HttpContext.User);
        var ip = resultContext.HttpContext.GetClientIp();
        var ipTool = IpTool.Search(ip);

        string location = ipTool.Province + "-" + ipTool.City;
        var pathList = resultContext.HttpContext.Request.Path.Value?.Split("/") ?? [];

        var actionDescriptor = _actionContextAccessor.ActionContext?.ActionDescriptor;
        var authorizeAttribute = actionDescriptor?.EndpointMetadata
            .OfType<AuthorizeAttribute>()
            .FirstOrDefault();

        var permission = authorizeAttribute?.Policy?.Split(":") ?? [];
        var logEntity = new OperatorLog
        {
            Id = Guid.NewGuid(),
            OperIp = ip,
            Title =pathList.Length>2 ? pathList[2] : "",
            OperType =permission.Length>2 ? permission[2] :"",
            OperLocation = location,
            RequestMethod = resultContext.HttpContext.Request.Method,
            Method = resultContext.HttpContext.Request.Path.Value,
            OperUser = _currentUser.UserName,
            RequestParam = await resultContext.HttpContext.GetRequestValue(resultContext.HttpContext.Request.Method),
        };

        
        await _sugarContext.DbContext.Insertable(logEntity).ExecuteCommandAsync();
    }
}
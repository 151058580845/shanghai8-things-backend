using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers;

public class BaseController : ControllerBase
{
    protected ResponseWrapper<T> Success<T>(T t, string info = "", int statusCode = StatusCodes.Status200OK)
    {
        return new ResponseWrapper<T>()
        {
            Info = info,
            Data = t,
            Status = statusCode
        };
    }
}
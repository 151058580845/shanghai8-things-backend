using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Shared.Exceptions;
using Hgzn.Mes.Domain.Shared.Utilities;
using Microsoft.Extensions.Localization;

namespace Hgzn.Mes.WebApi.Exceptions
{
    public static class ExceptionExtension
    {
        public static ExceptionReadDto Localize(this Exception exception, IStringLocalizer stringLocalizer)
        {
            var result = exception switch
            {
                NotFoundException or NotAcceptableException or ForbiddenException =>
                    new ExceptionReadDto() { Info = stringLocalizer[(exception as CustomException)!.ExceptionCode] },
                _ => SettingUtil.IsDevelopment ? new ExceptionReadDto
                {
                    Info = exception.Message.Split("\r\n", StringSplitOptions.TrimEntries)[0],
                    StackTrace = exception.StackTrace?.Split("\r\n", StringSplitOptions.TrimEntries)[0],
                    Inner = exception.InnerException?.Message.Split("\r\n", StringSplitOptions.TrimEntries)[0]
                } : new ExceptionReadDto
                {
                    Info = exception.Message.Split("\r\n", StringSplitOptions.TrimEntries)[0],
                    StackTrace = null,
                    Inner = null
                }
            };
            return result;
        }
    }
}
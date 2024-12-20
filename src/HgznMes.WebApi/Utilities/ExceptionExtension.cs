using HgznMes.Application.Dtos;
using HgznMes.Domain.Shared.Exceptions;
using HgznMes.Domain.Shared.Utilities;
using Microsoft.Extensions.Localization;

namespace HgznMes.WebApi.Exceptions
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
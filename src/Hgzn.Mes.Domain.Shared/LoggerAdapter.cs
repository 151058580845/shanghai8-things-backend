using Microsoft.Extensions.Logging;

namespace Hgzn.Mes.Domain.Shared
{
    public class LoggerAdapter
    {
        public static ILogger<LoggerAdapter> Logger = null!;

        public static void Initialize(ILogger<LoggerAdapter> logger)
        {
            Logger ??= logger;
        }
    }
}
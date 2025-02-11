using Microsoft.Extensions.Logging;

namespace Hgzn.Mes.Domain.Shared
{
    public class LoggerAdapter
    {
        private static ILogger<LoggerAdapter> _logger = null!;

        public static void Initialize(ILogger<LoggerAdapter> logger)
        {
            _logger ??= logger;
        }

        public static void LogWarning(string str) => _logger.LogWarning(str);
        public static void LogInformation(string str) => _logger.LogInformation(str);
        public static void LogError(string str) => _logger.LogError(str);
        public static void LogTrace(string str) => _logger.LogTrace(str);
        public static void LogCritical(string str) => _logger.LogCritical(str);
        public static void LogDebug(string str) => _logger.LogDebug(str);
    }
}
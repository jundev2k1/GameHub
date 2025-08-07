using game_x.application.Contract.Infrastructure.Logger;
using Microsoft.Extensions.Logging;

namespace game_x.infrastructure.logger;

public class LoggerAdapter<T> : IAppLogger<T>
{
    private readonly ILogger<T> _logger;
    private string TypeName { get; set; }

    private const string ServicePrefix = "<game-x.api> →";

    public LoggerAdapter(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<T>();
        TypeName = typeof(T).Name;
    }

    public void LogInformation(string message, params object[] args)
    {
        //_logger.LogInformation(message, args);
        _logger.LogInformation($"{ServicePrefix} [{TypeName}] {message}", args);
    }

    public void LogWarning(string message, params object[] args)
    {
        //_logger.LogWarning(message, args);
        _logger.LogInformation($"{ServicePrefix} [{TypeName}] {message}", args);
    }

    public void LogError(string message, params object[] args)
    {
        //_logger.LogError($"[{TypeName}] {message}", args);
        _logger.LogError($"{ServicePrefix} [{TypeName}] {message}", args);
    }
}

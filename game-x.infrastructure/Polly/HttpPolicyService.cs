using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Polly;
using Polly;
using Polly.Extensions.Http;

namespace game_x.infrastructure.Polly;

public class HttpPolicyService(IAppLogger<HttpPolicyService> logger) : IHttpPolicyService, IServices
{
    public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // 2s, 4s, 8s
                onRetry: (outcome, timespan, retryCount, context) =>
                logger.LogWarning($"[Polly] Retry {retryCount} after {timespan.TotalSeconds}s due to: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}")
            );
    }

}

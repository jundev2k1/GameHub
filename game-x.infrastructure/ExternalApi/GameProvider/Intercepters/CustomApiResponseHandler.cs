using game_x.application.Exceptions;
using game_x.share.ExternalApi.GameProvider.Dtos;
using game_x.share.Helper;
using Microsoft.Extensions.Logging;

namespace game_x.infrastructure.ExternalApi.GameProvider.Intercepters;

public sealed class CustomApiResponseHandler(ILogger<CustomApiResponseHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct = default)
    {
        var response = await base.SendAsync(request, ct);

        if (response.Content == null)
            return response;

        var content = await response.Content.ReadAsStringAsync(ct);
        var isSuccess = JsonHelper.TryParseJson<ResponseBase>(content, out var res);
        if (isSuccess && !res!.IsSuccess)
        {
            logger.LogWarning("API returned error payload: {Payload}\n - Error: {code}\n - Message: {message}", content, res?.ErrorCode, res?.ErrorMessage);
            throw new ExternalServiceException();
        }

        return response;
    }
}

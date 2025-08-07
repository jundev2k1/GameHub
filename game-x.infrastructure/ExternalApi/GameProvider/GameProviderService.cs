using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Exceptions;
using game_x.share.Extensions;
using game_x.share.ExternalApi.GameProvider.Dtos;
using game_x.share.ExternalApi.GameProvider.Dtos.Login;
using Newtonsoft.Json;

namespace game_x.infrastructure.ExternalApi.GameProvider;

public sealed class GameProviderService(
    IAppLogger<GameProviderService> logger,
    IGameProviderApi gameApi,
    IGameAesEncryptor aesEncryptor) : IGameProviderService
{
    public async Task<LoginResponse> LoginAsync(LoginRequest data, string language, string ip)
    {
        try
        {
            logger.LogInformation("Send login request to GameProvider: account = {Accound}, gamecode = {Gamecode}", data.Account, data.Gamecode);

            var json = JsonConvert.SerializeObject(data);
            var request = aesEncryptor.Encrypt(json);
            var bodyPayload = new PayloadRequest
            {
                Data = request,
            };

            var result = await gameApi.LoginAsync(bodyPayload, language, ip);
            if (!result.IsSuccessStatusCode || result.Content == null || !result.Content.IsSuccess)
            {
                var errorDetail = result.Content;
                logger.LogError($"Response failed: Status={result.StatusCode} errorCode={errorDetail?.ErrorCode} message={errorDetail?.ErrorMessage}");

                throw new ExternalServiceException();
            }

            logger.LogInformation("Login request successful，url: {url}", result.Content.Url);
            return result.Content!;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send login request to GameProvider: {Ex}", ex);
            throw new BadRequestException(MessageCode.System.DependencyFailure);
        }
    }
}

using game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Exceptions;
using game_x.share.ExternalApi.GameBaccarat.Dtos.Login;
using System.Text.Json;

namespace game_x.infrastructure.ExternalApi.GameBaccarat;

public sealed class GameBaccaratService(
    IGameBaccaratApi gameApi,
    IAppLogger<GameBaccaratService> logger) : IGameBaccaratService
{
    public async Task<GameBaccaratLoginResponse> LoginAsync(GameBaccaratLoginRequest request)
    {
        try
        {
            logger.LogInformation("Send login request to GameProvider: account = {Accound}, gamecode = {Gamecode}", request.Account, request.Gamecode);

            var result = await gameApi.LoginAsync(request);
            logger.LogInformation(JsonSerializer.Serialize(result));
            if (!result.IsSuccessStatusCode || result.Content == null)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");
                throw new ExternalServiceException();
            }

            var response = result.Content;
            logger.LogInformation(JsonSerializer.Serialize(result.Content));
            if (!response!.Success)
            {
                logger.LogError($"Response failed: Code={response.MessageCode} - Message={response.Message}");
                throw new ExternalServiceException();
            }

            logger.LogInformation("Login request successful，url: {url}", response!.Data!.Url);
            return response!.Data!;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send login request to GameBaccarat: {Ex}", ex);
            throw;
        }
    }
}

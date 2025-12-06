using game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Exceptions;
using game_x.share.ExternalApi.GameBaccarat.Dtos.Login;
using game_x.share.ExternalApi.GameBaccarat.Dtos.Register;

namespace game_x.infrastructure.ExternalApi.GameBaccarat;

public sealed class GameBaccaratService(
    IGameBaccaratApi gameApi,
    IAppLogger<GameBaccaratService> logger) : IGameBaccaratService
{
    public async Task<GameBaccaratLoginResponse> LoginAsync(GameBaccaratLoginRequest request)
    {
        try
        {
            logger.LogInformation("Send login request to Baccarat Platform: account = {Accound}, gamecode = {Gamecode}", request.Account, request.Gamecode);

            var result = await gameApi.LoginAsync(request);
            if (!result.IsSuccessStatusCode || result.Content == null)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");
                throw new ExternalServiceException();
            }

            var response = result.Content;
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

    public async Task<GameBaccaratRegisterResponse> RegisterAsync(GameBaccaratRegisterRequest request)
    {
        try
        {
            logger.LogInformation(
                "Send register request to Baccarat Platform: account = {Accound}, gamecode = {Password}, nickname={Nickname}",
                request.Account,
                request.Password,
                request.Nickname);

            var result = await gameApi.RegisterAsync(request);
            if (!result.IsSuccessStatusCode || result.Content == null)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");
                throw new ExternalServiceException();
            }

            var response = result.Content;
            if (!response!.Success)
            {
                logger.LogError($"Response failed: Code={response.MessageCode} - Message={response.Message}");
                throw new ExternalServiceException();
            }

            logger.LogInformation("Register request successful，UserId: {UserId}", response!.Data!.UserId);
            return response!.Data!;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send register request to GameBaccarat: {Ex}", ex);
            throw;
        }
    }
}

using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Exceptions;
using game_x.share.ExternalApi.GameProvider.Dtos;
using game_x.share.ExternalApi.GameProvider.Dtos.Login;
using game_x.share.ExternalApi.GameProvider.Dtos.Register;
using game_x.share.ExternalApi.GameProvider.Dtos.Wallet;
using Newtonsoft.Json;

namespace game_x.infrastructure.ExternalApi.GameProvider;

public sealed class GameProviderService(
    IAppLogger<GameProviderService> logger,
    IGameProviderApi gameApi,
    IGameAesEncryptor aesEncryptor,
    IGameProviderCacheService gameProviderCache) : IGameProviderService
{
    public async Task<LoginResponse> LoginAsync(LoginRequest data, string ip)
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

            var result = await gameApi.LoginAsync(bodyPayload, gameProviderCache.Language, ip);
            if (!result.IsSuccessStatusCode || result.Content == null)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");

                throw new ExternalServiceException();
            }

            var resJson = aesEncryptor.Decrypt(result.Content.Data);
            var response = JsonConvert.DeserializeObject<LoginResponse>(resJson);
            if (!response!.IsSuccess)
            {
                logger.LogError($"Response failed: Code={response.ErrorCode} - Message={response.ErrorMessage}");
                throw new ExternalServiceException();
            }

            logger.LogInformation("Login request successful，url: {url}", response!.Url);
            return response!;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send login request to GameProvider: {Ex}", ex);
            throw;
        }
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest data)
    {
        try
        {
            logger.LogInformation("Send register request to GameProvider: account = {Accound}", data.Account);

            var json = JsonConvert.SerializeObject(data);
            var request = aesEncryptor.Encrypt(json);
            var bodyPayload = new PayloadRequest
            {
                Data = request,
            };

            var result = await gameApi.RegisterAsync(bodyPayload, gameProviderCache.Language);
            if (!result.IsSuccessStatusCode || result.Content == null)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");
                throw new ExternalServiceException();
            }

            var resJson = aesEncryptor.Decrypt(result.Content.Data);
            var response = JsonConvert.DeserializeObject<RegisterResponse>(resJson);
            if (!response!.IsSuccess)
            {
                logger.LogError($"Response failed: Code={response.ErrorCode} - Message={response.ErrorMessage}");
                throw new ExternalServiceException();
            }

            logger.LogInformation("Register request successful，url: {id}", response!.Id.ToString());
            return response!;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send register request to GameProvider: {Ex}", ex);
            throw;
        }
    }

    public async Task<WalletResponse> GetWalletAsync(WalletRequest data)
    {
        try
        {
            logger.LogInformation("Send get wallet request to GameProvider: account = {Accound}", data.Account);

            var json = JsonConvert.SerializeObject(data);
            var request = aesEncryptor.Encrypt(json);
            var bodyPayload = new PayloadRequest
            {
                Data = request,
            };

            var result = await gameApi.GetWalletAsync(bodyPayload, gameProviderCache.Language);
            if (!result.IsSuccessStatusCode || result.Content == null)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");
                throw new ExternalServiceException();
            }

            var resJson = aesEncryptor.Decrypt(result.Content.Data);
            var response = JsonConvert.DeserializeObject<WalletResponse>(resJson);
            if (!response!.IsSuccess)
            {
                logger.LogError($"Response failed: Code={response.ErrorCode} - Message={response.ErrorMessage}");
                throw new ExternalServiceException();
            }

            logger.LogInformation("Get wallet request successful，Quota: {quota}, Currency: {currency}", response.Quota.ToString(), response.Currency);
            return response!;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send get wallet request to GameProvider: {Ex}", ex);
            throw;
        }
    }
}

using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Exceptions;
using game_x.share.ExternalApi.GameProvider.Dtos;
using game_x.share.ExternalApi.GameProvider.Dtos.Deposit;
using game_x.share.ExternalApi.GameProvider.Dtos.Login;
using game_x.share.ExternalApi.GameProvider.Dtos.Logout;
using game_x.share.ExternalApi.GameProvider.Dtos.Register;
using game_x.share.ExternalApi.GameProvider.Dtos.Report;
using game_x.share.ExternalApi.GameProvider.Dtos.Wallet;
using game_x.share.ExternalApi.GameProvider.Dtos.Withdrawal;
using Newtonsoft.Json;

namespace game_x.infrastructure.ExternalApi.GameProvider;

public sealed class GameProviderService(
    IAppLogger<GameProviderService> logger,
    IGameProviderApi gameApi,
    IGameAesEncryptor aesEncryptor,
    IGameProviderCacheService gameProviderCache) : IGameProviderService
{
    public async Task<GameLoginResponse> LoginAsync(GameLoginRequest data, string ip)
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

            var result = await gameApi.LoginAsync(bodyPayload, gameProviderCache.GetLanguage(data.Account), ip);
            if (!result.IsSuccessStatusCode || result.Content == null)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");

                throw new ExternalServiceException();
            }

            var resJson = aesEncryptor.Decrypt(result.Content.Data);
            var response = JsonConvert.DeserializeObject<GameLoginResponse>(resJson);
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

    public async Task<GameLogoutResponse> LogoutAsync(GameLogoutRequest data)
    {
        try
        {
            var json = JsonConvert.SerializeObject(data);
            var request = aesEncryptor.Encrypt(json);
            var bodyPayload = new PayloadRequest
            {
                Data = request,
            };

            var result = await gameApi.LogoutAsync(bodyPayload, gameProviderCache.GetLanguage(data.Account));
            if (!result.IsSuccessStatusCode || result.Content == null)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");
                throw new ExternalServiceException();
            }

            var resJson = aesEncryptor.Decrypt(result.Content.Data);
            logger.LogInformation("Full logout response: {response}", resJson);

            var logoutResponse = JsonConvert.DeserializeObject<GameLogoutResponse>(resJson);
            if (logoutResponse == null)
            {
                logger.LogError("Failed to deserialize logout response");
                throw new ExternalServiceException();
            }

            if (!logoutResponse.IsSuccess)
            {
                logger.LogError("Logout response failed: Code={ErrorCode} - Message={ErrorMessage}",
                    logoutResponse.ErrorCode ?? "Unknown",
                    logoutResponse.ErrorMessage ?? "Unknown error");
            }
            else
            {
                logger.LogInformation("Logout request successful");
            }

            return logoutResponse;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send deposit request to GameProvider: {Ex}", ex);
            throw;
        }
    }

    public async Task<GameRegisterResponse> RegisterAsync(GameRegisterRequest data)
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

            var result = await gameApi.RegisterAsync(bodyPayload, gameProviderCache.GetLanguage(data.Account));
            if (!result.IsSuccessStatusCode || result.Content == null)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");
                throw new ExternalServiceException();
            }

            var resJson = aesEncryptor.Decrypt(result.Content.Data);
            var response = JsonConvert.DeserializeObject<GameRegisterResponse>(resJson);
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

    public async Task<GameWalletResponse> GetWalletAsync(GameWalletRequest data)
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

            var result = await gameApi.GetWalletAsync(bodyPayload, gameProviderCache.GetLanguage(data.Account));
            if (!result.IsSuccessStatusCode || result.Content == null)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");
                throw new ExternalServiceException();
            }

            var resJson = aesEncryptor.Decrypt(result.Content.Data);
            logger.LogInformation("Full wallet response: {response}", resJson);
            var response = JsonConvert.DeserializeObject<GameWalletResponse>(resJson);
            if (!response!.IsSuccess)
            {
                logger.LogError("Response failed from GameProvider");
                throw new ExternalServiceException();
            }

            logger.LogInformation("Get wallet request successful，Quota: {quota}, Currency: {currency}",
                response.Data.Quota.ToString(),
                response.Data.Currency);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send get wallet request to GameProvider: {Ex}", ex);
            throw;
        }
    }

    public async Task<GameDepositResponse> DepositWalletAsync(GameDepositRequest data)
    {
        try
        {
            logger.LogInformation("Send deposit request to GameProvider: account = {Account}, quota = {Quota}, sno = {Sno}", data.Account, data.Quota, data.Sno);

            var json = JsonConvert.SerializeObject(data);
            var request = aesEncryptor.Encrypt(json);
            var bodyPayload = new PayloadRequest
            {
                Data = request,
            };

            var result = await gameApi.DepositAsync(bodyPayload, gameProviderCache.GetLanguage(data.Account));
            if (!result.IsSuccessStatusCode || result.Content == null)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");
                throw new ExternalServiceException();
            }

            var resJson = aesEncryptor.Decrypt(result.Content.Data);
            logger.LogInformation("Full deposit response: {response}", resJson);

            var gameDepositResponse = JsonConvert.DeserializeObject<GameDepositResponse>(resJson);
            if (gameDepositResponse == null)
            {
                logger.LogError("Failed to deserialize deposit response");
                throw new ExternalServiceException();
            }

            if (!gameDepositResponse.IsSuccess)
            {
                logger.LogError("Deposit response failed: Code={ErrorCode} - Message={ErrorMessage}",
                    gameDepositResponse.ErrorCode ?? "Unknown",
                    gameDepositResponse.ErrorMessage ?? "Unknown error");
            }
            else
            {
                logger.LogInformation("Deposit request successful");
            }

            return gameDepositResponse;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send deposit request to GameProvider: {Ex}", ex);
            throw;
        }
    }

    public async Task<GameWithdrawalResponse> WithdrawalWalletAsync(GameWithdrawalRequest data)
    {
        try
        {
            logger.LogInformation("Send withdrawal request to GameProvider: account = {Account}, quota = {Quota}, sno = {Sno}", data.Account, data.Quota, data.Sno);

            var json = JsonConvert.SerializeObject(data);
            var request = aesEncryptor.Encrypt(json);
            var bodyPayload = new PayloadRequest
            {
                Data = request,
            };

            var result = await gameApi.WithdrawalAsync(bodyPayload, gameProviderCache.GetLanguage(data.Account));
            if (!result.IsSuccessStatusCode || result.Content == null)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");
                throw new ExternalServiceException();
            }

            var resJson = aesEncryptor.Decrypt(result.Content.Data);
            logger.LogInformation("Full withdrawal response: {response}", resJson);

            var gameWithdrawalResponse = JsonConvert.DeserializeObject<GameWithdrawalResponse>(resJson);
            if (gameWithdrawalResponse == null)
            {
                logger.LogError("Failed to deserialize withdrawal response");
                throw new ExternalServiceException();
            }

            if (!gameWithdrawalResponse.IsSuccess)
            {
                logger.LogError("Withdrawal response failed: Code={ErrorCode} - Message={ErrorMessage}",
                    gameWithdrawalResponse.ErrorCode ?? "Unknown",
                    gameWithdrawalResponse.ErrorMessage ?? "Unknown error");
            }
            else
            {
                logger.LogInformation("Withdrawal request successful");
            }

            return gameWithdrawalResponse;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send withdrawal request to GameProvider: {Ex}", ex);
            throw;
        }
    }

    public async Task<GameReportResponse> GetReportAsync(GameReportRequest data, string account)
    {
        try
        {
            logger.LogInformation("Send Report request to GameProvider: start date = {StartDate}, end date = {EndDate}", data.StartDate, data.EndDate);

            var json = JsonConvert.SerializeObject(data);
            var request = aesEncryptor.Encrypt(json);
            var bodyPayload = new PayloadRequest
            {
                Data = request,
            };

            var result = await gameApi.ReportAsync(bodyPayload, gameProviderCache.GetLanguage(account));
            if (!result.IsSuccessStatusCode || result.Content == null)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");
                throw new ExternalServiceException();
            }

            var resJson = aesEncryptor.Decrypt(result.Content.Data);
            logger.LogInformation("Full withdrawal response: {response}", resJson);

            var gameWithdrawalResponse = JsonConvert.DeserializeObject<GameReportResponse>(resJson);
            if (gameWithdrawalResponse == null)
            {
                logger.LogError("Failed to deserialize withdrawal response");
                throw new ExternalServiceException();
            }

            if (!gameWithdrawalResponse.IsSuccess)
            {
                logger.LogError("Withdrawal response failed: Code={ErrorCode} - Message={ErrorMessage}",
                    gameWithdrawalResponse.ErrorCode ?? "Unknown",
                    gameWithdrawalResponse.ErrorMessage ?? "Unknown error");
            }
            else
            {
                logger.LogInformation("Withdrawal request successful");
            }

            return gameWithdrawalResponse;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send withdrawal request to GameProvider: {Ex}", ex);
            throw;
        }
    }
}

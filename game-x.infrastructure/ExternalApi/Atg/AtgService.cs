using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.Atg;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Exceptions;
using game_x.share.ExternalApi.Atg.Dtos.GameProvider;
using game_x.share.ExternalApi.Atg.Dtos.GetGameBalance;
using game_x.share.ExternalApi.Atg.Dtos.GetGames;
using game_x.share.ExternalApi.Atg.Dtos.Register;
using game_x.share.ExternalApi.Atg.Dtos.UpdateGameBalance;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.infrastructure.ExternalApi.Atg;

public class AtgService(
    IAtgApi gameApi,
    IAppLogger<AtgService> logger,
    IOptions<AtgSettings> settings,
    IGameProviderCacheService cacheService) : IAtgService
{
    private readonly int _providerId = settings.Value.ProviderId;
    private async Task<string> GetAccessTokenAsync()
    {
        try
        {
            var (token, expiredTime) = cacheService.GetProviderToken(GameConstants.PLATFORM_ID_ATG);
            if (token == null || expiredTime == null || expiredTime < DateTime.UtcNow)
            {
                logger.LogInformation("Send getting accessToken request to ATG Platform.");
            
                var result = await gameApi.GetAccessTokenAsync(
                    xOperator: settings.Value.XOperator,
                    xKey: settings.Value.XOperatorKey);
                var content = result.Content;
            
                if (content == null || !result.IsSuccessful || result.Content == null)
                {
                    logger.LogError($"Response failed: ErrorCode={result.Content}, ErrorMessage={result.Error?.Message}");
                    throw new ExternalServiceException();
                }
            
                cacheService.SetProviderToken(GameConstants.PLATFORM_ID_ATG, content.Data.Token, DateTime.UtcNow.AddMinutes(9));
                return content.Data.Token;
            }
            return token;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send get accessToken request to Atg Platform: {Ex}", ex);
            throw;
        }
    }
    
    public async Task<bool> RegisterAsync(AtgRegisterRequest request)
    {
        try
        {
            logger.LogInformation("Send registering request to ATG Platform.");

            var token = await GetAccessTokenAsync();
            var result = await gameApi.RegisterAsync(
                token: token,
                payload: request);
      
            if (!result.IsSuccessful || result.Content == null)
            {
                logger.LogError($"Response failed: ErrorCode={result.StatusCode}, ErrorMessage={result.Error?.Content}");
                throw new ExternalServiceException();
            }
            
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send get accessToken request to Atg Platform: {Ex}", ex);
            throw;
        }
    }
    
    public async Task<GameProviderResponse> GetGameProvidersAsync()
    {
        try
        {
            logger.LogInformation("Send retrieving game provider request to ATG Platform.");

            var token = await GetAccessTokenAsync();
            var result = await gameApi.GetGameProvidersAsync(token: token);
            var content = result.Content?.Data;
            
            if (content == null || !result.IsSuccessful || result.Content == null)
            {
                logger.LogError($"Response failed: ErrorCode={result.Content}, ErrorMessage={result.Error?.Message}");
                throw new ExternalServiceException();
            }
            
            return content;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send retrieving game provider request to Atg Platform: {Ex}", ex);
            throw;
        }
    }
    
    public async Task<GetGameBalanceResponse> GetGameBalanceAsync(string username)
    {
        try
        {
            logger.LogInformation("Send retrieving game balance request to ATG Platform.");

            var token = await GetAccessTokenAsync();
            var result = await gameApi.GetGameBalanceAsync(
                token: token,
                providerId: _providerId,
                username: username);
            var content = result.Content?.Data;
            
            if (content == null || !result.IsSuccessful || result.Content == null)
            {
                logger.LogError($"Response failed: ErrorCode={result.Content}, ErrorMessage={result.Error?.Message}");
                throw new ExternalServiceException();
            }
            
            return content;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send retrieving game balance request to Atg Platform: {Ex}", ex);
            throw;
        }
    }
    
    public async Task<GetGameBalanceResponse> UpdateGameBalanceAsync(
        string username,
        decimal balance,
        string action,
        string transferId) 
    {
        try
        {
            logger.LogInformation("Send updating game balance request to ATG Platform.");

            var token = await GetAccessTokenAsync();
            var payload = new UpdateGameBalanceRequest{
                Username = username,
                Balance = balance,
                Action = action,
                TransferId = transferId,
            };
            var result = await gameApi.UpdateGameBalanceAsync(
                token: token,
                providerId: _providerId,
                payload: payload);
            var content = result.Content?.Data;
            
            if (content == null || !result.IsSuccessful || result.Content == null)
            {
                logger.LogError($"Response failed: ErrorCode={result.Content}, ErrorMessage={result.Error?.Message}");
                throw new ExternalServiceException();
            }
            
            return content;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send updating game balance request to Atg Platform: {Ex}", ex);
            throw;
        }
    }
    
    public async Task<ICollection<GameItem>> GetGamesAsync()
    {
        try
        {
            logger.LogInformation("Send retrieving game provider request to ATG Platform.");

            var token = await GetAccessTokenAsync();
            var result = await gameApi.GetGamesAsync(
                token: token,
                provider: _providerId
                );
            var content = result.Content?.Data;
            
            if (content == null || !result.IsSuccessful || result.Content == null)
            {
                logger.LogError($"Response failed: ErrorCode={result.Content}, ErrorMessage={result.Error?.Message}");
                throw new ExternalServiceException();
            }
            
            return content.Games;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send retrieving game provider request to Atg Platform: {Ex}", ex);
            throw;
        }
    }
    
    public async Task<string> PlayGameAsync(string username, string gameCode, string? fullname)
    {
        try
        {
            logger.LogInformation("Send retrieving game key request to ATG Platform.");

            var key = await GetGameKeyAsync(username, gameCode, fullname);
            var token = await GetAccessTokenAsync();
            var result = await gameApi.PlayGameAsync(
                token: token,
                providerId: _providerId,
                key: key
            );
            var content = result.Content?.Data;
            
            if (content == null || !result.IsSuccessful || result.Content == null)
            {
                logger.LogError($"Response failed: ErrorCode={result.Content}, ErrorMessage={result.Error?.Message}");
                throw new ExternalServiceException();
            }
            
            return content.Url;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send retrieving game key request to Atg Platform: {Ex}", ex);
            throw;
        }
    }
    
    private async Task<string> GetGameKeyAsync(string username, string gameCode, string? fullname)
    {
        try
        {
            logger.LogInformation("Send retrieving game key request to ATG Platform.");

            var token = await GetAccessTokenAsync();
            var result = await gameApi.GetGameKeyAsync(
                token: token,
                username: username,
                providerId: _providerId,
                gameCode: gameCode,
                fullname: fullname
            );
            var content = result.Content?.Data;
            
            if (content == null || !result.IsSuccessful || result.Content == null)
            {
                logger.LogError($"Response failed: ErrorCode={result.Content}, ErrorMessage={result.Error?.Message}");
                throw new ExternalServiceException();
            }
            
            return content.Key;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send retrieving game key request to Atg Platform: {Ex}", ex);
            throw;
        }
    }
}
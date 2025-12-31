using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.SasSlot;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Exceptions;
using game_x.share.Extensions;
using game_x.share.ExternalApi.SasSlot.Dtos.Login;
using game_x.share.Settings;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace game_x.infrastructure.ExternalApi.SasSlot;

public sealed class SasSlotService(
    IOptions<GameSlotSettings> settings,
    ISasSlotApi gameApi,
    IAsymmetricCryptoService asymmetricCryptoService,
    IAsymmetricKeyCacheService asymmetricKeyCacheService,
    IAppLogger<SasSlotService> logger) : ISasSlotService
{
    private const string DefaultSignatureAlg = "ES256";
    private const string DefaultKeyId = "nmy-2025-12";

    public async Task<string> LoginAsync(string account, string nickname)
    {
        var request = new SasSlotLoginRequest
        {
            PlatformCode = settings.Value.Code,
            ExtUserId = account,
            Nickname = nickname,
            Nonce = Guid.NewGuid().ToString(),
        };
        var signature = Sign(request);
        try
        {
            logger.LogInformation("Send login request to SAS Slot: account = {Account}, nickname = {nickname}", request.ExtUserId, request.Nickname);

            var result = await gameApi.LoginAsync(
                request,
                signature,
                DefaultSignatureAlg,
                DefaultKeyId);
            if (!result.IsSuccessStatusCode || result.Content == null)
            {
                logger.LogError($"Response failed: Status={result.StatusCode} | {result.ReasonPhrase ?? string.Empty} | {result.RequestMessage?.RequestUri?.ToStringOrEmpty()} | " + result.Content != null ? JsonSerializer.Serialize(result.Content) : string.Empty);
                throw new ExternalServiceException();
            }

            var response = result.Content;

            logger.LogInformation("Login request successful，url: {url}, launchUrl: {launchUrl}", response.Url, response.LaunchUrl);
            return response.Url.IsNullOrWhiteSpace()
                ? response.LaunchUrl
                : response.Url;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send login request to SAS Slot: {Ex}", ex);
            throw;
        }
    }

    public async Task RegisterAsync(string account, string nickname)
    {
        // TODO (SAS Slot): add the SAS Slot's logic to register external member
        await Task.CompletedTask;
    }

    public async Task<decimal> GetWalletAsync(string account)
    {
        // TODO (SAS Slot): add the SAS Slot's logic to get wallet
        await Task.CompletedTask;
        return 0;
    }

    private string Sign<T>(T request) where T : class
    {
        var GameXPrivateKey = asymmetricKeyCacheService.GameXPrivateKey;
        var signature = asymmetricCryptoService.Sign(GameXPrivateKey, request);
        return signature;
    }
}

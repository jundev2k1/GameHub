using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.SasSlot;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Exceptions;
using game_x.share.Extensions;
using game_x.share.ExternalApi.SasSlot.Dtos.DeletePublicKey;
using game_x.share.ExternalApi.SasSlot.Dtos.Deposit;
using game_x.share.ExternalApi.SasSlot.Dtos.GetWallet;
using game_x.share.ExternalApi.SasSlot.Dtos.Login;
using game_x.share.ExternalApi.SasSlot.Dtos.RegisterPublicKey;
using game_x.share.ExternalApi.SasSlot.Dtos.Withdrawal;
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
            Ts = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        };
        try
        {
            var signature = Sign(request);
            logger.LogInformation("Send login request to SAS Slot: account = {Account}, nickname = {nickname}", request.ExtUserId, request.Nickname);

            var result = await gameApi.LoginAsync(
                request,
                signature,
                DefaultSignatureAlg,
                DefaultKeyId);
            if (!result.IsSuccessStatusCode || result.Content == null)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");
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
        var request = new SasSlotGetWalletRequest
        {
            PlatformCode = settings.Value.Code,
            ExtUserId = account,
            Nonce = Guid.NewGuid().ToString(),
            Ts = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        };
        try
        {
            var signature = Sign(request);
            logger.LogInformation("Send the get wallet request to SAS Slot: account = {Account}", request.ExtUserId);

            var result = await gameApi.GetWalletAsync(
                request,
                signature,
                DefaultSignatureAlg,
                DefaultKeyId);
            if (!result.IsSuccessStatusCode || result.Content == null)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");
                throw new ExternalServiceException();
            }

            var response = result.Content;

            logger.LogInformation("Get wallet request successful，data: {wallet}", JsonSerializer.Serialize(response));
            return response.TotalBalance;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send the get wallet request to SAS Slot: {Ex}", ex);
            throw;
        }
    }

    public async Task DepositAsync(string account, decimal amount, string sno)
    {
        var request = new SasSlotDepositRequest
        {
            PlatformCode = settings.Value.Code,
            ExtUserId = account,
            RefId = sno,
            Amount = amount,
            IsPromo = false,
            Nonce = Guid.NewGuid().ToString(),
            Ts = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        };
        try
        {
            var signature = Sign(request);
            logger.LogInformation("Send deposit request to SAS Slot: account = {Account}, Sno = {refId}", request.ExtUserId, request.RefId);

            var result = await gameApi.DepositAsync(
                request,
                signature,
                DefaultSignatureAlg,
                DefaultKeyId);
            if (!result.IsSuccessStatusCode)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");
                throw new ExternalServiceException();
            }

            logger.LogInformation("Deposit request successful");
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send deposit request to SAS Slot: {Ex}", ex);
            throw;
        }
    }

    public async Task WithdrawalAsync(string account, decimal amount, string sno)
    {
        var request = new SasSlotWithdrawalRequest
        {
            PlatformCode = settings.Value.Code,
            ExtUserId = account,
            RefId = sno,
            Amount = amount,
            IsPromo = false,
            Nonce = Guid.NewGuid().ToString(),
            Ts = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        };
        try
        {
            var signature = Sign(request);
            logger.LogInformation("Send withdrawal request to SAS Slot: account = {Account}, Sno = {refId}", request.ExtUserId, request.RefId);

            var result = await gameApi.WithdrawalAsync(
                request,
                signature,
                DefaultSignatureAlg,
                DefaultKeyId);
            if (!result.IsSuccessStatusCode)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");
                throw new ExternalServiceException();
            }

            logger.LogInformation("Withdrawal request successful");
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send withdrawal request to SAS Slot: {Ex}", ex);
            throw;
        }
    }

    public async Task RegisterPublicKeyAsync()
    {
        var request = new SasSlotRegisterPublicKeyRequest
        {
            PlatformCode = settings.Value.Code,
            KeyId = DefaultKeyId,
            PublicKeyPem = asymmetricKeyCacheService.GameXPublicKey,
            Nonce = Guid.NewGuid().ToString(),
            Ts = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        };
        try
        {
            var signature = Sign(request);
            logger.LogInformation("Send register public key request to SAS Slot.");

            var result = await gameApi.RegisterPublicKeyAsync(
                request,
                signature,
                DefaultSignatureAlg,
                DefaultKeyId);
            if (!result.IsSuccessStatusCode)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");
                throw new ExternalServiceException();
            }

            logger.LogInformation("Register public key request successful");
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send the register public key request to SAS Slot: {Ex}", ex);
            throw;
        }
    }

    public async Task DeletePublicKeyAsync()
    {
        var request = new SasSlotDeletePublicKeyRequest
        {
            PlatformCode = settings.Value.Code,
            KeyId = DefaultKeyId,
            Nonce = Guid.NewGuid().ToString(),
            Ts = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        };
        try
        {
            var signature = Sign(request);
            logger.LogInformation("Send delete public key request to SAS Slot.");

            var result = await gameApi.DeletePublicKeyAsync(
                request,
                signature,
                DefaultSignatureAlg,
                DefaultKeyId);
            if (!result.IsSuccessStatusCode)
            {
                logger.LogError($"Response failed: Status={result.StatusCode}");
                throw new ExternalServiceException();
            }

            logger.LogInformation("Delete public key request successful");
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send the delete public key request to SAS Slot: {Ex}", ex);
            throw;
        }
    }

    private string Sign<T>(T request) where T : class
    {
        var GameXPrivateKey = asymmetricKeyCacheService.GameXPrivateKey;
        var signature = asymmetricCryptoService.Sign(GameXPrivateKey, request);
        return signature;
    }
}

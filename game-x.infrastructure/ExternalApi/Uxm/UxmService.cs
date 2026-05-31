using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Exceptions;
using game_x.share.ExternalApi.Base;
using game_x.share.ExternalApi.Uxm.Dtos;
using game_x.share.ExternalApi.Uxm.Dtos.ApiRequests.Deposit;
using game_x.share.ExternalApi.Uxm.Dtos.ApiRequests.Withdrawal;
using game_x.share.ExternalApi.Uxm.Enums;
using game_x.share.Helper;
using Refit;

namespace game_x.infrastructure.ExternalApi.Uxm;

public sealed class UxmService(
    IAppLogger<UxmService> logger,
    IUxmApi uxmApi,
    IAsymmetricKeyCacheService asymmetricKeyCache,
    IAppSettingCacheService appSettingCache,
    IAsymmetricCryptoService asymmetricCrypto) : IUxmService
{
    private static readonly int[] InsufficientErrorCodes =
    [
        (int)UxmErrorCode.InsufficientMerchantBalance,
        (int)UxmErrorCode.InsufficientWalletBalance
    ];

    public async Task<UxmWithdrawalResponse> WithdrawalAsync(
        decimal amount,
        string orderNumber,
        string to,
        string? remark)
    {
        var payload = new UxmWithdrawalRequest(
            appSettingCache.UxmMerchantNumber,
            amount,
            orderNumber,
            to,
            remark);
        logger.LogInformation($@"
            Send withdrawal request to UXM:
            MerchantNumber={payload.MerchantNumber},
            To = {payload.To},
            Amount = {payload.Amount},
            OrderNumber = {payload.OrderNumber}");

        try
        {
            var request = new SecureRequest<UxmWithdrawalRequest>
            {
                Data = payload,
                Signature = asymmetricCrypto.Sign(asymmetricKeyCache.GameXPrivateKey, payload)
            };

            var response = await uxmApi.WithdrawalAsync(request);
            var content = ValidateApiResponse(response);

            // Verify UXM signature
            var isValid = asymmetricCrypto.VerifySignature(asymmetricKeyCache.UxmPublicKey, content.Data, content.Signature);
            if (!isValid) throw new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");

            logger.LogInformation("Withdrawal request successful，OrderUid: {OrderUid}", content.Data.OrderUid);
            return content.Data;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send withdrawal request to UXM: {Ex}", ex);
            throw;
        }
    }

    public async Task<UxmDepositResponse> DepositAsync(
        decimal amount,
        string orderNumber,
        string userId,
        string remark)
    {
        var payload = new UxmDepositRequest(
            appSettingCache.UxmMerchantNumber,
            amount,
            orderNumber,
            userId,
            remark);
        logger.LogInformation($@"
            Send deposit request to UXM:
            MerchantNumber={payload.MerchantNumber},
            UserId={payload.UserId},
            Amount = {payload.Amount},
            OrderNumber = {payload.OrderNumber}");

        try
        {
            var request = new SecureRequest<UxmDepositRequest>
            {
                Data = payload,
                Signature = asymmetricCrypto.Sign(asymmetricKeyCache.GameXPrivateKey, payload)
            };
            var response = await uxmApi.DepositAsync(request);
            var content = ValidateApiResponse(response);

            // Verify UXM signature
            var isValid = asymmetricCrypto.VerifySignature(asymmetricKeyCache.UxmPublicKey, content.Data, content.Signature);
            if (!isValid) throw new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");

            logger.LogInformation("Deposit request successful，OrderUid: {OrderUid}", content.Data.OrderUid);
            return content.Data;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send deposit request to UXM: {Ex}", ex);
            throw;
        }
    }

    private SecureResponse<T> ValidateApiResponse<T>(ApiResponse<SecureResponse<T>> response)
    {
        // Check if the request was successful
        if (response.IsSuccessStatusCode && response.Content != null)
            return response.Content;

        // Handle logging and throwing error
        var apiEx = response.Error;
        logger.LogError(
            apiEx ?? new ExternalServiceException("UXM failed") as Exception,
            "UXM failed. Status={Status}, Reason={Reason}, ErrorMessage={ErrorMessage}, ErrorContent={ErrorContent}",
            response.StatusCode,
            apiEx?.ReasonPhrase ?? response.ReasonPhrase ?? string.Empty,
            apiEx?.Message ?? string.Empty,
            apiEx?.Content ?? string.Empty);

        // Wrapping and throwing UXM exception
        throw GetUxmErrors(response);
    }

    private static Exception GetUxmErrors<T>(ApiResponse<SecureResponse<T>> response)
    {
        var errorContent = JsonHelper.ConvertJson<UxmErrorResponse>(response.Error?.Content ?? "{}");

        // Check if this is an insufficient balance error
        if (InsufficientErrorCodes.Contains(errorContent.ErrorCode))
            return new BadRequestException(MessageCode.Accounting.InsufficientBalance, new { message = response.Error?.Content });

        // Fallback
        return new ExternalServiceException("UXM failed");
    }
}

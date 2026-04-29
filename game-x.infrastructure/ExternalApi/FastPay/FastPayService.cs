using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.FastPay;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Exceptions;
using game_x.share.ExternalApi.Base;
using game_x.share.ExternalApi.FastPay.Dtos;
using game_x.share.ExternalApi.FastPay.Dtos.ApiRequests.Deposit;
using game_x.share.ExternalApi.FastPay.Dtos.ApiRequests.Withdrawal;
using game_x.share.ExternalApi.FastPay.Enums;
using game_x.share.Helper;
using Refit;

namespace game_x.infrastructure.ExternalApi.FastPay;

public sealed class FastPayService(
    IAppLogger<FastPayService> logger,
    IFastPayApi fastPayApi,
    IAppSettingCacheService appSettingCache,
    IAsymmetricKeyCacheService asymmetricKeyCache,
    IAsymmetricCryptoService asymmetricCrypto) : IFastPayService
{
    private static readonly int[] InsufficientErrorCodes =
    [
        (int)FastPayErrorCode.InsufficientMerchantBalance,
        (int)FastPayErrorCode.InsufficientWalletBalance
    ];

    public async Task<FastPayDepositResponse> DepositAsync(
        decimal amount,
        string orderNumber,
        string userId,
        string remark)
    {
        var payload = new FastPayDepositRequest(
            appSettingCache.FastPayMerchantNumber,
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
            var request = new SecureRequest<FastPayDepositRequest>
            {
                Data = payload,
                Signature = asymmetricCrypto.Sign(asymmetricKeyCache.GameXPrivateKey, payload)
            };
            var response = await fastPayApi.DepositAsync(request);
            var content = ValidateApiResponse(response);

            // Verify Fast Pay signature
            var isValid = asymmetricCrypto.VerifySignature(asymmetricKeyCache.FastPayPublicKey, content.Data, content.Signature);
            if (!isValid) throw new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");

            logger.LogInformation("Deposit request successful，OrderUid: {OrderUid}", content.Data.OrderUid);
            return content.Data;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send deposit request to Fast Pay: {Ex}", ex);
            throw;
        }
    }

    public async Task<FastPayWithdrawalResponse> WithdrawalAsync(
        decimal amount,
        string orderNumber,
        string to,
        string? remark)
    {
        var payload = new FastPayWithdrawalRequest(
            appSettingCache.FastPayMerchantNumber,
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
            var request = new SecureRequest<FastPayWithdrawalRequest>
            {
                Data = payload,
                Signature = asymmetricCrypto.Sign(asymmetricKeyCache.GameXPrivateKey, payload)
            };

            var response = await fastPayApi.WithdrawalAsync(request);
            var content = ValidateApiResponse(response);

            // Verify Fast Pay signature
            var isValid = asymmetricCrypto.VerifySignature(asymmetricKeyCache.FastPayPublicKey, content.Data, content.Signature);
            if (!isValid) throw new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");

            logger.LogInformation("Withdrawal request successful，OrderUid: {OrderUid}", content.Data.OrderUid);
            return content.Data;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send withdrawal request to Fast Pay: {Ex}", ex);
            throw;
        }
    }

    private SecureResponse<T> ValidateApiResponse<T>(ApiResponse<SecureResponse<T>> response)
    {
        // Check if request was successfully
        if (response.IsSuccessStatusCode && response.Content != null)
            return response.Content;

        // Handle logging and throwing error
        var apiEx = response.Error;
        logger.LogError(
            apiEx ?? new ExternalServiceException("Fast Pay failed") as Exception,
            "Fast Pay failed. Status={Status}, Reason={Reason}, ErrorMessage={ErrorMessage}, ErrorContent={ErrorContent}",
            response.StatusCode,
            apiEx?.ReasonPhrase ?? response.ReasonPhrase ?? string.Empty,
            apiEx?.Message ?? string.Empty,
            apiEx?.Content ?? string.Empty);

        // Wrapping and throwing Fast Pay exception
        throw GetFastPayErrors(response);
    }

    private static Exception GetFastPayErrors<T>(ApiResponse<SecureResponse<T>> response)
    {
        var errorContent = JsonHelper.ConvertJson<FastPayErrorResponse>(response.Error?.Content ?? "{}");

        // Check if this is insufficient balance error
        if (InsufficientErrorCodes.Contains(errorContent.ErrorCode))
            return new BadRequestException(MessageCode.Accounting.InsufficientBalance, new { message = response.Error?.Content });

        // Fallback
        return new ExternalServiceException("UXM failed");
    }
}

using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Exceptions;
using game_x.share.Extensions;
using game_x.share.ExternalApi.Uxm.Dtos;
using game_x.share.Helper;
using Refit;

namespace game_x.infrastructure.ExternalApi.Uxm;

public sealed class UxmService(IAppLogger<UxmService> logger, IUxmApi uxmApi) : IUxmService
{
    public async Task<SecureResponse<UxmWithdrawalOrderResponseData>> CreateWithdrawalOrderAsync(
      SecureRequest<UxmWithdrawalOrderRequest> data)
    {
        try
        {
            logger.LogInformation("Send withdrawal request to UXM: MerchantNumber={MerchantNumber}, To = {To}, Amount = {Amount}, OrderNumber = {OtcOrderNumber}", 
                data.Data.MerchantNumber, 
                data.Data.To,
                data.Data.Amount, 
                data.Data.OrderNumber);
     
            var response = await uxmApi.CreateProxyWithdrawalOrderAsync(data);
            var content = ValidateApiResponse(response);
            logger.LogInformation("Withdrawal request successful，OrderUid: {OrderUid}", content.Data.OrderUid!);
            return content;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send withdrawal request to UXM: {Ex}", ex);
            throw;
        }
    }

    public async Task<SecureResponse<UxmDepositOrderResponseData>> CreateDepositOrderAsync(
        SecureRequest<UxmDepositOrderRequest> data)
    {
        try
        {
            logger.LogInformation("Send deposit request to UXM: MerchantNumber={MerchantNumber}, UserId={UserId}, Amount = {Amount}, OrderNumber = {OtcOrderNumber}", 
                data.Data.MerchantNumber, 
                data.Data.UserId, 
                data.Data.Amount, 
                data.Data.OrderNumber);

            var response = await uxmApi.CreateProxyDepositOrderAsync(data);
            var content = ValidateApiResponse(response);
            logger.LogInformation("Deposit request successful，OrderUid: {OrderUid}", content.Data.OrderUid);
            return content;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send deposit request to UXM: {Ex}", ex);
            throw;
        }
    }

    private SecureResponse<T> ValidateApiResponse<T>(ApiResponse<SecureResponse<T>> response)
    {
        var content = response.Content;
        if (!response.IsSuccessStatusCode || content == null)
        {
            var apiEx = response.Error;
            logger.LogError(
                apiEx ?? new Exception(),
                "UXM failed. Status={Status}, Reason={Reason}, ErrorMessage={ErrorMessage}, ErrorContent={ErrorContent}",
                response.StatusCode,
                apiEx?.ReasonPhrase ?? response.ReasonPhrase ?? string.Empty,
                apiEx?.Message ?? string.Empty,
                apiEx?.Content ?? string.Empty);
                
            ValidateUxmErrors(response);
            throw new ExternalServiceException("UXM failed");
        }
        return content;
    }

    private void ValidateUxmErrors<T>(ApiResponse<SecureResponse<T>> response)
    {
        if (response.Error == null) return;
        var errorContent = JsonHelper.ConvertJson<UxmErrorResponse>(response.Error.Content ?? "{}");
        if(new []
           {
               UxmErrorCode.InsufficientMerchantBalance, 
               UxmErrorCode.InsufficientWalletBalance
           }.Contains((UxmErrorCode)errorContent.ErrorCode))
            throw new BadRequestException(MessageCode.Accounting.InsufficientBalance, new {message = response.Error.Content});
    }
}

public sealed class UxmErrorResponse
{
    public string? Type { get; init; }
    public string? Title { get; init; }
    public int Status { get; init; }
    public object? Errors { get; init; }
    public int ErrorCode { get; init; }
}
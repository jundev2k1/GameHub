using System.Text.Json;
using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Exceptions;
using game_x.share.ExternalApi.Etl998.Converters;
using game_x.share.ExternalApi.Etl998.Dtos;
using game_x.share.ExternalApi.Etl998.Dtos.Wallet;
using game_x.share.ExternalApi.Etl998.Dtos.CancelTransfer;
using game_x.share.ExternalApi.Etl998.Dtos.ChangePassword;
using game_x.share.ExternalApi.Etl998.Dtos.ForwardGame;
using game_x.share.ExternalApi.Etl998.Dtos.IsAccountExist;
using game_x.share.ExternalApi.Etl998.Dtos.ModifyBettingLimit;
using game_x.share.ExternalApi.Etl998.Dtos.PrepareTransfer;
using game_x.share.ExternalApi.Etl998.Dtos.Register;
using game_x.share.ExternalApi.Etl998.Dtos.SearchRecord;
using game_x.share.ExternalApi.Etl998.Dtos.SearchTransfer;
using Refit;

namespace game_x.infrastructure.ExternalApi.Etl998;

public class Etl998Service(
    IEtl998Api gameApi,
    IAppLogger<Etl998Service> logger) : IEtl998Service
{
    public async Task<IReadOnlyCollection<Etl998RegisterResponse>> RegisterAsync(Etl998RegisterRequest req)
    {
        try
        {
            logger.LogInformation("Send creating a new account request to Etl998 Platform: account = {Account}", req.Account);

            var result = await gameApi.RegisterAsync(req);
            var content = result.Content;
            
            if (content?.ErrorCode != 0 || !result.IsSuccessful || result.Content == null)
            {
                logger.LogError($"Response failed: ErrorCode={result.Content?.ErrorCode}, ErrorMessage={result.Content?.ErrorMessage ?? result.Error?.Message}");
                throw new ExternalServiceException();
            }
            
            return DeserializeResult<Etl998RegisterResponse>(content.Result);
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send creating request to Etl998 Platform: {Ex}", ex);
            throw;
        }
    }

    public async Task<bool> IsAccountExistAsync(IsAccountExistRequest req)
    {
        try
        {
            logger.LogInformation("Send checking account existence request to Etl998 Platform: account = {Account}", req.Account);

            var response = await gameApi.IsAccountExistAsync(req);
            var content = response.Content;
            if (!response.IsSuccessStatusCode || !response.IsSuccessful || response.Content == null)
            {
                logger.LogError($"Response failed: ErrorCode={content?.ErrorCode}, ErrorMessage={content?.ErrorMessage ?? response.Error?.Message}");
                throw new ExternalServiceException();
            }
            
            logger.LogInformation("Account existence check successful，account: {Account}", req.Account);
            return response.Content?.ErrorCode != 0;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send checking account existence to Etl998 Platform: {Ex}", ex);
            throw;
        }
    }

    public async Task<IReadOnlyCollection<Etl998WalletResponse>> GetWalletAsync(Etl998WalletRequest req)
    {
        try
        {
            logger.LogInformation("Send retrieving account balance request to Etl998 Platform: account = {Account}", req.Account);

            var response = await gameApi.GetWalletAsync(req);
            var result = ValidateApiResponse(response);
            return DeserializeResult<Etl998WalletResponse>(result);
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send retrieving account balance request to Etl998 Platform: {Ex}", ex);
            throw;
        }
    }
    
    public async Task<IReadOnlyCollection<Etl998TransferResponse>> PrepareTransferAsync(Etl998TransferRequest req)
    {
        try
        {
            logger.LogInformation("Send retrieving confirm transfer request to Etl998 Platform: account = {Account}, credit = {Credit}, type={Type}, customerOrderId={CustomerOrderId}", 
                req.Account,
                req.Credit,
                req.Type,
                req.CustomerOrderId);

            var response = await gameApi.PrepareTransferAsync(req);
            var result = ValidateApiResponse(response);
            return DeserializeResult<Etl998TransferResponse>(result);
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send retrieving confirm transfer request to Etl998 Platform: {Ex}", ex);
            throw;
        }
    }
    
    public async Task<IReadOnlyCollection<Etl998TransferResponse>> ConfirmTransferAsync(Etl998TransferRequest req)
    {
        try
        {
            logger.LogInformation("Send retrieving confirm transfer request to Etl998 Platform: account = {Account}, credit = {Credit}, type={Type}, customerOrderId={CustomerOrderId}", 
                req.Account,
                req.Credit,
                req.Type,
                req.CustomerOrderId);

            var response = await gameApi.ConfirmTransferAsync(req);
            var result = ValidateApiResponse(response);
            return DeserializeResult<Etl998TransferResponse>(result);
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send retrieving confirm transfer request to Etl998 Platform: {Ex}", ex);
            throw;
        }
    }
    
    public async Task<IReadOnlyCollection<SearchTransferResponse>> SearchTransferAsync(SearchTransferRequest req)
    {
        try
        {
            logger.LogInformation("Send searching transfer request to Etl998 Platform: account = {Account}, dateStart = {DateStart}, dateEnd={DateEnd}, customerOrderId={CustomerOrderId}", 
                req.Account,
                req.DateStart,
                req.DateEnd,
                req.CustomerOrderId);

            var response = await gameApi.SearchTransferAsync(req);
            var result = ValidateApiResponse(response);
            return DeserializeResult<SearchTransferResponse>(result);
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send searching transfer request to Etl998 Platform: {Ex}", ex);
            throw;
        }
    }
    
    public async Task<IReadOnlyCollection<CancelTransferResponse>> CancelTransferAsync(CancelTransferRequest req)
    {
        try
        {
            logger.LogInformation("Send cancelling transfer request to Etl998 Platform: account = {Account}, dateStart = {DateStart}, dateEnd={DateEnd}, customerOrderId={CustomerOrderId}", 
                req.Account,
                req.DateStart,
                req.DateEnd,
                req.CustomerOrderId);

            var response = await gameApi.CancelTransferAsync(req);
            var result = ValidateApiResponse(response);
            return DeserializeResult<CancelTransferResponse>(result);
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send cancelling transfer request to Etl998 Platform: {Ex}", ex);
            throw;
        }
    }
    
    public async Task<IReadOnlyCollection<ForwardGameResponse>> ForwardGameAsync(ForwardGameRequest req)
    {
        try
        {
            logger.LogInformation("Send forwarding game request to Etl998 Platform: account = {Account}, dm = {Dm}", 
                req.Account,
                req.Dm);

            var response = await gameApi.ForwardGameAsync(req);
            var result = ValidateApiResponse(response);
            var data = DeserializeResult<ForwardGameResponse>(result);
            logger.LogInformation("Send forwarding game request to Etl998 Platform: account = {Account}, gameUrl = {GameUrl}", 
                req.Account,
                data.FirstOrDefault()?.GameUrl ?? String.Empty);
            return data;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send forwarding game request to Etl998 Platform: {Ex}", ex);
            throw;
        }
    }
    
    public async Task<IReadOnlyCollection<SearchRecordResponse>> SearchRecordAsync(SearchRecordRequest req)
    {
        try
        {
            logger.LogInformation("Send searching record request to Etl998 Platform: account = {Account}, dateStart = {DateStart}, dateEnd = {DateEnd}", 
                req.Account,
                req.DateStart,
                req.DateEnd);

            var response = await gameApi.SearchRecordAsync(req);
            var result = ValidateApiResponse(response);
            return DeserializeResult<SearchRecordResponse>(result);
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send searching record request to Etl998 Platform: {Ex}", ex);
            throw;
        }
    }
    
    public async Task<bool> ChangePasswordAsync(ChangePasswordRequest req)
    {
        try
        {
            logger.LogInformation("Send changing password request to Etl998 Platform: account = {Account}", req.Account);

            var response = await gameApi.ChangePasswordAsync(req);
            var content = response.Content;
            if (!response.IsSuccessStatusCode || !response.IsSuccessful || response.Content == null)
            {
                logger.LogError($"Response failed: ErrorCode={content?.ErrorCode}, ErrorMessage={content?.ErrorMessage ?? response.Error?.Message}");
                throw new ExternalServiceException();
            }
            
            logger.LogInformation("Password change request successful，account: {Account}", req.Account);
            return response.Content?.ErrorCode == 0;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send changing password to Etl998 Platform: {Ex}", ex);
            throw;
        }
    }
    
    public async Task<bool> ModifyBettingLimitAsync(ModifyBettingLimitRequest req)
    {
        try
        {
            logger.LogInformation("Send modifying betting limit request to Etl998 Platform: account = {Account}", req.Account);

            var response = await gameApi.ModifyBettingLimitsAsync(req);
            var content = response.Content;
            if (!response.IsSuccessStatusCode || !response.IsSuccessful || response.Content == null)
            {
                logger.LogError($"Response failed: ErrorCode={content?.ErrorCode}, ErrorMessage={content?.ErrorMessage ?? response.Error?.Message}");
                throw new ExternalServiceException();
            }
            
            logger.LogInformation("Betting limit modification successful，account: {Account}", req.Account);
            return response.Content?.ErrorCode != 0;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send modifying betting limit to Etl998 Platform: {Ex}", ex);
            throw;
        }
    }
    
    // --- Helpers ---
    private JsonElement ValidateApiResponse(ApiResponse<BaseResponse> response)
    {
        var content = response.Content;
        if (content?.ErrorCode != 0 || !response.IsSuccessStatusCode || !response.IsSuccessful || response.Content == null)
        {
            logger.LogError($"Response failed: ErrorCode={content?.ErrorCode}, ErrorMessage={content?.ErrorMessage ?? response.Error?.Message}");
            throw new ExternalServiceException();
        }
        return content.Result;
    }
    
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new FlexibleDecimalConverter(),
            new FlexibleNullableDateTimeConverter()
        }
    };
    
    private static IReadOnlyCollection<T> DeserializeResult<T>(JsonElement elem)
        where T : class
    {
        if (elem.ValueKind is JsonValueKind.Null or JsonValueKind.String)
            return [];

        if (elem.ValueKind == JsonValueKind.Array)
            return elem.Deserialize<IReadOnlyCollection<T>>(JsonOptions) ?? [];

        if (elem.ValueKind == JsonValueKind.Object)
        {
            var item = elem.Deserialize<T>(JsonOptions);
            return item is null ? [] : [item];
        }
        
        return [];
    }
}
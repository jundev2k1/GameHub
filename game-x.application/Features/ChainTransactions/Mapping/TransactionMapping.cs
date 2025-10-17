using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.ChainTransactions.Dtos;
using game_x.share.ExternalApi.PaymentGateway.Dtos;
using game_x.share.ExternalApi.Uxm.Dtos;

namespace game_x.application.Features.ChainTransactions.Mapping;

public static class TransactionMapping
{
    public static DepositOrderRequest ToPaymentGatewayDepositOrderRequest(
        this Transaction tx,
        string merchantNumber,
        string platformId,
        int providerId)
    {
        var result = tx.Adapt<DepositOrderRequest>();
        return result with
        {
            UserId = tx.UserId,
            PlatformId = Int32.Parse(platformId),
            MerchantId = merchantNumber,
            OrderNumber = tx.TransactionInternal?.OrderNumber ?? string.Empty,
            Amount = tx.Amount,
            ProviderId = providerId,
            Remark = tx.Note ?? string.Empty
        };
    }
    
    public static WithdrawalOrderRequest ToPaymentGatewayWithdrawalOrderRequest(
        this Transaction tx,
        string merchantNumber,
        int platformId,
        int providerId)
    {
        var result = tx.Adapt<WithdrawalOrderRequest>();
        return result with
        {
            PlatformId = platformId,
            MerchantId = merchantNumber,
            WalletAddress = tx.TransactionInternal?.ToAddress,
            OrderNumber = tx.TransactionInternal?.OrderNumber ?? string.Empty,
            Amount = tx.Amount,
            ProviderId = providerId,
            Remark = tx.Note ?? string.Empty
        };
    }
    
    public static UxmDepositOrderRequest ToUxmDepositOrderRequest(
        this Transaction tx,
        string merchantNumber)
    {
        var result = tx.Adapt<UxmDepositOrderRequest>();
        return result with {
            UserId = tx.UserId!,
            MerchantNumber = merchantNumber,
            OrderNumber = tx.TransactionInternal?.OrderNumber ?? string.Empty,
            Amount = tx.Amount,
            Remark = tx.Note ?? string.Empty
        };
    }
    
    public static UxmWithdrawalOrderRequest ToUxmWithdrawalOrderRequest(
        this Transaction transaction,
        string merchantNumber)
    {
        var result = transaction.Adapt<UxmWithdrawalOrderRequest>();
        return result with {
            MerchantNumber = merchantNumber,
            OrderNumber = transaction.TransactionInternal?.OrderNumber ?? String.Empty,
            Amount =  transaction.Amount,
            To = transaction.TransactionInternal?.ToAddress ?? string.Empty,
            Remark = transaction.Note ?? string.Empty
        };
    }
    
    public static PaginationResult<ListTransactionInternalDto> ToSearchResult(this PaginationResult<Transaction> data)
    {
        var result = new PaginationResult<ListTransactionInternalDto>(
            items: [.. data.Items.Adapt<IEnumerable<ListTransactionInternalDto>>()],
            totalItems: data.TotalItems,
            totalPages: data.TotalPages,
            pageIndex: data.PageNumber,
            pageSize: data.PageSize
        );
        return result;
    }
}
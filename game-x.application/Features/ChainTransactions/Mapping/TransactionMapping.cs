using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.ChainTransactions.Dtos;
using game_x.share.ExternalApi.PaymentGateway.Dtos;
using game_x.share.ExternalApi.Uxm.Dtos;

namespace game_x.application.Features.ChainTransactions.Mapping;

public static class TransactionMapping
{
    public static DepositOrderRequest ToPaymentGatewayDepositOrderRequest(this Transaction tx, int providerId)
    {
        var result = tx.Adapt<DepositOrderRequest>();
        return result with
        {
            UserId = tx.UserId,
            OrderNumber = tx.TransactionInternal?.OrderNumber ?? string.Empty,
            Amount = tx.Amount,
            ProviderId = providerId,
            Remark = tx.Note ?? string.Empty
        };
    }
    
    public static WithdrawalOrderRequest ToPaymentGatewayWithdrawalOrderRequest(this Transaction tx, int providerId)
    {
        var result = tx.Adapt<WithdrawalOrderRequest>();
        return result with
        {
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
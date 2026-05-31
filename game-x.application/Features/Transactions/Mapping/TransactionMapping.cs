using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Transactions.Dtos;
using game_x.share.ExternalApi.PaymentGateway.Dtos;
using game_x.share.ExternalApi.Uxm.Dtos.ApiRequests.Deposit;
using game_x.share.ExternalApi.Uxm.Dtos.ApiRequests.Withdrawal;

namespace game_x.application.Features.Transactions.Mapping;

public static class TransactionMapping
{
    public static DepositOrderRequest ToPaymentGatewayDepositOrderRequest(this Transaction tx, int providerId)
    {
        return new DepositOrderRequest
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
        return new WithdrawalOrderRequest
        {
            WalletAddress = tx.TransactionInternal?.ToAddress,
            OrderNumber = tx.TransactionInternal?.OrderNumber ?? string.Empty,
            Amount = tx.Amount,
            ProviderId = providerId,
            Remark = tx.Note ?? string.Empty
        };
    }

    public static PaginationResult<ListTransactionInternalDto> ToSearchResult(this PaginationResult<Transaction> data)
    {
        var result = new PaginationResult<ListTransactionInternalDto>(
            items: data.Items.Select(i => i.Adapt<ListTransactionInternalDto>()),
            totalItems: data.TotalItems,
            totalPages: data.TotalPages,
            pageIndex: data.PageNumber,
            pageSize: data.PageSize
        );
        return result;
    }
}
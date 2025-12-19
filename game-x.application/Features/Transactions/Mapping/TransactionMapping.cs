using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Games.Admin.Queries.GetGamesByCriteria;
using game_x.application.Features.Games.Dtos;
using game_x.application.Features.Transactions.Client.Queries.GetMyWalletTransactions;
using game_x.application.Features.Transactions.Dtos;
using game_x.share.Extensions;
using game_x.share.ExternalApi.PaymentGateway.Dtos;
using game_x.share.ExternalApi.Uxm.Dtos;

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

    public static UxmDepositOrderRequest ToUxmDepositOrderRequest(
        this Transaction tx,
        string merchantNumber)
    {
        return new UxmDepositOrderRequest
        (
            UserId: tx.UserId,
            MerchantNumber: merchantNumber,
            OrderNumber: tx.TransactionInternal?.OrderNumber ?? string.Empty,
            Amount: tx.Amount,
            Remark: tx.Note ?? string.Empty
        );
    }

    public static UxmWithdrawalOrderRequest ToUxmWithdrawalOrderRequest(
        this Transaction transaction,
        string merchantNumber)
    {
        return new UxmWithdrawalOrderRequest
        (
            MerchantNumber: merchantNumber,
            OrderNumber: transaction.TransactionInternal?.OrderNumber ?? String.Empty,
            Amount: transaction.Amount,
            To: transaction.TransactionInternal?.ToAddress ?? string.Empty,
            Remark: transaction.Note ?? string.Empty
        );
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
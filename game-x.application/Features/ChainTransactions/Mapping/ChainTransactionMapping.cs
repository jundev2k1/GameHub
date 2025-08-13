using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.ChainTransactions.Dtos;
using game_x.share.ExternalApi.Uxm.Dtos;

namespace game_x.application.Features.ChainTransactions.Mapping;

public static class ChainTransactionMapping
{
    public static UxmWithdrawalOrderRequest ToUxmWithdrawalOrderRequestData(
        this ChainTransaction transaction,
        string merchantNumber)
    {
        var result = transaction.Adapt<UxmWithdrawalOrderRequest>();
        return result with {
            MerchantNumber = merchantNumber,
            OrderNumber = transaction.OrderNumber,
            Amount =  transaction.Amount,
            To = transaction.ToAddress ?? string.Empty,
            Remark = transaction.Note ?? string.Empty
        };
    }
    
    public static PaginationResult<ChainTransactionDto> ToSearchResult(this PaginationResult<ChainTransaction> data)
    {
        var result = new PaginationResult<ChainTransactionDto>(
            items: [.. data.Items.Select(item => item.Adapt<ChainTransactionDto>())],
            totalItems: data.TotalItems,
            totalPages: data.TotalPages,
            pageIndex: data.PageNumber,
            pageSize: data.PageSize
        );
        return result;
    }
}
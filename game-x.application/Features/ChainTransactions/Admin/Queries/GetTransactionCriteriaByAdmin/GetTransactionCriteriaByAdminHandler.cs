using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions.FilterExtensions;
using game_x.application.Features.ChainTransactions.Dtos;
using game_x.application.Features.ChainTransactions.Mapping;

namespace game_x.application.Features.ChainTransactions.Admin.Queries.GetTransactionCriteriaByAdmin;

public sealed class GetTransactionCriteriaByAdminHandler(
    ICriteriaBuilder<Transaction> builder, 
    ITransactionRepo transactionRepo)
    : IQueryHandler<GetTransactionCriteriaByAdminQuery, PaginationResult<ListTransactionInternalDto>>
{
    public async Task<PaginationResult<ListTransactionInternalDto>> Handle(GetTransactionCriteriaByAdminQuery request, CancellationToken ct = default)
    {
        var items = await transactionRepo.GetInternalTransactionsAsync(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                searchByKeyCondition: keyword => x => 
                    x.PublicId.ToString().Contains(keyword) 
                    || x.TransactionInternal!.OrderUid!.Contains(keyword)
                    || x.TransactionInternal!.OrderNumber.Contains(keyword),
                options: TransactionFilterExtensions.InternalOptions),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);
        
        var result = items.ToSearchResult();
        return result;
    }
}
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions.FilterExtensions;
using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Features.Transactions.Client.Queries.GetMyWalletTransactions;

public sealed class GetMyWalletTransactionsHandler(
    ICriteriaBuilder<WalletTransactionDto> builder,
    ITransactionRepo transactionRepo,
    IUserAccessor userAccessor) : IQueryHandler<GetMyWalletTransactionsQuery, PaginationResult<WalletTransactionDto>>
{
    public async Task<PaginationResult<WalletTransactionDto>> Handle(GetMyWalletTransactionsQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var items = await transactionRepo.GetMyWalletTransactionsAsync(
            userId,
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                options: TransactionFilterExtensions.WalletTransactionOptions),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);

        var result = items;
        return items;
    }
}

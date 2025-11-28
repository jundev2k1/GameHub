using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.SystemWallets.DTOs;
using game_x.application.Features.SystemWallets.Mapping;
using game_x.share.Extensions;
using System.Linq.Expressions;

namespace game_x.application.Features.SystemWallets.Queries.GetSystemWalletTransactions;

public sealed class GetSystemWalletTransactionsHandler(
    ICriteriaBuilder<SystemWalletTransaction> criteriaBuilder,
    ISystemWalletRepo systemWalletRepo) : IQueryHandler<GetSystemWalletTransactionsQuery, PaginationResult<SystemWalletTransactionDto>>
{
    public async Task<PaginationResult<SystemWalletTransactionDto>> Handle(GetSystemWalletTransactionsQuery request, CancellationToken ct = default)
    {
        var searchResult = await systemWalletRepo.GetsByCriteriaAsync(
            query => criteriaBuilder.Apply(query, request.Filters, request.Sorts, options: this.Options),
            request.PageIndex,
            request.PageSize,
            ct);

        return searchResult.ToSearchResult();
    }

    private readonly Dictionary<string, Func<object, Expression<Func<SystemWalletTransaction, bool>>>> Options = new()
    {
        ["type"] = FilterByType
    };

    private static Expression<Func<SystemWalletTransaction, bool>> FilterByType(object value)
    {
        var raw = value.ToStringOrEmpty();
        if (raw.IsNullOrWhiteSpace()) return _ => true;

        if (!Enum.TryParse<SystemWalletType>(raw, true, out var type))
            return _ => false;

        return swt => swt.Wallet.Type == type;
    }
}

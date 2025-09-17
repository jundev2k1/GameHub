using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.Dtos;

namespace game_x.application.Features.Accounts.Common.Queries.GetUserWithSuggestions;

public sealed class GetUserWithSuggestionsHandler(IUserRepo userRepo)
    : IQueryHandler<GetUserWithSuggestionsQuery, UserSummaryInfo[]>
{
    public async Task<UserSummaryInfo[]> Handle(GetUserWithSuggestionsQuery request, CancellationToken ct = default)
    {
        var items = await userRepo.GetUserWithSuggestionsAsync(
            request.Keyword ?? string.Empty,
            request.IsKycConfirmed,
            request.IBankAccountConfirmed,
            request.PageSize,
            false,
            ct);
        return [.. items.Select(u => u.Adapt<UserSummaryInfo>())];
    }
}

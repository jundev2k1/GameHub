using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.Dtos;

namespace game_x.application.Features.Accounts.Admin.Queries.GetUserWithSuggestions;

public sealed class GetUserWithSuggestionsHandler(IUserRepo userRepo)
    : IQueryHandler<GetUserWithSuggestionsQuery, UserSummaryForAdmin[]>
{
    public async Task<UserSummaryForAdmin[]> Handle(GetUserWithSuggestionsQuery request, CancellationToken ct = default)
    {
        var items = await userRepo.GetUserWithSuggestionsAsync(
            request.Keyword ?? string.Empty,
            request.IsKycConfirmed,
            request.IBankAccountConfirmed,
            request.PageSize,
            true,
            ct);
        return [.. items.Select(u => u.Adapt<UserSummaryForAdmin>())];
    }
}

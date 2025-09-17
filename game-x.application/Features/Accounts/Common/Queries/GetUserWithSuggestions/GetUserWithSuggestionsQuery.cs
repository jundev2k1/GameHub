using game_x.application.Features.Accounts.Dtos;

namespace game_x.application.Features.Accounts.Common.Queries.GetUserWithSuggestions;

public record GetUserWithSuggestionsQuery(
    string? Keyword = "",
    bool? IsKycConfirmed = true,
    bool? IBankAccountConfirmed = true,
    int PageSize = 10) : IQuery<UserSummaryInfo[]>;

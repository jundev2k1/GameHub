using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.Dtos;

namespace game_x.application.Features.Accounts.Common.Queries.GetUserWithSuggestions;

public sealed class GetUserWithSuggestionsHandler(IUserAccessor userAccessor, IUserRepo userRepo)
    : IQueryHandler<GetUserWithSuggestionsQuery, UserSummaryInfo[]>
{
    public async Task<UserSummaryInfo[]> Handle(GetUserWithSuggestionsQuery request, CancellationToken ct = default)
    {
        var userRole = userAccessor.GetRoles();
        if (!userRole.IsAdmin && request.Roles != null && request.Roles.Contains(AppRoles.Admin))
            throw new BadRequestException($"This role ({userRole}) does not sufficient permissions. Only administrators can get administrator information.");

        var items = await userRepo.GetUserWithSuggestionsAsync(
            request.Keyword ?? string.Empty,
            request.IsKycConfirmed,
            request.IsBankAccountConfirmed,
            request.PageSize,
            request.Roles,
            ct);
        return [.. items.Select(u => u.Adapt<UserSummaryInfo>())];
    }
}

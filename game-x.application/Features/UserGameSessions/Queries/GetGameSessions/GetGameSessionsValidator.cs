using game_x.application.Common.Filters;
using game_x.application.Features.UserGameSessions.Dtos;

namespace game_x.application.Features.UserGameSessions.Queries.GetGameSessions;

public sealed class GetGameSessionsValidator : AbstractValidator<GetGameSessionsQuery>
{
    private readonly string[] _allowFields =
    {
        nameof(UserGameSessionSearchItemDto.PlatformId),
        nameof(UserGameSessionSearchItemDto.PlatformName),
        nameof(UserGameSessionSearchItemDto.GameName),
        nameof(UserGameSessionSearchItemDto.GameId),
        nameof(UserGameSessionSearchItemDto.GameCode),
        nameof(UserGameSessionSearchItemDto.UserId),
        nameof(UserGameSessionSearchItemDto.Nickname),
        nameof(UserGameSessionSearchItemDto.BalanceSnapshot),
        nameof(UserGameSessionSearchItemDto.ConnectedAt),
        nameof(UserGameSessionSearchItemDto.DisconnectedAt),
    };

    public GetGameSessionsValidator()
    {
        RuleForEach(x => x.Filters)
            .Custom(ValidateFilterField);

        RuleForEach(x => x.Sorts)
            .Custom(ValidateSortField);

        RuleFor(x => x.PageIndex)
            .GreaterThan(0).WithMessage("Page index must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("Page size must be greater than or equal 1")
            .LessThanOrEqualTo(1000).WithMessage("Page size must be less than or equal 1000");
    }

    private void ValidateFilterField(QueryFilter filter, ValidationContext<GetGameSessionsQuery> context)
    {
        if (_allowFields.All(f => !f.Equals(filter.Field, StringComparison.InvariantCultureIgnoreCase)))
            context.AddFailure($"Filter field '{filter.Field}' is not allowed.");
    }

    private void ValidateSortField(QuerySort sort, ValidationContext<GetGameSessionsQuery> context)
    {
        if (_allowFields.All(f => !f.Equals(sort.Field, StringComparison.InvariantCultureIgnoreCase)))
            context.AddFailure($"Sort field '{sort.Field}' is not allowed.");
    }
}

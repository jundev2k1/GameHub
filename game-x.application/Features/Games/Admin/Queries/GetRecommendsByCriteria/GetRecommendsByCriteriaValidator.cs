using game_x.application.Common.Filters;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetRecommendsByCriteria;

public sealed class GetRecommendsByCriteriaValidator : AbstractValidator<GetRecommendsByCriteriaQuery>
{
    private readonly string[] _allowFields =
    {
        "search",
        nameof(GameRecommendDto.Id),
        nameof(GameRecommendDto.Name),
        nameof(GameRecommendDto.Description),
        nameof(GameRecommendDto.Status),
        nameof(GameRecommendDto.StartDate),
        nameof(GameRecommendDto.EndDate),
        nameof(GameRecommendDto.CreatedAt),
        nameof(GameRecommendDto.UpdatedAt),
    };

    public GetRecommendsByCriteriaValidator()
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

    private void ValidateFilterField(QueryFilter filter, ValidationContext<GetRecommendsByCriteriaQuery> context)
    {
        if (_allowFields.All(f => f.ToLowerInvariant() != filter.Field.ToLowerInvariant()))
            context.AddFailure($"Filter field '{filter.Field}' is not allowed.");
    }

    private void ValidateSortField(QuerySort sort, ValidationContext<GetRecommendsByCriteriaQuery> context)
    {
        if (_allowFields.All(f => f.ToLowerInvariant() != sort.Field.ToLowerInvariant()))
            context.AddFailure($"Sort field '{sort.Field}' is not allowed.");
    }
}

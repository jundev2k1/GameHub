using FluentValidation;
using game_x.application.Common.Filters;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.LiveStreams.Categories.Queries.GetCategoriesByCriteria;

public sealed class GetCategoriesByCriteriaValidator : AbstractValidator<GetCategoriesByCriteriaQuery>
{
    private readonly string[] _allowFields =
    {
        "search",
        nameof(LiveStreamCategory.PublicId),
        nameof(LiveStreamCategory.Name),
        nameof(LiveStreamCategory.Description),
        nameof(LiveStreamCategory.Priority),
        nameof(LiveStreamCategory.IsActive),
        nameof(LiveStreamCategory.CreatedAt),
        nameof(LiveStreamCategory.UpdatedAt),
    };

    public GetCategoriesByCriteriaValidator()
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

    private void ValidateFilterField(QueryFilter filter, ValidationContext<GetCategoriesByCriteriaQuery> context)
    {
        if (_allowFields.All(f => f.ToLowerInvariant() != filter.Field.ToLowerInvariant()))
            context.AddFailure($"Filter field '{filter.Field}' is not allowed.");
    }

    private void ValidateSortField(QuerySort sort, ValidationContext<GetCategoriesByCriteriaQuery> context)
    {
        if (_allowFields.All(f => f.ToLowerInvariant() != sort.Field.ToLowerInvariant()))
            context.AddFailure($"Sort field '{sort.Field}' is not allowed.");
    }
}

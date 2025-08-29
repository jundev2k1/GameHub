using game_x.application.Common.Filters;

namespace game_x.application.Features.Kyc.Queries.GetKycByCriteria;

public sealed class GetKycByCriteriaValidator : AbstractValidator<GetKycByCriteriaQuery>
{
    private readonly string[] _allowFields =
    {
        "search",
        nameof(UserKyc.PublicId),
        nameof(UserKyc.UserId),
        nameof(UserKyc.FullName),
        nameof(UserKyc.DateOfBirth),
        nameof(UserKyc.ResidentialAddress),
        nameof(UserKyc.IdNumber),
        nameof(UserKyc.Status),
        nameof(UserKyc.KycType),
        nameof(UserKyc.SubmittedAt),
        nameof(UserKyc.ReviewedById),
        nameof(UserKyc.CreatedAt),
        nameof(UserKyc.UpdatedAt),
    };

    public GetKycByCriteriaValidator()
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

    private void ValidateFilterField(QueryFilter filter, ValidationContext<GetKycByCriteriaQuery> context)
    {
        if (_allowFields.All(f => f.ToLowerInvariant() != filter.Field.ToLowerInvariant()))
            context.AddFailure($"Filter field '{filter.Field}' is not allowed.");
    }

    private void ValidateSortField(QuerySort sort, ValidationContext<GetKycByCriteriaQuery> context)
    {
        if (_allowFields.All(f => f.ToLowerInvariant() != sort.Field.ToLowerInvariant()))
            context.AddFailure($"Sort field '{sort.Field}' is not allowed.");
    }
}

using game_x.application.Common.Filters;

namespace game_x.application.Features.AuditLogs.Admin.Queries.GetAuditCriteriaByAdmin;

public sealed class GetAuditCriteriaByAdminValidator : AbstractValidator<GetAuditCriteriaByAdminQuery>
{
    private readonly string[] _allowFields =
    {
        "search",
        nameof(AuditLog.PublicId),
        nameof(AuditLog.EntityName),
        nameof(AuditLog.EntityId),
        nameof(AuditLog.Action),
        nameof(AuditLog.ChangedByUserId),
        nameof(AuditLog.Source),
        nameof(AuditLog.CreatedAt)
    };

    public GetAuditCriteriaByAdminValidator()
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

    private void ValidateFilterField(QueryFilter filter, ValidationContext<GetAuditCriteriaByAdminQuery> context)
    {
        if (_allowFields.All(f => f.ToLowerInvariant() != filter.Field.ToLowerInvariant()))
            context.AddFailure($"Filter field '{filter.Field}' is not allowed.");
    }

    private void ValidateSortField(QuerySort sort, ValidationContext<GetAuditCriteriaByAdminQuery> context)
    {
        if (_allowFields.All(f => f.ToLowerInvariant() != sort.Field.ToLowerInvariant()))
            context.AddFailure($"Sort field '{sort.Field}' is not allowed.");
    }
}

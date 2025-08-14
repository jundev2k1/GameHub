namespace game_x.application.Features.Notifications.Shared.Queries.GetNotificationDetail;

public sealed class GetNotificationDetailValidator : AbstractValidator<GetNotificationDetailQuery>
{
    public GetNotificationDetailValidator()
    {
        RuleFor(x => x.PageNo)
            .GreaterThan(0).WithMessage($"{nameof(GetNotificationDetailQuery.PageNo)} must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 500).WithMessage($"{nameof(GetNotificationDetailQuery.PageSize)} must be between 1 and 500.");
    }
}

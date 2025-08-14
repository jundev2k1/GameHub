namespace game_x.application.Features.Notifications.Shared.Queries.GetAdjacentNotifications;

public sealed class GetAdjacentNotificationsValidator: AbstractValidator<GetAdjacentNotificationsQuery>
{
    public GetAdjacentNotificationsValidator()
    {
        RuleFor(x => x.CurrentId)
            .NotEmpty().WithMessage("Current notification ID cannot be empty.")
            .NotNull().WithMessage("Current notification ID cannot be null.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than zero.");
    }
}

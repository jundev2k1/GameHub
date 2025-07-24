namespace game_x.application.Features.OrderManagement.Admin.Commands.ReviewOrderByAdmin;

public sealed class ReviewOrderByAdminValidator : AbstractValidator<ReviewOrderByAdminCommand>
{
    public ReviewOrderByAdminValidator()
    {
        RuleFor(x => x.OrderStatus)
            .Must(status => new[] { OrderStatus.Approved.Value, OrderStatus.Cancelled.Value }.Contains(status))
            .WithMessage("Status is invalid.");
    }
}

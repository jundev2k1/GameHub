namespace game_x.application.Features.OrderManagement.Staff.Commands.ReviewOrderByStaff;

public sealed class ReviewOrderByStaffValidator : AbstractValidator<ReviewOrderByStaffCommand>
{
    public ReviewOrderByStaffValidator()
    {
        RuleFor(x => x.OrderStatus)
            .Must(status => new[] { OrderStatus.Approved.Value, OrderStatus.Cancelled.Value }.Contains(status))
            .WithMessage("Status is invalid.");
    }
}
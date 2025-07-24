namespace game_x.application.Features.OrderManagement.Admin.Commands.UpdateOrderInfoByAdmin;

public sealed class UpdateOrderInfoByAdminValidator : AbstractValidator<UpdateOrderInfoByAdminCommand>
{
    public UpdateOrderInfoByAdminValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.PriceOfUnit)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Unit price must be zero or greater.");

        RuleFor(x => x.CurrencyUnit)
            .NotEmpty()
            .WithMessage("Currency unit is required.")
            .MaximumLength(10)
            .WithMessage("Currency unit must not exceed 10 characters.");

        RuleFor(x => x.OrderStatus)
            .NotEmpty()
            .WithMessage("Order status is required.")
            .MaximumLength(50)
            .WithMessage("Order status must not exceed 50 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes must not exceed 500 characters.");
    }
}

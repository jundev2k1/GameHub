namespace game_x.application.Features.OrderManagement.Staff.Commands.TraceV2.EstimateQuote;

public sealed class EstimateQuoteValidator : AbstractValidator<EstimateQuoteCommand>
{
    public EstimateQuoteValidator()
    {
        RuleFor(o => o.OrderType)
            .Must(OrderType.IsValid)
            .WithMessage($"Invalid {nameof(EstimateQuoteCommand.OrderType)}.");

        RuleFor(o => o.PricingMode)
            .IsInEnum()
            .WithMessage($"Invalid {nameof(EstimateQuoteCommand.PricingMode)}.");

        RuleFor(o => o.Amount)
            .NotEmpty()
            .WithMessage($"{nameof(EstimateQuoteCommand.Amount)} is not empty.")
            .GreaterThan(0)
            .WithMessage($"{nameof(EstimateQuoteCommand.Amount)} must be greater than 0.");

        RuleFor(o => o.FiatType)
            .IsInEnum()
            .WithMessage($"Invalid {nameof(EstimateQuoteCommand.FiatType)}.");

        RuleFor(o => o.CryptoType)
            .IsInEnum()
            .WithMessage($"Invalid {nameof(EstimateQuoteCommand.CryptoType)}.");
    }
}
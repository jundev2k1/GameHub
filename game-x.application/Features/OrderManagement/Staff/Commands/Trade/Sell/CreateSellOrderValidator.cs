namespace game_x.application.Features.OrderManagement.Staff.Commands.Trade.Sell;

public sealed class CreateSellOrderValidator : AbstractValidator<CreateSellOrderCommand>
{
    public CreateSellOrderValidator()
    {
        RuleFor(o => o.MemberId)
            .NotEmpty().WithMessage($"{nameof(CreateSellOrderCommand.MemberId)} is not empty.");

        RuleFor(o => o.BankAccountId)
            .NotEmpty().WithMessage($"{nameof(CreateSellOrderCommand.BankAccountId)} is not empty.");

        RuleFor(o => o.FiatAmount)
            .NotEmpty().WithMessage($"{nameof(CreateSellOrderCommand.FiatAmount)} is not empty.")
            .GreaterThan(0).WithMessage($"{nameof(CreateSellOrderCommand.FiatAmount)} must be geater than 0.");

        RuleFor(o => o.FiatType)
            .Must(fiatType => CurrencyUnit.IsValid(fiatType ?? string.Empty))
            .WithMessage($"Invalid {nameof(CreateSellOrderCommand.FiatType)}.");
    }
}

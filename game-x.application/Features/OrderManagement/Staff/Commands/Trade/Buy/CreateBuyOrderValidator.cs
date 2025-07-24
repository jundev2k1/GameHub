using game_x.application.Extensions;

namespace game_x.application.Features.OrderManagement.Staff.Commands.Trade.Buy;

public sealed class CreateBuyOrderValidator : AbstractValidator<CreateBuyOrderCommand>
{
    public CreateBuyOrderValidator()
    {
        RuleFor(o => o.MerchantNumber)
            .NotEmpty().WithMessage($"{nameof(CreateBuyOrderCommand.MerchantNumber)} is not empty.")
            .IsNumber(nameof(CreateBuyOrderCommand.MerchantNumber));

        RuleFor(o => o.MemberId)
            .NotEmpty().WithMessage($"{nameof(CreateBuyOrderCommand.MemberId)} is not empty.");

        RuleFor(o => o.PayerBankAccountName)
            .NotEmpty().WithMessage($"{nameof(CreateBuyOrderCommand.PayerBankAccountName)} is not empty.");

        RuleFor(o => o.FiatAmount)
            .NotEmpty().WithMessage($"{nameof(CreateBuyOrderCommand.FiatAmount)} is not empty.")
            .GreaterThan(0).WithMessage($"{nameof(CreateBuyOrderCommand.FiatAmount)} must be geater than 0.");

        RuleFor(o => o.FiatType)
            .Must(fiatType => CurrencyUnit.IsValid(fiatType ?? string.Empty))
            .WithMessage($"Invalid {nameof(CreateBuyOrderCommand.FiatType)}.");
    }
}

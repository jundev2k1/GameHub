using game_x.application.Extensions;

namespace game_x.application.Features.OrderManagement.Staff.Commands.TraceV2.Buy;

public sealed class CreateBuyOrderV2Validator : AbstractValidator<CreateBuyOrderV2Command>
{
    public CreateBuyOrderV2Validator()
    {
        RuleFor(o => o.MemberId)
            .NotEmpty().WithMessage($"{nameof(CreateBuyOrderV2Command.MemberId)} is not empty.");

        RuleFor(o => o.PayerBankAccountName)
            .NotEmpty().WithMessage($"{nameof(CreateBuyOrderV2Command.PayerBankAccountName)} is not empty.");

        RuleFor(o => o.Amount)
            .NotEmpty().WithMessage($"{nameof(CreateBuyOrderV2Command.Amount)} is not empty.")
            .GreaterThan(0).WithMessage($"{nameof(CreateBuyOrderV2Command.Amount)} must be greater than 0.");

        RuleFor(o => o.PricingMode)
            .NotNull()
            .WithMessage("PricingMode is not null.")
            .IsInEnum()
            .WithMessage($"Invalid {nameof(CreateBuyOrderV2Command.PricingMode)}.");

        RuleFor(o => o.FiatType)
            .NotNull()
            .WithMessage("FiatType is not null.")
            .IsInEnum()
            .WithMessage($"Invalid {nameof(CreateBuyOrderV2Command.FiatType)}.");

        RuleFor(o => o.CryptoType)
            .NotNull()
            .WithMessage("CryptoType is not null.")
            .IsInEnum()
            .WithMessage($"Invalid {nameof(CreateBuyOrderV2Command.CryptoType)}.");
    }
}
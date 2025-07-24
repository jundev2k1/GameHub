namespace game_x.application.Features.OrderManagement.Staff.Commands.TraceV2.Sell;

public sealed class CreateSellOrderV2Validator : AbstractValidator<CreateSellOrderV2Command>
{
    public CreateSellOrderV2Validator()
    {
        RuleFor(o => o.MemberId)
            .NotEmpty().WithMessage($"{nameof(CreateSellOrderV2Command.MemberId)} is not empty.");

        RuleFor(o => o.PayeeAccountNumber)
            .NotEmpty().WithMessage($"{nameof(CreateSellOrderV2Command.PayeeAccountNumber)} is not empty.");
        
        RuleFor(o => o.PayeeName)
            .NotEmpty().WithMessage($"{nameof(CreateSellOrderV2Command.PayeeName)} is not empty.");
        
        RuleFor(o => o.PayeeBranchCode)
            .NotEmpty().WithMessage($"{nameof(CreateSellOrderV2Command.PayeeBranchCode)} is not empty.");
        
        RuleFor(o => o.PayeeBankName)
            .NotEmpty().WithMessage($"{nameof(CreateSellOrderV2Command.PayeeBankName)} is not empty.");
        
        RuleFor(o => o.Amount)
            .NotEmpty().WithMessage($"{nameof(CreateSellOrderV2Command.Amount)} is not empty.")
            .GreaterThan(0).WithMessage($"{nameof(CreateSellOrderV2Command.Amount)} must be greater than 0.");

        RuleFor(o => o.PricingMode)
            .NotNull()
            .WithMessage("PricingMode is not null.")
            .IsInEnum()
            .WithMessage($"Invalid {nameof(CreateSellOrderV2Command.PricingMode)}.");

        RuleFor(o => o.FiatType)
            .NotNull()
            .WithMessage("FiatType is not null.")
            .IsInEnum()
            .WithMessage($"Invalid {nameof(CreateSellOrderV2Command.FiatType)}.");

        RuleFor(o => o.CryptoType)
            .NotNull()
            .WithMessage("CryptoType is not null.")
            .IsInEnum()
            .WithMessage($"Invalid {nameof(CreateSellOrderV2Command.CryptoType)}.");
    }
}
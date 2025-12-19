namespace game_x.application.Features.Transactions.Client.Commands.TraceV2.TronUsdtWithdrawalV2;

public sealed class TronUsdtWithdrawalV2Validator : AbstractValidator<TronUsdtWithdrawalV2Command>
{
    public TronUsdtWithdrawalV2Validator()
    {
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0)
            .WithMessage("Amount must be greater than zero");
        RuleFor(x => x.To)
            .NotEmpty()
            .WithMessage($"{nameof(TronUsdtWithdrawalV2Command.To)} is required.");
        RuleFor(x => x.CryptoTokenId)
            .NotEmpty()
            .WithMessage($"{nameof(TronUsdtWithdrawalV2Command.CryptoTokenId)} is required.");
        RuleFor(x => x.Provider)
            .IsInEnum().WithMessage($"{nameof(TronUsdtWithdrawalV2Command.Provider)} is not valid.");
    }
}

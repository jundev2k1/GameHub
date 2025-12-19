namespace game_x.application.Features.Transactions.Client.Commands.TraceV1.TronUsdtWithdrawal;

public sealed class TronUsdtWithdrawalValidator : AbstractValidator<TronUsdtWithdrawalCommand>
{
    public TronUsdtWithdrawalValidator()
    {
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0)
            .WithMessage("Amount must be greater than zero");
        RuleFor(x => x.To)
            .NotEmpty()
            .WithMessage($"{nameof(TronUsdtWithdrawalCommand.To)} is required.");
        RuleFor(x => x.CryptoTokenId)
            .NotEmpty()
            .WithMessage($"{nameof(TronUsdtWithdrawalCommand.CryptoTokenId)} is required.");
    }
}

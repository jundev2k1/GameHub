namespace game_x.application.Features.Games.Client.Commands.GameWallet.Withdrawal;

public sealed class WalletWithdrawalValidator : AbstractValidator<WalletWithdrawalCommand>
{
    public WalletWithdrawalValidator()
    {
        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage($"{nameof(WalletWithdrawalCommand.Amount)} is required.")
            .GreaterThanOrEqualTo(0).WithMessage($"{nameof(WalletWithdrawalCommand.Amount)} must be greater than zero.");

        RuleFor(x => x.CryptoTokenId)
            .NotEmpty().WithMessage($"{nameof(WalletWithdrawalCommand.CryptoTokenId)} is required.");
    }
}


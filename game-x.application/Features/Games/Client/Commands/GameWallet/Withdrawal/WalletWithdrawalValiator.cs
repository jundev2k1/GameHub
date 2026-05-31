namespace game_x.application.Features.Games.Client.Commands.GameWallet.Withdrawal;

public sealed class WalletWithdrawalValidator : AbstractValidator<WalletWithdrawalCommand>
{
    public WalletWithdrawalValidator()
    {
        RuleFor(x => x.CryptoTokenId)
            .NotEmpty().WithMessage($"{nameof(WalletWithdrawalCommand.CryptoTokenId)} is required.");
    }
}


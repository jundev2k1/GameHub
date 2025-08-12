namespace game_x.application.Features.Games.Commands.GameWallet.Withdrawal;

public sealed class WalletWithdrawalValidator : AbstractValidator<WalletWithdrawalCommand>
{
    public WalletWithdrawalValidator()
    {
        RuleFor(x => x.Quota)
            .NotEmpty().WithMessage($"{nameof(WalletWithdrawalCommand.Quota)} is required.")
            .GreaterThanOrEqualTo(0).WithMessage($"{nameof(WalletWithdrawalCommand.Quota)} must be greater than zero.");

    }
}

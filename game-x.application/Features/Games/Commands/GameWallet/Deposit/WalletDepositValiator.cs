namespace game_x.application.Features.Games.Commands.GameWallet.Deposit;

public sealed class WalletDepositValidator : AbstractValidator<WalletDepositCommand>
{
    public WalletDepositValidator()
    {
        RuleFor(x => x.Quota)
            .NotEmpty().WithMessage($"{nameof(WalletDepositCommand.Quota)} is required.")
            .GreaterThanOrEqualTo(0).WithMessage($"{nameof(WalletDepositCommand.Quota)} must be greater than zero.");

    }
}

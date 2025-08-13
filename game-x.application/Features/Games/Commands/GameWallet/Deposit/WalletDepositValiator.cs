namespace game_x.application.Features.Games.Commands.GameWallet.Deposit;

public sealed class WalletDepositValidator : AbstractValidator<WalletDepositCommand>
{
    public WalletDepositValidator()
    {
        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage($"{nameof(WalletDepositCommand.Amount)} is required.")
            .GreaterThanOrEqualTo(0).WithMessage($"{nameof(WalletDepositCommand.Amount)} must be greater than zero.");

    }
}

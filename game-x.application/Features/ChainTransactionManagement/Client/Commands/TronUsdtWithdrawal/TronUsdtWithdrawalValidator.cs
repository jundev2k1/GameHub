namespace game_x.application.Features.ChainTransactionManagement.Client.Commands.TronUsdtWithdrawal;

public sealed class TronUsdtWithdrawalValidator : AbstractValidator<TronUsdtWithdrawalCommand>
{
    public TronUsdtWithdrawalValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero");
        RuleFor(x => x.To)
            .NotEmpty()
            .WithMessage($"{nameof(TronUsdtWithdrawalCommand.To)} is required.");
    }
}

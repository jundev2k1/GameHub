
namespace game_x.application.Features.ChainTransactions.Client.Commands.TronUsdtDeposit;

public sealed class TronUsdtDepositValidator : AbstractValidator<TronUsdtDepositCommand>
{
    public TronUsdtDepositValidator()
    {
        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage($"{nameof(TronUsdtDepositCommand.Amount)} is required.")
            .GreaterThanOrEqualTo(10).WithMessage($"{nameof(TronUsdtDepositCommand.Amount)} must be at least 10 USDT.");

        RuleFor(x => x.Note)
            .MaximumLength(200).WithMessage($"{nameof(TronUsdtDepositCommand.Note)} cannot exceed 200 characters.");
    }
}

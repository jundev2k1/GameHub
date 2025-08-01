
namespace game_x.application.Features.ChainTransactionManagement.Client.Commands.TronUsdtDeposit;

public sealed class TronUsdtDepositValidator : AbstractValidator<TronUsdtDepositCommand>
{
    public TronUsdtDepositValidator()
    {
        RuleFor(x => x.amount)
            .NotEmpty().WithMessage($"{nameof(TronUsdtDepositCommand.amount)} is required.")
            .GreaterThanOrEqualTo(10).WithMessage($"{nameof(TronUsdtDepositCommand.amount)} must be at least 10 USDT.");

        RuleFor(x => x.remark)
            .MaximumLength(200).WithMessage($"{nameof(TronUsdtDepositCommand.remark)} cannot exceed 200 characters.");
    }
}

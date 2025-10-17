
namespace game_x.application.Features.ChainTransactions.Client.Commands.TraceV2.TronUsdtDepositV2;

public sealed class TronUsdtDepositV2Validator : AbstractValidator<TronUsdtDepositV2Command>
{
    public TronUsdtDepositV2Validator()
    {
        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage($"{nameof(TronUsdtDepositV2Command.Amount)} is required.")
            .GreaterThanOrEqualTo(10).WithMessage($"{nameof(TronUsdtDepositV2Command.Amount)} must be at least 10 USDT.");

        RuleFor(x => x.Note)
            .MaximumLength(200).WithMessage($"{nameof(TronUsdtDepositV2Command.Note)} cannot exceed 200 characters.");
        
        RuleFor(x => x.Provider)
            .IsInEnum().WithMessage($"{nameof(TronUsdtDepositV2Command.Provider)} is not valid.");
    }
}
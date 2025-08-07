namespace game_x.application.Features.ChainTransactions.Shared.Commands.Callback.CryptoTransactionCallback;

public sealed class CryptoTransactionCallbackValidator : AbstractValidator<CryptoTransactionCallbackCommand>
{
    public CryptoTransactionCallbackValidator()
    {
        RuleFor(x => x.Data.ActualAmount).GreaterThan(0)
            .WithMessage("Amount must be greater than zero");
    }
}

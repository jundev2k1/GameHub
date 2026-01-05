namespace game_x.application.Features.Transactions.Admin.Commands.CreateTransaction;

public sealed class CreateTransactionValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid transaction type.")
            .Must(type => type is TransactionType.Deposit or TransactionType.Withdrawal).WithMessage("Transaction type just allow deposit or withdrawal.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.");
    }
}

namespace game_x.application.Features.Transactions.Admin.Commands.CreateTransaction;

public record CreateTransactionCommand(
    TransactionTypeRequest Type,
    string UserId,
    string OrderUId,
    Guid CryptoTokenId,
    decimal Amount,
    string Message) : ICommand;

public enum TransactionTypeRequest
{
    Deposit = 1,
    Withdrawal = 2,
    FastPayDeposit = 3,
}

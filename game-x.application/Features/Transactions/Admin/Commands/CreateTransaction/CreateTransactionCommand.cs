namespace game_x.application.Features.Transactions.Admin.Commands.CreateTransaction;

public record CreateTransactionCommand(TransactionType Type, string UserId, string Sno, Guid CryptoTokenId, decimal Amount, string Message) : ICommand;
 
namespace game_x.application.Features.Transactions.Admin.Commands.CancelTransaction;

public record CancelTransactionCommand(Guid TransactionId) : ICommand;

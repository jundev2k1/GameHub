namespace game_x.application.Features.ChainTransactions.Admin.Commands.AdminReviewWithdrawalOrder;

public record AdminReviewWithdrawalOrderCommand(Guid OrderId, ChainTransactionStatus OrderStatus) : ICommand;
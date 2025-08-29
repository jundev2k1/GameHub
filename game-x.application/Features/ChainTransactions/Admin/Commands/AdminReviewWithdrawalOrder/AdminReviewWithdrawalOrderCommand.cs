using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Admin.Commands.AdminReviewWithdrawalOrder;

public record AdminReviewWithdrawalOrderCommand(Guid? OrderId, TransactionStatus OrderStatus) : ICommand<ListTransactionInternalDto>;
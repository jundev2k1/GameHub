using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Features.Transactions.Admin.Commands.AdminReviewWithdrawalOrder;

public record AdminReviewWithdrawalOrderCommand(Guid? OrderId, TransactionStatus OrderStatus) : ICommand<ListTransactionInternalDto>;
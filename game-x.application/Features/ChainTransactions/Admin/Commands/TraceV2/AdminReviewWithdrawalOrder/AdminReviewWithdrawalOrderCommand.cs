using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Admin.Commands.TraceV2.AdminReviewWithdrawalOrder;

public record AdminReviewWithdrawalOrderCommand(Guid? OrderId, TransactionStatus OrderStatus) : ICommand<ListTransactionInternalDto>;
using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Admin.Commands.TraceV2.AdminReviewWithdrawalOrderV2;

public record AdminReviewWithdrawalOrderV2Command(Guid? OrderId, TransactionStatus OrderStatus) : ICommand<ListTransactionInternalDto>;
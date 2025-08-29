using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Events.OnWithdrawalOrderReviewed;

public record OnWithdrawalOrderReviewedEvent(TransactionInternalDto Transaction) : IApplicationEvent;
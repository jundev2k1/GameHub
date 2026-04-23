using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Events.Transaction.OnWithdrawalOrderReviewed;

public record OnWithdrawalOrderReviewedEvent(TransactionInternalDto Transaction) : IApplicationEvent;
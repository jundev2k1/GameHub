using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Events.Transactions.OnWithdrawalOrderReviewed;

public record OnWithdrawalOrderReviewedEvent(TransactionInternalDto Transaction) : IApplicationEvent;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Queries.GetMyGameTransactionDetail;

public record GetMyGameTransactionDetailQuery(Guid TransactionId) : IQuery<GameTransactionDetailDto>;
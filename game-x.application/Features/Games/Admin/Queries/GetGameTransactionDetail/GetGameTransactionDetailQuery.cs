using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Client.Queries.GetGameTransactionDetail;

public record GetGameTransactionDetailQuery(Guid TransactionId) : IQuery<GameTransactionDetailDto>;
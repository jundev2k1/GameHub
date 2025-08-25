using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Client.Queries.GetGameTransactionDetail;

public sealed class GetGameTransactionDetailHandler(
    IGameTransactionRepo gameTransactionRepo)
    : IQueryHandler<GetGameTransactionDetailQuery, GameTransactionDetailDto>
{
    public async Task<GameTransactionDetailDto> Handle(GetGameTransactionDetailQuery request, CancellationToken ct = default)
    {
        var result = await gameTransactionRepo.GetByIdAsync(request.TransactionId, ct);
        return result.Adapt<GameTransactionDetailDto>();
    }
}

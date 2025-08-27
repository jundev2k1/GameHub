using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Client.Queries.GetMyGameTransactionDetail;

public sealed class GetMyGameTransactionDetailHandler(
    IGameTransactionRepo gameTransactionRepo)
    : IQueryHandler<GetMyGameTransactionDetailQuery, GameTransactionDetailDto>
{
    public async Task<GameTransactionDetailDto> Handle(GetMyGameTransactionDetailQuery request, CancellationToken ct = default)
    {
        var result = await gameTransactionRepo.GetByIdAsync(request.TransactionId, ct);
        return result.Adapt<GameTransactionDetailDto>();
    }
}
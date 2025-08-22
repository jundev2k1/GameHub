using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Queries.GetMyGameTransactionDetail;

public sealed class GetMyGameTransactionDetailHandler(
    IUserAccessor userAccessor,
    IGameTransactionRepo  gameTransactionRepo)
    : IQueryHandler<GetMyGameTransactionDetailQuery, GameTransactionDetailDto>
{
    public async Task<GameTransactionDetailDto> Handle(GetMyGameTransactionDetailQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var result = await gameTransactionRepo.GetByIdAndUserIdAsync(userId, request.TransactionId, ct);
        return result.Adapt<GameTransactionDetailDto>();
    }
}
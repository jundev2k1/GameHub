using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Client.Queries.GetMyGameTransactionDetail;

public sealed class GetMyGameTransactionDetailHandler(
    IUserAccessor userAccessor,
    ITransactionRepo transactionRepo)
    : IQueryHandler<GetMyGameTransactionDetailQuery, TransactionExternalDetailDto>
{
    public async Task<TransactionExternalDetailDto> Handle(GetMyGameTransactionDetailQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var result = await transactionRepo.GetByIdAndUserIdAsync(userId, request.TransactionId, ct);
        return result.Adapt<TransactionExternalDetailDto>();
    }
}
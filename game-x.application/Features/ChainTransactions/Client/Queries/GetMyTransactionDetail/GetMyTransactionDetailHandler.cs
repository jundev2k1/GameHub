using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Client.Queries.GetMyTransactionDetail;

public sealed class GetMyTransactionDetailHandler(
    IUserAccessor userAccessor,
    ITransactionRepo transactionRepo)
    : IQueryHandler<GetMyTransactionDetailQuery, TransactionInternalDetailDto>
{
    public async Task<TransactionInternalDetailDto> Handle(GetMyTransactionDetailQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var result = await transactionRepo.GetByIdAndUserIdAsync(userId, request.TransactionId, ct);
        return result.Adapt<TransactionInternalDetailDto>();
    }
}
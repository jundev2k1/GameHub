using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGameTransactionDetail;

public sealed class GetGameTransactionDetailHandler(
    ITransactionRepo transactionRepo)
    : IQueryHandler<GetGameTransactionDetailQuery, TransactionExternalDetailDto>
{
    public async Task<TransactionExternalDetailDto> Handle(GetGameTransactionDetailQuery request, CancellationToken ct = default)
    {
        var result = await transactionRepo.GetExternalByIdAsync(request.TransactionId, ct);
        return result.Adapt<TransactionExternalDetailDto>();
    }
}

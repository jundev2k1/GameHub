using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Features.Transactions.Admin.Queries.GetTransactionDetailById;

public sealed class GetTransactionDetailByIdHandler(
    ITransactionRepo transactionRepo) : IQueryHandler<GetTransactionDetailByIdQuery, TransactionInternalDetailDto>
{
    public async Task<TransactionInternalDetailDto> Handle(GetTransactionDetailByIdQuery request, CancellationToken ct = default)
    {
        var dto = await transactionRepo.GetInternalByIdAsync(request.TransactionId, ct);
        var result = dto.Adapt<TransactionInternalDetailDto>();
        return result;
    }
}

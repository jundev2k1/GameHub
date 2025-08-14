using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.UserUsdtLedgers.Dtos;

namespace game_x.application.Features.ChainTransactions.Client.Queries.GetUsdtLedgerDetail;

public sealed class GetUserUsdtLedgerDetailHandler(
    IUserAccessor userAccessor,
    IUserUsdtLedgerRepo userUsdtLedgerRepo)
    : IQueryHandler<GetUserUsdtLedgerDetailQuery, UserUsdtLedgerDetailDto>
{
    public async Task<UserUsdtLedgerDetailDto> Handle(GetUserUsdtLedgerDetailQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var result = await userUsdtLedgerRepo.GetDetailByUserAsync(userId, request.UsdtLedgerId, ct);
        
        return result.Adapt<UserUsdtLedgerDetailDto>();
    }
}
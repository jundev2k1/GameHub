using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Kyc.Queries.GetKycStatus;

public sealed class GetKycStatusHandler(IUserRepo userRepo, IUserAccessor userAccessor)
    : IQueryHandler<GetKycStatusQuery, GetKycStatusResult>
{
    public async Task<GetKycStatusResult> Handle(GetKycStatusQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var (status, rejectionReason) = await userRepo.GetKycStatusAsync(userId, ct);

        return new GetKycStatusResult(status, rejectionReason);
    }
}

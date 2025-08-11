using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.BankAccountVerifications.Queries.GetBankAccountStatus;

public sealed class GetBankAccountHandler(IUserRepo userRepo, IUserAccessor userAccessor)
    : IQueryHandler<GetBankAccountStatusQuery, GetBankAccountStatusResult>
{
    public async Task<GetBankAccountStatusResult> Handle(GetBankAccountStatusQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var (status, rejectionReason) = await userRepo.GetKycStatusAsync(userId, ct);

        return new GetBankAccountStatusResult(status, rejectionReason);
    }
}

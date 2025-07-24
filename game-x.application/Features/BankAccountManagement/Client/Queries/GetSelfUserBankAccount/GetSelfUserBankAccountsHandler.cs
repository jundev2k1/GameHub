using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.BankAccountManagement.Dtos;

namespace game_x.application.Features.BankAccountManagement.Client.Queries.GetSelfUserBankAccount;

public sealed class GetSelfUserBankAccountsHandler(
    IBankAccountRepo bankAccountRepo,
    IUserAccessor userAccessor)
    : IQueryHandler<GetSelfUserBankAccountsQuery, GetSelfUserBankAccountsResult>
{
    public async Task<GetSelfUserBankAccountsResult> Handle(GetSelfUserBankAccountsQuery request, CancellationToken ct = default)
    {
        var targetUserId = userAccessor.GetUserId();
        var accounts = await bankAccountRepo.GetsByOwnerIdAsync(targetUserId, ct);

        var result = accounts
            .Select(account =>
            {
                var data = account.Adapt<BankAccountDetailDto>();
                data.BankAccountCode = account.PublicId.ToString();
                return data;
            })
            .ToArray();

        return new GetSelfUserBankAccountsResult(result);
    }
}

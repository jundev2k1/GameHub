using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.BankAccountManagement.Dtos;
using game_x.application.Features.BankAccountManagement.Client.Queries.GetSelfUserBankAccount;

namespace game_x.application.Features.BankAccountManagement.Staff.Queries.GetBankAccountForUserByStaff;

public sealed class GetBankAccountsForUserByStaffHandler(
    IBankAccountRepo bankAccountRepo)
    : IQueryHandler<GetBankAccountsForUserByStaffQuery, GetSelfUserBankAccountsResult>
{
    public async Task<GetSelfUserBankAccountsResult> Handle(GetBankAccountsForUserByStaffQuery request, CancellationToken cancellationToken)
    {

        var accounts = await bankAccountRepo.GetsByOwnerIdAsync(request.UserId, cancellationToken);

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

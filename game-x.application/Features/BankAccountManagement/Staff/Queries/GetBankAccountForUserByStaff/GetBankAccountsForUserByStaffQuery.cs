using game_x.application.Features.BankAccountManagement.Dtos;
using game_x.application.Features.BankAccountManagement.Client.Queries.GetSelfUserBankAccount;

namespace game_x.application.Features.BankAccountManagement.Staff.Queries.GetBankAccountForUserByStaff;

public record GetBankAccountsForUserByStaffQuery(string UserId) : IQuery<GetSelfUserBankAccountsResult>;

public record GetBankAccountsForUserByStaffResult(BankAccountDetailDto[] Items);

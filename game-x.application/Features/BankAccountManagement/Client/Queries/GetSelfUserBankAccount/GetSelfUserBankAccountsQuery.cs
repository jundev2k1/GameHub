using game_x.application.Features.BankAccountManagement.Dtos;

namespace game_x.application.Features.BankAccountManagement.Client.Queries.GetSelfUserBankAccount;

public record GetSelfUserBankAccountsQuery() : IQuery<GetSelfUserBankAccountsResult>;

public record GetSelfUserBankAccountsResult(BankAccountDetailDto[] Items);

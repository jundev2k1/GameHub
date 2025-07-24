using game_x.application.Features.BankAccountManagement.Dtos;

namespace game_x.application.Features.BankAccountManagement.Client.Queries.GetBankAccountDetailByUser;

public record GetBankAccountDetailByUserQuery(Guid BankAccountCode) : IQuery<BankAccountDetailDto>;

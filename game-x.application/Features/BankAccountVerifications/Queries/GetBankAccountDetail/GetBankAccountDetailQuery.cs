using game_x.application.Features.BankAccountVerifications.Dtos;

namespace game_x.application.Features.BankAccountVerifications.Queries.GetBankAccountDetail;

public record GetBankAccountDetailQuery(Guid BankAccountId) : IQuery<BankAccountProfileDto>;

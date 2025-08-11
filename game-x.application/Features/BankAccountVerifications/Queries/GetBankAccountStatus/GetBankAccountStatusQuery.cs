namespace game_x.application.Features.BankAccountVerifications.Queries.GetBankAccountStatus;

public record GetBankAccountStatusQuery : IQuery<GetBankAccountStatusResult>;

public record GetBankAccountStatusResult(KycStatus Status, string? RejectionReason = null);

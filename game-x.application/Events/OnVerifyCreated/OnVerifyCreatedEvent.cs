using game_x.application.Features.Accounts.User.Dtos;
using game_x.application.Features.BankAccountVerifications.Dtos;
using game_x.application.Features.Kyc.Dtos;

namespace game_x.application.Events.OnVerifyCreated;

public record OnVerifyCreatedEvent(
    string UserId,
    VerificationStatusType VerificationStatusType,
    UserKycListItemDto? UserKycDto = null,
    BankAccountListItemDto? BankAccountDto = null
) : IApplicationEvent;

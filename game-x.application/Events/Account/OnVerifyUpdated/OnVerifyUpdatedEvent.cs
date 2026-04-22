using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Events.OnVerifyUpdated;

public record OnVerifyUpdatedEvent(string UserId, Guid PublicId, VerificationStatusDto VerificationStatus) : IApplicationEvent;
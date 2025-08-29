using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Events.OnVerifyCreated;

public record OnVerifyCreatedEvent(string UserId, VerificationCreatedDto VerificationCreated) : IApplicationEvent;
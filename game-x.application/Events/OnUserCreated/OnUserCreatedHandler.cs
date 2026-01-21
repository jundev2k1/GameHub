using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Services.EmailProcessor;
using game_x.application.Services.Verification;

namespace game_x.application.Events.OnUserCreated;

public sealed class OnUserCreatedHandler(IEmailVerificationProcessor emailVerification, ISpamProtectionCacheService spamProtection)
    : IApplicationEventHandler<OnUserCreatedEvent>
{
    public async Task Handle(OnUserCreatedEvent @event, CancellationToken ct = default)
    {
        // Send verification code via mail
        emailVerification.SendVerificationEmail(@event.Email, VerificationPurposes.EmailVerification);

        // Anti-spam resend email, block resend for 60 seconds
        await spamProtection.SetResendCooldownAsync(@event.Email);
    }
}
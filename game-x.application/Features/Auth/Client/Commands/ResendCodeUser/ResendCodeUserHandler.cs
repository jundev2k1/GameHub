using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Services.EmailProcessor;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Auth.Client.Commands.ResendCodeUser;

public sealed class ResendCodeUserHandler(
    IEmailVerificationProcessor emailVerification,
    IUserRepo userRepo,
    ISpamProtectionCacheService spamProtection) : ICommandHandler<ResendCodeUserCommand>
{
    public async Task<Unit> Handle(ResendCodeUserCommand request, CancellationToken ct = default)
    {
        var targetUser = await userRepo.GetUserByEmailAsync(request.Email, ct);
        if (targetUser.EmailConfirmed) throw new BadRequestException(MessageCode.User.EmailAlreadyVerified);

        if (!await spamProtection.CanResendVerifyCodeAsync(request.Email))
        {
            var waitTime = await spamProtection.GetResendWaitTimeAsync(request.Email);
            var cooldownSeconds = waitTime.HasValue ? (int)waitTime.Value.TotalSeconds : 0;
            throw new BadRequestException(
                MessageCode.User.VerifyResendCooldown,
                $"Resend code {cooldownSeconds}",
                new { Cooldown = cooldownSeconds });
        }

        emailVerification.SendVerificationEmail(request.Email);
        await spamProtection.SetResendCooldownAsync(request.Email, TimeSpan.FromSeconds(60));
        return Unit.Value;
    }
}

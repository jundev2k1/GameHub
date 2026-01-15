using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.Services.EmailProcessor;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Services.Verification;
using game_x.share.Extensions;

namespace game_x.application.Features.Auth.Client.Commands.ResendCodeUser;

public sealed class ResendCodeUserHandler(
    IAuthService authService,
    IUserRepo userRepo,
    IUserAccessor userAccessor,
    IEmailVerificationProcessor emailVerification,
    ISpamProtectionCacheService spamProtection) : ICommandHandler<ResendCodeUserCommand>
{
    public async Task<Unit> Handle(ResendCodeUserCommand request, CancellationToken ct = default)
    {
        var targetEmail = request.Email;

        // Case: ForgotPassword, validate current user identity
        if (request.Purpose is VerificationPurposes.ChangePassword or VerificationPurposes.Withdrawal)
        {
            var userId = userAccessor.GetUserId();
            var targetUser = await userRepo.GetUserByIdAsync(userId, ct);

            // Check: user must have "User" role
            var role = await authService.GetRolesAsync(targetUser);
            if (!role.IsUser)
                throw new ForbiddenException();

            // Check: email must be confirmed before requesting password reset
            if (!targetUser.EmailConfirmed)
                throw new BadRequestException(MessageCode.User.UserNotConfirmed);

            targetEmail = targetUser.Email;
        }

        if (targetEmail.IsNullOrEmpty())
            throw new UnauthorizedException();

        // Check: prevent resend if cooldown is still active
        if (!await spamProtection.CanResendVerifyCodeAsync(targetEmail!))
        {
            var waitTime = await spamProtection.GetResendWaitTimeAsync(targetEmail!);
            var cooldownSeconds = waitTime.HasValue ? (int)waitTime.Value.TotalSeconds : 0;

            // Return cooldown error with remaining wait time
            throw new BadRequestException(
                MessageCode.User.VerifyResendCooldown,
                $"Resend code {cooldownSeconds}",
                new { Cooldown = cooldownSeconds });
        }

        // Action: send verification email
        emailVerification.SendVerificationEmail(targetEmail!, request.Purpose);

        // Action: set resend cooldown (default: 60s)
        await spamProtection.SetResendCooldownAsync(targetEmail!, TimeSpan.FromSeconds(60));
        return Unit.Value;
    }
}

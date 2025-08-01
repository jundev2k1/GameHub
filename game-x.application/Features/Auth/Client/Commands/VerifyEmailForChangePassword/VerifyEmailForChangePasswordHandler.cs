using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.Services.EmailProcessor;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Services.Verification;

namespace game_x.application.Features.Auth.Client.Commands.VerifyEmailForChangePassword;

public sealed class VerifyEmailForChangePasswordHandler(
    IEmailVerificationProcessor emailVerification,
    IUserRepo userRepo,
    IAuthService authService,
    IUserAccessor userAccessor,
    ISpamProtectionCacheService spamProtection,
    IResetTokenCacheService resetTokenCache)
    : ICommandHandler<VerifyEmailForChangePasswordCommand, VerifyEmailForChangePasswordResult>
{
    public async Task<VerifyEmailForChangePasswordResult> Handle(VerifyEmailForChangePasswordCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        // 1. Validate the valid user and check role user
        var targetUser = await userRepo.GetUserByIdAsync(userId, ct);
        var role = await authService.GetRolesAsync(targetUser);
        if (!role.IsUser) throw new ForbiddenException();

        if (!targetUser.EmailConfirmed)
            throw new BadRequestException(MessageCode.User.UserNotConfirmed);

        // 2. Check if email is temporarily locked due to too many failed attempts
        var isLocked = await spamProtection.IsVerifyLockedAsync(targetUser.Email!);
        if (isLocked)
        {
            var retryTime = await spamProtection.GetVerifyRetryAfterAsync(targetUser.Email!);
            var retrySeconds = (int)retryTime.Value.TotalSeconds;
            throw new BadRequestException(
                MessageCode.User.VerifyTooManyFailedAttempts,
                new { Cooldown = retrySeconds });
        }

        // 3. Validate the provided verification code
        var isValid = emailVerification.VerifyEmail(targetUser.Email!, request.Code, VerificationPurposes.ChangePassword);
        if (!isValid)
        {
            await spamProtection.RegisterVerifyFailureAsync(targetUser.Email!);
            throw new BadRequestException(MessageCode.System.InvalidVerifyCode);
        }

        // 4. Confirm user's email if not already confirmed
        var token = Guid.NewGuid().ToString("N");
        resetTokenCache.StoreToken(token, targetUser.Email!, TimeSpan.FromMinutes(30));

        // 5. Reset failed attempt counter on success
        await spamProtection.ResetVerifyAttemptAsync(targetUser.Email!);
        return new VerifyEmailForChangePasswordResult(token, DateTime.UtcNow.AddMinutes(30));
    }
}

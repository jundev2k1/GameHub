using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Services.EmailProcessor;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Services.Verification;

namespace game_x.application.Features.Auth.Client.Commands.VerifyEmailForResetPassword;

public sealed class VerifyEmailForResetPasswordHandler(
    IEmailVerificationProcessor emailVerification,
    IUserRepo userRepo,
    IAuthService authService,
    ISpamProtectionCacheService spamProtection,
    IResetTokenCacheService resetTokenCache)
    : ICommandHandler<VerifyEmailForResetPasswordCommand, VerifyEmailForPasswordResetResult>
{
    public async Task<VerifyEmailForPasswordResetResult> Handle(VerifyEmailForResetPasswordCommand request, CancellationToken ct = default)
    {
        // 1. Check if email is temporarily locked due to too many failed attempts
        var isLocked = await spamProtection.IsVerifyLockedAsync(request.Email);
        if (isLocked)
        {
            var retryTime = await spamProtection.GetVerifyRetryAfterAsync(request.Email);
            var retrySeconds = (int)retryTime.Value.TotalSeconds;
            throw new BadRequestException(
                MessageCode.User.VerifyTooManyFailedAttempts,
                new { Cooldown = retrySeconds });
        }

        // 2. Validate the provided verification code
        var isValid = emailVerification.VerifyEmail(request.Email, request.Code, VerificationPurposes.ForgotPassword);
        if (!isValid)
        {
            await spamProtection.RegisterVerifyFailureAsync(request.Email);
            throw new BadRequestException(MessageCode.System.InvalidVerifyCode);
        }

        // 3. Validate the valid user and check role user
        var targetUser = await userRepo.GetUserByEmailAsync(request.Email, ct);
        var role = await authService.GetRolesAsync(targetUser);
        if (!role.IsUser) throw new ForbiddenException();

        // 4. Confirm user's email if not already confirmed
        var token = Guid.NewGuid().ToString("N");
        resetTokenCache.StoreToken(token, request.Email, TimeSpan.FromMinutes(30));

        // 5. Reset failed attempt counter on success
        await spamProtection.ResetVerifyAttemptAsync(request.Email);
        return new VerifyEmailForPasswordResetResult(token, DateTime.UtcNow.AddMinutes(30));
    }
}

using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Services.EmailProcessor;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Services.Verification;

namespace game_x.application.Features.Auth.Client.Commands.VerifyEmailForRegistration;

public sealed class VerifyEmailForRegistrationHandler(
    IEmailVerificationProcessor emailVerification,
    IUserRepo userRepo,
    IUnitOfWork unitOfWork,
    ISpamProtectionCacheService spamProtection) : ICommandHandler<VerifyEmailForRegistrationCommand>
{
    public async Task<Unit> Handle(VerifyEmailForRegistrationCommand request, CancellationToken ct = default)
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
        var isValid = emailVerification.VerifyEmail(request.Email, request.Code, VerificationPurposes.EmailVerification);
        if (!isValid)
        {
            await spamProtection.RegisterVerifyFailureAsync(request.Email);
            throw new BadRequestException(MessageCode.System.InvalidVerifyCode);
        }

        // 3. Confirm user's email if not already confirmed
        await userRepo.UpdateByEmailAsync(
            request.Email,
            user =>
            {
                if (user.EmailConfirmed)
                    throw new BadRequestException(MessageCode.User.EmailAlreadyVerified);

                user.ConfirmEmail();
            },
            ct);
        await unitOfWork.SaveChangesAsync(ct);

        // 4. Reset failed attempt counter on success
        await spamProtection.ResetVerifyAttemptAsync(request.Email);
        return Unit.Value;
    }
}

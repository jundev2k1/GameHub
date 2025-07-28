using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Services.EmailProcessor;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Auth.Client.Commands.VerifyEmailUser;

public sealed class VerifyEmailUserHandler(
    IEmailVerificationProcessor emailVerification,
    IUserRepo userRepo,
    IUnitOfWork unitOfWork,
    ISpamProtectionCacheService spamProtection) : ICommandHandler<VerifyEmailUserCommand>
{
    public async Task<Unit> Handle(VerifyEmailUserCommand request, CancellationToken ct = default)
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
        var isValid = emailVerification.VerifyEmail(request.Email, request.Code);
        if (!isValid)
        {
            await spamProtection.RegisterVerifyFailureAsync(request.Email);
            throw new BadRequestException(MessageCode.System.InvalidVerifyCode);
        }

        // 3. Confirm user's email if not already confirmed
        await userRepo.UpdateByEmailAsync(
            request.Email,
            user => user.ConfirmEmail(),
            ct);
        await unitOfWork.SaveChangesAsync(ct);

        // 4. Reset failed attempt counter on success
        await spamProtection.ResetVerifyAttemptAsync(request.Email);
        return Unit.Value;
    }
}

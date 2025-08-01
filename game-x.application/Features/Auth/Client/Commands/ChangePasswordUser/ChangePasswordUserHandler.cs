using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.Extensions;

namespace game_x.application.Features.Auth.Client.Commands.ChangePasswordUser;

public sealed class ChangePasswordUserHandler(
    IUserAccessor userAccessor,
    IAuthService authService,
    IUserRepo userRepo,
    IUnitOfWork unitOfWork,
    IResetTokenCacheService resetTokenCache) : ICommandHandler<ChangePasswordUserCommand>
{
    public async Task<Unit> Handle(ChangePasswordUserCommand request, CancellationToken ct = default)
    {
        var tokenEmail = resetTokenCache.GetEmailByToken(request.Token);
        if (tokenEmail.IsNullOrWhiteSpace())
            throw new BadRequestException(MessageCode.System.InvalidOrMissingToken);

        // Validate user information from access token
        var userId = userAccessor.GetUserId();
        var targetUser = await userRepo.GetUserByIdAsync(userId, ct);
        var role = await authService.GetRolesAsync(targetUser);
        if (!role.IsUser)
            throw new ForbiddenException();

        // Check if Email from token in cache is valid
        if (targetUser.Email != tokenEmail)
            throw new BadRequestException(MessageCode.System.InvalidOrMissingToken);

        // Handle change password
        await unitOfWork.WithTransactionAsync(async () =>
        {
            // Case: Reset password
            await authService.ChangePasswordAsync(
                targetUser,
                request.OldPassword.Trim(),
                request.NewPassword.Trim(), ct);
            if (targetUser.EmailConfirmed) return;

            // Case: Confirm email if user not confirmed
            await userRepo.UpdateAsync(
                userId,
                user => user.ConfirmEmail(),
                ct);
        }, ct);

        // Remove token after change password successfully
        resetTokenCache.InvalidateToken(request.Token);

        return Unit.Value;
    }
}

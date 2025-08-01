using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Auth.Client.Commands.ResetPassword;

public sealed class ResetPasswordHandler(
    IUserAccessor userAccessor,
    IAuthService authService,
    IUserRepo userRepo,
    IUnitOfWork unitOfWork) : ICommandHandler<ResetPasswordCommand>
{
    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken ct = default)
    {
        // Validate user information from access token
        var userId = userAccessor.GetUserId();
        var targetUser = await userRepo.GetUserByIdAsync(userId, ct);
        var role = await authService.GetRolesAsync(targetUser);
        if (!role.IsUser)
            throw new ForbiddenException();

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

        return Unit.Value;
    }
}

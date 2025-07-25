using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IAuthService authService) : ICommandHandler<ChangePasswordCommand>
{
    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var user = await userRepo.GetUserByIdAsync(userId, ct);

        await ChangePasswordAsync(user, request.Password, request.NewPassword);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }

    private async Task ChangePasswordAsync(User user, string password, string newPassword)
    {
        var isValidPassword = await authService.IsValidPasswordAsync(user, password);
        if (!isValidPassword) throw new BadRequestException(MessageCode.User.UserChangePasswordFail);
        
        await authService.ChangePasswordAsync(user, password, newPassword);
    }
}

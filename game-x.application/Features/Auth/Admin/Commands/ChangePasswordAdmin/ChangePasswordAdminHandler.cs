using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Auth.Admin.Commands.ChangePasswordAdmin;

public sealed class ChangePasswordAdminHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IAuthService authService) : ICommandHandler<ChangePasswordAdminCommand>
{
    public async Task<Unit> Handle(ChangePasswordAdminCommand request, CancellationToken ct = default)
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

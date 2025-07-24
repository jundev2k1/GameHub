using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnInvalidUserChanged;

namespace game_x.application.Features.AccountManagement.Admin.Commands.UpdateUserStatusByAdmin;

public sealed class UpdateUserStatusByAdminHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IRoleRepo roleRepo,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<UpdateUserStatusByAdminCommand>
{
    public async Task<Unit> Handle(UpdateUserStatusByAdminCommand request, CancellationToken ct = default)
    {
        var isValidRole = await roleRepo.IsValidUserAsync(userId: request.UserId, role: AppRoles.User, ct);
        if (!isValidRole)
            throw new BadRequestException(MessageCode.System.InvalidParameters);
        
        await userRepo.UpdateAsync(
            request.UserId,
            user => user.UpdateStatus(request.Status),
            ct);
        await unitOfWork.SaveChangesAsync(ct);

        _ = eventDispatcher.Publish(new OnInvalidUserChangedEvent(), ct);
        return Unit.Value;
    }
}

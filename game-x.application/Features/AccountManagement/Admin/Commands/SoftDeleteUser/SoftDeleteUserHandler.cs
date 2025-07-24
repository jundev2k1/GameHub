using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnInvalidUserChanged;

namespace game_x.application.Features.AccountManagement.Admin.Commands.SoftDeleteUser;

public sealed class SoftDeleteUserCommandHandler(
    IUserRepo userRepo,
    IRoleRepo roleRepo,
    IUnitOfWork unitOfWork,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<SoftDeleteUserCommand>
{
    public async Task<Unit> Handle(SoftDeleteUserCommand request, CancellationToken ct = default)
    {
        var isValidRole = await roleRepo.IsValidUserAsync(userId: request.UserId, role: AppRoles.User, ct);
        if (!isValidRole)
            throw new BadRequestException(MessageCode.System.InvalidParameters);
        
        await userRepo.UpdateAsync(
            request.UserId,
            user => user.IsDeleted = true, ct);
        
        await unitOfWork.SaveChangesAsync(ct);
        _ = eventDispatcher.Publish(new OnInvalidUserChangedEvent(), ct);
        return Unit.Value;
    }
}

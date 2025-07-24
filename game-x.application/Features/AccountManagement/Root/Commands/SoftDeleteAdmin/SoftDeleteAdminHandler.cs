using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnInvalidUserChanged;

namespace game_x.application.Features.AccountManagement.Root.Commands.SoftDeleteAdmin;

public sealed class SoftDeleteAdminCommandHandler(
    IUserRepo userRepo,
    IRoleRepo roleRepo,
    IUnitOfWork unitOfWork,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<SoftDeleteAdminCommand>
{
    public async Task<Unit> Handle(SoftDeleteAdminCommand request, CancellationToken ct = default)
    {
        var isValidRole = await roleRepo.IsValidUserAsync(userId: request.UserId, role: AppRoles.Admin, ct);
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

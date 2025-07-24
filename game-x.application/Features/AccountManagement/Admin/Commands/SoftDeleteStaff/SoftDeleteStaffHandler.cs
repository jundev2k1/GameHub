using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnInvalidUserChanged;

namespace game_x.application.Features.AccountManagement.Admin.Commands.SoftDeleteStaff;

public sealed class SoftDeleteStaffHandler(
    IUserRepo userRepo,
    IUnitOfWork unitOfWork,
    IRoleRepo roleRepo,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<SoftDeleteStaffCommand>
{
    public async Task<Unit> Handle(SoftDeleteStaffCommand request, CancellationToken ct = default)
    {
        var isValidRole = await roleRepo.IsValidUserAsync(userId: request.UserId, role: AppRoles.Staff, ct);
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

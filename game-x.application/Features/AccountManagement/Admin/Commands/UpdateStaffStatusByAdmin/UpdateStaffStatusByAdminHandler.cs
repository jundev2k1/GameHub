using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnInvalidUserChanged;

namespace game_x.application.Features.AccountManagement.Admin.Commands.UpdateStaffStatusByAdmin;

public sealed class UpdateStaffStatusByAdminHandler(
    IUnitOfWork unitOfWork,
    IRoleRepo roleRepo,
    IUserRepo userRepo,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<UpdateStaffStatusByAdminCommand>
{
    public async Task<Unit> Handle(UpdateStaffStatusByAdminCommand request, CancellationToken ct = default)
    {
        var isValidRole = await roleRepo.IsValidUserAsync(userId: request.StaffId, role: AppRoles.Staff, ct);
        if (!isValidRole)
            throw new BadRequestException(MessageCode.System.InvalidParameters);
        
        await userRepo.UpdateAsync(
            request.StaffId!,
            user => user.UpdateStatus(request.Status),
            ct);
        await unitOfWork.SaveChangesAsync(ct);

        _ = eventDispatcher.Publish(new OnInvalidUserChangedEvent(), ct);
        return Unit.Value;
    }
}

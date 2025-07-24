using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.domain.Identity;

namespace game_x.application.Features.AccountManagement.Admin.Commands.CreateStaff;

public sealed class CreateStaffCommandHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IUserAccessor userAccessor) : ICommandHandler<CreateStaffCommand>
{
    public async Task<Unit> Handle(CreateStaffCommand request, CancellationToken ct)
    {
        var adminId = userAccessor.GetUserId();

        await userRepo.AddUserAsync(
            user: new AppUser
            {
                UserName = request.Username,
                EmailConfirmed = false,
                StaffExtension = StaffExtension.Create(adminId),
            },
            request.Password,
            AppRole.Of(AppRoles.Staff),
            ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}

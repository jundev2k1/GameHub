using game_x.application.Contract.Persistence.Repo;
using game_x.application.Utils;
using UserEntity = game_x.domain.Entities.User;

namespace game_x.application.Features.Accounts.Root.Commands.CreateAdmin;

public sealed class CreateAdminCommandHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo) : ICommandHandler<CreateAdminCommand>
{
    public async Task<Unit> Handle(CreateAdminCommand request, CancellationToken ct = default)
    {
        var dummyEmail = EmailUtils.GenerateDummyEmail(AppRoles.Admin);
        var newUser = UserEntity.Create(request.Username, dummyEmail);
        newUser.ConfirmEmail();

        await userRepo.AddUserAsync(
            user: newUser,
            rawPassword: request.Password,
            role: AppRole.Of(AppRoles.Admin),
            ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}

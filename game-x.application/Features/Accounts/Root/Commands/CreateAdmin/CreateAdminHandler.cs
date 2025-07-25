using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Accounts.Root.Commands.CreateAdmin;

public sealed class CreateAdminCommandHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo) : ICommandHandler<CreateAdminCommand>
{
    public async Task<Unit> Handle(CreateAdminCommand request, CancellationToken ct = default)
    {
        var newUser = domain.Entities.User.Create(request.Username, string.Empty);
        await userRepo.AddUserAsync(
            user: newUser,
            rawPassword: request.Password,
            role: AppRole.Of(AppRoles.Admin),
            ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}

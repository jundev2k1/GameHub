using game_x.application.Contract.Persistence.Repo;
using game_x.domain.Identity;

namespace game_x.application.Features.AccountManagement.Root.Commands.CreateAdmin;

public sealed class CreateAdminCommandHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo) : ICommandHandler<CreateAdminCommand>
{
    public async Task<Unit> Handle(CreateAdminCommand request, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await userRepo.AddUserAsync(
                user: new()
                {
                    UserName = request.Username,
                    EmailConfirmed = false,
                },
                rawPassword: request.Password,
                role: AppRole.Of(AppRoles.Admin),
                ct);
            await unitOfWork.CommitAsync(ct);
        }, ct);

        return Unit.Value;
    }
}

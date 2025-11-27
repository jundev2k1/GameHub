using game_x.application.Contract.Persistence.Repo;
using UserEntity = game_x.domain.Entities.User;

namespace game_x.application.Features.Accounts.Admin.Commands.CreateTalent;

public sealed class CreateTalentHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo) : ICommandHandler<CreateTalentCommand>
{
    public async Task<Unit> Handle(CreateTalentCommand request, CancellationToken ct = default)
    {
        var newUser = UserEntity.Create(request.Username, $"{request.Username}@gamex.local");
        newUser.ConfirmEmail();

        await userRepo.AddUserAsync(
            user: newUser,
            rawPassword: request.Password,
            role: AppRole.Of(AppRoles.Talent),
            ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}

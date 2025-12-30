using game_x.application.Contract.Persistence.Repo;
using UserEntity = game_x.domain.Entities.User;

namespace game_x.application.Features.Accounts.Admin.Commands.CreateCustomerSupport;

public sealed class CreateCustomerSupportHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo) : ICommandHandler<CreateCustomerSupportCommand>
{
    public async Task<Unit> Handle(CreateCustomerSupportCommand request, CancellationToken ct = default)
    {
        var newUser = UserEntity.Create(
            userName: request.Email,
            email: request.Email,
            nickName: request.Nickname,
            notes: request.Notes);
        await userRepo.AddUserAsync(
            user: newUser,
            rawPassword: request.Password,
            role: AppRole.Of(AppRoles.Cs),
            ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
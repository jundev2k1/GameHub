using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Accounts.Admin.Commands.CreateCustomerSupport;

public sealed class CreateCustomerSupportHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo) : ICommandHandler<CreateCustomerSupportCommand>
{
    public async Task<Unit> Handle(CreateCustomerSupportCommand request, CancellationToken ct = default)
    {
        var newUser = domain.Entities.User.Create(request.Username, string.Empty);
        await userRepo.AddUserAsync(
            user: newUser,
            rawPassword: request.Password,
            role: AppRole.Of(AppRoles.Cs),
            ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
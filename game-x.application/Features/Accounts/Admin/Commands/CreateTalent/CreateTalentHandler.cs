using game_x.application.Contract.Persistence.Repo;
using UserEntity = game_x.domain.Entities.User;

namespace game_x.application.Features.Accounts.Admin.Commands.CreateTalent;

public sealed class CreateTalentHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo) : ICommandHandler<CreateTalentCommand>
{
    public async Task<Unit> Handle(CreateTalentCommand request, CancellationToken ct = default)
    {
        var isExistUsername = await userRepo.IsExistUsernameAsync(request.Username, ct);
        if (isExistUsername) throw new BadRequestException(MessageCode.User.UserAlreadyExists);

        var isExistNickname = await userRepo.IsExistNicknameAsync(request.Nickname, ct);
        if (isExistNickname) throw new BadRequestException(MessageCode.User.NicknameAlreadyExists);

        var userId = Guid.CreateVersion7().ToString();

        // Create user info
        var newUser = UserEntity.Create(request.Username, $"{request.Username}@gamex.local");
        newUser.Id = userId;
        newUser.ConfirmEmail();

        // Add wallet
        var wallet = TalentWallet.Create(userId);
        newUser.AddTalentWallet(wallet);

        await userRepo.AddUserAsync(
            user: newUser,
            rawPassword: request.Password,
            role: AppRole.Of(AppRoles.Talent),
            ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}

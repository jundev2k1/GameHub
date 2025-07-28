using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Auth.Commands.Register.Client;

public sealed class RegistUserHandler(IUnitOfWork unitOfWork, IUserRepo userRepo)
    : ICommandHandler<RegistUserCommand, RegistUserResult>
{
    public async Task<RegistUserResult> Handle(RegistUserCommand request, CancellationToken ct = default)
    {
        var isExistMail = await userRepo.IsExistEmailAsync(request.Email, ct);
        if (isExistMail) throw new BadRequestException(MessageCode.User.EmailAlreadyExists);

        var isExistNickname = await userRepo.IsExistNicknameAsync(request.Nickname, ct);
        if (isExistNickname) throw new BadRequestException(MessageCode.User.NicknameAlreadyExists);

        var userId = string.Empty;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            var registUser = User.Create(
                userName: request.Email,
                email: request.Email,
                nickName: request.Nickname);
            await userRepo.AddUserAsync(registUser, request.Password, AppRole.Of(AppRoles.User), ct);
            userId = registUser.Id;
        }, ct);

        return new RegistUserResult(userId);
    }
}

using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserCreated;

<<<<<<<< HEAD:game-x.application/Features/Auth/Commands/Register/RegisterUser/RegisterUserHandler.cs
namespace game_x.application.Features.Auth.Commands.Register.RegisterUser;
========
namespace game_x.application.Features.Auth.Client.Commands.Register.Client;
>>>>>>>> 2e8dcb08b813dd652b6d058688d6ad8c4fed68a9:game-x.application/Features/Auth/Client/Commands/Register/Client/RegistUserHandler.cs

public sealed class RegisterUserHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<RegisterUserCommand, RegisterUserResult>
{
    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken ct = default)
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

        await eventDispatcher.Publish(new OnUserCreatedEvent(request.Email), ct);
        return new RegisterUserResult(userId);
    }
}

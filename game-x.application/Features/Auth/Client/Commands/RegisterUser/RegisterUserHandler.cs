using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserCreated;
using game_x.application.Utils;
using game_x.share.ExternalApi.GameProvider.Dtos.Register;
using UserEntity = game_x.domain.Entities.User;

namespace game_x.application.Features.Auth.Client.Commands.RegisterUser;

public sealed class RegisterUserHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IUserBalanceRepo userBalanceRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    IGameProviderService gameProvider,
    IGameAesEncryptor aesEncryptor,
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
            var registerUser = UserEntity.Create(
                userName: request.Email,
                email: request.Email,
                nickName: request.Nickname);
            var (urexUserName, urexPassword, urexRebateset) = await RegisterGameProviderUser(request.Nickname);
            var urexUser = UserExtend.Create(
                gameProviderAccount: urexUserName,
                gameProviderPassword: aesEncryptor.Encrypt(urexPassword),
                gameProviderNickname: registerUser.Nickname,
                gameProviderRebateset: urexRebateset);
            registerUser.AddUserExtend(urexUser);

            await userRepo.AddUserAsync(registerUser, request.Password, AppRole.Of(AppRoles.User), ct);
            await CreateUserBalancesAsync(registerUser);
            userId = registerUser.Id;
        }, ct);

        await eventDispatcher.Publish(new OnUserCreatedEvent(request.Email), ct);
        return new RegisterUserResult(userId);
    }

    private async Task CreateUserBalancesAsync(UserEntity user)
    {
        var activeTokens = await cryptoTokenRepo.GetAsync();

        var balances = activeTokens.Select(token => UserBalance.Create(
            userId: user.Id,
            cryptoTokenId: token.Id,
            amount: 0)).ToList();

        await userBalanceRepo.BulkInsertAsync(balances);
    }

    private async Task<(string UserName, string Password, decimal rebateset)> RegisterGameProviderUser(string nickName)
    {
        var suffix = DateTime.UtcNow.ToString("yyyyMMddHHmmssf");
        var account = $"Gx{suffix}";
        var password = GameProviderPasswordGenerator.Generate();
        var request = new GameRegisterRequest
        {
            Account = account,
            Passwd = password,
            Alias = nickName,
            Rebateset = 0M,
        };
        await gameProvider.RegisterAsync(request);
        return (account, password, 0M);
    }
}

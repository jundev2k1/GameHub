using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserCreated;
using game_x.share.ExternalApi.GameProvider.Dtos.Register;

namespace game_x.application.Features.Auth.Client.Commands.RegisterUser;

public sealed class RegisterUserHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IUserBalanceRepo userBalanceRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    IGameProviderService gameProvider,
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
            var registerUser = User.Create(
                userName: request.Email,
                email: request.Email,
                nickName: request.Nickname);
            await userRepo.AddUserAsync(registerUser, request.Password, AppRole.Of(AppRoles.User), ct);
            await CreateUserBalancesAsync(registerUser);
            await RegisterGameProviderUser(request.Nickname);
            userId = registerUser.Id;
        }, ct);

        await eventDispatcher.Publish(new OnUserCreatedEvent(request.Email), ct);
        return new RegisterUserResult(userId);
    }
    
    private async Task CreateUserBalancesAsync(User user)
    {
        var activeTokens = await cryptoTokenRepo.GetAsync();

        var balances = activeTokens.Select(token => UserBalance.Create(
            userId: user.Id, 
            cryptoTokenId: token.Id,
            amount: 0)).ToList();

        await userBalanceRepo.BulkInsertAsync(balances);
    }

    private async Task RegisterGameProviderUser(string nickName)
    {
        var accountId = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        var password = "Pw123123";
        var request = new RegisterRequest
        {
            Account = accountId,
            Passwd = password,
            Alias = nickName,
            Rebateset = 0.1M,
        };
        await gameProvider.RegisterAsync(request, "zh-Hant");
    }
}

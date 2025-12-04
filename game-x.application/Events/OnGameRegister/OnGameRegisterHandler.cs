using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Utils;
using game_x.share.ExternalApi.GameProvider.Dtos.Register;

namespace game_x.application.Events.OnGameRegister;

public sealed class OnGameRegisterHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IGameProviderService gameProvider,
    IGameAesEncryptor aesEncryptor) : IApplicationEventHandler<OnGameRegisterEvent>
{
    public async Task Handle(OnGameRegisterEvent @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await userRepo.UpdateUserExtendAsync(@event.UserId, async usrex =>
            {
                await UpdateUserExtendAsync(@event, usrex);
            }, ct);
        }, ct);
    }

    private async Task UpdateUserExtendAsync(OnGameRegisterEvent @event, UserExtend usrex)
    {
        // Platform: Game598
        if (@event.GamePlatformId == GameConstants.PLATFORM_ID_G598)
        {
            await RegisterGame598User(usrex, usrex.User.Nickname);
            return;
        }

        // Platform: Game Baccarat
        if (@event.GamePlatformId == GameConstants.PLATFORM_ID_GAMEBACCARAT)
        {
            await RegisterGameBaccaratUser(usrex, usrex.User.Nickname);
            return;
        }
    }

    private async Task RegisterGame598User(UserExtend usrex, string nickName)
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

        usrex.UpdateG598Account(account, aesEncryptor.Encrypt(password), nickName, 0M);
    }

    private async Task RegisterGameBaccaratUser(UserExtend usrex, string nickName)
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

        usrex.UpdateBaccaratAccount(account, aesEncryptor.Encrypt(password), nickName);
    }
}

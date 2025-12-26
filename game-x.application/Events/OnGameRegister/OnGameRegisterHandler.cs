using game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.application.Contract.Infrastructure.ExternalApi.SasSlot;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Utils;
using game_x.share.ExternalApi.Etl998.Dtos.IsAccountExist;
using game_x.share.ExternalApi.Etl998.Dtos.Register;
using game_x.share.ExternalApi.GameBaccarat.Dtos.Register;
using game_x.share.ExternalApi.GameProvider.Dtos.Register;

namespace game_x.application.Events.OnGameRegister;

public sealed class OnGameRegisterHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IGameProviderService gameProvider,
    IGameBaccaratService gameBaccarat,
    IEtl998Service etl998Service,
    ISasSlotService sasSlotService,
    IGameAesEncryptor gameAesEncryptor,
    IAesEncryptor aesEncryptor) : IApplicationEventHandler<OnGameRegisterEvent>
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

        // Platform: Game etl998
        if (@event.GamePlatformId == GameConstants.PLATFORM_ID_ETL998_GAMEBACCARAT)
        {
            await RegisterEtl998User(usrex, usrex.User.Nickname);
        }

        if (@event.GamePlatformId == GameConstants.PLATFORM_ID_SASSLOT)
        {
            await RegisterSasSlotUser(usrex, usrex.User.Nickname);
        }
    }

    private async Task RegisterGame598User(UserExtend usrex, string nickName)
    {
        var suffix = DateTime.UtcNow.ToString("yyyyMMddHHmmssf");
        var account = $"Gx{suffix}";
        var password = GameProviderPasswordGenerator.Generate(5, 13);
        var request = new GameRegisterRequest
        {
            Account = account,
            Passwd = password,
            Alias = nickName,
            Rebateset = 0M,
        };
        await gameProvider.RegisterAsync(request);

        usrex.UpdateG598Account(account, gameAesEncryptor.Encrypt(password), nickName, 0M);
    }

    private async Task RegisterGameBaccaratUser(UserExtend usrex, string nickName)
    {
        var suffix = DateTime.UtcNow.ToString("yyyyMMddHHmmssf");
        var account = $"Gx{suffix}";
        var password = GameProviderPasswordGenerator.Generate(8, 20);
        var request = new GameBaccaratRegisterRequest
        {
            Account = account,
            Password = password,
            Nickname = nickName
        };
        var response = await gameBaccarat.RegisterAsync(request);
        usrex.UpdateBaccaratAccount(response.UserId, account, aesEncryptor.Encrypt(password), nickName);
    }

    private async Task RegisterEtl998User(UserExtend usrex, string nickName)
    {
        var suffix = DateTime.UtcNow.ToString("yyyyMMddHHmmssf");
        var account = $"Gx{suffix}".ToLower();
        var password = GameProviderPasswordGenerator.Generate(8, 20);
        var limitId = BettingLimitGroups.All[4].LimitId;
        var request = new Etl998RegisterRequest
        {
            Account = account,
            Password = password,
            Nickname = nickName,
            Ximalv = 10,
            Ximatype = 1,
            FatherId = 0,
            Tables = limitId.ToString()
        };
        var isExisted = await etl998Service.IsAccountExistAsync(new IsAccountExistRequest { Account = account });
        if (!isExisted)
        {
            var response = await etl998Service.RegisterAsync(request);
            var data = response.FirstOrDefault();
            if(data != null)
                usrex.UpdateEtl998Account(
                    account: account, 
                    nickname: nickName, 
                    password: aesEncryptor.Encrypt(password),
                    limitId: limitId);
        }
    }

    private async Task RegisterSasSlotUser(UserExtend usrex, string nickName)
    {
        var suffix = DateTime.UtcNow.ToString("yyyyMMddHHmmssf");
        var account = $"Gx{suffix}";
        await sasSlotService.RegisterAsync(account, nickName);
        usrex.UpdateSasSlotAccount(account, nickName);
    }
}
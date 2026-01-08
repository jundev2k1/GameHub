using game_x.application.Contract.Infrastructure.ExternalApi.Atg;
using game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.application.Contract.Infrastructure.ExternalApi.SasSlot;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Utils;
using game_x.share.ExternalApi.Atg.Dtos.Register;
using game_x.share.ExternalApi.Etl998.Dtos.IsAccountExist;
using game_x.share.ExternalApi.Etl998.Dtos.Register;
using game_x.share.ExternalApi.GameBaccarat.Dtos.Register;
using game_x.share.ExternalApi.GameProvider.Dtos.Login;
using game_x.share.ExternalApi.GameProvider.Dtos.Register;

namespace game_x.application.Events.OnGameRegister;

public sealed class OnGameRegisterHandler(
    IUserAccessor userAccessor,
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IGameProviderService gameProvider,
    IGameBaccaratService gameBaccarat,
    IEtl998Service etl998Service,
    ISasSlotService sasSlotService,
    IAtgService atgService,
    IGameAesEncryptor gameAesEncryptor,
    IAesEncryptor aesEncryptor) : IApplicationEventHandler<OnGameRegisterEvent>
{
    public async Task Handle(OnGameRegisterEvent @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await userRepo.UpdateUserExtendAsync(@event.UserId, async ue =>
            {
                await UpdateUserExtendAsync(@event, ue);
            }, ct);
        }, ct);
    }

    private async Task UpdateUserExtendAsync(OnGameRegisterEvent @event, UserExtend ue)
    {
        // Platform: Game598
        if (@event.GamePlatformId == GameConstants.PLATFORM_ID_G598)
        {
            await RegisterGame598User(ue, ue.User.Nickname);
            return;
        }

        // Platform: Game Baccarat
        if (@event.GamePlatformId == GameConstants.PLATFORM_ID_GAMEBACCARAT)
        {
            await RegisterGameBaccaratUser(ue, ue.User.Nickname);
            return;
        }

        // Platform: Game etl998
        if (@event.GamePlatformId == GameConstants.PLATFORM_ID_ETL998_GAMEBACCARAT)
        {
            await RegisterEtl998User(ue, ue.User.Nickname);
        }

        // Platform: SAS Slot
        if (@event.GamePlatformId == GameConstants.PLATFORM_ID_SASSLOT)
        {
            await RegisterSasSlotUser(ue, ue.User.Nickname);
        }
        
        if (@event.GamePlatformId == GameConstants.PLATFORM_ID_ATG)
        {
            await RegisterAtgUser(ue);
        }
    }

    private async Task RegisterGame598User(UserExtend usrex, string nickName)
    {
        // Register a new account
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

        // Try to log in for the first time
        var loginRequest = new GameLoginRequest
        {
            Account = account,
            Passwd = password,
            Gamecode = "KRF3",
            Address = "lobby",
            Locale = "zh-Hant",
            ReturnUrl = "",
        };
        var ip = userAccessor.GetIpAddress();
        await gameProvider.LoginAsync(loginRequest, ip);

        // Update Game 598 account
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

    private async Task RegisterEtl998User(UserExtend ue, string nickName)
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
                ue.UpdateEtl998Account(
                    account: account, 
                    nickname: nickName, 
                    password: aesEncryptor.Encrypt(password),
                    limitId: limitId);
        }
    }

    private async Task RegisterSasSlotUser(UserExtend ue, string nickName)
    {
        var suffix = DateTime.UtcNow.ToString("yyyyMMddHHmmssf");
        var account = $"Gx{suffix}";
        await sasSlotService.LoginAsync(account, nickName);
        ue.UpdateSasSlotAccount(account, nickName);
    }
    
    private async Task RegisterAtgUser(UserExtend ue)
    {
        var suffix = DateTime.UtcNow.ToString("yyyyMMddHHmmssf");
        var account = $"Gx{suffix}".ToLower();
        var request = new AtgRegisterRequest
        {
            Username = account,
            Email = ue.User.Email ?? String.Empty,
            Fullname = ue.User.Nickname
        };
        var isSuccess = await atgService.RegisterAsync(request);
        if(isSuccess)
            ue.UpdateAtgAccount(
                userName: account,
                email: ue.User.Email ?? String.Empty,
                fullname: ue.User.Nickname);
    }
}
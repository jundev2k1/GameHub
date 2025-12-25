using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Games.Services;
using game_x.share.ExternalApi.Etl998.Dtos.ForwardGame;
using game_x.share.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.Games.Client.Commands.Etl998.ForwardGame;

public sealed class ForwardGameHandler(
    IUserAccessor userAccessor,
    IGamePlatformService gamePlatformService,
    IUserRepo userRepo,
    IEtl998Service service,
    ILogger<ForwardGameHandler> logger,
    IOptions<Etl998Settings> settings,
    IAesEncryptor aesEncryptor): ICommandHandler<ForwardGameCommand, IReadOnlyCollection<ForwardGameResponse>>
{
    public async Task<IReadOnlyCollection<ForwardGameResponse>> Handle(ForwardGameCommand cmd, CancellationToken ct = default)
    {
        try
        {
            var userId = userAccessor.GetUserId();
            var targetUser = await userRepo.GetUserByIdAsync(userId, ct);

            if (!targetUser.EmailConfirmed)
                throw new BadRequestException(MessageCode.User.UserNotConfirmed);

            targetUser = await gamePlatformService.EnsureExternalAccountCreatedAsync(
                user: targetUser,
                gamePlatformId: GameConstants.PLATFORM_ID_ETL998_GAMEBACCARAT,
                ct: ct);

            var accountName = targetUser.UserExtend?.Etl998ProviderAccount;
            var password = targetUser.UserExtend?.Etl998ProviderPassword;
            if (accountName == null || password == null)
                throw new BadRequestException(MessageCode.System.SystemError, "The ETL998 account does not exist.");
            var request = new ForwardGameRequest
            {
                Account = accountName, 
                Password = aesEncryptor.Decrypt(password),
                Dm = settings.Value.Host
            };
            return await service.ForwardGameAsync(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new BadRequestException(MessageCode.System.SystemError, ex.Message);
        }
    }
}
using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Games.Services;
using game_x.share.ExternalApi.Etl998.Dtos.ModifyBettingLimit;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Games.Client.Commands.Etl998.ModifyBettingLimit;

public sealed class ModifyBettingLimitHandler(
    IUserAccessor userAccessor,
    IEtl998Service service,
    IUserRepo userRepo,
    IGamePlatformService gamePlatformService,
    ILogger<ModifyBettingLimitHandler> logger): ICommandHandler<ModifyBettingLimitCommand, bool>
{
    public async Task<bool> Handle(ModifyBettingLimitCommand cmd, CancellationToken ct = default)
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
            if (accountName == null)
                throw new BadRequestException(MessageCode.System.SystemError, "The ETL998 account does not exist.");
            
            var request = new ModifyBettingLimitRequest
            {
                Account = accountName,
                Tables = cmd.Tables
            };
            return await service.ModifyBettingLimitAsync(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new BadRequestException(MessageCode.System.SystemError, ex.Message);
        }
    }
}
using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Games.Services;
using game_x.share.ExternalApi.Etl998.Converters;
using game_x.share.ExternalApi.Etl998.Dtos.SearchRecord;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Games.Client.Commands.Etl998.SearchRecord;

public sealed class SearchRecordHandler(
    IUserAccessor userAccessor,
    IGamePlatformService gamePlatformService,
    IUserRepo userRepo,
    IEtl998Service service,
    ILogger<SearchRecordHandler> logger,
    IAesEncryptor aesEncryptor): ICommandHandler<SearchRecordCommand, IReadOnlyCollection<SearchRecordResult>>
{
    public async Task<IReadOnlyCollection<SearchRecordResult>> Handle(SearchRecordCommand cmd, CancellationToken ct = default)
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
            
            var request = new SearchRecordRequest
            {
                Account = accountName, 
                Password = aesEncryptor.Decrypt(password),
                DateStart = EtlDateTimeConverter.ToEtlDate(cmd.DateStart),
                DateEnd = EtlDateTimeConverter.ToEtlDate(cmd.DateEnd),
                PageIndex = cmd.PageIndex,
                PageSize = cmd.PageSize
            };
            var response = await service.SearchRecordAsync(request);
            return response.Select(x => x.Adapt<SearchRecordResult>() with
            {
                BetTime = EtlDateTimeConverter.ToUtc(x.BetTime),
                SettlementTime = EtlDateTimeConverter.ToUtc(x.SettlementTime),
            }).ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new BadRequestException(MessageCode.System.SystemError, ex.Message);
        }
    }
}
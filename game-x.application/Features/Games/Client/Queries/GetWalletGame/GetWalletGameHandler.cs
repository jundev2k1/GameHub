using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.ExternalApi.GameProvider.Dtos.Wallet;

namespace game_x.application.Features.Games.Client.Queries.WalletGame;

public sealed class GetWalletGameHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IGameProviderService gameProvider) : ICommandHandler<GetWalletGameQuery, GetWalletGameResult>
{
    public async Task<GetWalletGameResult> Handle(GetWalletGameQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var targetUser = await userRepo.GetUserByIdAsync(userId, ct);
        if (targetUser.UserExtend is null)
            throw new NotFoundException("User extend is not exists.");

        // Check: email must be confirmed before requesting password reset
        if (!targetUser.EmailConfirmed)
            throw new BadRequestException(MessageCode.User.UserNotConfirmed);

        var externalRequest = new WalletRequest
        {
            Account = targetUser.UserExtend.GameProviderAccount
        };
        var result = await gameProvider.GetWalletAsync(externalRequest);
        return new GetWalletGameResult(result.Quota);
    }
}

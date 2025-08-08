using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.ExternalApi.GameProvider.Dtos.Login;

namespace game_x.application.Features.Games.Commands.LoginGame;

public sealed class LoginGameHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IGameProviderService gameProvider,
    IGameAesEncryptor aesEncryptor) : ICommandHandler<LoginGameCommand, LoginGameResult>
{
    public async Task<LoginGameResult> Handle(LoginGameCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var targetUser = await userRepo.GetUserByIdAsync(userId, ct);
        if (targetUser.UserExtend is null)
            throw new NotFoundException("User extend is not exists.");

        // Check: email must be confirmed before requesting password reset
        if (!targetUser.EmailConfirmed)
            throw new BadRequestException(MessageCode.User.UserNotConfirmed);

        var externalRequest = new LoginRequest
        {
            Account = targetUser.UserExtend.GameProviderAccount,
            Passwd = aesEncryptor.Decrypt(targetUser.UserExtend.GameProviderPassword),
            Gamecode = request.GameCode,
            Address = request.Address,
            Locale = request.Locale,
            ReturnUrl = request.ReturnUrl,
        };
        var result = await gameProvider.LoginAsync(externalRequest, request.Locale, request.IpAddress!);
        return new LoginGameResult(result.Url);
    }
}

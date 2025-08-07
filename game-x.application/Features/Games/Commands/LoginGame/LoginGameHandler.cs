using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.share.ExternalApi.GameProvider.Dtos.Login;

namespace game_x.application.Features.Games.Commands.LoginGame;

public sealed class LoginGameHandler(IGameProviderService gameProvider) : ICommandHandler<LoginGameCommand, LoginGameResult>
{
    public async Task<LoginGameResult> Handle(LoginGameCommand request, CancellationToken ct = default)
    {
        var externalRequest = new LoginRequest
        {
            Account = "game_123",
            Passwd = "Aa123456",
            Gamecode = request.GameCode,
            Address = request.Address,
            Locale = request.Locale,
            ReturnUrl = request.ReturnUrl,
        };
        var result = await gameProvider.LoginAsync(externalRequest, request.Language, request.IpAddress!);
        return new LoginGameResult(result.Url);
    }
}

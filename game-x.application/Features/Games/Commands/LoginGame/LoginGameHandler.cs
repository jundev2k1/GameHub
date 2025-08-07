using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.share.ExternalApi.GameProvider.Dtos.Login;

namespace game_x.application.Features.Games.Commands.LoginGame;

public sealed class LoginGameHandler(IGameProviderService gameProvider) : ICommandHandler<LoginGameCommand, LoginGameResult>
{
    public async Task<LoginGameResult> Handle(LoginGameCommand request, CancellationToken ct = default)
    {
        var externalRequest = new LoginRequest
        {
            Account = "20250807102055575",
            Passwd = "Pw12312",
            Gamecode = request.GameCode,
            Address = request.Address,
            Locale = request.Locale,
            ReturnUrl = request.ReturnUrl,
        };
        var ipTest = "https://home.tichluyvang.com";
        var result = await gameProvider.LoginAsync(externalRequest, request.Language, ipTest!);
        return new LoginGameResult(result.Url);
    }
}

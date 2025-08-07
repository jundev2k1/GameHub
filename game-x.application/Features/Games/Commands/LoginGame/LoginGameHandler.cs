namespace game_x.application.Features.Games.Commands.LoginGame;

public sealed class LoginGameHandler : ICommandHandler<LoginGameCommand, LoginGameResult>
{
    public async Task<LoginGameResult> Handle(LoginGameCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return new LoginGameResult("");
    }
}

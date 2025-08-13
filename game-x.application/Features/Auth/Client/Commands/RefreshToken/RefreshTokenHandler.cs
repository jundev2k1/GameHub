namespace game_x.application.Features.Auth.Client.Commands.RefreshToken;

public sealed class RefreshTokenHandler : ICommandHandler<RefreshTokenCommand, RefreshTokenResult>
{
    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return new RefreshTokenResult(string.Empty, string.Empty);
    }
}

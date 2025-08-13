namespace game_x.application.Features.Accounts.User.Commands.RevokeToken;

public sealed class RevokeTokenHandler : ICommandHandler<RevokeTokenCommand>
{
    public async Task<Unit> Handle(RevokeTokenCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return Unit.Value;
    }
}

namespace game_x.application.Features.Auth.Commands.Verify.VerifyEmailUser;

public sealed class VerifyEmailUserHandler : ICommandHandler<VerifyEmailUserCommand>
{
    public async Task<Unit> Handle(VerifyEmailUserCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return Unit.Value;
    }
}

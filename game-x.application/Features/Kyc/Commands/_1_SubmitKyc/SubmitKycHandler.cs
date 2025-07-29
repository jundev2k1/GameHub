namespace game_x.application.Features.Kyc.Commands._1_SubmitKyc;

public sealed class SubmitKycHandler : ICommandHandler<SubmitKycCommand>
{
    public async Task<Unit> Handle(SubmitKycCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return Unit.Value;
    }
}

namespace game_x.application.Features.Kyc.Commands._3_ResubmitKyc;

public sealed class ResubmitKycHandler : ICommandHandler<ResubmitKycCommand>
{
    public async Task<Unit> Handle(ResubmitKycCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return Unit.Value;
    }
}

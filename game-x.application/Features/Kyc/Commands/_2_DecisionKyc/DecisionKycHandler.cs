namespace game_x.application.Features.Kyc.Commands._2_DecisionKyc;

public sealed class DecisionKycHandler : ICommandHandler<DecisionKycCommand>
{
    public async Task<Unit> Handle(DecisionKycCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return Unit.Value;
    }
}

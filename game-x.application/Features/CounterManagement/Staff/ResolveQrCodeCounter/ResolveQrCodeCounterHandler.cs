using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.CounterManagement.Staff.ResolveQrCodeCounter;

public sealed class ResolveQrCodeCounterHandler(ICounterTokenRepo counterTokenRepo)
    : ICommandHandler<ResolveQrCodeCounterCommand, ResolveQrCodeCounterResult>
{
    public async Task<ResolveQrCodeCounterResult> Handle(ResolveQrCodeCounterCommand request, CancellationToken ct = default)
    {
        var counterId = await counterTokenRepo.GetByTokenAsync(request.QrCode, ct);
        return new ResolveQrCodeCounterResult(counterId.Counter.PublicId);
    }
}

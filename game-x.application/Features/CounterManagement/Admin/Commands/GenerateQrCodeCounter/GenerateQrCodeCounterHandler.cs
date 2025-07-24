using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.CounterManagement.Admin.Commands.GenerateQrCodeCounter;

public sealed class GenerateQrCodeCounterHandler(ICounterTokenRepo counterTokenRepo)
    : ICommandHandler<GenerateQrCodeCounterCommand, GenerateQrCodeCounterResult>
{
    public async Task<GenerateQrCodeCounterResult> Handle(GenerateQrCodeCounterCommand request, CancellationToken ct)
    {
        var counterToken = await counterTokenRepo.GetByIdAsync(request.CounterId, ct);
        return new GenerateQrCodeCounterResult(counterToken.Token);
    }
}

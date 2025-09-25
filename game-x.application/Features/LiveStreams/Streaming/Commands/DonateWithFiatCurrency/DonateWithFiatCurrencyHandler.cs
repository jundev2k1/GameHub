namespace game_x.application.Features.LiveStreams.Streaming.Commands.DonateWithFiatCurrency;

public sealed class DonateWithFiatCurrencyHandler : ICommandHandler<DonateWithFiatCurrencyCommand>
{
    public async Task<Unit> Handle(DonateWithFiatCurrencyCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;

        return Unit.Value;
    }
}

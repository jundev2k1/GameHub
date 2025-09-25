namespace game_x.application.Features.LiveStreams.Streaming.Commands.DonateWithFiatCurrency;

public record DonateWithFiatCurrencyCommand(string StreamKey, Guid GiftId, string Message) : ICommand;

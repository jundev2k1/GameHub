namespace game_x.application.Features.CounterManagement.Staff.ResolveQrCodeCounter;

public record ResolveQrCodeCounterCommand(string QrCode) : ICommand<ResolveQrCodeCounterResult>;

public record ResolveQrCodeCounterResult(Guid CounterId);

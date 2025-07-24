namespace game_x.application.Features.CounterManagement.Admin.Commands.GenerateQrCodeCounter;

public record GenerateQrCodeCounterCommand(Guid CounterId) : ICommand<GenerateQrCodeCounterResult>;

public record GenerateQrCodeCounterResult(string CounterToken);

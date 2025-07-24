namespace game_x.application.Features.CounterManagement.Admin.Commands.SoftDeleteCounter;

public record SoftDeleteCounterCommand(Guid CounterId) : ICommand;
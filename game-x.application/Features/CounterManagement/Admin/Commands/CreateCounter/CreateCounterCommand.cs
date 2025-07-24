namespace game_x.application.Features.CounterManagement.Admin.Commands.CreateCounter;

public sealed class CreateCounterCommand : ICommand
{
    public required string CounterName { get; set; }
    public required string Location { get; set; }
    public required string Description { get; set; }
}

namespace game_x.application.Features.LiveStreams.Commands.CreateSchedule;

public sealed class CreateScheduleHandler : ICommand<CreateScheduleCommand>
{
    public async Task<Unit> Handle(CreateScheduleCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return Unit.Value;
    }
}

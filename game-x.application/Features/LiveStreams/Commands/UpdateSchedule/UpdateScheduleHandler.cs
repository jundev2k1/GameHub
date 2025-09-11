namespace game_x.application.Features.LiveStreams.Commands.UpdateSchedule;

public sealed class UpdateScheduleHandler : ICommand<UpdateScheduleCommand>
{
    public async Task<Unit> Handle(UpdateScheduleCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return Unit.Value;
    }
}

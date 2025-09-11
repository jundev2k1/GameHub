namespace game_x.application.Features.LiveStreams.Commands.CancelSchedule;

public sealed class CancelScheduleHandler : ICommand<CancelScheduleCommand>
{
    public async Task<Unit> Handle(CancelScheduleCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return Unit.Value;
    }
}
